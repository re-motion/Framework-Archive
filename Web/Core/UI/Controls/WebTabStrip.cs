using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

[ToolboxData("<{0}:WebTabStrip runat=server></{0}:WebTabStrip>")]
public class WebTabStrip : WebControl, IPostBackEventHandler
{
  // statics
  private static readonly object s_selectedIndexChangedEvent = new object();

  // types

  // fields
  private WebTabCollection _tabs;
  private WebTab _selectedTab;
  private bool _hasTabsCreated;
  private Triplet[] _tabsViewState;
  private Style _separatorStyle;
  private Style _tabStyle;
  private Style _tabHoverStyle;
  private Style _tabSelectedStyle;

  /// <summary> Initalizes a new instance. </summary>
  public WebTabStrip (Control ownerControl)
  {
    _tabs = new WebTabCollection (ownerControl);
    _tabs.SetParent (this);
    this._tabStyle = new Style();
    this._tabHoverStyle = new Style();
    this._tabSelectedStyle = new Style();
    this._separatorStyle = new Style();
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTabStrip()
    :this (null)
  {
  }

  /// <summary> Implementation of the <see cref="IPostBackEventHandler"/> interface. </summary>
  /// <param name="eventArgument"> &lt;node path&gt;</param>
  void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);
    EnsureTabsCreated();

    eventArgument = eventArgument.Trim();
    HandleClickEvent (eventArgument);
  }

  /// <summary> Handles the click event for a tab. </summary>
  /// <param name="eventArgument"> The id of the tab. </param>
  private void HandleClickEvent (string eventArgument)
  {
    WebTab clickedTab = _tabs.Find (eventArgument);
    if (clickedTab != SelectedTab)
    {
      SetSelectedTab (clickedTab);
      OnSelectedIndexChange();
    }
  }

  private void EnsureTabsCreated()
  {
    if (_hasTabsCreated)
      return;

    if (_tabsViewState != null)
      LoadTabsViewStateRecursive (_tabsViewState, _tabs);

    _hasTabsCreated = true;
  }

  protected override void LoadViewState(object savedState)
  {
    if (savedState != null)
    {
      object[] values = (object[]) savedState;
      base.LoadViewState(values[0]);
      _tabsViewState = (Triplet[]) values[1];
    }
  }

  protected override object SaveViewState()
  {
    object[] values = new object[2];
    values[0] = base.SaveViewState();
    values[1] = SaveNodesViewStateRecursive (_tabs);
    return values;
  }

  /// <summary> Loads the settings of the <paramref name="tabs"/> from <paramref name="tabsViewState"/>. </summary>
  private void LoadTabsViewStateRecursive (Triplet[] tabsViewState, WebTabCollection tabs)
  {
    // Not the most efficient method, but be once the tab strip is more advanced.
    foreach (Triplet tabViewState in tabsViewState)
    {
      string tabID = (string) tabViewState.First;
      WebTab tab = _tabs.Find (tabID);
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
      WebTab tab = _tabs[i];    
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

  protected virtual void OnSelectedIndexChange ()
  {
    if (_selectedTab != null)
      _selectedTab.OnSelectionChanged();

    EventHandler handler = (EventHandler) Events[s_selectedIndexChangedEvent];
    if (handler != null)
      handler (this, EventArgs.Empty);
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
    }
  }

  /// <summary> Gets the currently selected tab. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public WebTab SelectedTab
  {
    get { return _selectedTab; }
  }

  /// <summary> Gets the tabs displayed by this tab strip. </summary>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [MergableProperty(false)]
  //  Default category
  [Description ("The tabs displayed by this tab strip.")]
  [DefaultValue ((string) null)]
  public virtual WebTabCollection Tabs
  {
    get { return this._tabs; }
  }

  [Category("Style")]
  [Description("The style that you want to apply to a tab if it is not the selected tab.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style TabStyle
  {
    get { return _tabStyle; }
  }

  [Category("Style")]
  [Description("The style that you want to apply to the selected tab.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style TabSelectedStyle
  {
    get { return _tabSelectedStyle; }
  }

  [Category("Style")]
  [Description("The style that you want to apply to the separators.")]
  [NotifyParentProperty(true)]
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


/******************************************************************************************************************/


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

  protected override void RenderContents(HtmlTextWriter writer)
  {
    foreach (WebTab tab in this.Tabs)
    {
      if (tab.IsSelected)
        writer.AddAttribute (HtmlTextWriterAttribute.Class, null);  
      else
        writer.AddAttribute (HtmlTextWriterAttribute.Class, null);  
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      string postBackLink = null;
      if (! tab.IsSeparator)
        postBackLink = Page.GetPostBackClientHyperlink (this, tab.TabID);
      tab.RenderContents(writer, postBackLink);

      writer.RenderEndTag();
    }
  }

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
  internal void ResetSelectedIndex()
  {
    if (this.ViewState["SelectedIndex"] != null)
    {
      this.ViewState.Remove("SelectedIndex");
    }
  }


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

  [Description("TabStripSelectedIndex")]
  [PersistenceMode(PersistenceMode.Attribute)]
  [Category("Behavior")]
  [DefaultValue(0)]
  public int SelectedIndex
  {
    get
    {
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
      return -1;
    }
    set
    {
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
    }
  }

  [Category("Separator Defaults")]
  [DefaultValue("")]
  [PersistenceMode(PersistenceMode.Attribute)]
  [Description("SepDefaultImageUrl")]
  public string SepDefaultImageUrl
  {
    get
    {
      object obj1 = this.ViewState["SepDefaultImageUrl"];
      if (obj1 != null)
      {
        return (string) obj1;
      }
      return string.Empty;
    }
    set
    {
      this.ViewState["SepDefaultImageUrl"] = value;
    }
  }

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
}

}
