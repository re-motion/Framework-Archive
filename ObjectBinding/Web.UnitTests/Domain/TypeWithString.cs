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
  private string _firstValue;
  private string _secondValue;

  public TypeWithString ()
  {
  }

  public TypeWithString (string firstValue, string secondValue)
  {
    _firstValue = firstValue;
    _secondValue = secondValue;
  }

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

  public string FirstValue
  {
    get { return _firstValue; }
    set { _firstValue = value; }
  }

  public string SecondValue
  {
    get { return _secondValue; }
    set { _secondValue = value; }
  }
}

}
