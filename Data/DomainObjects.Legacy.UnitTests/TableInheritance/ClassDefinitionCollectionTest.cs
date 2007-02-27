using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance
{
  [TestFixture]
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
      ClassDefinition personClass = new ClassDefinition ("Person", null, c_testDomainProviderID, typeof (Person));
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
    public void ValidateDoesNotCheckAbstractClassIfClassTypeIsNotResolved ()
    {
      ClassDefinitionCollection classDefinitionsWithUnresolvedTypes = new ClassDefinitionCollection (false);

      ClassDefinition personClass = new ClassDefinition ("Person", null, c_testDomainProviderID, typeof (Person).AssemblyQualifiedName, false);
      classDefinitionsWithUnresolvedTypes.Add (personClass);

      classDefinitionsWithUnresolvedTypes.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateAbstractClassHandlesNullEntityNameWithInherited ()
    {
      ClassDefinition domainBaseClass = new ClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase));
      ClassDefinition personClass = new ClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), domainBaseClass);
      ClassDefinition customerClass = new ClassDefinition ("Customer", null, c_testDomainProviderID, typeof (Customer), personClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (customerClass);      

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateAbstractClassHandlesSameInheritedEntityName ()
    {
      ClassDefinition personClass = new ClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person));

      ClassDefinition customerClass = new ClassDefinition (
          "Customer", personClass.MyEntityName, c_testDomainProviderID, typeof (Customer), personClass);

      _classDefinitions.Add (personClass);
      _classDefinitions.Add (customerClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
        "Class 'Customer' must not specify an entity name 'DifferentEntityNameThanBaseClass'"
        + " which is different from inherited entity name 'TableInheritance_Person'.")]
    public void ValidateWithDifferentEntityNames ()
    {
      ClassDefinition personClass = new ClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person));

      ClassDefinition customerClass = new ClassDefinition (
          "Customer", "DifferentEntityNameThanBaseClass", c_testDomainProviderID, typeof (Customer), personClass);

      _classDefinitions.Add (personClass);
      _classDefinitions.Add (customerClass);

      _classDefinitions.Validate ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        "Validate cannot be invoked, because class 'Person' is a base class of a class in the collection,"
        + " but the base class is not part of the collection itself.")]
    public void ValidateWithBaseClassNotInCollection ()
    {
      ClassDefinition personClass = new ClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person));
      ClassDefinition customerClass = new ClassDefinition ("Customer", null, c_testDomainProviderID, typeof (Customer), personClass);

      _classDefinitions.Add (customerClass);
      _classDefinitions.Validate ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        "Validate cannot be invoked, because class 'Customer' is a derived class of 'Person', but is not part of the collection itself.")]
    public void ValidateWithDerivedClassNotInCollection ()
    {
      ClassDefinition personClass = new ClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person));
      ClassDefinition customerClass = new ClassDefinition ("Customer", null, c_testDomainProviderID, typeof (Customer), personClass);

      _classDefinitions.Add (personClass);
      _classDefinitions.Validate ();
    }

    [Test]
    public void ValidateEntityNameWithAbstractBaseClass ()
    {
      ClassDefinition domainBaseClass = new ClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase));
      ClassDefinition personClass = new ClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), domainBaseClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateWithoutBaseClass ()
    {
      ClassDefinition addressClass = new ClassDefinition ("Address", "TableInheritance_Address", c_testDomainProviderID, typeof (Address));

      _classDefinitions.Add (addressClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "Property 'OrganizationalUnitName' of class 'OrganizationalUnit' must not define column name 'NameColumn',"
        + " because class 'Person' in same inheritance hierarchy already defines property 'PersonName' with the same column name.")]
    public void ValidateWithSameColumnNameInDifferentInheritanceBranches ()
    {
      ClassDefinition domainBaseClass = new ClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase));

      ClassDefinition personClass = new ClassDefinition (
          "Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), domainBaseClass);

      personClass.MyPropertyDefinitions.Add (new PropertyDefinition ("PersonName", "NameColumn", "string", true, false, 100));

      ClassDefinition organizationalUnitClass = new ClassDefinition (
          "OrganizationalUnit", "TableInheritance_OrganizationalUnit", c_testDomainProviderID, typeof (OrganizationalUnit), domainBaseClass);

      organizationalUnitClass.MyPropertyDefinitions.Add (new PropertyDefinition ("OrganizationalUnitName", "NameColumn", "string", true, false, 100));

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (organizationalUnitClass);

      _classDefinitions.Validate ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
        "At least two classes in different inheritance branches derived from abstract class 'DomainBase'"
        + " specify the same entity name 'TableInheritance_Person', which is not allowed.")]
    public void ValidateWithSameEntityNamesInDifferentInheritanceBranches ()
    {
      ClassDefinition domainBaseClass = new ClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase));

      ClassDefinition personClass = new ClassDefinition (
          "Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), domainBaseClass);

      ClassDefinition organizationalUnitClass = new ClassDefinition (
          "OrganizationalUnit", "TableInheritance_Person", c_testDomainProviderID, typeof (OrganizationalUnit), domainBaseClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (organizationalUnitClass);

      _classDefinitions.Validate ();
    }

  }
}
