using System;
using System.Collections;
using System.Collections.Specialized;
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
//TODO: .net2.0 complier switch. Inherit from System.Web.UI.WebControls.MultiView
[CLSCompliant (false)]
public class TabbedMultiView : Rubicon.Web.UI.Controls.MultiPage
{
  //  constants

  // statics

  // types

  // fields
  private WebTabStrip _tabStrip;

  // construction and destruction
  public TabbedMultiView()
  {
    _tabStrip = new WebTabStrip (this);
  }

  // methods and properties
  protected override ControlCollection CreateControlCollection()
  {
    return new TabViewCollection (this);
  }

  protected override void CreateChildControls()
  {
    _tabStrip.ID = UniqueID + "_TabStrip";
    _tabStrip.Width = Unit.Percentage (100);
  }
  
  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Div; }
  }

  protected override void Render(HtmlTextWriter writer)
  {
    RenderBeginTag (writer);
    RenderContents (writer);
    RenderEndTag (writer);
  }

  protected override void RenderContents(HtmlTextWriter writer)
  {
    _tabStrip.RenderControl (writer);

    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.RenderBeginTag (HtmlTextWriterTag.Div);
    Control view = GetActiveView();
    if (view != null)
      view.RenderControl (writer);
    writer.RenderEndTag();
  }

  //TODO: .net 2.0 compiler switch. Method already exists in MultiView
  public TabView GetActiveView()
  {
    int selectedView = SelectedIndex;
    if (selectedView >= 0 && selectedView < Controls.Count)
      return (TabView) Controls[selectedView];
    return null;
  }

  public override ControlCollection Controls
  {
    get
    {
      EnsureChildControls();
      return base.Controls;
    }
  }

  //TODO: .net 2.0 compiler switch: Keyword new required
  public TabViewCollection TabViews
  { 
    get { return (TabViewCollection) Controls; }
  }

  //TODO: .net 2.0 compiler switch. Event already exists in MultiView
  public event EventHandler ActiveViewChanged
  {
    add { SelectedIndexChange += value; }
    remove { SelectedIndexChange -= value; }
  }

  internal void OnTabViewInserted (TabView view)
  {
    _tabStrip.Tabs.Add (new MultiViewTab (view.ID + "_Tab", view.Title, view.Icon));
    _tabStrip.Tabs.Add (WebTab.GetSeparator());
  }
}


public class MultiViewTab: WebTab
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
    TabView view = (TabView) TabStrip.FindControl (_target);
    TabbedMultiView multiView = (TabbedMultiView) view.Parent;
    int selectedPageView = multiView.Controls.IndexOf (view);
    multiView.SelectedIndex = selectedPageView;
//    if (multiPage != null)
//    {
//      if (_OldMultiPageIndex < 0)
//      {
//        this.SetTargetSelectedIndex();
//      }
//      if ((_OldMultiPageIndex >= 0) && (multiPage.SelectedIndex != _OldMultiPageIndex))
//      {
//        multiPage.FireSelectedIndexChangeEvent();
//      }
//    }
  }

//  private void SetTargetSelectedIndex()
//  {
//    int num1 = this.Tabs.ToArrayIndex(this.SelectedIndex);
//    if (num1 >= 0)
//    {
//      Tab tab1 = (Tab) this.Tabs[num1];
//      MultiPage page1 = this.Target;
//      if (page1 != null)
//      {
//        PageView view1 = (tab1 == null) ? null : tab1.Target;
//        if ((view1 != null) && !view1.Selected)
//        {
//          if (this._OldMultiPageIndex < 0)
//          {
//            this._OldMultiPageIndex = page1.SelectedIndex;
//          }
//          view1.Activate();
//        }
//      }
//    }
//  }

}

}
