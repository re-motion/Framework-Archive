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
  public class RdbmsClassReflectorTest: ClassReflectorTestBase
  {
    private ClassDefinitionChecker _classDefinitionChecker;
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinitionChecker = new ClassDefinitionChecker();
      _classDefinitions = new ClassDefinitionCollection();
    }

    [Test]
    public void GetClassDefinition_ForClassHavingClassIDAttribute()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassHavingClassIDAttribute));

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttribute", actual.ID);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttribute", actual.MyEntityName);
    }

    [Test]
    public void GetClassDefinition_ForClassWithStorageSpecificIdentifierAttribute()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (ClassHavingStorageSpecificIdentifierAttribute));

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassHavingStorageSpecificIdentifierAttribute", actual.ID);
      Assert.AreEqual ("ClassHavingStorageSpecificIdentifierAttributeTable", actual.MyEntityName);
    }

    [Test]
    public void GetClassDefinition_ForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute));

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute", actual.ID);
      Assert.AreEqual ("ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable", actual.MyEntityName);
    }

    [Test]
    public void GetClassDefinition_ForBaseClass()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (ClassWithMixedProperties));
      ClassDefinition expected = CreateClassWithMixedPropertiesClassDefinition();

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (DerivedClassWithMixedProperties));
      ClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition();

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (actual.BaseClass, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithBaseClassAlreadyInClassDefinitionCollection()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (DerivedClassWithMixedProperties));
      ClassDefinition expectedBaseClass = CreateClassWithMixedPropertiesClassDefinition();
      _classDefinitions.Add (expectedBaseClass);

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithDerivedClassAlreadyInClassDefinitionCollection()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (DerivedClassWithMixedProperties));
      ClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition();
      ClassDefinition expectedBaseClass = expected.BaseClass;
      _classDefinitions.Add (expectedBaseClass);
      _classDefinitions.Add (expected);

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expected, actual);
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    [Test]
    public void GetClassDefinition_ForClassWithOneSideRelationProperties()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (ClassWithOneSideRelationProperties));
      ClassDefinition expected = CreateClassWithOneSideRelationPropertiesClassDefinition();

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (ClassWithOneSideRelationProperties)));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The 'Rubicon.Data.DomainObjects.StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definiton.\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute, "
        + "property: Int32")]
    public void GetClassDefinition_ForDerivedClassHavingAnOverriddenPropertyWithMappingAttribute()
    {
      Type derivedClass = TestDomainFactory.ConfigurationMappingTestDomainWithErrors.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute",
          true,
          false);
      ClassReflector classReflector = new RdbmsClassReflector (derivedClass);

      classReflector.GetClassDefinition (_classDefinitions);
    }

    private ClassDefinition CreateClassWithMixedPropertiesClassDefinition()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithMixedProperties",
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
          "DerivedClassWithMixedProperties",
          null,
          "TestDomain",
          typeof (DerivedClassWithMixedProperties),
          false,
          CreateClassWithMixedPropertiesClassDefinition());

      CreatePropertyDefinitionsForDerivedClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithOneSideRelationPropertiesClassDefinition()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithOneSideRelationProperties",
          "ClassWithOneSideRelationProperties",
          "TestDomain",
          typeof (ClassWithOneSideRelationProperties),
          false);

      return classDefinition;
    }
  }
}