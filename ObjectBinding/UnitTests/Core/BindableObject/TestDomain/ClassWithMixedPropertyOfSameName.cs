using System;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObject]
  [Uses (typeof (MixinAddingProperty))]
  public class ClassWithMixedPropertyOfSameName
  {
    private string _mixedProperty;

    public string MixedProperty
    {
      get { return _mixedProperty; }
      set { _mixedProperty = value; }
    }
  }
}
