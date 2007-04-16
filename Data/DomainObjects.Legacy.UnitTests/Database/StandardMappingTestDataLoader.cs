using System;
using System.Data.SqlClient;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Factories;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Resources;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Database
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

    protected override void PerformLoadTestData (SqlConnection connection, SqlTransaction transaction, string sqlFileName)
    {
      base.PerformLoadTestData (connection, transaction, sqlFileName);
      LoadBlobs (connection, transaction);
    }

    private void LoadBlobs (SqlConnection connection, SqlTransaction transaction)
    {
      DomainObjectIDs domainObjectIDs = new DomainObjectIDs ();
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
