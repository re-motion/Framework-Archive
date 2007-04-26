using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
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
      XmlBasedClassDefinition personClass = new XmlBasedClassDefinition ("Person", null, c_testDomainProviderID, typeof (Person));
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

      XmlBasedClassDefinition personClass = new XmlBasedClassDefinition ("Person", null, c_testDomainProviderID, typeof (Person).AssemblyQualifiedName, false);
      classDefinitionsWithUnresolvedTypes.Add (personClass);

      classDefinitionsWithUnresolvedTypes.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateAbstractClassHandlesNullEntityNameWithInherited ()
    {
      XmlBasedClassDefinition domainBaseClass = new XmlBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase));
      XmlBasedClassDefinition personClass = new XmlBasedClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), domainBaseClass);
      XmlBasedClassDefinition customerClass = new XmlBasedClassDefinition ("Customer", null, c_testDomainProviderID, typeof (Customer), personClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (customerClass);      

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateAbstractClassHandlesSameInheritedEntityName ()
    {
      XmlBasedClassDefinition personClass = new XmlBasedClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person));

      XmlBasedClassDefinition customerClass = new XmlBasedClassDefinition (
          "Customer", personClass.MyEntityName, c_testDomainProviderID, typeof (Customer), personClass);

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
      XmlBasedClassDefinition personClass = new XmlBasedClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person));

      XmlBasedClassDefinition customerClass = new XmlBasedClassDefinition (
          "Customer", "DifferentEntityNameThanBaseClass", c_testDomainProviderID, typeof (Customer), personClass);

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
      XmlBasedClassDefinition personClass = new XmlBasedClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person));
      XmlBasedClassDefinition customerClass = new XmlBasedClassDefinition ("Customer", null, c_testDomainProviderID, typeof (Customer), personClass);

      _classDefinitions.Add (customerClass);
      _classDefinitions.Validate ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Validate cannot be invoked, because class 'Customer' is a derived class of 'Person', but is not part of the collection itself.")]
    public void ValidateWithDerivedClassNotInCollection ()
    {
      XmlBasedClassDefinition personClass = new XmlBasedClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person));
      XmlBasedClassDefinition customerClass = new XmlBasedClassDefinition ("Customer", null, c_testDomainProviderID, typeof (Customer), personClass);

      _classDefinitions.Add (personClass);
      _classDefinitions.Validate ();
    }

    [Test]
    public void ValidateEntityNameWithAbstractBaseClass ()
    {
      XmlBasedClassDefinition domainBaseClass = new XmlBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase));
      XmlBasedClassDefinition personClass = new XmlBasedClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), domainBaseClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateWithoutBaseClass ()
    {
      XmlBasedClassDefinition addressClass = new XmlBasedClassDefinition ("Address", "TableInheritance_Address", c_testDomainProviderID, typeof (Address));

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
      XmlBasedClassDefinition domainBaseClass = new XmlBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase));

      XmlBasedClassDefinition personClass = new XmlBasedClassDefinition (
          "Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), domainBaseClass);

      personClass.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (personClass, "PersonName", "NameColumn", "string", true, false, 100));

      XmlBasedClassDefinition organizationalUnitClass = new XmlBasedClassDefinition (
          "OrganizationalUnit", "TableInheritance_OrganizationalUnit", c_testDomainProviderID, typeof (OrganizationalUnit), domainBaseClass);

      organizationalUnitClass.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (organizationalUnitClass, "OrganizationalUnitName", "NameColumn", "string", true, false, 100));

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
      XmlBasedClassDefinition domainBaseClass = new XmlBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase));

      XmlBasedClassDefinition personClass = new XmlBasedClassDefinition (
          "Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), domainBaseClass);

      XmlBasedClassDefinition organizationalUnitClass = new XmlBasedClassDefinition (
          "OrganizationalUnit", "TableInheritance_Person", c_testDomainProviderID, typeof (OrganizationalUnit), domainBaseClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (organizationalUnitClass);

      _classDefinitions.Validate ();
    }

  }
}
