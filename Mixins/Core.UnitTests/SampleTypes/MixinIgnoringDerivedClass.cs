using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [Extends (typeof (BaseClassForDerivedClassIgnoredByMixin))]
  [IgnoresClass (typeof (DerivedClassIgnoredByMixins))]
  [IgnoresClass (typeof (GenericClassForMixinIgnoringDerivedClass<>))]
  [IgnoresClass (typeof (ClosedGenericClassForMixinIgnoringDerivedClass<int>))]
  public class MixinIgnoringDerivedClass
  {
  }
}