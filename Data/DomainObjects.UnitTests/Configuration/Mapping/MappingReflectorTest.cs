using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class MappingReflectorTest: ReflectionBasedMappingTest
  {
    [Test]
    public void GetResolveTypes()
    {
      IMappingLoader mappingReflector = new MappingReflector (GetType().Assembly);
      Assert.IsTrue (mappingReflector.ResolveTypes);
    }

    [Test]
    [Ignore ("Not implemented: All assemblies must be available during start-up.")]
    public void LazyLoadReferencedAssembly()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomain.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.ClassWithPropertyTypeInOtherAssembly", true, false);
      PropertyInfo propertyInfo = type.GetProperty ("Property");

      bool isAssemblyLoadedBeforePropertyTypeAccess = Array.Exists (
          AppDomain.CurrentDomain.GetAssemblies(),
          delegate (Assembly assembly) { return assembly.GetName().Name == "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ReferencedTestDomain"; });
      Assert.IsFalse (isAssemblyLoadedBeforePropertyTypeAccess, "Assembly is already loaded.");

      Dev.Null = propertyInfo.PropertyType;

      bool isAssemblyLoadedAfterPropertyTypeAccess = Array.Exists (
          AppDomain.CurrentDomain.GetAssemblies(),
          delegate (Assembly assembly) { return assembly.GetName().Name == "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ReferencedTestDomain"; });
      Assert.IsTrue (isAssemblyLoadedAfterPropertyTypeAccess, "Assembly was not loaded by accessing PropertyInfo.PropertyType.");
    }

    [Test]
    [Ignore ("TODO: MK: Fix this.")]
    public void GetClassDefinitions ()
    {
      MappingReflector mappingReflector = new MappingReflector (GetType().Assembly);

      ClassDefinitionCollection actualClassDefinitions = mappingReflector.GetClassDefinitions();

      Assert.IsNotNull (actualClassDefinitions);
      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker();
      classDefinitionChecker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, false, true);
    }

    [Test]
    [Ignore ("TODO: MK: Fix this.")]
    public void GetRelationDefinitions()
    {
      MappingReflector mappingReflector = new MappingReflector (GetType().Assembly);

      ClassDefinitionCollection actualClassDefinitions = mappingReflector.GetClassDefinitions();
      RelationDefinitionCollection actualRelationDefinitions = mappingReflector.GetRelationDefinitions (actualClassDefinitions);

      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker();
      classDefinitionChecker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, false, true);

      RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check (TestMappingConfiguration.Current.RelationDefinitions, actualRelationDefinitions, true);
    }
  }
}