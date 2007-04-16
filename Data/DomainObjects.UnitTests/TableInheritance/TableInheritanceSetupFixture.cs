using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.Database;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  [SetUpFixture]
  public class TableInheritanceSetUpFixture
  {
    private TestDataLoader _loader;

    [SetUp]
    public void SetUp()
    {
      TableInheritanceConfiguration.Initialize();

      _loader = new TestDataLoader (DatabaseTest.c_connectionString);
      _loader.SetDatabaseReadWrite (DatabaseTest.DatabaseName);
      _loader.LoadTestData ("CreateTableInheritanceTestData.sql");
      _loader.SetDatabaseReadOnly (DatabaseTest.DatabaseName);
    }
  }
}