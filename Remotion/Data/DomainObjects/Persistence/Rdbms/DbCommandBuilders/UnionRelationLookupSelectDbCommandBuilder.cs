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
using System.Data;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  /// <summary>
  /// The <see cref="UnionRelationLookupSelectDbCommandBuilder"/> builds a command that allows retrieving a set of records where a foreign key column
  /// matches a given <see cref="ObjectID"/> value from a set of unioned tables.
  /// </summary>
  public class UnionRelationLookupSelectDbCommandBuilder : DbCommandBuilder
  {
    private readonly UnionViewDefinition _unionViewDefinition;
    private readonly ISelectedColumnsSpecification _selectedColumns;
    private readonly IRdbmsStoragePropertyDefinition _foreignKeyColumn;
    private readonly ObjectID _foreignKeyValue;
    private readonly IOrderedColumnsSpecification _orderedColumns;

    public UnionRelationLookupSelectDbCommandBuilder (
        UnionViewDefinition unionViewDefinition,
        ISelectedColumnsSpecification selectedColumns,
        IRdbmsStoragePropertyDefinition foreignKeyColumn,
        ObjectID foreignKeyValue,
        IOrderedColumnsSpecification orderedColumns,
        ISqlDialect sqlDialect,
        IValueConverter valueConverter)
        : base (sqlDialect, valueConverter)
    {
      ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("foreignKeyColumn", foreignKeyColumn);
      ArgumentUtility.CheckNotNull ("foreignKeyValue", foreignKeyValue);
      ArgumentUtility.CheckNotNull ("orderedColumns", orderedColumns);

      _unionViewDefinition = unionViewDefinition;
      _selectedColumns = selectedColumns;
      _foreignKeyColumn = foreignKeyColumn;
      _foreignKeyValue = foreignKeyValue;
      _orderedColumns = orderedColumns;
    }

    public UnionViewDefinition UnionViewDefinition
    {
      get { return _unionViewDefinition; }
    }

    public ISelectedColumnsSpecification SelectedColumns
    {
      get { return _selectedColumns; }
    }

    public IRdbmsStoragePropertyDefinition ForeignKeyColumn
    {
      get { return _foreignKeyColumn; }
    }

    public ObjectID ForeignKeyValue
    {
      get { return _foreignKeyValue; }
    }

    public IOrderedColumnsSpecification OrderedColumns
    {
      get { return _orderedColumns; }
    }

    public override IDbCommand Create (IRdbmsProviderCommandExecutionContext commandExecutionContext)
    {
      ArgumentUtility.CheckNotNull ("commandExecutionContext", commandExecutionContext);

      var command = commandExecutionContext.CreateDbCommand ();

      var fullProjection = _orderedColumns.UnionWithSelectedColumns (_selectedColumns);

      var statement = new StringBuilder();
      var lookupColumn = _foreignKeyColumn.GetColumnForLookup();
      var parameter = AddCommandParameter (command, lookupColumn.Name, _foreignKeyValue);
      bool first = true;
      foreach (var table in _unionViewDefinition.GetAllTables())
      {
        if (!first)
          statement.Append (" UNION ALL ");
        
        AppendSelectClause (statement, fullProjection);
        AppendFromClause (statement, table);
        AppendComparingWhereClause (statement, lookupColumn, parameter);
        
        first = false;
      }

      AppendOrderByClause (statement, _orderedColumns);

      statement.Append (SqlDialect.StatementDelimiter);

      command.CommandText = statement.ToString();

      return command;
    }
  }
}