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
[Designer (typeof (WebTabStripDesigner))]
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
          this, Context, typeof (WebTreeView), ResourceType.Html, "TabStrip.css");
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
    foreach (WebTab tab in Tabs)
    {
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
      
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      string postBackLink = null;
      if (! tab.IsSeparator && ! tab.IsSelected)
      {
        Page page = Page;
        if (page == null && _ownerControl != null)
          page = _ownerControl.Page;
        if (page != null)
          postBackLink = page.GetPostBackClientHyperlink (this, tab.TabID);
      }
      tab.RenderContents(writer, postBackLink);

      writer.RenderEndTag();

      if (! tab.IsSeparator)
        tabsOnPane++;
    }
    if (isTabsPaneOpen)
      RenderEndTabsPane (writer);
  }

  protected virtual void RenderBeginTabsPane (HtmlTextWriter writer)
  {
    _tabsPaneStyle.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (_tabsPaneStyle.CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassTabsPane);
    writer.AddStyleAttribute ("white-space", "nowrap");
    writer.RenderBeginTag (HtmlTextWriterTag.Div);
  }

  protected virtual void RenderEndTabsPane (HtmlTextWriter writer)
  {
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

  /// <summary> The number of tabs displayed per pane. Ignores separators. </summary>
  /// <value> 
  ///   An integer greater than zero to limit the number of tabs per pane to the specified value,
  ///   or zero, less than zero or <see cref="NaInt32.Null"/> to show all tabs in a single pane.
  /// </value>
  [Category ("Appearance")]
  [Description ("The number of tabs displayed per page. Set TabsPaneSize to 0 to show all tabs in a single pane.")]
  [DefaultValue (typeof(NaInt32), "null")]
  public NaInt32 TabsPaneSize
  {
    get { return _tabsPaneSize; }
    set
    {
      if (value.IsNull || value.Value <= 0)
        _tabsPaneSize = NaInt32.Null;
      else
        _tabsPaneSize = value; 
    }
  }

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

  /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/> if it is selected. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripTabSelected</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="TabSelectedStyle"/>. </para>
  /// </remarks>
  protected virtual string CssClassTabSelected
  {
    get { return "tabStripTabSelected"; }
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

  #region Old stuff, all commented out

//  private int _CachedSelectedIndex;
//  private int _OldMultiPageIndex;
//  private const int NoSelection = -1;
//  private const int NotSet = -2;
//  public const string TabSeparatorTagName = "TabSeparator";
//  public const string TabStripTagName = "TabStrip";
//  public const string TabTagName = "Tab";
//  public const string TagNamespace = "TSNS";

//  protected override ControlCollection CreateControlCollection()
//  {
//    return new EmptyControlCollection(this);
//  }

//  protected override void OnInit(EventArgs e)
//  {
//    if (this._CachedSelectedIndex != -2)
//    {
//      this.SelectedIndex = this._CachedSelectedIndex;
//      this._CachedSelectedIndex = -2;
//    }
//    base.OnInit(e);
//  }

//  protected override void OnLoad(EventArgs e)
//  {
//    if ((this.Page != null) && !this.Page.IsPostBack)
//    {
//      if ((this.TargetID != string.Empty) && (this.Target == null))
//      {
//        throw new Exception(BaseRichControl.GetStringResource("TabStripInvalidTargetID"));
//      }
//      foreach (Tab1 in this.Tabs)
//      {
//        if (item1 is Tab)
//        {
//          Tab tab1 = (Tab)1;
//          PageView view1 = tab1.Target;
//          if ((tab1.TargetID != string.Empty) && (view1 == null))
//          {
//            throw new Exception(BaseRichControl.GetStringResource("TabInvalidTargetID"));
//          }
//        }
//      }
//      this.SetTargetSelectedIndex();
//    }
//    base.OnLoad(e);
//  }

//  protected override void OnPreRender(EventArgs e)
//  {
//    this.HelperData = this.SelectedIndex.ToString();
//    base.OnPreRender(e);
//  }

//  protected override bool ProcessData(string szData)
//  {
//    try
//    {
//      int num1 = Convert.ToInt32(szData);
//      if (this.SelectedIndex != num1)
//      {
//        this.SelectedIndex = num1;
//        return true;
//      }
//    }
//    catch
//    {
//    }
//    return false;
//  }

//  protected override void RenderDesignerPath(HtmlTextWriter writer)
//  {
//    writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
//    writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
//    writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
//    this.AddAttributesToRender(writer);
//    writer.RenderBeginTag(HtmlTextWriterTag.Table);
//    if (this.Orientation == Orientation.Horizontal)
//    {
//      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
//    }
//    base.RenderDesignerPath(writer);
//    if (this.Orientation == Orientation.Horizontal)
//    {
//      writer.RenderEndTag();
//    }
//    writer.RenderEndTag();
//  }
//
//  protected override void RenderDownLevelPath(HtmlTextWriter writer)
//  {
//    string[] textArray1 = new string[5] { "<script language=\"javascript\">", this.ClientHelperID, ".value=", this.SelectedIndex.ToString(), ";</script>" } ;
//    writer.WriteLine(string.Concat(textArray1));
//    writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
//    writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
//    writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
//    this.AddAttributesToRender(writer);
//    writer.RenderBeginTag(HtmlTextWriterTag.Table);
//    if (this.Orientation == Orientation.Horizontal)
//    {
//      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
//    }
//    base.RenderDownLevelPath(writer);
//    if (this.Orientation == Orientation.Horizontal)
//    {
//      writer.RenderEndTag();
//    }
//    writer.RenderEndTag();
//  }

//  protected override void RenderUpLevelPath(HtmlTextWriter writer)
//  {
//    writer.Write("<?XML:NAMESPACE PREFIX=\"TSNS\" /><?IMPORT NAMESPACE=\"TSNS\" IMPLEMENTATION=\"" + base.AddPathToFilename("tabstrip.htc") + "\" />");
//    writer.WriteLine();
//    this.AddAttributesToRender(writer);
//    writer.AddAttribute("selectedIndex", this.SelectedIndex.ToString());
//    if (this.Orientation == Orientation.Vertical)
//    {
//      writer.AddAttribute("orientation", "vertical");
//    }
//    if (this.TargetID != string.Empty)
//    {
//      writer.AddAttribute("targetID", this.Target.ClientID);
//    }
//    if (this.SepDefaultImageUrl != string.Empty)
//    {
//      writer.AddAttribute("sepDefaultImageUrl", this.SepDefaultImageUrl);
//    }
//    if (this.SepHoverImageUrl != string.Empty)
//    {
//      writer.AddAttribute("sepHoverImageUrl", this.SepHoverImageUrl);
//    }
//    if (this.SepSelectedImageUrl != string.Empty)
//    {
//      writer.AddAttribute("sepSelectedImageUrl", this.SepSelectedImageUrl);
//    }
//    string text1 = this.TabDefaultStyle.CssText;
//    if (text1 != string.Empty)
//    {
//      writer.AddAttribute("tabDefaultStyle", text1);
//    }
//    text1 = this.TabHoverStyle.CssText;
//    if (text1 != string.Empty)
//    {
//      writer.AddAttribute("tabHoverStyle", text1);
//    }
//    text1 = this.TabSelectedStyle.CssText;
//    if (text1 != string.Empty)
//    {
//      writer.AddAttribute("tabSelectedStyle", text1);
//    }
//    text1 = this.SepDefaultStyle.CssText;
//    if (text1 != string.Empty)
//    {
//      writer.AddAttribute("sepDefaultStyle", text1);
//    }
//    text1 = this.SepHoverStyle.CssText;
//    if (text1 != string.Empty)
//    {
//      writer.AddAttribute("sepHoverStyle", text1);
//    }
//    text1 = this.SepSelectedStyle.CssText;
//    if (text1 != string.Empty)
//    {
//      writer.AddAttribute("sepSelectedStyle", text1);
//    }
//    if (this.Page != null)
//    {
//      string text2 = this.ClientHelperID + ".value=event.index";
//      if (this.AutoPostBack)
//      {
//        text2 = text2 + ";if (getAttribute('_submitting') != 'true'){setAttribute('_submitting','true');try{" + this.Page.GetPostBackEventReference(this, string.Empty) + ";}catch(e){setAttribute('_submitting','false');}}";
//      }
//      writer.AddAttribute("onSelectedIndexChange", "JScript:" + text2);
//      writer.AddAttribute("onwcready", "JScript:try{" + this.ClientHelperID + ".value=selectedIndex}catch(e){}");
//    }
//    writer.RenderBeginTag("TSNS:TabStrip");
//    writer.WriteLine();
//    base.RenderUpLevelPath(writer);
//    writer.RenderEndTag();
//  }
//
//  internal void ResetSelectedIndex()
//  {
//    if (this.ViewState["SelectedIndex"] != null)
//    {
//      this.ViewState.Remove("SelectedIndex");
//    }
//  }
//

  //protected override void TrackViewState()
  //{
  //      base.TrackViewState();
  //      ((IStateManager) this.TabDefaultStyle).TrackViewState();
  //      ((IStateManager) this.TabHoverStyle).TrackViewState();
  //      ((IStateManager) this.TabSelectedStyle).TrackViewState();
  //      ((IStateManager) this.SepDefaultStyle).TrackViewState();
  //      ((IStateManager) this.SepHoverStyle).TrackViewState();
  //      ((IStateManager) this.SepSelectedStyle).TrackViewState();
  //      this.Tabs.TrackViewState();
  //}

//  protected override bool NeedHelper
//  {
//    get
//    {
//      return true;
//    }
//  }

//  [PersistenceMode(PersistenceMode.Attribute)]
//  [Category("Appearance")]
//  [Description("TabStripOrientation")]
//  [DefaultValue(0)]
//  public Orientation Orientation
//  {
//    get
//    {
//      object obj1 = this.ViewState["Orientation"];
//      if (obj1 != null)
//      {
//        return (Orientation) obj1;
//      }
//      return Orientation.Horizontal;
//    }
//    set
//    {
//      this.ViewState["Orientation"] = value;
//    }
//  }

//  [Description("TabStripSelectedIndex")]
//  [PersistenceMode(PersistenceMode.Attribute)]
//  [Category("Behavior")]
//  [DefaultValue(0)]
//  public int SelectedIndex
//  {
//    get
//    {
//      int num1 = this.Tabs.NumTabs;
//      if (num1 != 0)
//      {
//        int num2 = 0;
//        object obj1 = this.ViewState["SelectedIndex"];
//        if (obj1 != null)
//        {
//          num2 = (int) obj1;
//        }
//        else if (this._CachedSelectedIndex == -1)
//        {
//          return -1;
//        }
//        if ((num2 >= 0) && (num2 < num1))
//        {
//          return num2;
//        }
//      }
//      return -1;
//    }
//    set
//    {
//      if ((this.Tabs.NumTabs == 0) && (value > -2))
//      {
//        this._CachedSelectedIndex = value;
//      }
//      else
//      {
//        if ((value <= -2) || (value >= this.Tabs.NumTabs))
//        {
//          throw new ArgumentOutOfRangeException();
//        }
//        this.ViewState["SelectedIndex"] = value;
//        if (value >= 0)
//        {
//          this._OldMultiPageIndex = -1;
//          this.SetTargetSelectedIndex();
//        }
//      }
//    }
//  }

//  [Category("Separator Defaults")]
//  [DefaultValue("")]
//  [PersistenceMode(PersistenceMode.Attribute)]
//  [Description("SepDefaultImageUrl")]
//  public string SepDefaultImageUrl
//  {
//    get
//    {
//      object obj1 = this.ViewState["SepDefaultImageUrl"];
//      if (obj1 != null)
//      {
//        return (string) obj1;
//      }
//      return string.Empty;
//    }
//    set
//    {
//      this.ViewState["SepDefaultImageUrl"] = value;
//    }
//  }

//  [Browsable(false)]
//  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
//  [CLSCompliant (false)]
//  public MultiPage Target
//  {
//    get
//    {
//      string text1 = this.TargetID;
//      if (text1 != string.Empty)
//      {
//        Control control1 = null;
//        Control control2 = this.NamingContainer;
//        Control control3 = this.Page;
//        if (control2 != null)
//        {
//          control1 = control2.FindControl(text1);
//        }
//        if ((control1 == null) && (control3 != null))
//        {
//          control1 = control3.FindControl(text1);
//        }
//        if ((control1 != null) && (control1 is MultiPage))
//        {
//          return (MultiPage) control1;
//        }
//      }
//      return null;
//    }
//  }
//
//  [Category("Behavior")]
//  [DefaultValue("")]
//  [PersistenceMode(PersistenceMode.Attribute)]
//  [Description("TabStripTargetID")]
//  public string TargetID
//  {
//    get
//    {
//      object obj1 = this.ViewState["TargetID"];
//      if (obj1 != null)
//      {
//        return (string) obj1;
//      }
//      return string.Empty;
//    }
//    set
//    {
//      this.ViewState["TargetID"] = value;
//    }
//  }
  #endregion
}

}
