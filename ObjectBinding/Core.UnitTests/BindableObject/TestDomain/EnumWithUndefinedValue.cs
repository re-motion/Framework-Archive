using System;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [UndefinedEnumValue (EnumWithUndefinedValue.UndefinedValue)]
  public enum EnumWithUndefinedValue
  {
    Value1 = 1,
    Value2 = 2,
    Value3 = 3,
    UndefinedValue = -1
  }
}