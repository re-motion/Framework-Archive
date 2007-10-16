using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyFinderTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class PropertyFinderBaseMixinsTest : PropertyFinderBaseTestBase
  {
    [Test]
    public void FindPropertyInfos_ForInheritanceRoot ()
    {
      PropertyFinderBase propertyFinder = new StubPropertyFinderBase (typeof (TargetClassA), true);

      Assert.That (
          propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (typeof (TargetClassA))),
          Is.EquivalentTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (TargetClassBase), "P0"),
                      GetProperty (typeof (MixinBase), "P0a"),
                      GetProperty (typeof (TargetClassA), "P1"),
                      GetProperty (typeof (TargetClassA), "P2"),
                      GetProperty (typeof (MixinA), "P5"),
                      GetProperty (typeof (MixinC), "P7"),
                      GetProperty (typeof (MixinD), "P8"),
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForDerived ()
    {
      PropertyFinderBase propertyFinder = new StubPropertyFinderBase (typeof (TargetClassB), false);

      Assert.That (
          propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (typeof (TargetClassB))),
          Is.EquivalentTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (TargetClassB), "P3"),
                      GetProperty (typeof (TargetClassB), "P4"),
                      GetProperty (typeof (MixinB), "P6"),
                      GetProperty (typeof (MixinE), "P9"),
                  }));
    }
  }
}