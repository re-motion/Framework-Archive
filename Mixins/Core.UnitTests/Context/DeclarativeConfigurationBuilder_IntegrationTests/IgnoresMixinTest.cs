using System;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.SampleTypes;

namespace Remotion.Mixins.UnitTests.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class IgnoresMixinTest
  {
    [Test]
    public void BaseClass_HasMixins ()
    {
      Assert.IsTrue (TypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (NullMixin)));
      Assert.IsTrue (TypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (DerivedNullMixin)));
      Assert.IsTrue (TypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (DerivedDerivedNullMixin)));
      Assert.IsTrue (TypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsTrue (TypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod2<object>)));
    }

    [Test]
    public void DerivedClass_ExcludesMixins ()
    {
      Assert.IsTrue (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (NullMixin)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (DerivedNullMixin)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (DerivedDerivedNullMixin)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod2<object>)));
    }

    [Test]
    public void DerivedDerivedClass_ExcludesMixins ()
    {
      Assert.IsTrue (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (NullMixin)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (DerivedNullMixin)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (DerivedDerivedNullMixin)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod<>)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (DerivedClassIgnoringMixin), typeof (GenericMixinWithVirtualMethod2<object>)));
    }
  }
}