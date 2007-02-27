using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance
{
  [TestFixture]
  public class ClassDefinitionTest : TableInheritanceMappingTest
  {
    // types

    // static members and constants

    // member fields

    private ClassDefinition _domainBaseClass;
    private ClassDefinition _personClass;
    private ClassDefinition _customerClass;
    private ClassDefinition _addressClass;
    private ClassDefinition _organizationalUnitClass;

    // construction and disposing

    public ClassDefinitionTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _domainBaseClass = new ClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase));
      _personClass = new ClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), _domainBaseClass);
      _customerClass = new ClassDefinition ("Customer", null, c_testDomainProviderID, typeof (Customer), _personClass);
      _addressClass = new ClassDefinition ("Address", "TableInheritance_Address", c_testDomainProviderID, typeof (Address));
      
      _organizationalUnitClass = new ClassDefinition (
          "OrganizationalUnit", "TableInheritance_OrganizationalUnit", c_testDomainProviderID, typeof (OrganizationalUnit), _domainBaseClass);  
    }

    [Test]
    public void InitializeWithNullEntityName ()
    {
      Assert.IsNull (_domainBaseClass.MyEntityName);

      ClassDefinition classDefinition = new ClassDefinition ("DomainBase", null, c_testDomainProviderID, typeof (DomainBase).AssemblyQualifiedName, true);
      Assert.IsNull (classDefinition.MyEntityName);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void EntityNameMustNotBeEmptyWithClassType ()
    {
      new ClassDefinition ("DomainBase", string.Empty, c_testDomainProviderID, typeof (DomainBase));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void EntityNameMustNotBeEmptyWithClassTypeName ()
    {
      new ClassDefinition ("DomainBase", string.Empty, c_testDomainProviderID, typeof (DomainBase).AssemblyQualifiedName, true);
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
      ClassDefinition personClass = new ClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person));
      ClassDefinition customerClass = new ClassDefinition ("Customer", "TableInheritance_Person", c_testDomainProviderID, typeof (Customer), personClass);

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
      ClassDefinition domainBaseClass = new ClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase));
      ClassDefinition personClass = new ClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), domainBaseClass);
      ClassDefinition customerClass = new ClassDefinition ("Customer", "TableInheritance_Person", c_testDomainProviderID, typeof (Customer), personClass);

      string[] entityNames = domainBaseClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }
  }
}
