using System;
using System.ComponentModel;
using System.Drawing.Design;

using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.ObjectBinding.Design;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{

public class DomainObjectDataSource: BusinessObjectDataSource
{
  private string _typeName;
  private IBusinessObject _object;

  [Editor (typeof (ClassPickerEditor), typeof (UITypeEditor))]
  public string TypeName
  {
    get { return _typeName; }
    set { _typeName = value; }
  }

  public Type Type
  {
    get 
    {
      if (_typeName == null || _typeName.Length == 0)
        return null;
      return System.Type.GetType (_typeName); 
    }
  }

  public override IBusinessObject BusinessObject
  {
    get { return _object; }
    set { _object = value; }
  }

  public override IBusinessObjectClass BusinessObjectClass
  {
    get 
    { 
      Type type = Type;
      return (type == null) ? null : new DomainObjectClass (type); 
    }
  }

}
}
