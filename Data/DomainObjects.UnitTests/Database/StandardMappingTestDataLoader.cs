using System;
using System.Data.SqlClient;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.Resources;
using Rubicon.Development.UnitTesting.Data.SqlClient;

namespace Rubicon.Data.DomainObjects.UnitTests.Database
{
  public class StandardMappingDatabaseAgent : DatabaseAgent
  {
    public StandardMappingDatabaseAgent (string connectionString)
      : base (connectionString)
    {
    }

    protected override void ExecuteBatch (SqlConnection connection, SqlTransaction transaction, string sqlFileName)
    {
      base.ExecuteBatch (connection, transaction, sqlFileName);
      LoadBlobs (connection, transaction);
    }

    private void LoadBlobs (SqlConnection connection, SqlTransaction transaction)
    {
      DomainObjectIDs domainObjectIDs = StandardConfiguration.Instance.GetDomainObjectIDs();
      UpdateClassWithAllDataTypes (connection, transaction, domainObjectIDs.ClassWithAllDataTypes1, ResourceManager.GetImage1 ());
      UpdateClassWithAllDataTypes (connection, transaction, domainObjectIDs.ClassWithAllDataTypes2, ResourceManager.GetImage2 ());
    }

    private void UpdateClassWithAllDataTypes (SqlConnection connection, SqlTransaction transaction, ObjectID id, byte[] binary)
    {
      string updateText = "Update [TableWithAllDataTypes] set [Binary] = @binary where [ID] = @id";
      using (SqlCommand command = new SqlCommand (updateText, connection, transaction))
      {
        command.Parameters.AddWithValue ("@binary", binary);
        command.Parameters.AddWithValue ("@id", id.Value);
        command.ExecuteNonQuery ();
      }
    }
  }
}
