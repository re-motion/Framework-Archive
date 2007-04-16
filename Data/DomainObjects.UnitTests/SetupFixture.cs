using System;
using System.Data.SqlClient;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.Database;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private StandardMappingTestDataLoader _loader;

    [SetUp]
    public void SetUp()
    {
      StandardConfiguration.Initialize();

      SqlConnection.ClearAllPools();
      _loader = new StandardMappingTestDataLoader (DatabaseTest.c_connectionString);
      _loader.CreateDatabase ("CreateDB.sql");
      _loader.CreateDatabase ("SetupDB.sql");
      _loader.LoadTestData ("CreateTestData.sql");
      _loader.SetDatabaseReadOnly (DatabaseTest.DatabaseName);
    }

    [TearDown]
    public void TearDown()
    {
      _loader.SetDatabaseReadWrite (DatabaseTest.DatabaseName);
      SqlConnection.ClearAllPools();
    }
  }
}