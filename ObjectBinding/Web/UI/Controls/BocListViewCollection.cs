using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> A collection of <see cref="BocColumnDefinitionSet"/> objects. </summary>
[Editor (typeof (BocColumnDefinitionSetCollectionEditor), typeof (UITypeEditor))]
public class BocColumnDefinitionSetCollection: ControlItemCollection
{
  /// <summary> Initializes a new instance. </summary>
  public BocColumnDefinitionSetCollection (IBusinessObjectBoundWebControl ownerControl)
    : base ((Control) ownerControl, new Type[] {typeof (BocColumnDefinitionSet)})
  {
  }

  public new BocColumnDefinitionSet[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (BocColumnDefinitionSet[]) arrayList.ToArray (typeof (BocColumnDefinitionSet));
  }

  public new BocColumnDefinitionSet this[int index]
  {
    get { return (BocColumnDefinitionSet) base[index]; }
    set { base[index] = value; }
  }
}

}
