using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  [Ignore]
  public class ReflectionBasedClassDefinitionTest : TableInheritanceMappingTest
  {
    // types

    // static members and constants

    // member fields

    private ReflectionBasedClassDefinition _domainBaseClass;
    private ReflectionBasedClassDefinition _personClass;
    private ReflectionBasedClassDefinition _customerClass;
    private ReflectionBasedClassDefinition _addressClass;
    private ReflectionBasedClassDefinition _organizationalUnitClass;

    // construction and disposing

    public ReflectionBasedClassDefinitionTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase), false);
      _personClass = new ReflectionBasedClassDefinition ((string) "Person", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Person), (bool) false, _domainBaseClass);
      _customerClass = new ReflectionBasedClassDefinition ("Customer", null, c_testDomainProviderID, typeof (Customer), false, _personClass);
      _addressClass = new ReflectionBasedClassDefinition ((string) "Address", (string) "TableInheritance_Address", (string) c_testDomainProviderID, typeof (Address), (bool) false);

      _organizationalUnitClass = new ReflectionBasedClassDefinition (
          (string) "OrganizationalUnit", (string) "TableInheritance_OrganizationalUnit", (string) c_testDomainProviderID, typeof (OrganizationalUnit), (bool) false, _domainBaseClass);  
    }

    [Test]
    public void InitializeWithNullEntityName ()
    {
      Assert.IsNull (_domainBaseClass.MyEntityName);

      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition ("DomainBase", null, c_testDomainProviderID, typeof (DomainBase), false);
      Assert.IsNull (classDefinition.MyEntityName);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void EntityNameMustNotBeEmptyWithClassType ()
    {
      new ReflectionBasedClassDefinition ((string) "DomainBase", string.Empty, (string) c_testDomainProviderID, typeof (DomainBase), (bool) false);
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
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ((string) "Person", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Person), (bool) false);
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ((string) "Customer", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Customer), (bool) false, personClass);

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
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase), false);
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ((string) "Person", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Person), (bool) false, domainBaseClass);
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ((string) "Customer", (string) "TableInheritance_Person", (string) c_testDomainProviderID, typeof (Customer), (bool) false, personClass);

      string[] entityNames = domainBaseClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }
  }
}
