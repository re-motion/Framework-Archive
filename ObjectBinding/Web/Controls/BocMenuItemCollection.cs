using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> A collection of <see cref="BocMenuItem"/> objects. </summary>
[Editor (typeof (BocMenuItemCollectionEditor), typeof (UITypeEditor))]
public class BocMenuItemCollection: WebMenuItemCollection
{
  /// <summary> Initializes a new instance. </summary>
  public BocMenuItemCollection (IBusinessObjectBoundWebControl ownerControl)
    : base ((Control) ownerControl, new Type[] {typeof (BocMenuItem)})
  {
  }

  public new BocMenuItem[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (BocMenuItem[]) arrayList.ToArray (typeof (BocMenuItem));
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new BocMenuItem this[int index]
  {
    get { return (BocMenuItem) List[index]; }
    set { List[index] = value; }
  }
}

}
