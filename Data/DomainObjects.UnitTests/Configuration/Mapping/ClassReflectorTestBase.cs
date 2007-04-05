using System;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  public class ClassReflectorTestBase: LegacyMappingTest
  {
    protected void CreatePropertyDefinitionsForClassWithMixedProperties (ClassDefinition classDefinition)
    {
      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedPropertiesNotInMapping.Boolean",
              "Boolean",
              "boolean",
              true,
              false,
              NaInt32.Null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.Int32",
              "Int32",
              "int32",
              true,
              false,
              NaInt32.Null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.String",
              "String",
              "string",
              true,
              true,
              NaInt32.Null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.PrivateString",
              "PrivateString",
              "string",
              true,
              true,
              NaInt32.Null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.UnidirectionalOneToOne",
              "UnidirectionalOneToOneID",
              TypeInfo.ObjectIDMappingTypeName,
              true,
              true,
              NaInt32.Null,
              true));
    }

    protected void CreatePropertyDefinitionsForDerivedClassWithMixedProperties (ClassDefinition classDefinition)
    {
      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithMixedProperties.String",
              "NewString",
              "string",
              true,
              true,
              NaInt32.Null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithMixedProperties.PrivateString",
              "DerivedPrivateString",
              "string",
              true,
              true,
              NaInt32.Null,
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithMixedProperties.OtherString",
              "OtherString",
              "string",
              true,
              true,
              NaInt32.Null,
              true));
    }
  }
}