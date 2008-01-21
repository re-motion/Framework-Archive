using System;
using NUnit.Framework;
using Rubicon.Mixins.Globalization;
using Rubicon.Mixins.UnitTests.Globalization.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Globalization.MixedMultiLingualResourcesTests
{
  [TestFixture]
  public class ExistsResourceTests
  {
    [Test]
    public void NoAttribute ()
    {
      Assert.IsFalse (MixedMultiLingualResources.ExistsResource (typeof (ClassWithoutMultiLingualResourcesAttributes)));
    }

    [Test]
    public void AttributesOnClass ()
    {
      Assert.IsTrue (MixedMultiLingualResources.ExistsResource (typeof (ClassWithMultiLingualResourcesAttributes)));
    }

    [Test]
    public void AttributesOnBase ()
    {
      Assert.IsTrue (MixedMultiLingualResources.ExistsResource (typeof (InheritedClassWithoutMultiLingualResourcesAttributes)));
    }

    [Test]
    public void AttributesOnBaseAndClass ()
    {
      Assert.IsTrue (MixedMultiLingualResources.ExistsResource (typeof (InheritedClassWithMultiLingualResourcesAttributes)));
    }

    [Test]
    public void AttributesFromMixin_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes> ().AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
      {
        Assert.IsTrue (MixedMultiLingualResources.ExistsResource (typeof (ClassWithoutMultiLingualResourcesAttributes)));
      }
    }

    [Test]
    public void AttributesFromMixinsAndBaseAndClass ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
          .EnterScope ())
      {
        Assert.IsTrue (MixedMultiLingualResources.ExistsResource (typeof (InheritedClassWithMultiLingualResourcesAttributes)));
      }
    }

    [Test]
    public void AttributesFromMixins ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
          .EnterScope ())
      {
        Assert.IsTrue (MixedMultiLingualResources.ExistsResource (typeof (ClassWithoutMultiLingualResourcesAttributes)));
      }
    }
  }
}