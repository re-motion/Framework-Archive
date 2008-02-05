using System;
using Rubicon.Globalization;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObject]
  [MultiLingualResources ("Rubicon.ObjectBinding.UnitTests.Core.Globalization.ClassWithMixedPropertyAndResources")]
  [Uses (typeof (MixinAddingProperty))]
  public class ClassWithMixedPropertyAndResources
  {
    private string _value1;

    public string Value1
    {
      get { return _value1; }
      set { _value1 = value; }
    }
  }
}
