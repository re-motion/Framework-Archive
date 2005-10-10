using System;
using System.ComponentModel;
using System.Drawing.Design;

using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
// TODO Doc: 
public class SearchObjectDataSource : BusinessObjectDataSource
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

      return System.Type.GetType (_typeName, true); 
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
      return new SearchObjectClass (type); 
    }
  }
}
}
