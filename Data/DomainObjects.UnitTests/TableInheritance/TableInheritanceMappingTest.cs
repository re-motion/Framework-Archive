using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.Database;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  public class TableInheritanceMappingTest : DatabaseTest
  {
    // types

    // static members and constants

    private const string c_createTestDataFileName = "CreateTestData.sql";

    //private static readonly MappingConfiguration s_mappingConfiguration = new MappingConfiguration (@"TableInheritanceMapping.xml", @"mapping.xsd");

    // member fields

    // construction and disposing

    public TableInheritanceMappingTest () : base (new TestDataLoader (c_connectionString), c_createTestDataFileName)
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      //MappingConfiguration.SetCurrent (s_mappingConfiguration);
    }
  }
}
