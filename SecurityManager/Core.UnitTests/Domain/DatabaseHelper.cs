using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.SecurityManager.UnitTests.Domain
{
  public class DatabaseHelper
  {
    public const string SetupDBScript = "SecurityManagerSetupDB.sql";

    public void SetupDB ()
    {
      IDbConnection connection = GetConnection ();
      IDbTransaction transaction = connection.BeginTransaction ();

      try
      {
        ExecuteSql (ReadFile (SetupDBScript), connection, transaction);
      }
      catch
      {
        transaction.Rollback ();
        throw;
      }

      transaction.Commit ();
    }

    private void ExecuteSql (string sql, IDbConnection connection, IDbTransaction transaction)
    {
      string[] sqlScriptParts = Regex.Split (sql, @"^[ \t]*GO[ \t]*(\r\n)?", RegexOptions.IgnoreCase | RegexOptions.Multiline);

      foreach (string sqlScriptPart in sqlScriptParts)
      {
        if (sqlScriptPart.Replace ("\r", "").Replace ("\n", "").Replace ("\t", "").Trim () != string.Empty)
        {
          using (IDbCommand command = connection.CreateCommand())
          {
            command.Transaction = transaction;
            command.CommandText = sqlScriptPart;

            command.ExecuteNonQuery ();
          }
        }
      }
    }

    private string ReadFile (string file)
    {
      using (StreamReader reader = new StreamReader (file, Encoding.Default))
      {
        return reader.ReadToEnd ();
      }
    }

    private IDbConnection GetConnection ()
    {
      RdbmsProviderDefinition providerDefinition = (RdbmsProviderDefinition) StorageProviderConfiguration.Current.StorageProviderDefinitions["SecurityManager"];
      IDbConnection connection = new SqlConnection (providerDefinition.ConnectionString);
      connection.Open ();

      return connection;
    }
  }
}
