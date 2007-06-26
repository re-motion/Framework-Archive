using System;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Development.UnitTesting.Data.SqlClient;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  public class TableInheritanceMappingTest: DatabaseTest
  {
    public const string CreateTestDataFileName = "DataDomainObjects_CreateTableInheritanceTestData.sql";
    public const string TableInheritanceTestDomainProviderID = "TableInheritanceTestDomain";

    public TableInheritanceMappingTest()
      : base (new DatabaseAgent (TestDomainConnectionString), CreateTestDataFileName)
    {
    }

    public override void TestFixtureSetUp()
    {
      base.TestFixtureSetUp();
      DomainObjectsConfiguration.SetCurrent (TableInheritanceConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent (StandardConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent (null);
    }

    public override void SetUp()
    {
      base.SetUp();
      DomainObjectsConfiguration.SetCurrent (TableInheritanceConfiguration.Instance.GetDomainObjectsConfiguration ());
      MappingConfiguration.SetCurrent (TableInheritanceConfiguration.Instance.GetMappingConfiguration ());
      ConfigurationWrapper.SetCurrent (null);
      ClientTransactionScope.SetCurrentTransaction (null);
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return TableInheritanceConfiguration.Instance.GetDomainObjectIDs (); }
    }
  }
}