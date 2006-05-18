using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text;

using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.Resources;

namespace Rubicon.Data.DomainObjects.UnitTests.Database
{
  public class StandardMappingTestDataLoader : TestDataLoader
  {
    // types

    // static members and constants


    // member fields


    // construction and disposing

    public StandardMappingTestDataLoader (string connectionString) : base (connectionString)
    {
    }

    // methods and properties

    protected override void PerformLoad (SqlConnection connection, SqlTransaction transaction, string sqlFileName)
    {
      base.PerformLoad (connection, transaction, sqlFileName);
      LoadBlobs (connection, transaction);
    }

    private void LoadBlobs (SqlConnection connection, SqlTransaction transaction)
    {
      UpdateClassWithAllDataTypes (connection, transaction, DomainObjectIDs.ClassWithAllDataTypes1, ResourceManager.GetImage1 ());
      UpdateClassWithAllDataTypes (connection, transaction, DomainObjectIDs.ClassWithAllDataTypes2, ResourceManager.GetImage2 ());
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
