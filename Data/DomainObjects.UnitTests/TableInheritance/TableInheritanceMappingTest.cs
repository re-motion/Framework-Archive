using System;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Development.UnitTesting.Data.SqlClient;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  public class TableInheritanceMappingTest: DatabaseTest
  {
    public const string CreateTestDataFileName = "DataDomainObjects_CreateTableInheritanceTestData.sql";

    public TableInheritanceMappingTest()
      : base (new DatabaseAgent (TestDomainConnectionString), CreateTestDataFileName)
    {
    }

    public override void TestFixtureSetUp()
    {
      base.TestFixtureSetUp();
      DomainObjectsConfiguration.SetCurrent (TableInheritanceConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent (StandardConfiguration.Instance.GetMappingConfiguration());
    }

    public override void SetUp()
    {
      base.SetUp();
      DomainObjectsConfiguration.SetCurrent (TableInheritanceConfiguration.Instance.GetDomainObjectsConfiguration ());
      MappingConfiguration.SetCurrent (TableInheritanceConfiguration.Instance.GetMappingConfiguration ());
      ClientTransaction.SetCurrent (null);
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return TableInheritanceConfiguration.Instance.GetDomainObjectIDs (); }
    }
  }
}