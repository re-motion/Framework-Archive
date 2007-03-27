using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ClassReflectorTest: StandardMappingTest
  {
    private ClassDefinitionChecker _classDefinitionChecker;

    public override void SetUp()
    {
      base.SetUp();

      _classDefinitionChecker = new ClassDefinitionChecker();
    }

    [Test]
    public void GetMetadata()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithMixedProperties));
      ClassDefinition expected = CreateClassWithMixedPropertiesClassDefinition();

      ClassDefinition actual = classReflector.GetMetadata();

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
    }

    private ClassDefinition CreateClassWithMixedPropertiesClassDefinition()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties",
          "ClassWithMixedProperties",
          "TestDomain",
          typeof (ClassWithMixedProperties));

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
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.UnidirectionalOneToOne",
              "UnidirectionalOneToOne",
              TypeInfo.ObjectIDMappingTypeName,
              true,
              true,
              NaInt32.Null,
              true));

      return classDefinition;
    }
  }
}