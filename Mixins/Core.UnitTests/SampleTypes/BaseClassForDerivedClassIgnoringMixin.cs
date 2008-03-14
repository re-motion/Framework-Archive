using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [Uses (typeof (NullMixin))]
  [Uses (typeof (DerivedNullMixin))]
  [Uses (typeof (DerivedDerivedNullMixin))]
  [Uses (typeof (GenericMixinWithVirtualMethod<>))]
  [Uses (typeof (GenericMixinWithVirtualMethod2<object>))]
  public class BaseClassForDerivedClassIgnoringMixin
  {
  }
}