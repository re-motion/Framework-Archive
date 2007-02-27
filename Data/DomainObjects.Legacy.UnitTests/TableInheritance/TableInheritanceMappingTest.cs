using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Database;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance
{
  public class TableInheritanceMappingTest : DatabaseTest
  {
    // types

    // static members and constants

    private const string c_createTestDataFileName = "CreateTableInheritanceTestData.sql";

    private static readonly MappingConfiguration s_mappingConfiguration = MappingConfiguration.CreateConfigurationFromFileBasedLoader(@"TableInheritanceMapping.xml");

    // member fields

    private DomainObjectIDs _domainObjectIDs;
	
    // construction and disposing

    public TableInheritanceMappingTest () : base (new TestDataLoader (c_connectionString), c_createTestDataFileName)
    {
    }

    // methods and properties

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return _domainObjectIDs; }
    }

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      _domainObjectIDs = new DomainObjectIDs ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.SetCurrent (null);
    }
  }
}
