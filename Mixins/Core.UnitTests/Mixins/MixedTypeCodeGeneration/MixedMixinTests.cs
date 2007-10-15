using System;
using NUnit.Framework;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Mixins.MixedTypeCodeGeneration
{
  [TestFixture]
  public class MixedMixinTests
  {
    [Test]
    public void DoubleMixinOverrides_CreateMixinInstance ()
    {
      MixinMixingClass instance = ObjectFactory.Create<MixinMixingClass> ().With ();
      Assert.IsNotNull (Mixin.Get<MixinMixingMixin> (instance));
    }

    [Test]
    public void DoubleMixinOverrides_CreateClassInstance ()
    {
      ClassWithMixedMixin instance = ObjectFactory.Create<ClassWithMixedMixin> ().With();
      Assert.IsNotNull (Mixin.Get<MixinMixingClass> (instance));
      Assert.IsNotNull (Mixin.Get<MixinMixingMixin> (Mixin.Get<MixinMixingClass> (instance)));

      Assert.AreEqual ("MixinMixingMixin-MixinMixingClass-ClassWithMixedMixin.StringMethod (3)", instance.StringMethod (3));
    }
  }
}
