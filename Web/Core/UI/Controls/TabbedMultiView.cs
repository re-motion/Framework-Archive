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
public class TabbedMultiView: WebControl
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

  // construction and destruction
  public TabbedMultiView()
  {
    _tabStrip = new WebTabStrip (this);
    _multiViewInternal = new TabbedMultiView.MultiView ();
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
    MultiViewTab tab = new MultiViewTab ();
    tab.TabID = view.ID + "_Tab";
    tab.Text = view.Title;
    tab.Icon = view.Icon;
    tab.Target = view.ID;
    _tabStrip.Tabs.Add (tab);
    _tabStrip.Tabs.Add (WebTab.GetSeparator());
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

  protected override void RenderContents (HtmlTextWriter writer)
  {
    EnsureChildControls();
    _tabStrip.RenderControl (writer);
    writer.AddAttribute (HtmlTextWriterAttribute.Width, _multiViewInternal.Width.ToString());
    writer.RenderBeginTag (HtmlTextWriterTag.Div);
    Control view = _multiViewInternal.GetActiveView();
    if (view != null)
      view.RenderControl (writer);
    writer.RenderEndTag();
  }

  public override ControlCollection Controls
  {
    get
    {
      EnsureChildControls();
      return base.Controls;
    }
  }

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
}

}