using System;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{

public class TypeWithString: ReflectionBusinessObject
{
  private string _stringValue;
  private string[] _stringArray;

  public string StringValue
  {
    get { return _stringValue; }
    set { _stringValue = value; }
  }

  public string[] StringArray
  {
    get { return _stringArray; }
    set { _stringArray = value; }
  }
}

}
