using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.Web.UI;
using Rubicon.Web;
using Rubicon.Web.Utilities;
using Rubicon.Utilities;
using Rubicon.Collections;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UI.Controls
{

/*
  Edit
  Duplicate
  ---------
  Öffnen
  auto:list1:menu:cut:text = "Ausschneiden"
*/
/// <remarks>
///   Work in Progress
/// </remarks>
public class DropDownMenu: WebControl, IControl, IPostBackEventHandler
{
  //  HACK: EscapeJavaScript will be moved to extra class 
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
 
  private static readonly object s_eventCommandClickEvent = new object();
  private static readonly object s_wxeFunctionCommandClickEvent = new object();

  private const string c_dropDownIcon = "DropDownMenuArrow.gif";

  private string _titleText = "";
  private string _titleIcon = "";
  private bool _isReadOnly = false;
  private bool _enableGrouping = true;
  private string _getSelectionCount = "";

  private MenuItemCollection _menuItems;

	public DropDownMenu (Control ownerControl, Type[] supportedMenuItemTypes)
	{
    _menuItems = new MenuItemCollection (ownerControl, supportedMenuItemTypes);
	}

  public DropDownMenu (Control ownerControl)
    : this (ownerControl, new Type[] {typeof (MenuItem)})
	{
  }

  public DropDownMenu ()
    : this (null, new Type[] {typeof (MenuItem)})
  {
  }

  /// <summary> Implements interface <see cref="IPostBackEventHandler"/>. </summary>
  /// <param name="eventArgument"> &lt;index&gt; </param>
  public void RaisePostBackEvent (string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

    //  First part: index
    int index;
    try 
    {
      if (eventArgument.Length == 0)
        throw new FormatException();
      index = int.Parse (eventArgument);
    }
    catch (FormatException)
    {
      throw new ArgumentException ("First part of argument 'eventArgument' must be an integer. Expected format: '<index>'.");
    }

    if (index >= _menuItems.Count)
      throw new ArgumentOutOfRangeException ("Index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed menu items.'");

    MenuItem item = _menuItems[index];
    if (item.Command == null)
      throw new ArgumentOutOfRangeException ("The DropDownMenu '" + ID + "' does not have a command associated with menu item " + index + ".");

    switch (item.Command.Type)
    {
      case CommandType.Event:
      {
        OnEventCommandClick (item);
        break;
      }
      case CommandType.WxeFunction:
      {
        OnWxeFunctionCommandClick (item);
        break; 
      }
      default:
      {
        break;
      }
    }

  }

  /// <summary> Fires the <see cref="EventCommandClick"/> event. </summary>
  protected virtual void OnEventCommandClick (MenuItem item)
  {
    MenuItemClickEventHandler clickHandler = (MenuItemClickEventHandler) Events[s_eventCommandClickEvent];
    if (clickHandler != null)
    {
      MenuItemClickEventArgs e = new MenuItemClickEventArgs (item);
      clickHandler (this, e);
    }
  }

  /// <summary> Fires the <see cref="WxeFunctionCommandClick"/> event. </summary>
  protected virtual void OnWxeFunctionCommandClick (MenuItem item)
  {
    MenuItemClickEventHandler clickHandler = (MenuItemClickEventHandler) Events[s_wxeFunctionCommandClickEvent];
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

    key = typeof (DropDownMenu).FullName + ID;
    if (! Page.IsStartupScriptRegistered (key))
    {
      StringBuilder script = new StringBuilder();
      script.Append ("DropDownMenu_AddMenuInfo (\r\n\t");
      script.AppendFormat ("new DropDownMenu_MenuInfo ('{0}', new Array (\r\n", ClientID);
      bool isFirstItem = true;

      MenuItem[] menuItems;
      if (_enableGrouping)
        menuItems = _menuItems.GroupMenuItems (true);
      else
        menuItems = _menuItems.ToArray();
      
      for (int i = 0; i < menuItems.Length; i++)
      {
        MenuItem menuItem = menuItems[i];
        if (isFirstItem)
          isFirstItem = false;
        else
          script.AppendFormat (",\r\n");
        AppendMenuItem (script, menuItem, _menuItems.IndexOf (menuItem));
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
          href = Page.GetPostBackClientHyperlink (this, argument);
          //  HACK: EscapeJavaScript will be moved to extra class 
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
    string iconDisabled = (StringUtility.IsNullOrEmpty (menuItem.IconDisabled) ? "null" : "'" +  menuItem.IconDisabled + "'");
    stringBuilder.AppendFormat (
        "\t\tnew DropDownMenu_ItemInfo ('{0}', '{1}', '{2}', {3}, {4}, {5}, {6}, {7}, {8})",
        menuItemIndex.ToString(), 
        menuItem.Category, 
        menuItem.Text, 
        icon, 
        iconDisabled, 
        (int) menuItem.RequiredSelection, 
        menuItem.IsDisabled ? "true" : "false",
        href, 
        target);
  }

  protected override void Render (HtmlTextWriter writer)
  {
    foreach (string key in Style.Keys)
      writer.AddStyleAttribute (key, Style[key]);
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
    string getSelectionCount = (StringUtility.IsNullOrEmpty (_getSelectionCount) ? "null" : _getSelectionCount);
    string script = "DropDownMenu_OnClick (this, '" + ClientID + "', " + getSelectionCount + ");";
    writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
    writer.AddAttribute ("id", ClientID + "_MenuDiv");
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
    writer.AddAttribute ("id", ClientID + "_HeadDiv");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHead);
    writer.AddAttribute ("OnMouseOver", "DropDownMenu_OnHeadMouseOver (this)");
    writer.AddAttribute ("OnMouseOut", "DropDownMenu_OnHeadMouseOut (this)");
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

  [DefaultValue (false)]
  public bool IsReadOnly
  {
    get { return _isReadOnly; }
    set { _isReadOnly = value; }
  }

  [DefaultValue (true)]
  public bool EnableGrouping
  {
    get { return _enableGrouping; }
    set { _enableGrouping = value; }
  }

  [DefaultValue ("")]
  public string GetSelectionCount
  {
    get { return _getSelectionCount; }
    set { _getSelectionCount = value; }
  }

  /// <summary> Occurs when a command of type <see cref="CommandType.Event"/> is clicked. </summary>
  [Category ("Action")]
  [Description ("Occurs when a command of type Event is clicked.")]
  public event MenuItemClickEventHandler EventCommandClick
  {
    add { Events.AddHandler (s_eventCommandClickEvent, value); }
    remove { Events.RemoveHandler (s_eventCommandClickEvent, value); }
  }

  /// <summary> Occurs when a command of type <see cref="CommandType.WxeFunction"/> is clicked. </summary>
  [Category ("Action")]
  [Description ("Occurs when a command of type WxeFunction is clicked.")]
  public event MenuItemClickEventHandler WxeFunctionCommandClick
  {
    add { Events.AddHandler (s_wxeFunctionCommandClickEvent, value); }
    remove { Events.RemoveHandler (s_wxeFunctionCommandClickEvent, value); }
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

  protected virtual string CssClassItemFocus
  { get { return "dropDownMenuItemFocus"; } }

  protected virtual string CssClassItemTextPane
  { get { return "dropDownMenuItemTextPane"; } }

  protected virtual string CssClassItemIconPane
  { get { return "dropDownMenuItemIconPane"; } }
}

}
