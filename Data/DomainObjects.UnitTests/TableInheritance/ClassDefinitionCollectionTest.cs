using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  public class ClassDefinitionCollectionTest : TableInheritanceMappingTest
  {
    private ClassDefinitionCollection _classDefinitions;

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
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), false, domainBaseClass);
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
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), false);

      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition (
          "Customer", personClass.MyEntityName, c_testDomainProviderID, typeof (Customer), false, personClass);

      _classDefinitions.Add (personClass);
      _classDefinitions.Add (customerClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    [Ignore ("TODO: Implement")]
    [ExpectedException (typeof (MappingException), 
        ExpectedMessage = "Class 'Customer' must not specify an entity name 'DifferentEntityNameThanBaseClass'"
        + " which is different from inherited entity name 'TableInheritance_Person'.")]
    public void ValidateWithDifferentEntityNames ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), false);

      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition (
          "Customer", "DifferentEntityNameThanBaseClass", c_testDomainProviderID, typeof (Customer), false, personClass);

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
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), false);
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", null, c_testDomainProviderID, typeof (Customer), false, personClass);

      _classDefinitions.Add (customerClass);
      _classDefinitions.Validate ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Validate cannot be invoked, because class 'Customer' is a derived class of 'Person', but is not part of the collection itself.")]
    public void ValidateWithDerivedClassNotInCollection ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), false);
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", null, c_testDomainProviderID, typeof (Customer), false, personClass);

      _classDefinitions.Add (personClass);
      _classDefinitions.Validate ();
    }

    [Test]
    public void ValidateEntityNameWithAbstractBaseClass ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), false, domainBaseClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateWithoutBaseClass ()
    {
      ReflectionBasedClassDefinition addressClass = new ReflectionBasedClassDefinition ("Address", "TableInheritance_Address", c_testDomainProviderID, typeof (Address), false);

      _classDefinitions.Add (addressClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    [Ignore ("TODO: Implement")]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Property 'OrganizationalUnitName' of class 'OrganizationalUnit' must not define column name 'NameColumn',"
        + " because class 'Person' in same inheritance hierarchy already defines property 'PersonName' with the same column name.")]
    public void ValidateWithSameColumnNameInDifferentInheritanceBranches ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase), true);

      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition (
          "Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), false, domainBaseClass);

      personClass.MyPropertyDefinitions.Add (new PropertyDefinition ("PersonName", "NameColumn", "string", true, false, 100));

      ReflectionBasedClassDefinition organizationalUnitClass = new ReflectionBasedClassDefinition (
          "OrganizationalUnit", "TableInheritance_OrganizationalUnit", c_testDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass);

      organizationalUnitClass.MyPropertyDefinitions.Add (new PropertyDefinition ("OrganizationalUnitName", "NameColumn", "string", true, false, 100));

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (organizationalUnitClass);

      _classDefinitions.Validate ();
    }

    [Test]
    [Ignore("TODO: Implement")]
    [ExpectedException (typeof (MappingException), 
        ExpectedMessage = "At least two classes in different inheritance branches derived from abstract class 'DomainBase'"
        + " specify the same entity name 'TableInheritance_Person', which is not allowed.")]
    public void ValidateWithSameEntityNamesInDifferentInheritanceBranches ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase), true);

      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition (
          "Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), false, domainBaseClass);

      ReflectionBasedClassDefinition organizationalUnitClass = new ReflectionBasedClassDefinition (
          "OrganizationalUnit", "TableInheritance_Person", c_testDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (organizationalUnitClass);

      _classDefinitions.Validate ();
    }
  }
}
