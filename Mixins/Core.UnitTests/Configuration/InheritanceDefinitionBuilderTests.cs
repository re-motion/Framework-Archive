using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using ConsoleDumper=Rubicon.Mixins.Validation.ConsoleDumper;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class InheritanceDefinitionBuilderTests
  {
    [Test]
    public void InheritedIntroducedInterfaces ()
    {
      BaseClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IMixinIII4)));
    }

    [Test]
    public void InheritedFaceDependencies ()
    {
      BaseClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinFaceDependingOnInheritedInterface),
          typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII4)));

      MixinDefinition m1 = bt1.Mixins[typeof (MixinFaceDependingOnInheritedInterface)];
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII4)));
    }

    [Test]
    public void InheritedBaseDependencies ()
    {
      BaseClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (BaseType1),
              typeof (MixinBaseDependingOnInheritedInterface),
              typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII4)));

      MixinDefinition m1 = bt1.Mixins[typeof (MixinBaseDependingOnInheritedInterface)];
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII4)));
    }


  }
}
