using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Rubicon.Web.UI.Design;

namespace Rubicon.Web.UI.Controls
{

/// <summary> A collection of <see cref="MenuItem"/> objects. </summary>
[Editor (typeof (MenuItemCollectionEditor), typeof (UITypeEditor))]
public class MenuItemCollection: ControlItemCollection
{
  /// <summary> Initializes a new instance. </summary>
  public MenuItemCollection (Control ownerControl, Type[] supportedTypes)
    : base (ownerControl, supportedTypes)
  {
  }

  /// <summary> Initializes a new instance. </summary>
  public MenuItemCollection (Control ownerControl)
    : this (ownerControl, new Type[] {typeof (MenuItem)})
  {
  }

  public new MenuItem[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (MenuItem[]) arrayList.ToArray (typeof (MenuItem));
  }

  protected internal new MenuItem this[int index]
  {
    get { return (MenuItem) List[index]; }
    set { List[index] = value; }
  }
}

}
