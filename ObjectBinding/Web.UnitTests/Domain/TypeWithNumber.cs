using System;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding.Reflection;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{

public class TypeWithNumber: ReflectionBusinessObject
{
  private int _intValue;
  private NaInt32 _naInt32Value;

  public int IntValue
  {
    get { return _intValue; }
    set { _intValue = value; }
  }

  public NaInt32 NaInt32Value
  {
    get { return _naInt32Value; }
    set { _naInt32Value = value; }
  }
}

}
