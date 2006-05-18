using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text;

using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.Resources;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Database
{
  public class TestDataLoader
  {
    // types

    // static members and constants

    // member fields

    private string _connectionString;

    // construction and disposing

    public TestDataLoader (string connectionString)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("connectionString", connectionString);

      _connectionString = connectionString;
    }

    // methods and properties

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
      using (SqlCommand command = new SqlCommand (File.ReadAllText (sqlFile, Encoding.Default), connection, transaction))
      {
        command.ExecuteNonQuery ();
      }
    }
  }
}
