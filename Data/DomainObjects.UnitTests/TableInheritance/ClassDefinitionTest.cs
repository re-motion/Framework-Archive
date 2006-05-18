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
    public void CheckBaseClassWithNullEntityName ()
    {
      ClassDefinition domainBaseClass = new ClassDefinition ("DomainBase", null, c_testDomainProviderID, typeof (DomainBase));
      ClassDefinition personClass = new ClassDefinition ("Person", "TableInheritance_Person", c_testDomainProviderID, typeof (Person), domainBaseClass);

      Assert.IsNull (domainBaseClass.EntityName);
      Assert.IsNotNull (personClass.EntityName);
    }
  }
}
