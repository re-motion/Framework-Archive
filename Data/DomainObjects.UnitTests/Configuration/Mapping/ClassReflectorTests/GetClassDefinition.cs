using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ClassReflectorTests
{
  [TestFixture]
  public class GetClassDefinition : TestBase
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
    public void GetClassDefinition_ForBaseClass ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithMixedProperties));
      ReflectionBasedClassDefinition expected = CreateClassWithMixedPropertiesClassDefinition();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties));
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      _classDefinitionChecker.Check (expected.BaseClass, actual.BaseClass);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (actual.BaseClass, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithBaseClassAlreadyInClassDefinitionCollection ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties));
      ReflectionBasedClassDefinition expectedBaseClass = CreateClassWithMixedPropertiesClassDefinition();
      _classDefinitions.Add (expectedBaseClass);

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithDerivedClassAlreadyInClassDefinitionCollection ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties));
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition();
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
    public void GetClassDefinition_ForClassWithOneSideRelationProperties ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithOneSideRelationProperties));
      ReflectionBasedClassDefinition expected = CreateClassWithOneSideRelationPropertiesClassDefinition();

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
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttribute", actual.ID);
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
    public void GetClassDefinition_ForDerivedClassWithStorageSpecificIdentifierAttribute ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithStorageSpecificIdentifierAttribute));
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithStorageSpecificIdentifierAttributeClassDefinition();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      _classDefinitionChecker.Check (expected.BaseClass, actual.BaseClass);
      Assert.AreEqual (2, _classDefinitions.Count);
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
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "The domain object type cannot redefine the 'Rubicon.Data.DomainObjects.StorageGroupAttribute' already defined on base type "
        + "'Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.BaseClassWithStorageGroupAttribute'.\r\n"
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.Derived2ClassWithStorageGroupAttribute")]
    public void GetClassDefinition_ForDerivedClassWithRedefinedStorageGroupAttribute ()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.Derived2ClassWithStorageGroupAttribute",
          true,
          false);

      ClassReflector classReflector = new ClassReflector (type);

      classReflector.GetClassDefinition (_classDefinitions);
    }

    private ReflectionBasedClassDefinition CreateClassWithMixedPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithMixedProperties",
          "ClassWithMixedProperties",
          c_testDomainProviderID,
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
          c_testDomainProviderID,
          typeof (DerivedClassWithMixedProperties),
          false,
          CreateClassWithMixedPropertiesClassDefinition());

      CreatePropertyDefinitionsForDerivedClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithOneSideRelationPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithOneSideRelationProperties",
          "ClassWithOneSideRelationProperties",
          c_testDomainProviderID,
          typeof (ClassWithOneSideRelationProperties),
          false);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateBaseClassWithoutStorageSpecificIdentifierAttributeDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "BaseClassWithoutStorageSpecificIdentifierAttribute",
          "BaseClassWithoutStorageSpecificIdentifierAttribute",
          c_testDomainProviderID,
          typeof (BaseClassWithoutStorageSpecificIdentifierAttribute),
          true);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateDerivedClassWithStorageSpecificIdentifierAttributeClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "DerivedClassWithStorageSpecificIdentifierAttribute",
          "DerivedClassWithStorageSpecificIdentifierAttribute",
          c_testDomainProviderID,
          typeof (DerivedClassWithStorageSpecificIdentifierAttribute),
          false,
          CreateBaseClassWithoutStorageSpecificIdentifierAttributeDefinition());

      return classDefinition;
    }
  }
}