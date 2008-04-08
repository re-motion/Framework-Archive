using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  [Extends (typeof (BaseClassForDerivedClassIgnoredByMixin))]
  [IgnoresClass (typeof (DerivedClassIgnoredByMixins))]
  [IgnoresClass (typeof (GenericClassForMixinIgnoringDerivedClass<>))]
  [IgnoresClass (typeof (ClosedGenericClassForMixinIgnoringDerivedClass<int>))]
  public class MixinIgnoringDerivedClass
  {
  }
}