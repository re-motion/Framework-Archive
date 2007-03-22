using System;
using System.Data.SqlClient;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderTest : SqlProviderBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SqlProviderTest ()
    {
    }

    // methods and properties

    [Test]
    public void IsConnectedFalse ()
    {
      RdbmsProvider rdbmsProvider = Provider;

      Assert.IsFalse (rdbmsProvider.IsConnected);
    }

    [Test]
    public void ConnectionHandling ()
    {
      RdbmsProvider rdbmsProvider = Provider;

      rdbmsProvider.Connect ();
      Assert.IsTrue (rdbmsProvider.IsConnected);
      rdbmsProvider.Disconnect ();
      Assert.IsFalse (rdbmsProvider.IsConnected);
    }

    [Test]
    public void Disposing ()
    {
      using (StorageProvider provider = Provider)
      {
        provider.LoadDataContainer (DomainObjectIDs.Order1);
      }

      RdbmsProvider rdbmsProvider = Provider;
      Assert.IsFalse (rdbmsProvider.IsConnected);
    }

    [Test]
    public void GetParameterName ()
    {
      Assert.AreEqual ("@parameter", Provider.GetParameterName ("parameter"));
      Assert.AreEqual ("@parameter", Provider.GetParameterName ("@parameter"));
    }

    [Test]
    public void ConnectionReturnsSqlConnection ()
    {
      // Note: If Provider.Connection returns a SqlConnection instead of IDbConnection, the line below does not create a compiler error.
      SqlConnection sqlConnection = Provider.Connection;
    }

    [Test]
    public void TransactionReturnsSqlTransaction ()
    {
      // Note: If Provider.Transaction returns a SqlTransaction instead of IDbTransaction, the line below does not create a compiler error.
      SqlTransaction sqlTransaction = Provider.Transaction;
    }

    [Test]
    [ExpectedException (typeof (ObjectDisposedException))]
    public void GetColumnsFromSortExpressionChecksForDisposal ()
    {
      Provider.Dispose ();
      Provider.GetColumnsFromSortExpression ("StorageSpecificName asc");
    }
  }
}
