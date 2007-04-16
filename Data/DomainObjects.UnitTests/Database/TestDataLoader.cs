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

      using (SqlConnection connection = new SqlConnection (_connectionString))
      {
        connection.Open ();
        using (SqlCommand command = new SqlCommand (string.Format ("ALTER DATABASE [{0}] SET READ_WRITE WITH ROLLBACK IMMEDIATE", database), connection))
        {
          command.ExecuteNonQuery ();
        }
      }
    }

    public void SetDatabaseReadOnly (string database)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("database", database);

      using (SqlConnection connection = new SqlConnection (_connectionString))
      {
        connection.Open ();
        using (SqlCommand command = new SqlCommand (string.Format ("ALTER DATABASE [{0}] SET READ_ONLY WITH ROLLBACK IMMEDIATE", database), connection))
        {
          command.ExecuteNonQuery ();
        }
      }
    }

    public void Load (string sqlFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sqlFileName", sqlFileName);

      using (SqlConnection connection = new SqlConnection (_connectionString))
      {
        connection.Open ();

        using (SqlTransaction transaction = connection.BeginTransaction ())
        {
          PerformLoad (connection, transaction, sqlFileName);
          transaction.Commit ();
        }
      }
    }

    protected virtual void PerformLoad (SqlConnection connection, SqlTransaction transaction, string sqlFileName)
    {
      ExecuteSqlFile (connection, transaction, sqlFileName);
    }

    protected void ExecuteSqlFile (SqlConnection connection, SqlTransaction transaction, string sqlFile)
    {
      string fullPath = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, sqlFile);
      using (SqlCommand command = new SqlCommand (File.ReadAllText (fullPath, Encoding.Default), connection, transaction))
      {
        command.ExecuteNonQuery ();
      }
    }
  }
}
