using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> A collection of <see cref="BocColumnDefinition"/> objects. </summary>
[Editor (typeof (BocColumnDefinitionCollectionEditor), typeof (UITypeEditor))]
public class BocColumnDefinitionCollection: BusinessObjectControlItemCollection
{
  /// <summary> Initializes a new instance. </summary>
  public BocColumnDefinitionCollection (IBusinessObjectBoundWebControl ownerControl)
    : base (ownerControl, new Type[] {typeof (BocColumnDefinition)})
  {
  }

  public new BocColumnDefinition[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (BocColumnDefinition[]) arrayList.ToArray (typeof (BocColumnDefinition));
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new BocColumnDefinition this[int index]
  {
    get { return (BocColumnDefinition) List[index]; }
    set { List[index] = value; }
  }
}

}
