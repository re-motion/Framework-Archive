using System;
using System.Data.SqlClient;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.Database;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance;
using Rubicon.Development.UnitTesting.Data.SqlClient;

namespace Rubicon.Data.DomainObjects.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private StandardMappingDatabaseAgent _standardMappingDatabaseAgent;

    [SetUp]
    public void SetUp()
    {
      StandardConfiguration.Initialize();

      SqlConnection.ClearAllPools();

      DatabaseAgent masterAgent = new DatabaseAgent (DatabaseTest.MasterConnectionString);
      masterAgent.ExecuteBatch ("DataDomainObjects_CreateDB.sql", false);
      DatabaseAgent testDomainAgent = new DatabaseAgent (DatabaseTest.TestDomainConnectionString);
      testDomainAgent.ExecuteBatch ("DataDomainObjects_SetupDB.sql", true);

      _standardMappingDatabaseAgent = new StandardMappingDatabaseAgent (DatabaseTest.TestDomainConnectionString);
      _standardMappingDatabaseAgent.ExecuteBatch (ReflectionBasedMappingTest.CreateTestDataFileName, true);
      _standardMappingDatabaseAgent.ExecuteBatch (TableInheritanceMappingTest.CreateTestDataFileName, true);
      _standardMappingDatabaseAgent.SetDatabaseReadOnly (DatabaseTest.DatabaseName);
    }

    [TearDown]
    public void TearDown()
    {
      _standardMappingDatabaseAgent.SetDatabaseReadWrite (DatabaseTest.DatabaseName);
      SqlConnection.ClearAllPools();
    }
  }
}