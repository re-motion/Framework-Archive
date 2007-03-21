using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class MappingReflectorTest
  {
    [Test]
    public void GetResolveTypes()
    {
      MappingReflector mappingReflector = new MappingReflector();
      Assert.IsTrue (mappingReflector.ResolveTypes);
    }

    //[Test]
    //public void GetClassDefinitions()
    //{
    //  Assembly testDomain = TestDomainFactory.ConfigurationMappingTestDomain;
    //  Assert.AreEqual (2, testDomain.GetTypes().Length);

    //  MappingReflector mappingReflector = new MappingReflector();
    //  mappingReflector.GetClassDefinitions();
    //}
  }
}