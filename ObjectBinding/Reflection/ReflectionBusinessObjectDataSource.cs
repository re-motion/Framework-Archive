using System;
using Rubicon.ObjectBinding;

namespace Rubicon.ObjectBinding.Reflection
{
public class ReflectionBusinessObjectDataSource: BusinessObjectDataSource
{
  private string _typeName;
  private IBusinessObject _object;

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
      return (type == null) ? null : new ReflectionBusinessObjectClass (type); 
    }
  }

}
}
