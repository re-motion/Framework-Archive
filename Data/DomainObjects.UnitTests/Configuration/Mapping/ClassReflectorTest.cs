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
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp()
    {
      base.SetUp();

      _classDefinitionChecker = new ClassDefinitionChecker();
      _classDefinitions = new ClassDefinitionCollection();
    }

    [Test]
    public void GetMetadata_ForBaseClass()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithMixedProperties), _classDefinitions);
      ClassDefinition expected = CreateClassWithMixedPropertiesClassDefinition();

      ClassDefinition actual = classReflector.GetMetadata();

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void GetMetadata_ForDerivedClass ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties), _classDefinitions);
      ClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition ();

      ClassDefinition actual = classReflector.GetMetadata ();

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (actual.BaseClass, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void GetMetadata_ForDerivedClassWithBaseClassAlreadyInClassDefinitionCollection ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties), _classDefinitions);
      ClassDefinition expectedBaseClass = CreateClassWithMixedPropertiesClassDefinition();
      _classDefinitions.Add (expectedBaseClass);

      ClassDefinition actual = classReflector.GetMetadata ();

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    [Test]
    public void GetMetadata_ForDerivedClassWithDerivedClassAlreadyInClassDefinitionCollection ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties), _classDefinitions);
      ClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition ();
      ClassDefinition expectedBaseClass = expected.BaseClass;
      _classDefinitions.Add (expectedBaseClass);
      _classDefinitions.Add (expected);

      ClassDefinition actual = classReflector.GetMetadata ();

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expected, actual);
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    private ClassDefinition CreateClassWithMixedPropertiesClassDefinition()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties",
          "ClassWithMixedProperties",
          "TestDomain",
          typeof (ClassWithMixedProperties));

      CreatePropertyDefinitionsForClassWithMixedProperties(classDefinition);

      return classDefinition;
    }

    private ClassDefinition CreateDerivedClassWithMixedPropertiesClassDefinition ()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithMixedProperties",
          "DerivedClassWithMixedProperties",
          "TestDomain",
          typeof (DerivedClassWithMixedProperties),
          CreateClassWithMixedPropertiesClassDefinition());

      CreatePropertyDefinitionsForDerivedClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private void CreatePropertyDefinitionsForClassWithMixedProperties (ClassDefinition classDefinition)
    {
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
    }

    private void CreatePropertyDefinitionsForDerivedClassWithMixedProperties (ClassDefinition classDefinition)
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