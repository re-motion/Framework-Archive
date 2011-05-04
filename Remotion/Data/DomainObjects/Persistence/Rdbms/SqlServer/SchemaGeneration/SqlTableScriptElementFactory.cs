// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using System.Linq;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// The <see cref="SqlTableScriptElementFactory"/> is responsible to create script-elements for tables in a sql-server database.
  /// </summary>
  public class SqlTableScriptElementFactory : ITableScriptElementFactory
  {
    public IScriptElement GetCreateElement (TableDefinition tableDefinition)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);

      var columnDeclarationList = SeparatedStringBuilder.Build (",\r\n", tableDefinition.Columns.Select (GetColumnDeclaration));
      var primaryKeyConstraintString = GetPrimaryKeyDeclaration (tableDefinition);
      return
          new ScriptStatement (
              string.Format (
                  "CREATE TABLE [{0}].[{1}]\r\n(\r\n{2}{3}\r\n)",
                  tableDefinition.TableName.SchemaName ?? CompositeScriptBuilder2.DefaultSchema,
                  tableDefinition.TableName.EntityName,
                  columnDeclarationList,
                  primaryKeyConstraintString));
    }

    public IScriptElement GetDropElement (TableDefinition tableDefinition)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);

      return new ScriptStatement(
        string.Format("IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = '{1}' AND TABLE_SCHEMA = '{0}')\r\n"
                         + "  DROP TABLE [{0}].[{1}]",
                  tableDefinition.TableName.SchemaName ?? CompositeScriptBuilder2.DefaultSchema,
                  tableDefinition.TableName.EntityName));
    }

    private string GetColumnDeclaration (SimpleColumnDefinition column)
    {
      return string.Format ("  [{0}] {1} {2}", column.Name, column.StorageType, column.IsNullable ? "NULL" : "NOT NULL");
    }

    private string GetPrimaryKeyDeclaration (TableDefinition tableDefinition)
    {
      var primaryKeyConstraint = tableDefinition.Constraints.OfType<PrimaryKeyConstraintDefinition>().SingleOrDefault();
      if (primaryKeyConstraint == null)
        return string.Empty;

      //TODO RM-3975
      var columnNameList = SeparatedStringBuilder.Build (", ", primaryKeyConstraint.Columns.Select (c => "[" + c.Name + "]"));
      return string.Format (
          ",\r\n  CONSTRAINT [{0}] PRIMARY KEY {1} ({2})",
          primaryKeyConstraint.ConstraintName,
          primaryKeyConstraint.IsClustered ? "CLUSTERED" : "NONCLUSTERED",
          columnNameList);
    }
  }
}