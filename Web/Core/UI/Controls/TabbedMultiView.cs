using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI.Design;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.Web.UI.Controls
{

[ToolboxData("<{0}:TabbedMultiView id=\"MultiView\" runat=\"server\" cssclass=\"tabbedMultiView\"></{0}:TabbedMultiView>")]
[DefaultEvent ("ActiveViewChanged")]
public class TabbedMultiView: WebControl, IControl
{
  // constants
  private const string c_tabIDSuffix = "_Tab";
  // statics

  // types

#if ! net20
  [CLSCompliant (false)]
  protected internal class MultiView: Rubicon.Web.UI.Controls.MultiPage
#else
  protected internal class MultiView: System.Web.UI.WebControls.MultiView
#endif
  {
#if ! net20
    bool _isLoadViewStateComplete = false;
#endif

    protected internal MultiView ()
    {
    }

    protected override ControlCollection CreateControlCollection()
    {
      return new TabViewCollection (this);
    }

    internal void OnTabViewInserted (TabView view)
    {
      Parent.OnTabViewInserted (view);
    }
 
    protected new TabbedMultiView Parent
    {
      get { return (TabbedMultiView) base.Parent; }
    }

#if ! net20
    protected override void LoadViewState(object savedState)
    {
      base.LoadViewState (savedState);
      _isLoadViewStateComplete = true;
    }

    public TabView GetActiveView()
    {
      int selectedView = SelectedIndex;
      if (selectedView >= 0 && selectedView < Controls.Count)
        return (TabView) Controls[selectedView];
      return null;
    }

    public void SetActiveView (TabView view)
    {
      int index = Controls.IndexOf (view);
      if (index < 0)
        throw new HttpException (string.Format ("The view {0} cannot be found inside {1}, the ActiveView must be a View control directly inside a MultiView.", (view == null) ? "null" : view.ID, ID));
      if (SelectedIndex != index)
      {
        SelectedIndex = index;
        if (_isLoadViewStateComplete || (Page != null && ! Page.IsPostBack))
          OnSelectedIndexChange (EventArgs.Empty);
      }
    }

    public event EventHandler ActiveViewChanged
    {
      add { SelectedIndexChange += value; }
      remove { SelectedIndexChange -= value; }
    }
#endif
  }

  protected internal class MultiViewTab: WebTab
  {
    string _target;

    /// <summary> Initalizes a new instance. </summary>
    public MultiViewTab (string tabID, string text, IconInfo icon)
      : base (tabID, text, icon)
    {
    }

    /// <summary> Initalizes a new instance. </summary>
    public MultiViewTab (string tabID, string text, string iconUrl)
      : this (tabID, text, new IconInfo (iconUrl))
    {
    }

    /// <summary> Initalizes a new instance. </summary>
    public MultiViewTab (string tabID, string text)
      : this (tabID, text, string.Empty)
    {
    }

    /// <summary> Initalizes a new instance. </summary>
    public MultiViewTab()
      : this (null, null, new IconInfo ())
    {
    }

    public string Target
    {
      get { return _target; }
      set { _target = value; }
    }

    public override void OnSelectionChanged()
    {
      TabbedMultiView multiView = ((TabbedMultiView) OwnerControl);
      TabView view = (TabView) multiView.MultiViewInternal.FindControl (_target);
      multiView.SetActiveView (view);
    }
  }

  // fields
  private WebTabStrip _tabStrip;
  private TabbedMultiView.MultiView _multiViewInternal;
  private PlaceHolder _topControl;
  private PlaceHolder _bottomControl;

  private Style _tabStripStyle;
  private Style _activeViewStyle;
  private Style _topControlsStyle;
  private Style _bottomControlsStyle;

  // construction and destruction
  public TabbedMultiView()
  {
    CreateControls();
    _tabStripStyle = new Style();
    _activeViewStyle = new Style();
    _topControlsStyle = new Style();
    _bottomControlsStyle = new Style();
  }

  private void CreateControls()
  {
    _tabStrip = new WebTabStrip (this);
    _multiViewInternal = new TabbedMultiView.MultiView ();
    _topControl = new PlaceHolder();
    _bottomControl = new PlaceHolder();
  }

  // methods and properties
  protected override void CreateChildControls()
  {
    _tabStrip.ID = ID + "_TabStrip";
    Controls.Add (_tabStrip);

    _multiViewInternal.ID = ID + "_MultiView"; 
    Controls.Add (_multiViewInternal);

    _topControl.ID = ID + "_TopControl";
    Controls.Add (_topControl);

    _bottomControl.ID = ID + "_BottomControl";
    Controls.Add (_bottomControl);
  }
  
  private void OnTabViewInserted (TabView view)
  {
    EnsureChildControls();

    _tabStrip.Tabs.Add (WebTab.GetSeparator());

    MultiViewTab tab = new MultiViewTab ();
    tab.TabID = view.ID + c_tabIDSuffix;
    tab.Text = view.Title;
    tab.Icon = view.Icon;
    tab.Target = view.ID;
    _tabStrip.Tabs.Add (tab);
  }

#if ! net20
  [CLSCompliant (false)]
#endif
  public void SetActiveView (TabView view)
  {
    MultiViewInternal.SetActiveView (view);
    TabView activeView = GetActiveView();
    WebTab nextActiveTab = _tabStrip.Tabs.Find (activeView.ID + c_tabIDSuffix);
    nextActiveTab.IsSelected = true;
  }

#if ! net20
  [CLSCompliant (false)]
#endif
  public TabView GetActiveView()
  {
    return (TabView) MultiViewInternal.GetActiveView();
  }

  protected override void LoadViewState(object savedState)
  {
    base.LoadViewState (savedState);
  }

  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Table; }
  }

  protected override void OnPreRender(EventArgs e)
  {
    string key = typeof (TabbedMultiView).FullName + "_Style";
    string styleSheetUrl = null;
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (TabbedMultiView), ResourceType.Html, "TabbedMultiView.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, styleSheetUrl);
    }

    base.OnPreRender (e);
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);
    if (ControlHelper.IsDesignMode (this, Context))
      writer.AddStyleAttribute ("height", "50pt");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
  }

  protected override void RenderContents (HtmlTextWriter writer)
  {
    EnsureChildControls();
    
    RenderTopControls (writer);
    RenderTabStrip (writer);
    RenderActiveView (writer);
    RenderBottomControls (writer);
  }

  protected virtual void RenderTabStrip (HtmlTextWriter writer)
  {
    writer.RenderBeginTag (HtmlTextWriterTag.Tr); // begin tr
    
    writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "0%");
    _tabStripStyle.AddAttributesToRender (writer);
    writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

    _tabStrip.Style["width"] = "100%";
    if (StringUtility.IsNullOrEmpty (_tabStripStyle.CssClass))
      _tabStrip.CssClass = CssClassTabStrip;
    _tabStrip.RenderControl (writer);

    writer.RenderEndTag(); // end td
    writer.RenderEndTag(); // end tr
  }

  protected virtual void RenderActiveView (HtmlTextWriter writer)
  {
    writer.RenderBeginTag (HtmlTextWriterTag.Tr); // begin tr
    
    if (ControlHelper.IsDesignMode (this, Context))
      writer.AddStyleAttribute ("border", "solid 1px black");
    writer.AddStyleAttribute ("vertical-align", "top");
    _tabStripStyle.AddAttributesToRender (writer);
    writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td
    
    _activeViewStyle.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (_activeViewStyle.CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassActiveView);
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
    writer.AddStyleAttribute ("overflow", "auto");
    writer.AddAttribute (HtmlTextWriterAttribute.Id, ClientID + "_ActiveView");
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin div
    
    Control view = _multiViewInternal.GetActiveView();
    if (view != null)
    {
      foreach (Control control in view.Controls)
        control.RenderControl (writer);
    }
    
    writer.RenderEndTag(); // end div

    writer.RenderEndTag(); // end td
    writer.RenderEndTag(); // end tr
  }

  protected virtual void RenderTopControls (HtmlTextWriter writer)
  {
    if (_topControl.Controls.Count == 0)
      return;
    
    writer.RenderBeginTag (HtmlTextWriterTag.Tr); // begin tr
    writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "0%");
    writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

    _topControlsStyle.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (_topControlsStyle.CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassTopControls);
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin div

    _topControl.RenderControl (writer);

    writer.RenderEndTag(); // end div

    writer.RenderEndTag(); // end td
    writer.RenderEndTag(); // end tr
  }

  protected virtual void RenderBottomControls (HtmlTextWriter writer)
  {
    if (_bottomControl.Controls.Count == 0)
      return;
    
    writer.RenderBeginTag (HtmlTextWriterTag.Tr); // begin tr
    writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "0%");
    writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

    _bottomControlsStyle.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (_bottomControlsStyle.CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBottomControls);
    writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin div

    _bottomControl.RenderControl (writer);

    writer.RenderEndTag(); // end div

    writer.RenderEndTag(); // end td
    writer.RenderEndTag(); // end tr
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public override ControlCollection Controls
  {
    get
    {
      EnsureChildControls();
      return base.Controls;
    }
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Browsable (false)]
  public TabViewCollection Views
  { 
    get { return (TabViewCollection) MultiViewInternal.Controls; }
  }

  /// <summary>
  ///   Fired everytime the active view is changed after the <c>LoavViewState</c> phase or if it is not a post back.
  /// </summary>
  public event EventHandler ActiveViewChanged
  {
    add { MultiViewInternal.ActiveViewChanged += value; }
    remove { MultiViewInternal.ActiveViewChanged -= value; }
  }

  protected WebTabStrip TabStrip
  {
    get 
    {
      EnsureChildControls();
      return _tabStrip; 
    }
  }

#if ! net20
  [CLSCompliant (false)]
#endif
  protected TabbedMultiView.MultiView MultiViewInternal
  {
    get
    {
      EnsureChildControls();
      return _multiViewInternal; 
    }
  }

  [Category ("Style")]
  [Description ("The style that you want to apply to the tab strip section.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style TabStripStyle
  {
    get { return _tabStripStyle; }
  }

  [Category ("Style")]
  [Description ("The style that you want to apply to the active view.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style ActiveViewStyle
  {
    get { return _activeViewStyle; }
  }

  [Category ("Style")]
  [Description ("The style that you want to apply to a pane of tabs.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style TabsPaneStyle
  {
    get { return _tabStrip.TabsPaneStyle; }
  }

  [Category ("Style")]
  [Description ("The style that you want to apply to a tab that is neither selected nor a separator.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style TabStyle
  {
    get { return _tabStrip.TabStyle; }
  }

  [Category ("Style")]
  [Description ("The style that you want to apply to the selected tab.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style TabSelectedStyle
  {
    get { return _tabStrip.TabSelectedStyle; }
  }

  [Category ("Style")]
  [Description ("The style that you want to apply to the separators.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style SeparatorStyle
  {
    get { return _tabStrip.SeparatorStyle; }
  }

  [Category ("Style")]
  [Description ("The style that you want to the top section. The height cannot be provided in percent.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style TopControlsStyle
  {
    get { return _topControlsStyle; }
  }

  [Category ("Style")]
  [Description ("The style that you want to apply to the bottom section. The height cannot be provided in percent.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style BottomControlsStyle
  {
    get { return _bottomControlsStyle; }
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Browsable (false)]
  public ControlCollection TopControls
  {
    get { return _topControl.Controls; }
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Browsable (false)]
  public ControlCollection BottomControls
  {
    get { return _bottomControl.Controls; }
  }


  /// <summary>
  ///   Clears all items.
  /// </summary>
  /// <remarks>
  ///   Note that clearing <see cref="Views"/> is not sufficient, as other controls are created implicitly.
  /// </remarks>
  public void Clear()
  {
    CreateControls();
    Controls.Clear();
    CreateChildControls();
    //  Views.Clear();
    //  TabStrip.Tabs.Clear();
    //  MultiViewInternal.Controls.Clear();
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
//    get { return _tabStrip.TabsPaneSize; }
//    set
//    {
//      if (value.IsNull || value.Value <= 0)
//        _tabStrip.TabsPaneSize = NaInt32.Null;
//      else
//        _tabStrip.TabsPaneSize = value; 
//    }
//  }

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="TabbedMultiView"/>'s tab strip. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabbedMultiViewTabStrip</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="TabStripStyle"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassTabStrip
  {
    get { return "tabbedMultiViewTabStrip"; }
  }

  /// <summary> Gets the CSS-Class applied to the <see cref="TabbedMultiView"/>'s active view. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabbedMultiViewActiveView</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="ActiveViewStyle"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassActiveView
  {
    get { return "tabbedMultiViewActiveView"; }
  }

  /// <summary> Gets the CSS-Class applied to the top section. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabbedMultiViewTopControls</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="TopControlsStyle"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassTopControls
  {
    get { return "tabbedMultiViewTopControls"; }
  }

  /// <summary> Gets the CSS-Class applied to the bottom section. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabbedMultiViewBottomControls</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="BottomControlsStyle"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBottomControls
  {
    get { return "tabbedMultiViewBottomControls"; }
  }
  #endregion
}

}