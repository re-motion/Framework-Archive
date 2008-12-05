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
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Data.SqlClient
{
  /// <summary>Use the <see cref="DatabaseAgent"/> for setting up the database during unit testing.</summary>
  public class DatabaseAgent
  {
    private readonly string _connectionString;

    public DatabaseAgent (string connectionString)
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

    public int ExecuteBatch (string sqlFileName, bool useTransaction)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sqlFileName", sqlFileName);

      int count = 0;
      using (IDbConnection connection = CreateConnection())
      {
        connection.Open();
        if (useTransaction)
        {
          using (IDbTransaction transaction = connection.BeginTransaction ())
          {
            count = ExecuteBatch (connection, sqlFileName, transaction);
            transaction.Commit ();
          }
        }
        else
        {
          count = ExecuteBatch (connection, sqlFileName, null);
        }
      }

      return count;
    }

    protected virtual IDbConnection CreateConnection ()
    {
      return new SqlConnection (_connectionString);
    }

    protected virtual IDbCommand CreateCommand (IDbConnection connection, string commandText, IDbTransaction transaction)
    {
      IDbCommand command = connection.CreateCommand ();
      command.CommandType = CommandType.Text;
      command.CommandText = commandText;
      command.Transaction = transaction;
      return command;
    }

    public int ExecuteCommand (string commandText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("commandText", commandText);

      using (IDbConnection connection = CreateConnection())
      {
        connection.Open();
        return ExecuteCommand (connection, commandText, null);
      }
    }

    public object ExecuteScalarCommand (string commandText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("commandText", commandText);

      using (IDbConnection connection = CreateConnection())
      {
        connection.Open ();
        return ExecuteScalarCommand(connection, commandText, null);
      }
    }

    protected virtual int ExecuteBatch (IDbConnection connection, string sqlFileName, IDbTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("connection", connection);
      ArgumentUtility.CheckNotNullOrEmpty ("sqlFileName", sqlFileName);

      int count = 0;
      foreach (string commandText in GetCommandTextBatchesFromFile (sqlFileName))
        count += ExecuteCommand (connection, commandText, transaction);
      return count;
    }

    protected virtual int ExecuteCommand (IDbConnection connection, string commandText, IDbTransaction transaction)
    {
      using (IDbCommand command = CreateCommand (connection, commandText, transaction))
      {
        return command.ExecuteNonQuery();
      }
    }

    protected virtual object ExecuteScalarCommand (IDbConnection connection, string commandText, IDbTransaction transaction)
    {
      using (IDbCommand command = CreateCommand (connection, commandText, transaction))
      {
        return command.ExecuteScalar ();
      }
    }

    private string[] GetCommandTextBatchesFromFile (string sqlFileName)
    {
      if (!Path.IsPathRooted (sqlFileName))
      {
        string assemblyUrl = typeof (DatabaseAgent).Assembly.CodeBase;
        Uri uri = new Uri (assemblyUrl);
        sqlFileName = Path.Combine (Path.GetDirectoryName(uri.LocalPath), sqlFileName);
      }
      string fileContent = File.ReadAllText (sqlFileName, Encoding.Default);
      return fileContent.Split (new string[] {"\r\nGO\r\n", "\nGO\n"}, StringSplitOptions.RemoveEmptyEntries);
    }
  }
}
