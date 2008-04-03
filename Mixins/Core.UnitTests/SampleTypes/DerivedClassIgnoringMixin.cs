using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [IgnoresMixin (typeof (DerivedNullMixin))]
  [IgnoresMixin (typeof (GenericMixinWithVirtualMethod<>))]
  [IgnoresMixin (typeof (GenericMixinWithVirtualMethod2<>))]
  public class DerivedClassIgnoringMixin : BaseClassForDerivedClassIgnoringMixin
  {
  }
}