using System;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ClassReflectorTests
{
  public class TestBaseForHierarchyWithoutStorageGroup : StandardMappingTest
  {
    protected void CreatePropertyDefinitionsForClassWithoutStorageGroupWithMixedProperties (ReflectionBasedClassDefinition classDefinition)
    {
      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.Int32",
              "Int32",
              typeof (int),
              null,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.String",
              "String",
              typeof (string),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.PrivateString",
              "PrivateString",
              typeof (string),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.UnidirectionalOneToOne",
              "UnidirectionalOneToOneID",
              typeof (ObjectID),
              true,
              null,
              true));
    }

    protected void CreatePropertyDefinitionsForDerivedClassWithoutStorageGroupWithMixedProperties (ReflectionBasedClassDefinition classDefinition)
    {
      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithoutStorageGroupWithMixedProperties.String",
              "NewString",
              typeof (string),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithoutStorageGroupWithMixedProperties.PrivateString",
              "DerivedPrivateString",
              typeof (string),
              true,
              null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithoutStorageGroupWithMixedProperties.OtherString",
              "OtherString",
              typeof (string),
              true,
              null,
              true));
    }
  }
}