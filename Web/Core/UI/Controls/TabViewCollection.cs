using System;
using System.Web.UI;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

//TODO: .net2.0 complier switch. Inherit from System.Web.UI.ViewCollection
public class TabViewCollection : ControlCollection
{
  //TODO: .net2.0 complier switch. TabbedMultiView is CLS-complient in .net 2.0
  [CLSCompliant (false)]
  public TabViewCollection (TabbedMultiView owner) : base (owner)
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
    Owner.OnTabViewInserted ((TabView) view);
  }

  //TODO: .net2.0 complier switch. TabView is CLS-complient in .net 2.0
  [CLSCompliant (false)]
  public new TabView this[int index]
  {
    get { return (TabView) base[index]; }
  }

  //TODO: .net2.0 complier switch. TabView is CLS-complient in .net 2.0
  [CLSCompliant (false)]
  protected new TabbedMultiView Owner
  {
    get { return (TabbedMultiView) base.Owner; }
  }
}

}
