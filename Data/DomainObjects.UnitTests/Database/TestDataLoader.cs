using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Database
{
  public class TestDataLoader
  {
    private string _connectionString;

    public TestDataLoader (string connectionString)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("connectionString", connectionString);

      _connectionString = connectionString;
    }

    public void SetDatabaseReadWrite (string database)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("database", database);
      ExecuteCommand (string.Format ("ALTER DATABASE [{0}] SET READ_WRITE WITH ROLLBACK IMMEDIATE", database));
    }

    public void SetDatabaseReadOnly (string database)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("database", database);
      ExecuteCommand (string.Format ("ALTER DATABASE [{0}] SET READ_ONLY WITH ROLLBACK IMMEDIATE", database));
    }

    public void CreateDatabase (string sqlFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sqlFileName", sqlFileName);

      using (SqlConnection connection = new SqlConnection ("Integrated Security=SSPI;Initial Catalog=master;Data Source=localhost; Max Pool Size=1;"))
      {
        connection.Open();
        foreach (string commandText in GetCommandTextBatchesFromFile (sqlFileName))
        {
          using (SqlCommand command = new SqlCommand (commandText, connection))
          {
            command.ExecuteNonQuery();
          }
        }
      }
    }

    public void SetUpDatabase (string sqlFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sqlFileName", sqlFileName);

      using (SqlConnection connection = new SqlConnection (_connectionString))
      {
        connection.Open();

        foreach (string commandText in GetCommandTextBatchesFromFile (sqlFileName))
        {
          using (SqlTransaction transaction = connection.BeginTransaction())
          {
            using (SqlCommand command = new SqlCommand (commandText, connection, transaction))
            {
              command.ExecuteNonQuery();
            }
          }
        }
      }
    }

    public void LoadTestData (string sqlFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sqlFileName", sqlFileName);

      using (SqlConnection connection = new SqlConnection (_connectionString))
      {
        connection.Open();

        using (SqlTransaction transaction = connection.BeginTransaction())
        {
          PerformLoadTestData (connection, transaction, sqlFileName);
          transaction.Commit();
        }
      }
    }

    protected virtual void PerformLoadTestData (SqlConnection connection, SqlTransaction transaction, string sqlFileName)
    {
      ExecuteSqlFile (connection, transaction, sqlFileName);
    }

    protected void ExecuteSqlFile (SqlConnection connection, SqlTransaction transaction, string sqlFile)
    {
      using (SqlCommand command = new SqlCommand (GetCommandTextFromFile (sqlFile), connection, transaction))
      {
        command.ExecuteNonQuery();
      }
    }

    private void ExecuteCommand (string commandText)
    {
      using (SqlConnection connection = new SqlConnection (_connectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand (commandText, connection))
        {
          command.ExecuteNonQuery();
        }
      }
    }

    private string GetCommandTextFromFile (string sqlFile)
    {
      return File.ReadAllText (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, sqlFile), Encoding.Default);
    }

    private string[] GetCommandTextBatchesFromFile (string sqlFile)
    {
      return GetCommandTextFromFile (sqlFile).Split (new string[] { "\r\nGO\r\n" }, StringSplitOptions.RemoveEmptyEntries);
    }
  }
}