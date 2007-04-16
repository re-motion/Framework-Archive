using System;
using System.Data;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.Database;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests
{
  public abstract class DatabaseTest
  {
    // types

    // static members and constants

    public const string DatabaseName = "TestDomain";
    public const string c_connectionString = "Integrated Security=SSPI;Initial Catalog=TestDomain;Data Source=localhost; Max Pool Size=1;";

    public const string c_testDomainProviderID = "TestDomain";
    public const string c_unitTestStorageProviderStubID = "UnitTestStorageProviderStub";

    // member fields

    private TestDataLoader _loader;
    private string _createTestDataFileName;
    private bool _isDatabaseModifyable;

    // construction and disposing

    protected DatabaseTest (TestDataLoader loader, string createTestDataFileName)
    {
      ArgumentUtility.CheckNotNull ("loader", loader);
      ArgumentUtility.CheckNotNullOrEmpty ("createTestDataFileName", createTestDataFileName);

      _loader = loader;
      _createTestDataFileName = createTestDataFileName;
    }

    // methods and properties

    [SetUp]
    public virtual void SetUp ()
    {
    }

    [TearDown]
    public virtual void TearDown ()
    {
      if (_isDatabaseModifyable)
      {
        _loader.Load (_createTestDataFileName);
      }
    }

    [TestFixtureTearDown]
    public virtual void TestFixtureTearDown ()
    {
      if (_isDatabaseModifyable)
      {
        _loader.SetDatabaseReadOnly (DatabaseName);
        _isDatabaseModifyable = false;
      }
    }

    protected void SetDatabaseModifyable ()
    {
      if (!_isDatabaseModifyable)
      {
        _isDatabaseModifyable = true;
        _loader.SetDatabaseReadWrite (DatabaseName);
      }
    }

    protected IDbCommand CreateCommand (string table, Guid id, IDbConnection connection)
    {
      IDbCommand command = connection.CreateCommand ();
      command.CommandText = string.Format ("SELECT * FROM [{0}] where ID = @id", table);

      IDbDataParameter parameter = command.CreateParameter ();
      parameter.ParameterName = "@id";
      parameter.Value = id;
      command.Parameters.Add (parameter);

      return command;
    }
  }
}
