using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI.Design;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.Web.UI.Controls
{

public interface IWindowStateManager
{
  object GetData (string key);
  void SetData (string key, object value);
}

[Designer (typeof (TabbedMenuDesigner))]
public class TabbedMenu: WebControl
{
  // constants
  private const string c_styleFileUrl = "TabbedMenu.css";

  // statics
  //private static readonly object s_clickEvent = new object();
  //private static readonly object s_selectionChangedEvent = new object();
  private static readonly string s_styleFileKey = typeof (TabbedMenu).FullName + "_Style";

  // types

  // fields
  private Style _mainMenuStyle;
  private Style _subMenuStyle;
  private Style _statusStyle;
  private WebTabStrip _mainMenuTabStrip;
  private WebTabStrip _subMenuTabStrip;
  private bool _isSubMenuTabStripRefreshed;
  private bool _isPastInitialization;

  protected WebTabStrip MainMenuTabStrip
  {
    get { return _mainMenuTabStrip; }
  }

  protected WebTabStrip SubMenuTabStrip
  {
    get { return _subMenuTabStrip; }
  }


  // construction and destruction
  public TabbedMenu()
  {
    _mainMenuTabStrip = new WebTabStrip (this, new Type[] {typeof (MainMenuTab)});
    _subMenuTabStrip = new WebTabStrip (this);
    _mainMenuStyle = new Style();
    _subMenuStyle = new Style();
    _statusStyle = new Style();
  }

  // methods and properties

  protected override void OnInit(EventArgs e)
  {
    EnsureChildControls();
    base.OnInit (e);
    _isPastInitialization = true;
    _mainMenuTabStrip.EnableSelectedTab = true;
    _mainMenuTabStrip.EnableViewState = false;
    _subMenuTabStrip.EnableSelectedTab = true;
    _subMenuTabStrip.EnableViewState = false;

    LoadSelectedTabs ();
  }

  protected virtual string SelectedTabIDsID
  {
    get { return "TabbedMenuSelected"; }
  }

  private void LoadSelectedTabs ()
  {
    string[] selectedTabIDs = GetSelectedTabIDsFromWindowState();
    if (selectedTabIDs == null)
      selectedTabIDs = GetSelectedTabIDsFromQueryString();
    if (selectedTabIDs == null)
      selectedTabIDs = new string[0];

    if (selectedTabIDs.Length > 0)
    {
      string selectedMainMenuItemID = selectedTabIDs[0];
      WebTab selectedMainMenuItem = _mainMenuTabStrip.Tabs.Find (selectedMainMenuItemID);
      selectedMainMenuItem.IsSelected = true;
    }
    RefreshSubMenuTabStrip (true);
    if (selectedTabIDs.Length > 1)
    {
      string selectedSubMenuItemID = selectedTabIDs[1];
      WebTab selectedSubMenuItem = _subMenuTabStrip.Tabs.Find (selectedSubMenuItemID);
      selectedSubMenuItem.IsSelected = true;
    }
  }

  private string[] GetSelectedTabIDsFromQueryString()
  {
    string value;
    if (Page is IWxePage)
    {
      value = PageUtility.GetUrlParameter (WxeContext.Current.QueryString, SelectedTabIDsID);
      value = System.Web.HttpUtility.UrlDecode (value, System.Web.HttpContext.Current.Response.ContentEncoding);
    }
    else
    {
      value = Context.Request.QueryString[SelectedTabIDsID];
    }
    if (value != null)
      return (string[]) TypeConversionServices.Current.Convert (typeof (string), typeof (string[]), value);
    else
      return null;
  }

  private string[] GetSelectedTabIDsFromWindowState()
  {
    IWindowStateManager windowStateManager = Page as IWindowStateManager;
    if (windowStateManager != null)
      return (string[]) windowStateManager.GetData (SelectedTabIDsID);
    else
      return null;
  }

  private void SaveSelectedTabs()
  {
    SaveSelectedTabsInWindowState();
  }

  private void SaveSelectedTabsInWindowState()
  {
    IWindowStateManager windowStateManager = Page as IWindowStateManager;
    if (windowStateManager == null)
      return;

    string[] tabIDs = GetTabIDs (
        (MainMenuTab) _mainMenuTabStrip.SelectedTab, 
        (SubMenuTab) _subMenuTabStrip.SelectedTab);
    windowStateManager.SetData (SelectedTabIDsID, tabIDs);
  }

  public string AddSelectedTabsToUrl (string url)
  {
    string[] tabIDs = GetTabIDs (
        (MainMenuTab) _mainMenuTabStrip.SelectedTab, 
        (SubMenuTab) _subMenuTabStrip.SelectedTab);

    string value = (string) TypeConversionServices.Current.Convert (typeof (string[]), typeof (string), tabIDs);
    return PageUtility.AddUrlParameter (url, SelectedTabIDsID, value);
  }

  public string AddTabsToUrl (string url, MenuTab menuItem)
  {
    ArgumentUtility.CheckNotNull ("menuItem", menuItem);
    string[] tabIDs;
    if (menuItem is MainMenuTab)
    {
      tabIDs = GetTabIDs ((MainMenuTab) menuItem, null);
    }
    else
    {
      SubMenuTab subMenuItem = (SubMenuTab) menuItem;
      tabIDs = GetTabIDs (subMenuItem.Parent, subMenuItem);
    }

    string value = (string) TypeConversionServices.Current.Convert (typeof (string[]), typeof (string), tabIDs);
    return PageUtility.AddUrlParameter (url, SelectedTabIDsID, value);
  }

  private string[] GetTabIDs (MainMenuTab mainMenuItem, SubMenuTab subMenuItem)
  {
    string[] tabIDs = new string[2];
    if (mainMenuItem == null)
    {
      tabIDs = new string[0];
    }
    else if (subMenuItem == null)
    {
      tabIDs = new string[1];
      tabIDs[0] = mainMenuItem.ItemID;
    }
    else
    {
      tabIDs = new string[2];
      tabIDs[0] = mainMenuItem.ItemID;
      tabIDs[1] = subMenuItem.ItemID;
    }
    return tabIDs;
  }

  private void EnsureSubMenuTabStripRefreshed ()
  {
    if (_isSubMenuTabStripRefreshed)
      return;
    RefreshSubMenuTabStrip (false);
  }

  internal void RefreshSubMenuTabStrip (bool resetSubMenu)
  {
    _isSubMenuTabStripRefreshed = true;
    if (resetSubMenu)
      _subMenuTabStrip.Tabs.Clear();
    MainMenuTab selectedMainMenuItem = (MainMenuTab) _mainMenuTabStrip.SelectedTab;
    if (selectedMainMenuItem != null)
      _subMenuTabStrip.Tabs.AddRange (selectedMainMenuItem.SubMenuTabs);
    if (_subMenuTabStrip.SelectedTab == null && _subMenuTabStrip.Tabs.Count > 0)
      _subMenuTabStrip.SetSelectedTab (_subMenuTabStrip.Tabs[0]);
  }

  protected override void CreateChildControls()
  {
    _mainMenuTabStrip.ID = ID + "_MainMenuTabStrip";
    Controls.Add (_mainMenuTabStrip);

    _subMenuTabStrip.ID = ID + "_SubMenuTabStrip";
    Controls.Add (_subMenuTabStrip);
  }

  protected override void OnPreRender(EventArgs e)
  {
    EnsureSubMenuTabStripRefreshed ();

    base.OnPreRender (e);

    string url = null;
    if (! HtmlHeadAppender.Current.IsRegistered (s_styleFileKey))
    {
      url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (TabbedMenu), ResourceType.Html, c_styleFileUrl);
      HtmlHeadAppender.Current.RegisterStylesheetLink (s_styleFileKey, url, HtmlHeadAppender.Priority.Library);
    }

    string value = AddSelectedTabsToUrl ("?");
    SaveSelectedTabs();
  }

  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Table; }
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
  }

  protected override void RenderContents (HtmlTextWriter writer)
  {
    EnsureChildControls();
   
    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
      WcagHelper.Instance.HandleError (1, this);

    writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin main menu row

    writer.AddAttribute (HtmlTextWriterAttribute.Colspan, "2");
    writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassMainMenuCell);
    writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin main menu cell
    _mainMenuTabStrip.CssClass = CssClassMainMenu;
    _mainMenuTabStrip.Width = Unit.Percentage (100);
    _mainMenuTabStrip.RenderControl (writer);
    writer.RenderEndTag(); // End main menu cell

    writer.RenderEndTag(); // End main menu row

    writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin sub menu row

    writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassSubMenuCell);
    writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin sub menu cell
    _subMenuTabStrip.Style["width"] = "auto";
    _subMenuTabStrip.CssClass = CssClassSubMenu;
    _subMenuTabStrip.RenderControl (writer);
    writer.RenderEndTag(); // End sub menu cell

    writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassStatusCell);
    writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin status cell
    writer.Write ("Tabbed Menu");
    writer.RenderEndTag(); // End status cell

    writer.RenderEndTag(); // End sub menu row
  }

  public override ControlCollection Controls
  {
    get
    {
      EnsureChildControls();
      return base.Controls;
    }
  }


  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Behavior")]
  [Description ("")]
  [DefaultValue ((string) null)]
  [Editor (typeof (MainMenuTabCollectionEditor), typeof (UITypeEditor))]
  public WebTabCollection Tabs
  {
    get
    {
      if (_isPastInitialization)
        EnsureSubMenuTabStripRefreshed ();
      return _mainMenuTabStrip.Tabs;
    }
  }

//  [Category ("Style")]
//  [Description ("The style that you want to apply to the main menu.")]
//  [NotifyParentProperty (true)]
//  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
//  [PersistenceMode (PersistenceMode.InnerProperty)]
//  public Style MainMenuStyle
//  {
//    get { return _mainMenuStyle; }
//  }
//
//  [Category ("Style")]
//  [Description ("The style that you want to apply to the sub menu.")]
//  [NotifyParentProperty (true)]
//  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
//  [PersistenceMode (PersistenceMode.InnerProperty)]
//  public Style SubMenuStyle
//  {
//    get { return _subMenuStyle; }
//  }
//
//  [Category ("Style")]
//  [Description ("The style that you want to apply to the status area.")]
//  [NotifyParentProperty (true)]
//  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
//  [PersistenceMode (PersistenceMode.InnerProperty)]
//  public Style StatusStyle
//  {
//    get { return _statusStyle; }
//  }

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the main menu's tab strip. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabbedMainMenu</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="MainMenuStyle"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassMainMenu
  {
    get { return "tabbedMainMenu"; }
  }

  /// <summary> Gets the CSS-Class applied to the sub menu's tab strip. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabbedSubMenu</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="SubMenuStyle"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassSubMenu
  {
    get { return "tabbedSubMenu"; }
  }

  /// <summary> Gets the CSS-Class applied to the main menu cell. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabbedMainMenuCell</c>. </para>
  /// </remarks>
  protected virtual string CssClassMainMenuCell
  {
    get { return "tabbedMainMenuCell"; }
  }

  /// <summary> Gets the CSS-Class applied to the sub menu cell. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabbedSubMenuCell</c>. </para>
  /// </remarks>
  protected virtual string CssClassSubMenuCell
  {
    get { return "tabbedSubMenuCell"; }
  }

  /// <summary> Gets the CSS-Class applied to the status cell. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabbedMenuStatusCell</c>. </para>
  /// </remarks>
  protected virtual string CssClassStatusCell
  {
    get { return "tabbedMenuStatusCell"; }
  }
  #endregion
}
}
