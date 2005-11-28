using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.UI.Design;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UI.Controls
{

/// <include file='doc\include\UI\Controls\WebTabStrip.xml' path='WebTabStrip/Class/*' />
[ToolboxData("<{0}:WebTabStrip runat=server></{0}:WebTabStrip>")]
[Designer (typeof (WebControlDesigner))]
public class WebTabStrip : WebControl, IControl, IPostBackDataHandler, IResourceDispatchTarget, IControlWithDesignTimeSupport
{
  //  constants
  /// <summary> The key identifying a tab resource entry. </summary>
  private const string c_resourceKeyTabs = "Tabs";

  // statics
  private static readonly object s_selectedIndexChangedEvent = new object();
  /// <summary> The log4net logger. </summary>
  private static readonly log4net.ILog s_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  // types

  // fields
  private WebTabCollection _tabs;
  private WebTab _selectedTab;
  private string _selectedItemID;
  private string _tabToBeSelected = null;
  private bool _hasTabsRestored;
  private bool _isRestoringTabs;
  private object _tabsControlState;
  private Style _tabsPaneStyle;
  private Style _separatorStyle;
  private Style _tabStyle;
  private Style _tabSelectedStyle;
  private Control _ownerControl;
  private bool _enableSelectedTab = false;

  public WebTabStrip (Control ownerControl, Type[] supportedMenuItemTypes)
  {
    _ownerControl = ownerControl;
    _tabs = new WebTabCollection (ownerControl, supportedMenuItemTypes);
    Tabs.SetTabStrip (this);
    _tabsPaneStyle = new Style();
    _tabStyle = new Style();
    _tabSelectedStyle = new Style();
    _separatorStyle = new Style();
  }

  public WebTabStrip (Control ownerControl)
    : this (ownerControl, new Type[] {typeof (WebTab)})
  {}

  public WebTabStrip()
    :this (null)
  {
  }

  bool IPostBackDataHandler.LoadPostData (string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
  {
    //  Is PostBack caused by this tab strip ?
    if (postCollection[ControlHelper.PostEventSourceID] == UniqueID)
    {
      _tabToBeSelected = postCollection[ControlHelper.PostEventArgumentID ];
      ArgumentUtility.CheckNotNullOrEmpty ("postCollection[\"__EVENTARGUMENT\"]", _tabToBeSelected);
      if (_tabToBeSelected != _selectedItemID)
        return true;
    }
    return false;
  }

  void IPostBackDataHandler.RaisePostDataChangedEvent()
  {
    EnsureTabsRestored();
    HandleSelectionChangeEvent (_tabToBeSelected);
  }

  /// <summary> Handles the click event for a tab. </summary>
  /// <param name="itemID"> The id of the tab. </param>
  private void HandleSelectionChangeEvent (string itemID)
  {
    WebTab currentTab = _selectedTab;
    SetSelectedTab (itemID);
    if (currentTab != _selectedTab)
      OnSelectedIndexChanged();
  }

  protected virtual void OnSelectedIndexChanged()
  {
    if (_selectedTab != null)
      _selectedTab.OnSelectionChanged();

    EventHandler handler = (EventHandler) Events[s_selectedIndexChangedEvent];
    if (handler != null)
      handler (this, EventArgs.Empty);
  }

  internal void EnsureTabsRestored()
  {
    if (_hasTabsRestored)
      return;
 
    _isRestoringTabs = true;
    if (_tabsControlState != null)
    {
      LoadTabsControlState (_tabsControlState, _tabs);
      _hasTabsRestored = true;
    }
    _isRestoringTabs = false;
  }

  protected override void LoadViewState(object savedState)
  {
    if (savedState != null)
    {
      object[] values = (object[]) savedState;
      base.LoadViewState(values[0]);
      _tabsControlState = values[1];
      _selectedItemID = (string) values[2];
    }
  }

  protected override object SaveViewState()
  {
    object[] values = new object[3];
    values[0] = base.SaveViewState();
    values[1] = SaveTabsControlState (_tabs);
    values[2] = _selectedItemID;
    return values;
  }

  /// <summary> Loads the settings of the <paramref name="tabs"/> from <paramref name="tabsControlState"/>. </summary>
  private void LoadTabsControlState (object tabsControlState, WebTabCollection tabs)
  {
    ((IControlStateManager) tabs).LoadControlState (tabsControlState);
  }

  /// <summary> Saves the settings of the  <paramref name="tabs"/> and returns this view state </summary>
  private object SaveTabsControlState (WebTabCollection tabs)
  {
    EnsureTabsRestored();
    return ((IControlStateManager) tabs).SaveControlState();
  }
  
  protected override void OnPreRender(EventArgs e)
  {
    string key = typeof (WebTabStrip).FullName + "_Style";
    string styleSheetUrl = null;
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (WebTabStrip), ResourceType.Html, "TabStrip.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, styleSheetUrl, HtmlHeadAppender.Priority.Library);
    }

   if (! ControlHelper.IsDesignMode ((IControl)this, Context) && Enabled)
      Page.RegisterRequiresPostBack (this);

    EnsureTabsRestored();
    
    base.OnPreRender (e);
  
    IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager (this, true);
    LoadResources (resourceManager);
  }

  /// <summary> Calls <see cref="Control.OnPreRender"/> on every invocation. </summary>
  /// <remarks> Used by the <see cref="WebControlDesigner"/>. </remarks>
  void IControlWithDesignTimeSupport.PreRenderForDesignMode()
  {
    if (! ControlHelper.IsDesignMode (this, Context))
      throw new InvalidOperationException ("PreRenderChildControlsForDesignMode may only be called during design time.");
    EnsureChildControls();
    OnPreRender (EventArgs.Empty);
  }

  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Table; }
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {  
    base.AddAttributesToRender (writer);
    
    if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
  }

  protected override void RenderContents (HtmlTextWriter writer)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);

    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
      WcagHelper.Instance.HandleError (1, this);

    WebTabCollection tabs = Tabs;
    
    if (   ControlHelper.IsDesignMode (this, Context)
        && tabs.Count == 0)
    {
      tabs = GetDesignTimeTabs(); 
    }

    RenderBeginTabsPane (writer);
    for (int i = 0; i < tabs.Count; i++)
    {
      WebTab tab = tabs[i];
      RenderTab (writer, tab);
    }
    RenderEndTabsPane (writer);
  }

  private WebTabCollection GetDesignTimeTabs()
  {
    WebTabCollection tabs = new WebTabCollection (null);
    for (int i = 0; i < 5; i++)
      tabs.Add (new WebTab (i.ToString(), "Tab " + (i + 1).ToString())); 
    return tabs;
  }

  private void RenderBeginTabsPane (HtmlTextWriter writer)
  {
    writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin Table Row
    writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin Table Cell

    bool isEmpty = Tabs.Count == 0;

    string backUpCssClass = _tabsPaneStyle.CssClass;
    if (isEmpty && ! StringUtility.IsNullOrEmpty (_tabsPaneStyle.CssClass))
      _tabsPaneStyle.CssClass += " " + CssClassTabsPaneEmpty;
    
    _tabsPaneStyle.AddAttributesToRender (writer);

    if (isEmpty && ! StringUtility.IsNullOrEmpty (_tabsPaneStyle.CssClass))
      _tabsPaneStyle.CssClass = backUpCssClass;
    
    if (StringUtility.IsNullOrEmpty (_tabsPaneStyle.CssClass))
    {
      string cssClass = CssClassTabsPane;
      if (isEmpty)
        cssClass += " " + CssClassTabsPaneEmpty;
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
    }

    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Div

    if (ControlHelper.IsDesignMode (this, Context))
    {
      writer.AddStyleAttribute ("list-style", "none");
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.AddStyleAttribute ("display", "inline");
    }
    if (isEmpty)
      writer.Write ("&nbsp;");
    writer.RenderBeginTag (HtmlTextWriterTag.Ul); // Begin List
  }

  private void RenderEndTabsPane (HtmlTextWriter writer)
  {
    writer.RenderEndTag(); // End List
    writer.RenderEndTag(); // End Div
    writer.RenderEndTag(); // End Table Cell
    writer.RenderEndTag(); // End Table Row
  }

  private string GetHref (WebTab tab)
  {
    if (Page != null)
      return Page.GetPostBackClientHyperlink (this, tab.ItemID);
    else if (ControlHelper.IsDesignMode ((Control) this))
      return "#";
    else
      throw new InvalidOperationException (string.Format ("WebTabStrip '{0}' is not part of a page."));
  }

  private void RenderTab (HtmlTextWriter writer, WebTab tab)
  {
    if (ControlHelper.IsDesignMode (this, Context))
    {
      writer.AddStyleAttribute ("float", "left");
      writer.AddStyleAttribute ("display", "block");
      writer.AddStyleAttribute ("white-space", "nowrap");
    }

    writer.RenderBeginTag (HtmlTextWriterTag.Li); // Begin list item
    
    RenderSeperator (writer);

    if (tab.IsSelected)
    {
      _tabSelectedStyle.AddAttributesToRender (writer);
      if (StringUtility.IsNullOrEmpty (_tabSelectedStyle.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabSelected);
    }
    else
    {
      _tabStyle.AddAttributesToRender (writer);
      if (StringUtility.IsNullOrEmpty (_tabStyle.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTab);
    }
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin tab span

    if (! tab.IsSelected || _enableSelectedTab)
      writer.AddAttribute (HtmlTextWriterAttribute.Href, GetHref (tab));
    writer.RenderBeginTag (HtmlTextWriterTag.A); // Begin anchor
    
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabTopBorder);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin top border span
 
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabBottomBorder);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin bottom border span

    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabLeftBorder);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin left border span
 
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabRightBorder);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin right border span

    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabTopLeftCorner);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin top left corner span
 
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabBottomLeftCorner);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin bottom left corner span

    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabTopRightCorner);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin top right corner span
 
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabBottomRightCorner);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin bottom right corner span

    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabContent);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin content span

    tab.RenderContents (writer);

    writer.RenderEndTag(); // End content span
    writer.RenderEndTag(); // End bottom right border span
    writer.RenderEndTag(); // End top right border span
    writer.RenderEndTag(); // End bottom left border span
    writer.RenderEndTag(); // End top left border span
    writer.RenderEndTag(); // End right border span
    writer.RenderEndTag(); // End left border span
    writer.RenderEndTag(); // End bottom border span
    writer.RenderEndTag(); // End left border span
    writer.RenderEndTag(); // End anchor

    writer.RenderEndTag(); // End tab span

    writer.RenderEndTag(); // End list item
    writer.WriteLine();
  }

  private void RenderSeperator (HtmlTextWriter writer)
  {
    _separatorStyle.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (_separatorStyle.CssClass))
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassSeparator);
    writer.RenderBeginTag (HtmlTextWriterTag.Span);
    writer.RenderBeginTag (HtmlTextWriterTag.Span);
    writer.Write ("&nbsp;");
    writer.RenderEndTag();
    writer.RenderEndTag();
  }

  /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
  /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
  void IResourceDispatchTarget.Dispatch (IDictionary values)
  {
    ArgumentUtility.CheckNotNull ("values", values);
    Dispatch (values);
  }

  /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
  /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
  protected virtual void Dispatch (IDictionary values)
  {
    HybridDictionary tabValues = new HybridDictionary();
    HybridDictionary propertyValues = new HybridDictionary();

    //  Parse the values

    foreach (DictionaryEntry entry in values)
    {
      string key = (string) entry.Key;
      string[] keyParts = key.Split (new Char[] {':'}, 3);

      //  Is a property/value entry?
      if (keyParts.Length == 1)
      {
        string property = keyParts[0];
        propertyValues.Add (property, entry.Value);
      }
        //  Is collection entry?
      else if (keyParts.Length == 3)
      {    
        //  Compound key: "collectionID:elementID:property"
        string collectionID = keyParts[0];
        string elementID = keyParts[1];
        string property = keyParts[2];

        IDictionary currentCollection = null;

        //  Switch to the right collection
        switch (collectionID)
        {
          case c_resourceKeyTabs:
          {
            currentCollection = tabValues;
            break;
          }
          default:
          {
            //  Invalid collection property
            s_log.Warn ("WebTabStrip '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain a collection property named '" + collectionID + "'.");
            break;
          }
        }       

        //  Add the property/value pair to the collection
        if (currentCollection != null)
        {
          //  Get the dictonary for the current element
          IDictionary elementValues = (IDictionary) currentCollection[elementID];

          //  If no dictonary exists, create it and insert it into the elements hashtable.
          if (elementValues == null)
          {
            elementValues = new HybridDictionary();
            currentCollection[elementID] = elementValues;
          }

          //  Insert the argument and resource's value into the dictonary for the specified element.
          elementValues.Add (property, entry.Value);
        }
      }
      else
      {
        //  Not supported format or invalid property
        s_log.Warn ("WebTabStrip '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' received a resource with an invalid or unknown key '" + key + "'. Required format: 'property' or 'collectionID:elementID:property'.");
      }
    }

    //  Dispatch simple properties
    ResourceDispatcher.DispatchGeneric (this, propertyValues);

    //  Dispatch to collections
    Tabs.Dispatch (tabValues, this, "Tabs");
  }

  /// <summary> Loads the resources into the control's properties. </summary>
  protected virtual void LoadResources (IResourceManager resourceManager)
  {
    if (resourceManager == null)
      return;

    if (Rubicon.Web.Utilities.ControlHelper.IsDesignMode ((Control) this))
      return;
    Tabs.LoadResources (resourceManager);
  }

  /// <summary> Sets the selected tab. </summary>
  internal void SetSelectedTab (WebTab tab)
  {
    if (! _isRestoringTabs)
      EnsureTabsRestored();

    if (tab != null && tab.TabStrip != this)
      throw new InvalidOperationException ("Only tabs that are part of this tab strip can be selected.");
    if (_selectedTab != tab)
    {
      if ((_selectedTab != null) && _selectedTab.IsSelected)
        _selectedTab.SetSelected (false);
      _selectedTab = tab;
      if ((_selectedTab != null) && ! _selectedTab.IsSelected)
        _selectedTab.SetSelected (true);
      
      if (_selectedTab == null)
        _selectedItemID = null;
      else
        _selectedItemID = _selectedTab.ItemID;
    }
  }

  private void SetSelectedTab (string itemID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);
    if (_selectedTab == null || _selectedTab.ItemID != itemID)
    {
      WebTab tab = Tabs.Find (itemID);
      if (tab != _selectedTab)
        SetSelectedTab (tab);
    }
  }

  /// <summary> Gets the currently selected tab. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public WebTab SelectedTab
  {
    get
    { 
      if (Tabs.Count > 0)
      {
        EnsureTabsRestored();
        if (! StringUtility.IsNullOrEmpty (_tabToBeSelected))
          SetSelectedTab (_tabToBeSelected);
      }
      return _selectedTab; 
    }
  }

  /// <summary> Gets the tabs displayed by this tab strip. </summary>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [MergableProperty (false)]
  //  Default category
  [Description ("The tabs displayed by this tab strip.")]
  [DefaultValue ((string) null)]
  public virtual WebTabCollection Tabs
  {
    get { return _tabs; }
  }

  [Description ("Determines whether to enable the selected tab.")]
  [DefaultValue (false)]
  public bool EnableSelectedTab
  {
    get { return _enableSelectedTab; }
    set { _enableSelectedTab = value; }
  }

  [Category ("Style")]
  [Description ("The style that you want to apply to a pane of tabs.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style TabsPaneStyle
  {
    get { return _tabsPaneStyle; }
  }

  [Category ("Style")]
  [Description ("The style that you want to apply to a tab that is neither selected nor a separator.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style TabStyle
  {
    get { return _tabStyle; }
  }

  [Category ("Style")]
  [Description ("The style that you want to apply to the selected tab.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style TabSelectedStyle
  {
    get { return _tabSelectedStyle; }
  }

  [Category ("Style")]
  [Description ("The style that you want to apply to the separators.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style SeparatorStyle
  {
    get { return _separatorStyle; }
  }

  /// <summary> Occurs when a node is clicked. </summary>
  [Category ("Action")]
  [Description ("Occurs when the selected tab has been changed.")]
  public event EventHandler SelectedIndexChanged
  {
    add { Events.AddHandler (s_selectedIndexChangedEvent, value); }
    remove { Events.RemoveHandler (s_selectedIndexChangedEvent, value); }
  }

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="WebTabStrip"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStrip</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  {
    get { return "tabStrip"; }
  }

  /// <summary> Gets the CSS-Class applied to a pane of <see cref="WebTab"/> items. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripTabsPane</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="TabsPaneStyle"/>. </para>
  /// </remarks>
  protected virtual string CssClassTabsPane
  {
    get { return "tabStripTabsPane"; }
  }

  /// <summary> Gets the CSS-Class applied to a pane of <see cref="WebTab"/> items if no items are present. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripTabsPane</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>div.tabStripTabsPane.readOnly</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassTabsPaneEmpty
  {
    get { return "empty"; }
  }

  /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/>. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripTab</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="TabStyle"/>. </para>
  /// </remarks>
  protected virtual string CssClassTab
  {
    get { return "tabStripTab"; }
  }

  /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/> if it is selected. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripTabSelected</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="TabSelectedStyle"/>. </para>
  /// </remarks>
  protected virtual string CssClassTabSelected
  {
    get { return "tabStripTabSelected"; }
  }

  /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the the top border. </summary>
  /// <remarks> 
  ///   <para> Class: <c>top</c>. </para>
  /// </remarks>
  protected virtual string CssClassTabTopBorder
  {
    get { return "top"; }
  }

  /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the the bottom border. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bottom</c>. </para>
  /// </remarks>
  protected virtual string CssClassTabBottomBorder
  {
    get { return "bottom"; }
  }

  /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the the left border. </summary>
  /// <remarks> 
  ///   <para> Class: <c>left</c>. </para>
  /// </remarks>
  protected virtual string CssClassTabLeftBorder
  {
    get { return "left"; }
  }

  /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the the right border. </summary>
  /// <remarks> 
  ///   <para> Class: <c>right</c>. </para>
  /// </remarks>
  protected virtual string CssClassTabRightBorder
  {
    get { return "right"; }
  }

  /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the the top left corner. </summary>
  /// <remarks> 
  ///   <para> Class: <c>topLeft</c>. </para>
  /// </remarks>
  protected virtual string CssClassTabTopLeftCorner
  {
    get { return "topLeft"; }
  }

  /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the the bottom left corner. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bottomLeft</c>. </para>
  /// </remarks>
  protected virtual string CssClassTabBottomLeftCorner
  {
    get { return "bottomLeft"; }
  }

  /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the the top right corner. </summary>
  /// <remarks> 
  ///   <para> Class: <c>topRight</c>. </para>
  /// </remarks>
  protected virtual string CssClassTabTopRightCorner
  {
    get { return "topRight"; }
  }

  /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the the bottom right corner. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bottomRight</c>. </para>
  /// </remarks>
  protected virtual string CssClassTabBottomRightCorner
  {
    get { return "bottomRight"; }
  }

  /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the content. </summary>
  /// <remarks> 
  ///   <para> Class: <c>content</c>. </para>
  /// </remarks>
  protected virtual string CssClassTabContent
  {
    get { return "content"; }
  }
  
  /// <summary> 
  ///   Gets the CSS-Class applied to a <see cref="WebTab"/> with <see cref="WebTab.IsSeparator"/> set 
  ///   <see langword="true"/>. 
  /// </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripTabSeparator</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="SeparatorStyle"/>. </para>
  /// </remarks>
  protected virtual string CssClassSeparator
  {
    get { return "tabStripTabSeparator"; }
  }
  #endregion
}

}
