using System;
using NUnit.Framework;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class MixAnalysisTests
  {
    [Test]
    public void MixAttributeIsAnalyzed ()
    {
      Assert.IsTrue (TypeFactory.GetActiveConfiguration (typeof (TargetClassForGlobalMix)).Mixins
          .ContainsKey (typeof (MixinForGlobalMix)));
      Assert.IsTrue (TypeFactory.GetActiveConfiguration (typeof (TargetClassForGlobalMix)).Mixins
          .ContainsKey (typeof (AdditionalDependencyForGlobalMix)));
    }

    [Test]
    public void AdditionalDependenciesAreAnalyzed ()
    {
      Assert.IsTrue (TypeFactory.GetActiveConfiguration (typeof (TargetClassForGlobalMix)).Mixins[typeof (MixinForGlobalMix)]
          .MixinDependencies.ContainsKey (typeof (AdditionalDependencyForGlobalMix)));
    }

    [Test]
    public void SuppressedMixinsAreAnalyzed ()
    {
      Assert.IsFalse (TypeFactory.GetActiveConfiguration (typeof (TargetClassForGlobalMix)).Mixins
          .ContainsKey (typeof (SuppressedMixinForGlobalMix)));
    }
  }
}