using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using ConsoleDumper=Mixins.Validation.ConsoleDumper;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class InheritanceDefinitionBuilderTests
  {
    [Test]
    public void InheritedIntroducedInterfaces ()
    {
      BaseClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.IntroducedInterfaces.HasItem (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.IntroducedInterfaces.HasItem (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.IntroducedInterfaces.HasItem (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.IntroducedInterfaces.HasItem (typeof (IMixinIII4)));
    }

    [Test]
    public void InheritedFaceDependencies ()
    {
      BaseClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinFaceDependingOnInheritedInterface),
          typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.RequiredFaceTypes.HasItem (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.RequiredFaceTypes.HasItem (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.RequiredFaceTypes.HasItem (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.RequiredFaceTypes.HasItem (typeof (IMixinIII4)));

      MixinDefinition m1 = bt1.Mixins[typeof (MixinFaceDependingOnInheritedInterface)];
      Assert.IsTrue (m1.ThisDependencies.HasItem (typeof (IMixinIII1)));
      Assert.IsTrue (m1.ThisDependencies.HasItem (typeof (IMixinIII2)));
      Assert.IsTrue (m1.ThisDependencies.HasItem (typeof (IMixinIII3)));
      Assert.IsTrue (m1.ThisDependencies.HasItem (typeof (IMixinIII4)));
    }

    [Test]
    public void InheritedBaseDependencies ()
    {
      BaseClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (BaseType1),
              typeof (MixinBaseDependingOnInheritedInterface),
              typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.HasItem (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.HasItem (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.HasItem (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.HasItem (typeof (IMixinIII4)));

      MixinDefinition m1 = bt1.Mixins[typeof (MixinBaseDependingOnInheritedInterface)];
      Assert.IsTrue (m1.BaseDependencies.HasItem (typeof (IMixinIII1)));
      Assert.IsTrue (m1.BaseDependencies.HasItem (typeof (IMixinIII2)));
      Assert.IsTrue (m1.BaseDependencies.HasItem (typeof (IMixinIII3)));
      Assert.IsTrue (m1.BaseDependencies.HasItem (typeof (IMixinIII4)));
    }


  }
}
