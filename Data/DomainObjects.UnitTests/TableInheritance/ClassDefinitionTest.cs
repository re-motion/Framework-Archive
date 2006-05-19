using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
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
    public void ValidateAbstractClass ()
    {
      ClassDefinition personClass = new ClassDefinition ("Person", null, c_testDomainProviderID, typeof (Person));

      try
      {
        personClass.Validate ();
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
      ClassDefinition personClass = new ClassDefinition ("Person", null, c_testDomainProviderID, typeof (Person).AssemblyQualifiedName, false);
      personClass.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateAbstractClassHandlesNullEntityNameWithInherited ()
    {
      _customerClass.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateAbstractClassHandlesSameInheritedEntityName ()
    {
      ClassDefinition personClass = new ClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person));

      ClassDefinition customerClass = new ClassDefinition (
          "Customer", personClass.MyEntityName, c_testDomainProviderID, typeof (Customer), personClass);

      customerClass.Validate ();

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

      customerClass.Validate ();
    }

    [Test]
    public void ValidateEntityNameWithAbstractBaseClass ()
    {
      _personClass.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateWithoutBaseClass ()
    {
      _addressClass.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void GetEntityName ()
    {
      Assert.IsNull (_domainBaseClass.GetEntityName ());
      Assert.AreEqual ("TableInheritance_Person", _personClass.GetEntityName ());
      Assert.AreEqual ("TableInheritance_Person", _customerClass.GetEntityName ());
    }
  }
}
