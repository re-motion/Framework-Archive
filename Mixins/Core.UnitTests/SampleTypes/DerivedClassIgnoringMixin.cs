using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [IgnoresMixin (typeof (DerivedNullMixin))]
  [IgnoresMixin (typeof (GenericMixinWithVirtualMethod<>))]
  [IgnoresMixin (typeof (GenericMixinWithVirtualMethod2<>))]
  public class DerivedClassIgnoringMixin : BaseClassForDerivedClassIgnoringMixin
  {
  }
}