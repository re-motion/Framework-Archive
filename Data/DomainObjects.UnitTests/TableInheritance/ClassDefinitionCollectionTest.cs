using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  [Ignore]
  public class ClassDefinitionCollectionTest : TableInheritanceMappingTest
  {
    // types

    // static members and constants

    // member fields

    private ClassDefinitionCollection _classDefinitions;

    // construction and disposing

    public ClassDefinitionCollectionTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _classDefinitions = new ClassDefinitionCollection ();
    }

    [Test]
    public void ValidateAbstractClass ()
    {
      ClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", null, c_testDomainProviderID, typeof (Person), false);
      _classDefinitions.Add (personClass);

      try
      {
        _classDefinitions.Validate ();
        Assert.Fail ("MappingException was expected.");
      }
      catch (MappingException ex)
      {
        string expectedMessage = string.Format (
            "Type '{0}' must be abstract, because neither class 'Person' nor its base classes specify an entity name.", 
            typeof (Person).AssemblyQualifiedName);

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    public void ValidateAbstractClassHandlesNullEntityNameWithInherited ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase), false);
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ((string) "Person", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Person), (bool) false, domainBaseClass);
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", null, c_testDomainProviderID, typeof (Customer), false, personClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (customerClass);      

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateAbstractClassHandlesSameInheritedEntityName ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ((string) "Person", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Person), (bool) false);

      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition (
          (string) "Customer", personClass.MyEntityName, (string) c_testDomainProviderID, typeof (Customer), (bool) false, personClass);

      _classDefinitions.Add (personClass);
      _classDefinitions.Add (customerClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
        ExpectedMessage = "Class 'Customer' must not specify an entity name 'DifferentEntityNameThanBaseClass'"
        + " which is different from inherited entity name 'TableInheritance_Person'.")]
    public void ValidateWithDifferentEntityNames ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ((string) "Person", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Person), (bool) false);

      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition (
          (string) "Customer", (string) "DifferentEntityNameThanBaseClass", (string) c_testDomainProviderID, typeof (Customer), (bool) false, personClass);

      _classDefinitions.Add (personClass);
      _classDefinitions.Add (customerClass);

      _classDefinitions.Validate ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Validate cannot be invoked, because class 'Person' is a base class of a class in the collection,"
        + " but the base class is not part of the collection itself.")]
    public void ValidateWithBaseClassNotInCollection ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ((string) "Person", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Person), (bool) false);
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", null, c_testDomainProviderID, typeof (Customer), false, personClass);

      _classDefinitions.Add (customerClass);
      _classDefinitions.Validate ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Validate cannot be invoked, because class 'Customer' is a derived class of 'Person', but is not part of the collection itself.")]
    public void ValidateWithDerivedClassNotInCollection ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ((string) "Person", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Person), (bool) false);
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", null, c_testDomainProviderID, typeof (Customer), false, personClass);

      _classDefinitions.Add (personClass);
      _classDefinitions.Validate ();
    }

    [Test]
    public void ValidateEntityNameWithAbstractBaseClass ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase), false);
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ((string) "Person", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Person), (bool) false, domainBaseClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateWithoutBaseClass ()
    {
      ReflectionBasedClassDefinition addressClass = new ReflectionBasedClassDefinition ((string) "Address", (string) "TableInheritance_Address", (string) c_testDomainProviderID, typeof (Address), (bool) false);

      _classDefinitions.Add (addressClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Property 'OrganizationalUnitName' of class 'OrganizationalUnit' must not define column name 'NameColumn',"
        + " because class 'Person' in same inheritance hierarchy already defines property 'PersonName' with the same column name.")]
    public void ValidateWithSameColumnNameInDifferentInheritanceBranches ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase), false);

      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition (
          (string) "Person", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Person), (bool) false, domainBaseClass);

      personClass.MyPropertyDefinitions.Add (new PropertyDefinition ("PersonName", "NameColumn", "string", true, false, 100));

      ReflectionBasedClassDefinition organizationalUnitClass = new ReflectionBasedClassDefinition (
          (string) "OrganizationalUnit", (string) "TableInheritance_OrganizationalUnit", (string) c_testDomainProviderID, typeof (OrganizationalUnit), (bool) false, domainBaseClass);

      organizationalUnitClass.MyPropertyDefinitions.Add (new PropertyDefinition ("OrganizationalUnitName", "NameColumn", "string", true, false, 100));

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (organizationalUnitClass);

      _classDefinitions.Validate ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
        ExpectedMessage = "At least two classes in different inheritance branches derived from abstract class 'DomainBase'"
        + " specify the same entity name 'TableInheritance_Person', which is not allowed.")]
    public void ValidateWithSameEntityNamesInDifferentInheritanceBranches ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase), false);

      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition (
          (string) "Person", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Person), (bool) false, domainBaseClass);

      ReflectionBasedClassDefinition organizationalUnitClass = new ReflectionBasedClassDefinition (
          (string) "OrganizationalUnit", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (OrganizationalUnit), (bool) false, domainBaseClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (organizationalUnitClass);

      _classDefinitions.Validate ();
    }

  }
}
