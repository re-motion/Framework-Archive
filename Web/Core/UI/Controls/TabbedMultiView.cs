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

[ToolboxData("<{0}:TabbedMultiView runat=server></{0}:TabbedMultiView>")]
[ParseChildren (true, "Controls")]
[PersistChildren (false)]
public class TabbedMultiView: WebControl, IControl
{
  // constants

  // statics

  // types

  //TODO: .net2.0 complier switch. Inherit from System.Web.UI.WebControls.MultiView
  [CLSCompliant (false)]
  protected internal class MultiView: Rubicon.Web.UI.Controls.MultiPage
  {
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

    //TODO: .net 2.0 compiler switch. Method already exists in MultiView
    public TabView GetActiveView()
    {
      int selectedView = SelectedIndex;
      if (selectedView >= 0 && selectedView < Controls.Count)
        return (TabView) Controls[selectedView];
      return null;
    }

    //TODO: .net 2.0 compiler switch. Method already exists in MultiView
    public void SetActiveView (TabView view)
    {
      int index = Controls.IndexOf (view);
      if (index < 0)
        throw new HttpException (string.Format ("The view {0} cannot be found inside {1}, the ActiveView must be a View control directly inside a MultiView.", (view == null) ? "null" : view.ID, ID));
      SelectedIndex = index;
    }

    //TODO: .net 2.0 compiler switch. Event already exists in MultiView
    public event EventHandler ActiveViewChanged
    {
      add { SelectedIndexChange += value; }
      remove { SelectedIndexChange -= value; }
    }
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
      multiView.ActivateView (view);
    }
  }

  // fields
  private WebTabStrip _tabStrip;
  private TabbedMultiView.MultiView _multiViewInternal;
  private Style _tabStripStyle;
  private Style _activeViewStyle;

  // construction and destruction
  public TabbedMultiView()
  {
    _tabStrip = new WebTabStrip (this);
    _multiViewInternal = new TabbedMultiView.MultiView ();
    _tabStripStyle = new Style();
    _activeViewStyle = new Style();
  }

  // methods and properties
  protected override void CreateChildControls()
  {
    _tabStrip.ID = ID + "_TabStrip";
    _tabStrip.Width = Unit.Percentage (100);
    Controls.Add (_tabStrip);

    _multiViewInternal.ID = ID + "_MultiView"; 
    _multiViewInternal.Width = Unit.Percentage (100);
    Controls.Add (_multiViewInternal);
  }
  
  private void OnTabViewInserted (TabView view)
  {
    EnsureChildControls();

    _tabStrip.Tabs.Add (WebTab.GetSeparator());

    MultiViewTab tab = new MultiViewTab ();
    tab.TabID = view.ID + "_Tab";
    tab.Text = view.Title;
    tab.Icon = view.Icon;
    tab.Target = view.ID;
    _tabStrip.Tabs.Add (tab);
  }

  public void ActivateView (int index)
  {
    ActivateView (Views[index]);
  }

  //TODO: .net2.0 complier switch. TabView is CLS-complient in .net 2.0
  [CLSCompliant (false)]
  public void ActivateView (TabView view)
  {
    MultiViewInternal.SetActiveView (view);
  }

  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Div; }
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

  protected override void RenderContents (HtmlTextWriter writer)
  {
    EnsureChildControls();
  
    _tabStripStyle.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (_tabStripStyle.CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassTabStrip);
    writer.RenderBeginTag (HtmlTextWriterTag.Div);
    _tabStrip.RenderControl (writer);
    writer.RenderEndTag();

    if (ControlHelper.IsDesignMode (this, Context))
    {
      writer.AddStyleAttribute ("border", "solid 1px black");
      writer.AddStyleAttribute ("height", "50pt");
    }
    writer.AddAttribute (HtmlTextWriterAttribute.Width, _multiViewInternal.Width.ToString());
    _activeViewStyle.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (_activeViewStyle.CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassActiveView);
    writer.RenderBeginTag (HtmlTextWriterTag.Div);
    Control view = _multiViewInternal.GetActiveView();
    if (view != null)
    {
      foreach (Control control in view.Controls)
        control.RenderControl (writer);
    }
    writer.RenderEndTag();
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

  [PersistenceMode (PersistenceMode.InnerDefaultProperty)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Browsable (false)]
  public TabViewCollection Views
  { 
    get { return (TabViewCollection) MultiViewInternal.Controls; }
  }

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

  //TODO: .net2.0 complier switch. TabbedMultiView is CLS-complient in .net 2.0
  [CLSCompliant (false)]
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
  ///   <para> Applied only if the <see cref="CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassTabStrip
  {
    get { return "tabbedMultiViewTabStrip"; }
  }

  /// <summary> Gets the CSS-Class applied to the <see cref="TabbedMultiActiveView"/>'s active view. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabbedMultiViewActiveView</c>. </para>
  ///   <para> Applied only if the <see cref="CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassActiveView
  {
    get { return "tabbedMultiViewActiveView"; }
  }
  #endregion
}

}