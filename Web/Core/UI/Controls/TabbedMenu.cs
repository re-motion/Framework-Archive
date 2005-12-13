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
public class TabbedMenu: WebControl, IControl
{
  // constants
  private const string c_styleFileUrl = "TabbedMenu.css";

  // statics
  private static readonly string s_styleFileKey = typeof (TabbedMenu).FullName + "_Style";
  private static readonly object s_eventCommandClickEvent = new object();

  // types

  // fields
  private Style _statusStyle;
  private WebTabStrip _mainMenuTabStrip;
  private WebTabStrip _subMenuTabStrip;
  private string _statusText;
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
    _mainMenuTabStrip = new WebTabStrip (new MainMenuTabCollection (this, new Type[] {typeof (MainMenuTab)}));
    _subMenuTabStrip = new WebTabStrip (this, new Type[] {typeof (SubMenuTab)});
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
    _mainMenuTabStrip.Click += new WebTabClickEventHandler (MainMenuTabStrip_Click);
    _subMenuTabStrip.EnableSelectedTab = true;
    _subMenuTabStrip.EnableViewState = false;
    _subMenuTabStrip.Click += new WebTabClickEventHandler (SubMenuTabStrip_Click);

    LoadSelectedTabs ();
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual string SelectedTabIDsID
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
      if (selectedMainMenuItem.IsVisible)
        selectedMainMenuItem.IsSelected = true;
    }
    RefreshSubMenuTabStrip (true);
    if (selectedTabIDs.Length > 1)
    {
      string selectedSubMenuItemID = selectedTabIDs[1];
      WebTab selectedSubMenuItem = _subMenuTabStrip.Tabs.Find (selectedSubMenuItemID);
      if (selectedSubMenuItem.IsVisible)
        selectedSubMenuItem.IsSelected = true;
    }
  }

  private string[] GetSelectedTabIDsFromQueryString()
  {
    if (ControlHelper.IsDesignMode (this, Context))
      return null;

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
    if (ControlHelper.IsDesignMode (this, Context))
      return null;

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

  /// <summary> Gets the parameters required for selecting the <paramref name="menuTab"/>. </summary>
  /// <param name="menuTab"> 
  ///   The <see cref="MenuTab"/> that should be selected by the when using the returned URL parameters. 
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <returns> 
  ///   A <see cref="NameValueCollection"/> that contains the URL parameters parameters required by this 
  ///   <see cref="TabbedMenu"/>.
  /// </returns>
  public NameValueCollection GetUrlParameters (MenuTab menuTab)
  {
    ArgumentUtility.CheckNotNull ("menuTab", menuTab);

    MainMenuTab mainMenuTab = menuTab as MainMenuTab;
    SubMenuTab subMenuTab = menuTab as SubMenuTab;
    
    string[] tabIDs;
    if (mainMenuTab != null)
      tabIDs = GetTabIDs (mainMenuTab, null);
    else
      tabIDs = GetTabIDs (subMenuTab.Parent, subMenuTab);

    string value = (string) TypeConversionServices.Current.Convert (typeof (string[]), typeof (string), tabIDs);

    NameValueCollection urlParameters = new NameValueCollection();
    urlParameters.Add (SelectedTabIDsID, value);
    return urlParameters;
  }

  /// <summary> 
  ///   Adds parameters required for re-selecting the currently selected <see cref="MenuTab"/> to the 
  ///   <paramref name="url"/>. 
  /// </summary>
  /// <param name="url"> The URL. Must not be <see langword="null"/> or empty. </param>
  /// <returns> The <paramref name="url"/> extended with the parameters required by this <see cref="TabbedMenu"/>. </returns>
  public string FormatUrl (string url)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("url", url);

    if (_subMenuTabStrip.SelectedTab != null)
      return FormatUrl (url, (SubMenuTab) _subMenuTabStrip.SelectedTab);
    else if (_mainMenuTabStrip.SelectedTab != null)
      return FormatUrl (url, (MainMenuTab) _mainMenuTabStrip.SelectedTab);
    else
      return url;
  }

  /// <summary> 
  ///   Adds the parameters required for selecting the <paramref name="menuTab"/> to the <paramref name="url"/>.
  /// </summary>
  /// <param name="url"> The URL. Must not be <see langword="null"/> or empty. </param>
  /// <param name="menuTab"> 
  ///   The <see cref="MenuTab"/> that should be selected by the <paramref name="url"/>. 
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <returns> The <paramref name="url"/> extended with the parameters required by this <see cref="TabbedMenu"/>. </returns>
  public string FormatUrl (string url, MenuTab menuTab)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("url", url);
    
    NameValueCollection urlParameters = GetUrlParameters (menuTab);
    for (int i = 0; i < urlParameters.Count; i++)
      url = PageUtility.AddUrlParameter (url, urlParameters.GetKey(i), urlParameters.Get(i));
  
    return url;
  }

  private string[] GetTabIDs (MainMenuTab mainMenuTab, SubMenuTab subMenuTab)
  {
    string[] tabIDs = new string[2];
    if (mainMenuTab == null)
    {
      tabIDs = new string[0];
    }
    else if (subMenuTab == null)
    {
      tabIDs = new string[1];
      tabIDs[0] = mainMenuTab.ItemID;
    }
    else
    {
      tabIDs = new string[2];
      tabIDs[0] = mainMenuTab.ItemID;
      tabIDs[1] = subMenuTab.ItemID;
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
      _subMenuTabStrip.SetSelectedTabInternal (_subMenuTabStrip.Tabs[0]);
  }

  protected override void CreateChildControls()
  {
    _mainMenuTabStrip.ID = ID + "_MainMenuTabStrip";
    Controls.Add (_mainMenuTabStrip);

    _subMenuTabStrip.ID = ID + "_SubMenuTabStrip";
    Controls.Add (_subMenuTabStrip);
  }

  /// <summary> 
  ///   Event handler for the <see cref="WebTabStrip.Click"/> of the <see cref="MainMenuTabStrip"/>.
  /// </summary>
  private void MainMenuTabStrip_Click (object sender, WebTabClickEventArgs e)
  {
    OnEventCommandClick ((MenuTab) e.Tab);
  }

  /// <summary> 
  ///   Event handler for the <see cref="WebTabStrip.Click"/> of the <see cref="SubMenuTabStrip"/>.
  /// </summary>
  private void SubMenuTabStrip_Click (object sender, WebTabClickEventArgs e)
  {
    OnEventCommandClick ((MenuTab) e.Tab);
  }

  private void HandleTabStripClick (MenuTab tab)
  {
    if (tab != null && tab.Command != null)
    {
      if (tab.Command.Type == CommandType.Event)
      {
        OnEventCommandClick (tab);
      }
      else if (tab.Command.Type == CommandType.WxeFunction)
      {
        if (Page is IWxePage)
          ExecuteWxeFunction ((IWxePage) Page, tab);
        else
          ExecuteWxeFunction (tab);
      }
    }
  }

  protected virtual void OnEventCommandClick (MenuTab tab)
  {
    if (tab != null && tab.Command != null)
      tab.Command.OnClick();

    MenuTabClickEventHandler handler = (MenuTabClickEventHandler) Events[s_eventCommandClickEvent];
    if (handler != null)
    {
      MenuTabClickEventArgs e = new MenuTabClickEventArgs (tab);
      handler (this, e);
    }
  }

  /// <summary> 
  ///   Executes the Wxe Function associated with the <paramref name="tab"/>. Used when the page is an 
  ///   <see cref="IWxePage"/>. 
  /// </summary>
  /// <exception cref="InvalidOperationException">
  ///   If called while the <see cref="Command.Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  /// </exception> 
  protected virtual void ExecuteWxeFunction (IWxePage page, MenuTab tab)
  {
    ArgumentUtility.CheckNotNull ("page", page);
    ArgumentUtility.CheckNotNull ("tab", tab);
    ArgumentUtility.CheckNotNull ("tab.Command", tab.Command);

    tab.Command.ExecuteWxeFunction (page, null);
  }

  /// <summary> 
  ///   Executes the Wxe Function associated with the <paramref name="tab"/>. Used when the page is not an 
  ///   <see cref="IWxePage"/>. 
  /// </summary>
  /// <exception cref="InvalidOperationException">
  ///   If called while the <see cref="Command.Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  /// </exception> 
  protected virtual void ExecuteWxeFunction (MenuTab tab)
  {
    ArgumentUtility.CheckNotNull ("tab", tab);
    ArgumentUtility.CheckNotNull ("tab.Command", tab.Command);
    if (tab.Command.Type != CommandType.WxeFunction)
      throw new InvalidOperationException ("Call to ExecuteWxeFunction not allowed unless tab.Command.Type is set to CommandType.WxeFunction.");

    string target = tab.Command.WxeFunctionCommand.Target;
    bool hasTarget = ! StringUtility.IsNullOrEmpty (target);

    NameValueCollection additionalUrlParameters = GetUrlParameters (tab);
    if (hasTarget)
    {
      string returnUrl = "javascript:window.close();";
      additionalUrlParameters.Add (WxeHandler.Parameters.ReturnUrl, returnUrl);
    }
    string url = tab.Command.GetWxeFunctionPermanentUrl (additionalUrlParameters);


    if (hasTarget)
    {
      string openScript = string.Format ("window.open('{0}', '{1}');", url, target);
      PageUtility.RegisterStartupScriptBlock (Page, "WxeExecuteFunction", openScript);
    }
    else
    {
      PageUtility.Redirect (Page.Response, url);
    }
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
    SaveSelectedTabs();
  }

  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Table; }
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);
    if (ControlHelper.IsDesignMode (this, Context))
    {
      writer.AddStyleAttribute ("width", "100%");
    }
    if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);
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

    _statusStyle.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (_statusStyle.CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassStatusCell);
    writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin status cell
    if (! StringUtility.IsNullOrEmpty (_statusText))
      writer.Write (_statusText);
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


  /// <summary> Gets the collection of <see cref="MainMenuTabs"/>. </summary>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Description ("")]
  [DefaultValue ((string) null)]
  [Editor (typeof (MainMenuTabCollectionEditor), typeof (UITypeEditor))]
  public MainMenuTabCollection Tabs
  {
    get
    {
      if (_isPastInitialization)
        EnsureSubMenuTabStripRefreshed ();
      return (MainMenuTabCollection) _mainMenuTabStrip.Tabs;
    }
  }

  /// <summary> Is raised when a tab with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
  [Category ("Action")]
  [Description ("Is raised when a tab with a command of type Event is clicked.")]
  public event MenuTabClickEventHandler EventCommandClick
  {
    add { Events.AddHandler (s_eventCommandClickEvent, value); }
    remove { Events.RemoveHandler (s_eventCommandClickEvent, value); }
  }

  /// <summary> Gets or sets the text displayed in the status area. </summary>
  [Description ("The text displayed in the status area.")]
  [DefaultValue ("")]
  public string StatusText
  {
    get { return _statusText; }
    set { _statusText = value; }
  }

  /// <summary> Gets the style applied to the status area. </summary>
  [Category ("Style")]
  [Description ("The style applied to the status area.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style StatusStyle
  {
    get { return _statusStyle; }
  }

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="WebTabStrip"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStrip</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  {
    get { return "tabbedMenu"; }
  }

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
