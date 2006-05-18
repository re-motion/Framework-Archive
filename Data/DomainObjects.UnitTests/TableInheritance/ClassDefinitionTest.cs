using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
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

    public void InitializeWithNullEntityName ()
    {
      ClassDefinition classDefinition = new ClassDefinition ("Person", null, DatabaseTest.c_testDomainProviderID, typeof (Person));
      Assert.IsNull (classDefinition.EntityName);
    }
  }
}
