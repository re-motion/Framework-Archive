using System;
using NUnit.Framework;
using Rubicon.Mixins.UnitTests.SampleTypes;

//[assembly: Mix (typeof (BaseTypeForGlobalMixAttribute), typeof (MixinForGlobalMixAttribute),
//    AdditionalDependencies = new Type[] { typeof (AdditionalDependencyForGlobalMixAttribute) },
//    SuppressedMixins = new Type[] { typeof (SuppressedMixinForGlobalMixAttribute) })]

//namespace Rubicon.Mixins.UnitTests.Configuration.Context.DeclarativeConfigurationBuilder_IntegrationTests
//{
//  [TestFixture]
//  public class MixAnalysisTests
//  {
//    [Test]
//    public void MixAttributeIsAnalyzed ()
//    {
//      Assert.IsTrue (TypeFactory.GetActiveConfiguration (typeof (BaseTypeForGlobalMixAttribute)).Mixins
//          .ContainsKey (typeof (MixinForGlobalMixAttribute)));
//    }

//    [Test]
//    public void AdditionalDependenciesAreAnalyzed ()
//    {
//      Assert.IsTrue (TypeFactory.GetActiveConfiguration (typeof (BaseTypeForGlobalMixAttribute)).Mixins[typeof (MixinForGlobalMixAttribute)]
//          .MixinDependencies.ContainsKey (typeof (AdditionalDependenyForGlobalMixAttribute)));
//    }

//    [Test]
//    public void SuppressedMixinsAreAnalyzed ()
//    {
//      Assert.IsFalse (TypeFactory.GetActiveConfiguration (typeof (BaseTypeForGlobalMixAttribute)).Mixins
//          .ContainsKey (typeof (SuppressedMixinForGlobalMixAttribute)));
//    }
//  }
//}