using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.Web.UI;
using Rubicon.Web;
using Rubicon.Web.Utilities;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

/*
  Edit
  Duplicate
  ---------
  �ffnen
  auto:list1:menu:cut:text = "Ausschneiden"
*/
/// <remarks>
///   Work in Progress
/// </remarks>
public class DropDownMenu: WebControl, IControl
{
  private const string c_dropDownIcon = "DropDownArrow.gif";

  private string _groupID;
  private string _titleText;
  private string _titleIcon;

  private MenuItemCollection _menuItems;

	public DropDownMenu (string groupID, IControl ownerControl, Type[] supportedMenuItemTypes)
	{
    ArgumentUtility.CheckNotNullOrEmpty ("groupID", groupID);
    _groupID = groupID;
    _menuItems = new MenuItemCollection (ownerControl, supportedMenuItemTypes);
	}

  public DropDownMenu (string groupID, IControl ownerControl)
      : this (groupID, ownerControl, new Type[] {typeof (MenuItem)})
	{
  }

  protected override void OnPreRender (EventArgs e)
  {
    string key = typeof (DropDownMenu).FullName + "_Style";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (DropDownMenu), ResourceType.Html, "DropDownMenu.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, url);
    }

    key = typeof (DropDownMenu).FullName + "_Script";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (DropDownMenu), ResourceType.Html, "DropDownMenu.js");
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, url);
    }

    MenuItem[] items = new MenuItem[] {
        new MenuItem ("open", "object", "�ffnen", "open.gif", null),
        new MenuItem ("cut", "edit", "Ausschneiden", "cut.gif", null),
        new MenuItem ("paste", "edit", "Einf�gen", "paste.gif", null),
        new MenuItem ("duplicate", "edit", "Duplizieren", "duplicate.gif", null),
        new MenuItem ("delete", "edit", "L�schen", "delete.gif", null),
        new MenuItem ("item", "item", "Item", "item.gif", null),
        new MenuItem ("item", "item", "Item", "item.gif", null),
        new MenuItem ("item", "item", "Item", "item.gif", null),
        new MenuItem ("item", "item", "Item", "item.gif", null)};
    key = typeof (DropDownMenu).FullName + _groupID;
    if (! Page.IsStartupScriptRegistered (key))
    {
      StringBuilder script = new StringBuilder();
      script.Append ("DropDownMenu_AddMenuInfo (\r\n\t");
      script.AppendFormat (
          "new DropDownMenu_MenuInfo ('{0}', new Array (\r\n",
          _groupID);
      bool isFirstItem = true;
      foreach (MenuItem item in items)
      {
        if (isFirstItem)
          isFirstItem = false;
        else
          script.AppendFormat (",\r\n");

        script.AppendFormat (
            "\t\tnew DropDownMenu_ItemInfo ('{0}', '{1}', '{2}', '{3}')",
            item.ID, item.Category, item.Text, item.Icon);
      }
      script.Append (" )"); // Close Array
      script.Append (" )"); // Close new MenuInfo
      script.Append (" );"); // Close AddMenuInfo
      PageUtility.RegisterStartupScriptBlock (Page, key, script.ToString());
    }

    base.OnPreRender (e);
  }

  protected override void Render (HtmlTextWriter writer)
  {
    if (! Width.IsEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Width.ToString());
    if (! Height.IsEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Height, Height.ToString());
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Control-Tag

    //  Menu-Div filling the control's div is required to apply internal css attributes
    //  for position, width and height. This allows the Head and th popup-div to align themselves
    writer.AddStyleAttribute ("position", "relative");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
    string script = "DropDownMenu_OnClick (this, '" + _groupID + "');";
    writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
    writer.AddAttribute ("id", ID + "_MenuDiv");
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Menu-Div

    //  Head-Div is used to group the title and the button, providing a single point of reference
    //  for the popup-div.
    writer.AddStyleAttribute ("position", "relative");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
    writer.AddAttribute ("id", ID + "_HeadDiv");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHead);
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Drop Down Head-Div

    
    //  Options Drop Down Button 
    writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
    writer.AddStyleAttribute ("min-height", "1em");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1em");
    writer.AddStyleAttribute ("float", "right");
    writer.AddStyleAttribute ("text-align", "center");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadButton);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin Drop Down Button-San
    string url = ResourceUrlResolver.GetResourceUrl (
        this, Context, typeof (DropDownMenu), ResourceType.Image, c_dropDownIcon);
    writer.AddAttribute (HtmlTextWriterAttribute.Src, url);
    writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
    writer.AddStyleAttribute ("vertical-align", "middle");
    writer.AddAttribute (HtmlTextWriterAttribute.Onclick, "return false;");
    writer.RenderBeginTag (HtmlTextWriterTag.Input);
    writer.RenderEndTag();
    writer.RenderEndTag();  // End Drop Down Button-Span

    //  TODO: IE 5.01 has trouble with height
    //  Options Drop Down Titel
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadTitle);
    writer.RenderBeginTag (HtmlTextWriterTag.Span);
    writer.Write (_titleText);
    writer.RenderEndTag();
    writer.RenderEndTag();  // End Drop Down Head-Div

//    //  Options Drop Down PopUp
//    writer.AddStyleAttribute ("position", "absolute");
//    writer.AddStyleAttribute ("right", "0px");
//    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassPopUp);
//    writer.RenderBeginTag (HtmlTextWriterTag.Div);
//    
//    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassItem);
//    writer.RenderBeginTag (HtmlTextWriterTag.Div);
//    writer.WriteLine ("Item 1"); // width: 100% spans menu across the whole options-div
//    writer.RenderEndTag();
//    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassItem);
//    writer.RenderBeginTag (HtmlTextWriterTag.Div);
//    writer.WriteLine ("Item 2");
//    writer.RenderEndTag();
//    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassItemHover);
//    writer.RenderBeginTag (HtmlTextWriterTag.Div);
//    writer.WriteLine ("Item 3");
//    writer.RenderEndTag();
//    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassItem);
//    writer.RenderBeginTag (HtmlTextWriterTag.Div);
//    writer.WriteLine ("Item 4");
//    writer.RenderEndTag();
//
//    writer.RenderEndTag();

    writer.RenderEndTag(); // End Menu-Div

    writer.RenderEndTag(); // End Control-Tag
  }

  public string TitleText
  {
    get { return _titleText; }
    set { _titleText = value; }
  }

  public string TitleIcon
  {
    get { return _titleIcon; }
    set { _titleIcon = value; }
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Behavior")]
  [Description ("The menu items displayed by this drop down menu.")]
  [DefaultValue ((string) null)]
  public MenuItemCollection MenuItems
  {
    get { return _menuItems; }
  }

  protected virtual string CssClassHead
  { get { return "dropDownMenuHead"; } }

  protected virtual string CssClassHeadFocus
  { get { return "dropDownMenuHeadFocus"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="DropDownMenu"/>'s title. </summary>
  /// <remarks> Class: <c></c> </remarks>
  protected virtual string CssClassHeadTitle
  { get { return "dropDownMenuHeadTitle"; } }

  protected virtual string CssClassHeadTitleFocus
  { get { return "dropDownMenuHeadTitleFocus"; } }

  protected virtual string CssClassHeadButton
  { get { return "dropDownMenuHeadButton"; } }

  protected virtual string CssClassMenuButtonFocus
  { get { return "dropDownMenuButtonFocus"; } }

  protected virtual string CssClassPopUp
  { get { return "dropDownMenuPopUp"; } }

  protected virtual string CssClassItem
  { get { return "dropDownMenuItem"; } }

  protected virtual string CssClassItemHover
  { get { return "dropDownMenuItemHover"; } }

  protected virtual string CssClassItemTextPane
  { get { return "dropDownMenuItemTextPane"; } }

  protected virtual string CssClassItemIconPane
  { get { return "dropDownMenuItemIconPane"; } }
}

}
