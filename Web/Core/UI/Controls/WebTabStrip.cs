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

namespace Rubicon.Web.UI.Controls
{

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
  private string _selectedTabID;
  private string _tabToBeSelected = null;
  private bool _hasTabsRestored;
  private Triplet[] _tabsViewState;
  private Style _tabsPaneStyle;
  private Style _separatorStyle;
  private Style _tabStyle;
  private Style _tabSelectedStyle;
  private NaInt32 _tabsPaneSize = NaInt32.Null;
  private Control _ownerControl;

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
    if (postCollection[ControlHelper.EventTarget] == ClientID)
    {
      _tabToBeSelected = postCollection[ControlHelper.EventArgument];
      ArgumentUtility.CheckNotNullOrEmpty ("postCollection[\"__EVENTARGUMENT\"]", _tabToBeSelected);
      if (_tabToBeSelected != _selectedTabID)
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
  /// <param name="tabID"> The id of the tab. </param>
  private void HandleSelectionChangeEvent (string tabID)
  {
    WebTab currentTab = _selectedTab;
    SetSelectedTab (tabID);
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

    if (_tabsViewState != null)
      LoadTabsViewStateRecursive (_tabsViewState, Tabs);

    _hasTabsRestored = true;
  }

  protected override void LoadViewState(object savedState)
  {
    if (savedState != null)
    {
      object[] values = (object[]) savedState;
      base.LoadViewState(values[0]);
      _tabsViewState = (Triplet[]) values[1];
      _selectedTabID = (string) values[2];
    }
  }

  protected override object SaveViewState()
  {
    object[] values = new object[3];
    values[0] = base.SaveViewState();
    values[1] = SaveNodesViewStateRecursive (Tabs);
    values[2] = _selectedTabID;
    return values;
  }

  /// <summary> Loads the settings of the <paramref name="tabs"/> from <paramref name="tabsViewState"/>. </summary>
  private void LoadTabsViewStateRecursive (Triplet[] tabsViewState, WebTabCollection tabs)
  {
    // Not the most efficient method, but be once the tab strip is more advanced.
    foreach (Triplet tabViewState in tabsViewState)
    {
      string tabID = (string) tabViewState.First;
      WebTab tab = Tabs.Find (tabID);
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
    // Not the most efficient method, but be once the tab strip is more advanced.
    Triplet[] tabsViewState = new Triplet[tabs.Count];
    for (int i = 0; i < tabs.Count; i++)
    {
      WebTab tab = Tabs[i];    
      Triplet tabViewState = new Triplet();
      tabViewState.First = tab.TabID;
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
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, styleSheetUrl);
    }

   if (! ControlHelper.IsDesignMode ((IControl)this, Context) && Enabled)
      Page.RegisterRequiresPostBack (this);

    EnsureTabsRestored();
    base.OnPreRender (e);
  }

  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Div; }
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);
  }

  protected override void RenderContents(HtmlTextWriter writer)
  {
    int tabsOnPane = 0;
    bool isTabsPaneOpen = false;
    WebTabCollection tabs = Tabs;
    
    if (   ControlHelper.IsDesignMode (this, Context)
        && tabs.Count == 0)
    {
      tabs = new WebTabCollection (null);
      for (int i = 0; i < 5; i++)
      {
        tabs.Add (new WebTab (i.ToString(), "Tab " + (i + 1).ToString())); 
        tabs.Add (WebTab.GetSeparator());
      }
    }

    for (int i = 0; i < tabs.Count; i++)
    {
      WebTab tab = tabs[i];

      if (   ! tab.IsSeparator
          && ! _tabsPaneSize.IsNull 
          && tabsOnPane == _tabsPaneSize.Value
          && isTabsPaneOpen)
      {
        RenderEndTabsPane (writer);
        tabsOnPane = 0;
        isTabsPaneOpen = false;
      }
    
      if (! isTabsPaneOpen)
      {
        RenderBeginTabsPane (writer);
        isTabsPaneOpen = true;
      }

      if (! tab.IsSeparator)
        tabsOnPane++;

      writer.AddStyleAttribute ("white-space", "nowrap");
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      while (tab != null && tab.IsSeparator)
      {
        RenderTab (writer, tab);
        tab = null;
        if (++i < tabs.Count)
        {
          tab = tabs[i];
          if (! tab.IsSeparator)
            break;
        }  
      }
      if (tab != null)  
        RenderTab (writer, tab);
      
      writer.RenderEndTag();
    }
    if (isTabsPaneOpen)
      RenderEndTabsPane (writer);
  }

  private void RenderTab (HtmlTextWriter writer, WebTab tab)
  {
      string postBackLink = null;
      if (! tab.IsSeparator && ! tab.IsSelected)
      {
        Page page = Page;
        if (page == null && _ownerControl != null)
          page = _ownerControl.Page;
        string postBackEvent = null;
        if (page != null)
          postBackEvent = page.GetPostBackClientEvent (this, tab.TabID);
        if (! StringUtility.IsNullOrEmpty (postBackEvent))
        {
          writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);
          postBackLink = "javascript:" + postBackEvent;
        }
      }
      writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      writer.AddStyleAttribute ("display", "inline");
      writer.RenderBeginTag (HtmlTextWriterTag.Table);
      writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      if (!tab.IsSeparator)
      {
        if (tab.IsSelected)
          writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassTabLeftBorderSelected);
        else
          writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassTabLeftBorder);
        writer.RenderBeginTag (HtmlTextWriterTag.Td);
        writer.RenderEndTag();
      }
      
      Style style = _tabStyle;
      string cssClass = CssClassTab;
      if (tab.IsSeparator)
      {
        style = _separatorStyle;
        cssClass = CssClassSeparator;
      }
      else if (tab.IsSelected)
      {
        style = _tabSelectedStyle;
        cssClass = CssClassTabSelected;
      }
      style.AddAttributesToRender (writer);
      if (StringUtility.IsNullOrEmpty (style.CssClass))
        writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
      
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      tab.RenderContents(writer, postBackLink);
      writer.RenderEndTag();

      if (!tab.IsSeparator)
      {
        if (tab.IsSelected)
          writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassTabRightBorderSelected);
        else
          writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassTabRightBorder);
        writer.RenderBeginTag (HtmlTextWriterTag.Td);
        writer.RenderEndTag();
      }

      writer.RenderEndTag(); // End tr
      writer.RenderEndTag(); // End table
  }

  protected virtual void RenderBeginTabsPane (HtmlTextWriter writer)
  {
    _tabsPaneStyle.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (_tabsPaneStyle.CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassTabsPane);
    if (! _tabsPaneSize.IsNull)
      writer.AddStyleAttribute ("white-space", "nowrap");
    writer.RenderBeginTag (HtmlTextWriterTag.Div);
    writer.RenderBeginTag (HtmlTextWriterTag.Span);

  }

  protected virtual void RenderEndTabsPane (HtmlTextWriter writer)
  {
    writer.RenderEndTag();
    writer.RenderEndTag();
  }

  /// <summary> 
  ///   Dispatches the resources passed in <paramref name="values"/> to the <see cref="WebTabStrip"/>'s properties. 
  /// </summary>
  /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
  public void Dispatch (IDictionary values)
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
    DispatchToTabs (Tabs, tabValues, "Tabs");
  }

  /// <summary>
  ///   Dispatches the resources passed in <paramref name="values"/> to the properties of the 
  ///   <see cref="WebTab"/> objects in the collection <paramref name="tabs"/>.
  /// </summary>
  private void DispatchToTabs (WebTabCollection tabs, IDictionary values, string collectionName)
  {
    foreach (DictionaryEntry entry in values)
    {
      string tabID = (string) entry.Key;
      
      bool isValidID = false;
      foreach (WebTab tab in tabs)
      {
        if (tab.TabID == tabID)
        {
          ResourceDispatcher.DispatchGeneric (tab, (IDictionary) entry.Value);
          isValidID = true;
          break;
        }
      }

      if (! isValidID)
      {
        //  Invalid collection element
        s_log.Debug ("WebTabStrip '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain an item with an ID of '" + tabID + "' inside the collection '" + collectionName + "'.");
      }
    }
  }

  /// <summary> Sets the selected tab. </summary>
  internal void SetSelectedTab (WebTab tab)
  {
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
        _selectedTabID = null;
      else
        _selectedTabID = _selectedTab.TabID;
    }
  }

  private void SetSelectedTab (string tabID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("tabID", tabID);
    if (_selectedTab == null || _selectedTab.TabID != tabID)
    {
      WebTab tab = Tabs.Find (tabID);
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

//  /// <summary> The number of tabs displayed per pane. Ignores separators. </summary>
//  /// <value> 
//  ///   An integer greater than zero to limit the number of tabs per pane to the specified value,
//  ///   or zero, less than zero or <see cref="NaInt32.Null"/> to show all tabs in a single pane.
//  /// </value>
//  [Category ("Appearance")]
//  [Description ("The number of tabs displayed per page. Set TabsPaneSize to 0 to show all tabs in a single pane.")]
//  [DefaultValue (typeof(NaInt32), "null")]
//  public NaInt32 TabsPaneSize
//  {
//    get { return _tabsPaneSize; }
//    set
//    {
//      if (value.IsNull || value.Value <= 0)
//        _tabsPaneSize = NaInt32.Null;
//      else
//        _tabsPaneSize = value; 
//    }
//  }

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="WebTabStrip"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStrip</c>. </para>
  ///   <para> Applied only if the <see cref="CssClass"/> is not set. </para>
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

  /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/>. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripTabLeftBorder</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="TabStyle"/>. </para>
  /// </remarks>
  protected virtual string CssClassTabLeftBorder
  {
    get { return "tabStripTabLeftBorder"; }
  }

  /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/>. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripTabRightBorder</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="TabStyle"/>. </para>
  /// </remarks>
  protected virtual string CssClassTabRightBorder
  {
    get { return "tabStripTabRightBorder"; }
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

  /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/> if it is selected. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripTabLeftBorderSelected</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="TabSelectedStyle"/>. </para>
  /// </remarks>
  protected virtual string CssClassTabLeftBorderSelected
  {
    get { return "tabStripTabLeftBorderSelected"; }
  }

  /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/> if it is selected. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripTabRightBorderSelected</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="TabSelectedStyle"/>. </para>
  /// </remarks>
  protected virtual string CssClassTabRightBorderSelected
  {
    get { return "tabStripTabRightBorderSelected"; }
  }

  /// <summary> 
  ///   Gets the CSS-Class applied to a <see cref="WebTab"/> with <see cref="WebTab.IsSeparator"/> set 
  ///   <see langword="true"/>. 
  /// </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripSeparator</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="SeparatorStyle"/>. </para>
  /// </remarks>
  protected virtual string CssClassSeparator
  {
    get { return "tabStripSeparator"; }
  }
  #endregion
}

}
