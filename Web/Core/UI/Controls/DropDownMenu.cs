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
public class DropDownMenu: WebControl, IControl, IPostBackEventHandler
{
  public static string EscapeJavaScript (string input)
  {
    StringBuilder output = new StringBuilder(input.Length + 5);
    for (int idxChars = 0; idxChars < input.Length; idxChars++)
    {
      char c = input[idxChars];
      switch (c)
      {
        case '\t':
        {
          output.Append (@"\t");
          break;
        }
        case '\n':
        {
          output.Append (@"\n");
          break;
        }
        case '\r':
        {
          output.Append (@"\r");
          break;
        }
        case '"':
        {
          output.Append ("\\\"");
          break;
        }
        case '\'':
        {
          output.Append (@"\'");
          break;
        }
        case '\\':
        {
          output.Append (@"\\");
          break;
        }
        case '\v':
        {
          output.Append (c);
          break;
        }
        case '\f':
        {
          output.Append (c);
          break;
        }
        default:
        {
          output.Append(c);
          break;
        }
      }
    }
    return output.ToString();
  }
 
  private static readonly object EventClick = new object();

  private const string c_dropDownIcon = "DropDownArrow.gif";

  private string _groupID;
  private string _titleText;
  private string _titleIcon;
  private bool _isReadOnly;

  private MenuItemCollection _menuItems;

	public DropDownMenu (string groupID, Control ownerControl, Type[] supportedMenuItemTypes)
	{
    ArgumentUtility.CheckNotNullOrEmpty ("groupID", groupID);
    _groupID = groupID;
    _menuItems = new MenuItemCollection (ownerControl, supportedMenuItemTypes);
	}

  public DropDownMenu (string groupID, Control ownerControl)
      : this (groupID, ownerControl, new Type[] {typeof (MenuItem)})
	{
  }

  /// <summary> Implements interface <see cref="IPostBackEventHandler"/>. </summary>
  /// <param name="eventArgument"> &lt;index&gt;[,&lt;ID&gt;] </param>
  public void RaisePostBackEvent (string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

    string[] eventArgumentParts = eventArgument.Split (new char[] {','}, 2);

    //  First part: index
    int index;
    eventArgumentParts[0] = eventArgumentParts[0].Trim();
    try 
    {
      if (eventArgumentParts[0].Length == 0)
        throw new FormatException();
      index = int.Parse (eventArgumentParts[0]);
    }
    catch (FormatException)
    {
      throw new ArgumentException ("First part of argument 'eventArgument' must be an integer. Expected format: '<index>[,<business-object-id>]'.");
    }

    //  Second part, optional: item ID
    string id = null;
    if (eventArgumentParts.Length == 2)
      id = eventArgumentParts[1].Trim();

    if (index >= _menuItems.Count)
      throw new ArgumentOutOfRangeException ("Index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed menu items.'");

    MenuItem item = _menuItems[index];
    if (item.Command == null)
      throw new ArgumentOutOfRangeException ("The DropDownMenu '" + ID + "' does not have a command associated with menu item " + index + ".");

    OnClick (index, id, item);
  }

  /// <summary> Fires the <see cref="Click"/> event. </summary>
  protected virtual void OnClick (int index, string id, MenuItem item)
  {
    MenuItemClickEventHandler clickHandler = (MenuItemClickEventHandler) Events[EventClick];
    if (clickHandler != null)
    {
      MenuItemClickEventArgs e = new MenuItemClickEventArgs (item);
      clickHandler (this, e);
    }
  }

  protected override void OnPreRender (EventArgs e)
  {
    string key = typeof (DropDownMenu).FullName + "_Style";
    string styleSheetUrl = null;
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (DropDownMenu), ResourceType.Html, "DropDownMenu.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, styleSheetUrl);
    }

    key = typeof (DropDownMenu).FullName + "_Script";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (DropDownMenu), ResourceType.Html, "DropDownMenu.js");
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, url);
    }

    //  Startup script initalizing the global values of the script.
    key = typeof (DropDownMenu).FullName+ "_Startup";
    if (! Page.IsStartupScriptRegistered (key))
    {
      if (styleSheetUrl == null)
      {
        styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            this, Context, typeof (DropDownMenu), ResourceType.Html, "DropDownMenu.css");
      }
      string script = string.Format ("DropDownMenu_InitializeGlobals ('{0}');", styleSheetUrl);
      PageUtility.RegisterStartupScriptBlock (Page, key, script);
    }

    key = typeof (DropDownMenu).FullName + _groupID;
    if (! Page.IsStartupScriptRegistered (key))
    {
      StringBuilder script = new StringBuilder();
      script.Append ("DropDownMenu_AddMenuInfo (\r\n\t");
      script.AppendFormat ("new DropDownMenu_MenuInfo ('{0}', new Array (\r\n", _groupID);
      bool isFirstItem = true;
      for (int i = 0; i < MenuItems.Count; i++)
      {
        MenuItem menuItem = MenuItems[i];
        if (isFirstItem)
          isFirstItem = false;
        else
          script.AppendFormat (",\r\n");

        AppendMenuItem (script, menuItem, i);
      }
      script.Append (" )"); // Close Array
      script.Append (" )"); // Close new MenuInfo
      script.Append (" );"); // Close AddMenuInfo
      PageUtility.RegisterStartupScriptBlock (Page, key, script.ToString());
    }

    base.OnPreRender (e);
  }

  private void AppendMenuItem (StringBuilder stringBuilder, MenuItem menuItem, int menuItemIndex)
  {
    string href = "null";
    string target = "null";

    if (menuItem.Command != null)
    {
      bool isActive =    menuItem.Command.Show == CommandShow.Always
                      || _isReadOnly && menuItem.Command.Show == CommandShow.ReadOnly
                      || ! _isReadOnly && menuItem.Command.Show == CommandShow.EditMode;

      bool isCommandEnabled = isActive && menuItem.Command.Type != CommandType.None;
      if (isCommandEnabled)
      {    
        bool isPostBackCommand =    menuItem.Command.Type == CommandType.Event 
                                || menuItem.Command.Type == CommandType.WxeFunction;
        if (isPostBackCommand)
        {
          string argument = menuItemIndex.ToString();
          if (menuItem.Command.Type == CommandType.WxeFunction)
            argument += "," + menuItem.Command.WxeFunctionCommand.Parameters;
          //string test = "test'test";
          //string s1 = Page.GetPostBackClientHyperlink (this, test);
          //string s2 = DropDownMenu.EscapeJavaScript (s1);
          href = Page.GetPostBackClientHyperlink (this, argument);
          //href = href.Replace ("'", @"\'");
          href = DropDownMenu.EscapeJavaScript (href);
          href = "'" + href + "'";
        }
        else if (menuItem.Command.Type == CommandType.Href)
        {
          href = "'" + menuItem.Command.HrefCommand.FormatHref (menuItemIndex.ToString(), menuItem.ItemID) + "'";
          target = "'" + menuItem.Command.HrefCommand.Target + "'";
        }
      }
    }

    string icon = (StringUtility.IsNullOrEmpty (menuItem.Icon) ? "null" : "'" +  menuItem.Icon + "'");
    stringBuilder.AppendFormat (
        "\t\tnew DropDownMenu_ItemInfo ('{0}', '{1}', '{2}', {3}, {4}, {5})",
        menuItemIndex.ToString(), menuItem.Category, menuItem.Text, icon, href, target);
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

    RenderHead (writer);

    writer.RenderEndTag(); // End Menu-Div

    writer.RenderEndTag(); // End Control-Tag
  }

  private void RenderHead (HtmlTextWriter writer)
  {
    //  Head-Div is used to group the title and the button, providing a single point of reference
    //  for the popup-div.
    writer.AddStyleAttribute ("position", "relative");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
    writer.AddAttribute ("id", ID + "_HeadDiv");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHead);
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Drop Down Head-Div

    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.RenderBeginTag (HtmlTextWriterTag.Table);  // Begin Drop Down Button Div
    writer.RenderBeginTag (HtmlTextWriterTag.Tr);
    
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadTitle);
    writer.RenderBeginTag (HtmlTextWriterTag.Td);
    writer.Write (_titleText);
    writer.RenderEndTag();

    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1em");
    writer.AddStyleAttribute ("text-align", "center");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadButton);
    writer.RenderBeginTag (HtmlTextWriterTag.Td);
    string url = ResourceUrlResolver.GetResourceUrl (
        this, Context, typeof (DropDownMenu), ResourceType.Image, c_dropDownIcon);
    writer.AddAttribute (HtmlTextWriterAttribute.Src, url);
    writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
    writer.AddStyleAttribute ("vertical-align", "middle");
    writer.AddAttribute (HtmlTextWriterAttribute.Onclick, "return false;");
    writer.RenderBeginTag (HtmlTextWriterTag.Input);
    writer.RenderEndTag();
    writer.RenderEndTag();

    writer.RenderEndTag(); // End Drop Down Button Table

    writer.RenderEndTag();

    ////  Options Drop Down Button 
    //writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
    //writer.AddStyleAttribute ("min-height", "1em");
    //writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1em");
    //writer.AddStyleAttribute ("float", "right");
    //writer.AddStyleAttribute ("text-align", "center");
    //writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadButton);
    //writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin Drop Down Button-San
    //string url = ResourceUrlResolver.GetResourceUrl (
    //    this, Context, typeof (DropDownMenu), ResourceType.Image, c_dropDownIcon);
    //writer.AddAttribute (HtmlTextWriterAttribute.Src, url);
    //writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
    //writer.AddStyleAttribute ("vertical-align", "middle");
    //writer.AddAttribute (HtmlTextWriterAttribute.Onclick, "return false;");
    //writer.RenderBeginTag (HtmlTextWriterTag.Input);
    //writer.RenderEndTag();
    //writer.RenderEndTag();  // End Drop Down Button-Span
    //
    ////  TODO: IE 5.01 has trouble with height
    ////  Options Drop Down Titel
    //writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadTitle);
    //writer.RenderBeginTag (HtmlTextWriterTag.Span);
    //writer.Write (_titleText);
    //writer.RenderEndTag();

    writer.RenderEndTag();  // End Drop Down Head-Div
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

  public bool IsReadOnly
  {
    get { return _isReadOnly; }
    set { _isReadOnly = value; }
  }

  /// <summary> 
  ///   Occurs when a command of type <see cref="CommandType.Event"/> 
  ///   or <see cref="CommandType.WxeFunction"/> is clicked. 
  /// </summary>
  [Category ("Action")]
  [Description ("Occurs when a command of type Event or WxeFunction is clicked.")]
  public event MenuItemClickEventHandler Click
  {
    add 
    {
      Events.AddHandler (EventClick, value);
    }
    remove 
    { 
      Events.RemoveHandler (EventClick, value);
    }
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

/// <summary>
///   Represents the method that handles the <c>Click</c> event raised when clicking on a menu item.
/// </summary>
public delegate void MenuItemClickEventHandler (object sender, MenuItemClickEventArgs e);

/// <summary>
///   Provides data for the <c>Click</c> event.
/// </summary>
public class MenuItemClickEventArgs: EventArgs
{
  /// <summary> The <see cref="MenuItem"/> that was clicked. </summary>
  private MenuItem _item;

  /// <summary> Initializes an instance. </summary>
  public MenuItemClickEventArgs (MenuItem item)
  {
    _item = item;
  }

  /// <summary> The <see cref="MenuItem"/> that was clicked. </summary>
  public MenuItem Item
  {
    get { return _item; }
  }
}

}
