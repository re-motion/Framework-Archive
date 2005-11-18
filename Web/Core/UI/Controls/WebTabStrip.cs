using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using log4net;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI.Design;
using Rubicon.Web.UI.Globalization;
using Rubicon.Globalization;

namespace Rubicon.Web.UI.Controls
{

/// <include file='doc\include\UI\Controls\WebTabStrip.xml' path='WebTabStrip/Class/*' />
[ToolboxData("<{0}:WebTabStrip runat=server></{0}:WebTabStrip>")]
public class WebTabStrip : WebControl, IControl, IPostBackDataHandler, IResourceDispatchTarget
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
  private Triplet[] _tabsViewState;
  private Style _tabsPaneStyle;
  private Style _separatorStyle;
  private Style _tabStyle;
  private Style _tabSelectedStyle;
  private Control _ownerControl;
  private WcagHelper _wcagHelper;

  /// <summary> Initalizes a new instance. </summary>
  public WebTabStrip (Control ownerControl)
  {
    _ownerControl = ownerControl;
    _tabs = new WebTabCollection (ownerControl);
    Tabs.SetParent (this);
    _tabsPaneStyle = new Style();
    _tabStyle = new Style();
    _tabSelectedStyle = new Style();
    _separatorStyle = new Style();
  }

  /// <summary> Initalizes a new instance. </summary>
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

  private void EnsureTabsRestored()
  {
    if (_hasTabsRestored)
      return;
 
    _isRestoringTabs = true;
    if (_tabsViewState != null)
    {
      LoadTabsViewStateRecursive (_tabsViewState, _tabs);
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
      _tabsViewState = (Triplet[]) values[1];
      _selectedItemID = (string) values[2];
    }
  }

  protected override object SaveViewState()
  {
    object[] values = new object[3];
    values[0] = base.SaveViewState();
    values[1] = SaveNodesViewStateRecursive (_tabs);
    values[2] = _selectedItemID;
    return values;
  }

  /// <summary> Loads the settings of the <paramref name="tabs"/> from <paramref name="tabsViewState"/>. </summary>
  private void LoadTabsViewStateRecursive (Triplet[] tabsViewState, WebTabCollection tabs)
  {
    // Not the most efficient method, but be once the tab strip is more advanced.
    for (int i = 0; i < tabsViewState.Length; i++)
    {
      Triplet tabViewState = (Triplet) tabsViewState[i];
      string itemID = (string) tabViewState.First;
      WebTab tab = tabs.Find (itemID);
      if (tab != null)
      {
        object[] values = (object[]) tabViewState.Second;
        bool isSelected = (bool) values[0];
        if (isSelected)
          tab.IsSelected = true;
        // LoadNodesViewStateRecursive ((Triplet[]) tabViewState.Third, tab.Children);
      }
    }
  }

  /// <summary> Saves the settings of the  <paramref name="tabs"/> and returns this view state </summary>
  private Triplet[] SaveNodesViewStateRecursive (WebTabCollection tabs)
  {
    EnsureTabsRestored();
    // Not the most efficient method, but be once the tab strip is more advanced.
    Triplet[] tabsViewState = new Triplet[tabs.Count];
    for (int i = 0; i < tabs.Count; i++)
    {
      WebTab tab = tabs[i];    
      Triplet tabViewState = new Triplet();
      tabViewState.First = tab.ItemID;
      object[] values = new object[1];
      values[0] = tab.IsSelected;
      tabViewState.Second = values;
      // tabViewState.Third = SaveNodesViewStateRecursive (tab.Children);
      tabsViewState[i] = tabViewState;
    }
    return tabsViewState;
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

  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Div; }
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    string backUpStyleWidth = Style["width"];
    if (! StringUtility.IsNullOrEmpty (Style["width"]))
      Style["width"] = null;
    Unit backUpWidth = Width; // base.Width and base.ControlStyle.Width
    if (! Width.IsEmpty)
      Width = Unit.Empty;
    
    base.AddAttributesToRender (writer);
    
    if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);

    if (! StringUtility.IsNullOrEmpty (backUpStyleWidth))
      Style["width"] = backUpStyleWidth;
    if (! backUpWidth.IsEmpty)
      Width = backUpWidth;

    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
  }

  protected override void RenderContents(HtmlTextWriter writer)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);

    if (WcagHelper.IsWcagDebuggingEnabled() && WcagHelper.IsWaiConformanceLevelARequired())
      WcagHelper.HandleError (1, this);

    WebTabCollection tabs = Tabs;
    
    if (   ControlHelper.IsDesignMode (this, Context)
        && tabs.Count == 0)
    {
      tabs = GetDesignTimeTabs(); 
    }

    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
    if (Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]))
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    else if (Width.IsEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Style["width"]);
    else
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Width.ToString());
    writer.RenderBeginTag (HtmlTextWriterTag.Table); // Begin Table

    RenderBeginTabsPane (writer);
    for (int i = 0; i < tabs.Count; i++)
    {
      WebTab tab = tabs[i];
      RenderTab (writer, tab);
    }
    RenderEndTabsPane (writer);

    writer.RenderEndTag(); // End Table
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

    _tabsPaneStyle.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (_tabsPaneStyle.CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassTabsPane);
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Div

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

    if (! tab.IsSelected)
      writer.AddAttribute (HtmlTextWriterAttribute.Href, GetHref (tab));
    writer.RenderBeginTag (HtmlTextWriterTag.A); // Begin anchor
    
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabLeftBorder);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin left border span
 
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabRightBorder);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin right border span

    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabContent);
    writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin content span

    tab.RenderContents (writer);

    writer.RenderEndTag(); // End content span
    writer.RenderEndTag(); // End right border span
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

  /// <summary> Gets an instance of the the <see cref="WcagHelper"/> type. </summary>
  protected virtual WcagHelper WcagHelper
  {
    get 
    {
      if (_wcagHelper == null)
        _wcagHelper = new WcagHelper();
      return _wcagHelper; 
    }
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

  /// <summary> Gets the CSS-Class applied to a <c>span</c> intendet for formatting the the left border. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripTabLeftBorder</c>. </para>
  /// </remarks>
  protected virtual string CssClassTabLeftBorder
  {
    get { return "tabStripTabLeftBorder"; }
  }

  /// <summary> Gets the CSS-Class applied to a <c>span</c> intendet for formatting the the right border. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripTabRightBorder</c>. </para>
  /// </remarks>
  protected virtual string CssClassTabRightBorder
  {
    get { return "tabStripTabRightBorder"; }
  }

  /// <summary> Gets the CSS-Class applied to a <c>span</c> intendet for formatting the content. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripTabContent</c>. </para>
  /// </remarks>
  protected virtual string CssClassTabContent
  {
    get { return "tabStripTabContent"; }
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
