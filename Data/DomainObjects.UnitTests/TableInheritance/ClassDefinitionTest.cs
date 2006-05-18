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

    // construction and disposing

    public ClassDefinitionTest ()
    {
    }

    // methods and properties

    [Test]
    public void InitializeWithNullEntityName ()
    {
      ClassDefinition classDefinition = new ClassDefinition ("DomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (DomainBase));
      Assert.IsNull (classDefinition.EntityName);

      classDefinition = new ClassDefinition ("DomainBase", null, c_testDomainProviderID, typeof (DomainBase).AssemblyQualifiedName, true);
      Assert.IsNull (classDefinition.EntityName);
    }

    [Test]
    public void ClassTypeWithoutEntityNameMustBeAbstract ()
    {
      try
      {
        new ClassDefinition ("Person", null, c_testDomainProviderID, typeof (Person));
        Assert.Fail ("MappingException was expected.");
      }
      catch (MappingException ex)
      {
        string expectedMessage = string.Format (
            "The provided type '{0}' must be abstract, because no entityName is specified.", typeof (Person).AssemblyQualifiedName);

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    public void ClassTypeNameWithoutEntityNameMustBeAbstract ()
    {
      try
      {
        new ClassDefinition (
            "Person", null, c_testDomainProviderID, 
            "Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain.Person, Rubicon.Data.DomainObjects.UnitTests", true);

        Assert.Fail ("MappingException was expected.");
      }
      catch (MappingException ex)
      {
        string expectedMessage = "The provided type"
            + " 'Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain.Person, Rubicon.Data.DomainObjects.UnitTests' must be abstract,"
            + " because no entityName is specified.";

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    public void ClassTypeIsNotCheckedToBeAbstractIfResolveClassTypeNameIsFalse ()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "Person", null, c_testDomainProviderID, typeof (Person).AssemblyQualifiedName, false);

      Assert.IsNull (classDefinition.EntityName);
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
  }
}
