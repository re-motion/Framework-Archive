// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  public class DatabaseHelper
  {
    public const string SetupDBScript = "SecurityManagerSetupDB.sql";
    public const string SetupConstraintsScript = "SecurityManagerSetupConstraints.sql";
    public const string SetupDBSpecialTablesScript = "SecurityManagerSetupDBSpecialTables.sql";

    public void SetupDB ()
    {
      IDbConnection connection = GetConnection ();
      IDbTransaction transaction = connection.BeginTransaction ();

      try
      {
        ExecuteSql (ReadFile (SetupDBScript), connection, transaction);
        ExecuteSql (ReadFile (SetupConstraintsScript), connection, transaction);
        ExecuteSql (ReadFile (SetupDBSpecialTablesScript), connection, transaction);
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
      RdbmsProviderDefinition providerDefinition = 
          (RdbmsProviderDefinition) DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions["SecurityManager"];
      IDbConnection connection = new SqlConnection (providerDefinition.ConnectionString);
      connection.Open ();

      return connection;
    }
  }
}
