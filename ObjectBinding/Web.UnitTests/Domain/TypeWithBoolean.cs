using System;
using Rubicon.ObjectBinding.Reflection;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{

public class TypeWithBoolean: ReflectionBusinessObject
{
  private bool _booleanValue;
  private bool? _nullableBooleanValue;

  public bool BooleanValue
  {
    get { return _booleanValue; }
    set { _booleanValue = value; }
  }

  public bool? NullableBooleanValue
  {
    get { return _nullableBooleanValue; }
    set { _nullableBooleanValue = value; }
  }
}

}
