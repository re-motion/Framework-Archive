using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Rubicon.Data.DomainObjects.UnitTests.Database
{
public class TestDataLoader : IDisposable
{
  // types

  // static members and constants

  // member fields

  private SqlConnection _connection;
  private bool _disposed = false;

  // construction and disposing

  public TestDataLoader (string connectionString)
  {
    _connection = new SqlConnection (connectionString);
    _connection.Open ();
  }

  // methods and properties

  public void ExecuteSqlFile (string sqlFile)
  {
    using (SqlCommand command = new SqlCommand (ReadFile (sqlFile), _connection))
    {
      command.ExecuteNonQuery ();
    }
  }

  private string ReadFile (string file)
  {
    using (StreamReader reader = new StreamReader (file, Encoding.Default))
    {
      return reader.ReadToEnd ();
    }
  }

  #region IDisposable Members

  public void Dispose()
  {
    Dispose (true);
    GC.SuppressFinalize(this);
  }

  #endregion

  private void Dispose (bool disposing)
  {
    if (!_disposed && disposing)
    {
      if (_connection != null)
      {
        _connection.Close ();
        _connection = null;
      }
      _disposed = true;
    }
  }
}
}
