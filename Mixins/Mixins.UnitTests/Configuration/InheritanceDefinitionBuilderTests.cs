using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class InheritanceDefinitionBuilderTests
  {
    [Test]
    public void InheritedIntroducedInterfaces ()
    {
      ApplicationDefinition configuration = DefBuilder.Build (typeof (BaseType1), typeof (MixinIntroducingInheritedInterface));
      BaseClassDefinition bt1 = configuration.BaseClasses[typeof (BaseType1)];
      Assert.IsTrue (bt1.IntroducedInterfaces.HasItem (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.IntroducedInterfaces.HasItem (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.IntroducedInterfaces.HasItem (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.IntroducedInterfaces.HasItem (typeof (IMixinIII4)));
    }

    [Test]
    public void InheritedFaceDependencies ()
    {
      ApplicationDefinition configuration = DefBuilder.Build (typeof (BaseType1), typeof (MixinFaceDependingOnInheritedInterface),
          typeof (MixinIntroducingInheritedInterface));
      BaseClassDefinition bt1 = configuration.BaseClasses[typeof (BaseType1)];
      Assert.IsTrue (bt1.RequiredFaceTypes.HasItem (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.RequiredFaceTypes.HasItem (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.RequiredFaceTypes.HasItem (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.RequiredFaceTypes.HasItem (typeof (IMixinIII4)));

      MixinDefinition m1 = bt1.Mixins[typeof (MixinFaceDependingOnInheritedInterface)];
      Assert.IsTrue (m1.ThisDependencies.HasItem (typeof (IMixinIII1)));
      Assert.IsTrue (m1.ThisDependencies.HasItem (typeof (IMixinIII2)));
      Assert.IsTrue (m1.ThisDependencies.HasItem (typeof (IMixinIII3)));
      Assert.IsTrue (m1.ThisDependencies.HasItem (typeof (IMixinIII4)));


      Validation.DefaultLog.ConsoleDumper.DumpLog (Validation.Validator.Validate (configuration));
    }

    [Test]
    public void InheritedBaseDependencies ()
    {
      ApplicationDefinition configuration = DefBuilder.Build (typeof (BaseType1), typeof (MixinBaseDependingOnInheritedInterface),
          typeof (MixinIntroducingInheritedInterface));
      BaseClassDefinition bt1 = configuration.BaseClasses[typeof (BaseType1)];
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
