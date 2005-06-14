using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> A collection of <see cref="BocListView"/> objects. </summary>
[Editor (typeof (BocListViewCollectionEditor), typeof (UITypeEditor))]
public class BocListViewCollection: BusinessObjectControlItemCollection
{
  /// <summary> Initializes a new instance. </summary>
  public BocListViewCollection (IBusinessObjectBoundWebControl ownerControl)
    : base (ownerControl, new Type[] {typeof (BocListView)})
  {
  }

  public new BocListView[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (BocListView[]) arrayList.ToArray (typeof (BocListView));
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new BocListView this[int index]
  {
    get { return (BocListView) List[index]; }
    set { List[index] = value; }
  }
}

}
