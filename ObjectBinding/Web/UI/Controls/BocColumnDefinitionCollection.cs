using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> A collection of <see cref="BocColumnDefinition"/> objects. </summary>
[Editor (typeof (BocColumnDefinitionCollectionEditor), typeof (UITypeEditor))]
public class BocColumnDefinitionCollection: ControlItemCollection
{
  /// <summary> Initializes a new instance. </summary>
  public BocColumnDefinitionCollection (IBusinessObjectBoundWebControl ownerControl)
    : base ((Control) ownerControl, new Type[] {typeof (BocColumnDefinition)})
  {
  }

  public new BocColumnDefinition[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (BocColumnDefinition[]) arrayList.ToArray (typeof (BocColumnDefinition));
  }

  public new BocColumnDefinition this[int index]
  {
    get { return (BocColumnDefinition) base[index]; }
    set { base[index] = value; }
  }
}

}
