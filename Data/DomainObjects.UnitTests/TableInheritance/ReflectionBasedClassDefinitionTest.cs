using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  public class ReflectionBasedClassDefinitionTest : TableInheritanceMappingTest
  {
    private ReflectionBasedClassDefinition _domainBaseClass;
    private ReflectionBasedClassDefinition _personClass;
    private ReflectionBasedClassDefinition _customerClass;
    private ReflectionBasedClassDefinition _addressClass;
    private ReflectionBasedClassDefinition _organizationalUnitClass;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), false);
      _personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, _domainBaseClass);
      _customerClass = new ReflectionBasedClassDefinition ("Customer", null, TableInheritanceTestDomainProviderID, typeof (Customer), false, _personClass);
      _addressClass = new ReflectionBasedClassDefinition ("Address", "TableInheritance_Address", TableInheritanceTestDomainProviderID, typeof (Address), false);

      _organizationalUnitClass = new ReflectionBasedClassDefinition (
          "OrganizationalUnit", "TableInheritance_OrganizationalUnit", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, _domainBaseClass);
    }

    [Test]
    public void InitializeWithNullEntityName ()
    {
      Assert.IsNull (_domainBaseClass.MyEntityName);

      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), false);
      Assert.IsNull (classDefinition.MyEntityName);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void EntityNameMustNotBeEmptyWithClassType ()
    {
      new ReflectionBasedClassDefinition ("DomainBase", string.Empty, TableInheritanceTestDomainProviderID, typeof (DomainBase), false);
    }

    [Test]
    public void NullEntityNameWithDerivedClass ()
    {
      Assert.IsNull (_domainBaseClass.MyEntityName);
      Assert.IsNotNull (_personClass.MyEntityName);
      Assert.IsNull (_customerClass.MyEntityName);
    }

    [Test]
    public void GetEntityName ()
    {
      Assert.IsNull (_domainBaseClass.GetEntityName ());
      Assert.AreEqual ("TableInheritance_Person", _personClass.GetEntityName ());
      Assert.AreEqual ("TableInheritance_Person", _customerClass.GetEntityName ());
    }

    [Test]
    public void GetAllConcreteEntityNamesForConreteSingle ()
    {
      string[] entityNames = _customerClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForConcrete ()
    {
      string[] entityNames = _personClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForConreteSingleWithEntityName ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false);
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

      string[] entityNames = customerClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForAbstractClass ()
    {
      string[] entityNames = _domainBaseClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (2, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
      Assert.AreEqual ("TableInheritance_OrganizationalUnit", entityNames[1]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForAbstractClassWithSameEntityNameInInheritanceHierarchy ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), false);
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

      string[] entityNames = domainBaseClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void ValidateMappingWithDerivationInDifferentEntitiesAndMatchingColumnNames ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition organizationalUnit = new ReflectionBasedClassDefinition ("OrganizationalUnit", "TableInheritance_OrganizationalUnit", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass);

      personClass.MyPropertyDefinitions.Add (new ReflectionBasedPropertyDefinition (personClass, "Name", "NameColumn", typeof (string), 100));
      organizationalUnit.MyPropertyDefinitions.Add (new ReflectionBasedPropertyDefinition (organizationalUnit, "Name", "NameColumn", typeof (string), 100));

      domainBaseClass.ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'OtherName' of class 'OrganizationalUnit' must not define storage specific name 'NameColumn', because class 'Person', "
        + "persisted in the same entity, already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithParallelDerivationInSameEntitiesAndMatchingColumnNames ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", "TableInheritance_DomainBase", TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition organizationalUnit = new ReflectionBasedClassDefinition ("OrganizationalUnit", null, TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass);

      personClass.MyPropertyDefinitions.Add (new ReflectionBasedPropertyDefinition (personClass, "Name", "NameColumn", typeof (string), 100));
      organizationalUnit.MyPropertyDefinitions.Add (new ReflectionBasedPropertyDefinition (organizationalUnit, "OtherName", "NameColumn", typeof (string), 100));

      domainBaseClass.ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'OtherName' of class 'Person' must not define storage specific name 'NameColumn', because class 'DomainBase', "
        + "persisted in the same entity, already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInSameEntityAndDuplicateColumnName ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);

      domainBaseClass.MyPropertyDefinitions.Add (new ReflectionBasedPropertyDefinition (domainBaseClass, "Name", "NameColumn", typeof (string), 100));
      personClass.MyPropertyDefinitions.Add (new ReflectionBasedPropertyDefinition (personClass, "OtherName", "NameColumn", typeof (string), 100));

      domainBaseClass.ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'OtherName' of class 'Customer' must not define storage specific name 'NameColumn', because class 'DomainBase', "
        + "persisted in the same entity, already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInSameEntityAndDuplicateColumnNameInBaseOfBaseClass ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

      domainBaseClass.MyPropertyDefinitions.Add (new ReflectionBasedPropertyDefinition (domainBaseClass, "Name", "NameColumn", typeof (string), 100));
      customerClass.MyPropertyDefinitions.Add (new ReflectionBasedPropertyDefinition (customerClass, "OtherName", "NameColumn", typeof (string), 100));

      domainBaseClass.ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'OtherName' of class 'Person' must not define storage specific name 'NameColumn', because class 'DomainBase', "
        + "persisted in the same entity, already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInUndefinedEntityAndDuplicateColumnName ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), true, domainBaseClass);

      domainBaseClass.MyPropertyDefinitions.Add (new ReflectionBasedPropertyDefinition (domainBaseClass, "Name", "NameColumn", typeof (string), 100));
      personClass.MyPropertyDefinitions.Add (new ReflectionBasedPropertyDefinition (personClass, "OtherName", "NameColumn", typeof (string), 100));

      domainBaseClass.ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }
  }
}
