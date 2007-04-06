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
  public class ClassReflectorTest: ClassReflectorTestBase
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
    public void GetClassDefinition_ForBaseClass()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithMixedProperties));
      ReflectionBasedClassDefinition expected = CreateClassWithMixedPropertiesClassDefinition ();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties));
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition ();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (actual.BaseClass, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithBaseClassAlreadyInClassDefinitionCollection()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties));
      ReflectionBasedClassDefinition expectedBaseClass = CreateClassWithMixedPropertiesClassDefinition ();
      _classDefinitions.Add (expectedBaseClass);

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithDerivedClassAlreadyInClassDefinitionCollection()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties));
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition ();
      ReflectionBasedClassDefinition expectedBaseClass = expected.BaseClass;
      _classDefinitions.Add (expectedBaseClass);
      _classDefinitions.Add (expected);

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expected, actual);
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    [Test]
    public void GetClassDefinition_ForClassWithOneSideRelationProperties()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithOneSideRelationProperties));
      ReflectionBasedClassDefinition expected = CreateClassWithOneSideRelationPropertiesClassDefinition ();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (ClassWithOneSideRelationProperties)));
    }

    [Test]
    public void GetClassDefinition_ForClassHavingClassIDAttribute ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassHavingClassIDAttribute));

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttribute",actual.ID);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttribute", actual.MyEntityName);
    }

    [Test]
    public void GetClassDefinition_ForClassWithStorageSpecificIdentifierAttribute ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassHavingStorageSpecificIdentifierAttribute));

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassHavingStorageSpecificIdentifierAttribute", actual.ID);
      Assert.AreEqual ("ClassHavingStorageSpecificIdentifierAttributeTable", actual.MyEntityName);
    }

    [Test]
    public void GetClassDefinition_ForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute));

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute", actual.ID);
      Assert.AreEqual ("ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable", actual.MyEntityName);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The 'Rubicon.Data.DomainObjects.StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definiton.\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute, "
        + "property: Int32")]
    public void GetClassDefinition_ForDerivedClassHavingAnOverriddenPropertyWithMappingAttribute()
    {
      Type derivedClass = TestDomainFactory.ConfigurationMappingTestDomainWithErrors.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute",
          true,
          false);
      ClassReflector classReflector = new ClassReflector (derivedClass);

      classReflector.GetClassDefinition(_classDefinitions);
    }

    [Test]
    public void GetClassDefinition_ForClassWithStorageProviderFromStorageGroup ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithStorageProviderFromStorageGroup));
      ReflectionBasedClassDefinition expected = CreateClassWithStorageProviderFromStorageGroup ();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
    }

    //TODO: Explicit default provider
    [Test]
    public void GetClassDefinition_ForClassWithDefaultStorageProvider ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithDefaultStorageProvider));
      ReflectionBasedClassDefinition expected = CreateClassWithDefaultStorageProvider ();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
    }

    private ReflectionBasedClassDefinition CreateClassWithMixedPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithMixedProperties",
          "ClassWithMixedProperties",
          "TestDomain",
          typeof (ClassWithMixedProperties),
          false);

      CreatePropertyDefinitionsForClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateDerivedClassWithMixedPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "DerivedClassWithMixedProperties",
          "DerivedClassWithMixedProperties",
          "TestDomain",
          typeof (DerivedClassWithMixedProperties),
          false,
          CreateClassWithMixedPropertiesClassDefinition ());

      CreatePropertyDefinitionsForDerivedClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithOneSideRelationPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithOneSideRelationProperties",
          "ClassWithOneSideRelationProperties",
          "TestDomain",
          typeof (ClassWithOneSideRelationProperties),
          false);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithDefaultStorageProvider ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithDefaultStorageProvider",
          "ClassWithDefaultStorageProvider",
          "TestDomain",
          typeof (ClassWithDefaultStorageProvider),
          false);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithStorageProviderFromStorageGroup ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithStorageProviderFromStorageGroup",
          "ClassWithStorageProviderFromStorageGroup",
          "UnitTestStorageProviderStub",
          typeof (ClassWithStorageProviderFromStorageGroup),
          false);

      return classDefinition;
    }
  }
}