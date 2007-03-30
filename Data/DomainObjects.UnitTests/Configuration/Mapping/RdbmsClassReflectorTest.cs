using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class RdbmsClassReflectorTest : ClassReflectorTestBase
  {
    private ClassDefinitionChecker _classDefinitionChecker;
    private ClassDefinitionCollection _classDefinitions;
    private List<RelationReflector> _relations;

    public override void SetUp()
    {
      base.SetUp();

      _classDefinitionChecker = new ClassDefinitionChecker();
      _classDefinitions = new ClassDefinitionCollection();
      _relations = new List<RelationReflector>();

    }

    [Test]
    public void GetMetadata_ForClassWithStorageSpecificIdentifier ()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (ClassHavingStorageSpecificIdentifierAttribute), _classDefinitions, _relations);

      ClassDefinition actual = classReflector.GetMetadata ();

      Assert.IsNotNull (actual);
      Assert.AreEqual (
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassHavingStorageSpecificIdentifierAttribute", 
        actual.ID);
      Assert.AreEqual ("TableName", actual.MyEntityName);
    }

    [Test]
    public void GetMetadata_ForBaseClass()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (ClassWithMixedProperties), _classDefinitions, _relations);
      ClassDefinition expected = CreateClassWithMixedPropertiesClassDefinition();

      ClassDefinition actual = classReflector.GetMetadata();

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
      Assert.AreEqual (1, _relations.Count);
    }

    [Test]
    public void GetMetadata_ForDerivedClass()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (DerivedClassWithMixedProperties), _classDefinitions, _relations);
      ClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition();

      ClassDefinition actual = classReflector.GetMetadata();

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (actual.BaseClass, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
      Assert.AreEqual (1, _relations.Count);
    }

    [Test]
    public void GetMetadata_ForDerivedClassWithBaseClassAlreadyInClassDefinitionCollection()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (DerivedClassWithMixedProperties), _classDefinitions, _relations);
      ClassDefinition expectedBaseClass = CreateClassWithMixedPropertiesClassDefinition();
      _classDefinitions.Add (expectedBaseClass);

      ClassDefinition actual = classReflector.GetMetadata();

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
      Assert.AreEqual (0, _relations.Count);
    }

    [Test]
    public void GetMetadata_ForDerivedClassWithDerivedClassAlreadyInClassDefinitionCollection()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (DerivedClassWithMixedProperties), _classDefinitions, _relations);
      ClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition();
      ClassDefinition expectedBaseClass = expected.BaseClass;
      _classDefinitions.Add (expectedBaseClass);
      _classDefinitions.Add (expected);

      ClassDefinition actual = classReflector.GetMetadata();

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expected, actual);
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
      Assert.AreEqual (0, _relations.Count);
    }

    [Test]
    public void GetMetadata_ForClassWithOneSideRelationProperties()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (ClassWithOneSideRelationProperties), _classDefinitions, _relations);
      ClassDefinition expected = CreateClassWithOneSideRelationPropertiesClassDefinition();

      ClassDefinition actual = classReflector.GetMetadata();

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (ClassWithOneSideRelationProperties)));
      Assert.AreEqual (0, _relations.Count);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The 'Rubicon.Data.DomainObjects.StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definiton.\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute, "
        + "property: Int32")]
    public void GetMetadata_ForDerivedClassHavingAnOverriddenPropertyWithMappingAttribute()
    {
      Type derivedClass = TestDomainFactory.ConfigurationMappingTestDomainWithErrors.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute",
          true,
          false);
      ClassReflector classReflector = new RdbmsClassReflector (derivedClass, _classDefinitions, _relations);

      classReflector.GetMetadata();
    }

    private ClassDefinition CreateClassWithMixedPropertiesClassDefinition()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties",
          "ClassWithMixedProperties",
          "TestDomain",
          typeof (ClassWithMixedProperties),
          false);

      CreatePropertyDefinitionsForClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ClassDefinition CreateDerivedClassWithMixedPropertiesClassDefinition()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithMixedProperties",
          null,
          "TestDomain",
          typeof (DerivedClassWithMixedProperties),
          false,
          CreateClassWithMixedPropertiesClassDefinition ());

      CreatePropertyDefinitionsForDerivedClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithOneSideRelationPropertiesClassDefinition()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties",
          "ClassWithOneSideRelationProperties",
          "TestDomain",
          typeof (ClassWithOneSideRelationProperties),
          false);

      return classDefinition;
    }
  }
}