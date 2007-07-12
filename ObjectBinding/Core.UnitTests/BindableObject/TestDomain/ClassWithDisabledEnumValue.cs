using System;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithDisabledEnumValue
  {
    private TestEnum _disabledFromProperty;

    public ClassWithDisabledEnumValue ()
    {
    }

    [DisableEnumValues(TestEnum.Value1)]
    public TestEnum DisabledFromProperty
    {
      get { return _disabledFromProperty; }
      set { _disabledFromProperty = value; }
    }
  }
}