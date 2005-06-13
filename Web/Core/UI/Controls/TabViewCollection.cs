using System;
using System.ComponentModel;
using System.Web.UI;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

#if ! net20
public class TabViewCollection: ControlCollection
#else
public class TabViewCollection: System.Web.UI.WebControls.ViewCollection
#endif
{
  public TabViewCollection (Control owner)
    : this ((TabbedMultiView.MultiView) owner)
  {
  }

  internal TabViewCollection (TabbedMultiView.MultiView owner)
    : base (owner)
  {
  }

  public override void Add (Control control)
  {
    ArgumentUtility.CheckNotNullAndType ("control", control, typeof (TabView));
    TabView view = (TabView) control;
    view.OnInsert (Owner);
    base.Add (view);
    Owner.OnTabViewInserted (view);
  }

  public override void AddAt(int index, Control control)
  {
    ArgumentUtility.CheckNotNullAndType ("control", control, typeof (TabView));
    TabView view = (TabView) control;
    view.OnInsert (Owner);
    base.AddAt (index, view);
    Owner.OnTabViewInserted (view);
  }

  public override void Remove (Control control)
  {
    ArgumentUtility.CheckNotNullAndType ("control", control, typeof (TabView));
    TabView view = (TabView) control;
    Owner.OnTabViewRemove (view);
    base.Remove (control);
    Owner.OnTabViewRemoved (view);
  }

  public override void RemoveAt (int index)
  {
    if (index < 0 || index > this.Count)
      throw new ArgumentOutOfRangeException ("index");
    TabView view = (TabView) this[index];
    Owner.OnTabViewRemove (view);
    base.RemoveAt (index);
    Owner.OnTabViewRemoved (view);
  }

#if net20
  [CLSCompliant (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new TabView this[int index]
  {
    get { return (TabView) base[index]; }
  }
#endif

#if ! net20
  [CLSCompliant (false)]
#endif
  private new TabbedMultiView.MultiView Owner
  {
    get { return (TabbedMultiView.MultiView) base.Owner; }
  }
}

}
