using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (BaseClassForDerivedClassIgnoredByMixin))]
  [IgnoresClass (typeof (DerivedClassIgnoredByMixins))]
  [IgnoresClass (typeof (GenericClassForMixinIgnoringDerivedClass<>))]
  [IgnoresClass (typeof (ClosedGenericClassForMixinIgnoringDerivedClass<int>))]
  public class MixinIgnoringDerivedClass
  {
  }
}