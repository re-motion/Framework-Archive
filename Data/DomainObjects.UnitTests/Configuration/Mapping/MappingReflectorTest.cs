using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class MappingReflectorTest:StandardMappingTest
  {
    [Test]
    public void GetResolveTypes()
    {
      MappingReflector mappingReflector = new MappingReflector (GetType ().Assembly);
      Assert.IsTrue (mappingReflector.ResolveTypes);
    }

    [Test]
    [Ignore]
    public void LazyLoadReferencedAssembly()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomain.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.ClassWithPropertyTypeInOtherAssembly", true, false);
      PropertyInfo propertyInfo = type.GetProperty ("Property");

      bool isAssemblyLoadedBeforePropertyTypeAccess = Array.Exists (
          AppDomain.CurrentDomain.GetAssemblies(),
          delegate (Assembly assembly) { return assembly.GetName ().Name == "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ReferencedTestDomain"; });
      Assert.IsFalse (isAssemblyLoadedBeforePropertyTypeAccess, "Assembly is already loaded.");

      Dev.Null = propertyInfo.PropertyType;

      bool isAssemblyLoadedAfterPropertyTypeAccess = Array.Exists (
          AppDomain.CurrentDomain.GetAssemblies(),
          delegate (Assembly assembly) { return assembly.GetName ().Name == "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ReferencedTestDomain"; });
      Assert.IsTrue (isAssemblyLoadedAfterPropertyTypeAccess, "Assembly was not loaded by accessing PropertyInfo.PropertyType.");
    }

    [Test]
    [Ignore]
    public void GetClassDefinitions ()
    {
      MappingReflector mappingReflector = new MappingReflector (GetType().Assembly);

      ClassDefinitionCollection actualClassDefinitions = mappingReflector.GetClassDefinitions ();

      Assert.IsNotNull (actualClassDefinitions);
      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker ();
      classDefinitionChecker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, false, true);
    }
  }
}