using System;
using System.Data;
using NUnit.Framework;
using Rubicon.Development.UnitTesting.Data.SqlClient;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests
{
  public abstract class DatabaseTest
  {
    // types

    // static members and constants

    public const string DatabaseName = "TestDomainLegacy";
    public const string TestDomainConnectionString = "Integrated Security=SSPI;Initial Catalog=TestDomainLegacy;Data Source=localhost; Max Pool Size=1;";
    public const string MasterConnectionString = "Integrated Security=SSPI;Initial Catalog=master;Data Source=localhost; Max Pool Size=1;";

    public const string c_testDomainProviderID = "TestDomain";
    public const string c_unitTestStorageProviderStubID = "UnitTestStorageProviderStub";

    // member fields

    private DatabaseAgent _databaseAgent;
    private string _createTestDataFileName;
    private bool _isDatabaseModifyable;

    // construction and disposing

    protected DatabaseTest (DatabaseAgent databaseAgent, string createTestDataFileName)
    {
      ArgumentUtility.CheckNotNull ("databaseAgent", databaseAgent);
      ArgumentUtility.CheckNotNullOrEmpty ("createTestDataFileName", createTestDataFileName);

      _databaseAgent = databaseAgent;
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
        _databaseAgent.ExecuteBatch (_createTestDataFileName, true);
      }
    }

    [TestFixtureTearDown]
    public virtual void TestFixtureTearDown ()
    {
      if (_isDatabaseModifyable)
      {
        _databaseAgent.SetDatabaseReadOnly (DatabaseName);
        _isDatabaseModifyable = false;
      }
    }

    protected void SetDatabaseModifyable ()
    {
      if (!_isDatabaseModifyable)
      {
        _isDatabaseModifyable = true;
        _databaseAgent.SetDatabaseReadWrite (DatabaseName);
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
