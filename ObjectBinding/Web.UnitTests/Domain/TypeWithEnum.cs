using System;
using Rubicon.ObjectBinding.Reflection;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{

public class TypeWithEnum: ReflectionBusinessObject
{
  private TestEnum _enumValue;

  public TestEnum EnumValue
  {
    get { return _enumValue; }
    set { _enumValue = value; }
  }
}

public enum TestEnum
{
  First,
  Second,
  Third
}

}
