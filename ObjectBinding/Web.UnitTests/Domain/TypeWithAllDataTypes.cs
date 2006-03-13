using System;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{

public class TypeWithAllDataTypes: ReflectionBusinessObject
{
  private string _stringValue;
  private int _int32Value;

  public TypeWithAllDataTypes ()
  {
  }

  public TypeWithAllDataTypes (string stringValue, int int32Value)
  {
    _stringValue = stringValue;
    _int32Value = int32Value;
  }

  public string StringValue
  {
    get { return _stringValue; }
    set { _stringValue = value; }
  }

  public int Int32Value
  {
    get { return _int32Value; }
    set { _int32Value = value; }
  }
}

}
