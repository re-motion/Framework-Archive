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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders
{
  /// <summary>
  /// The <see cref="SqlDbCommandBuilderFactory"/> creates SQL Server-specific <see cref="IDbCommandBuilder"/> instances.
  /// </summary>
  public class SqlDbCommandBuilderFactory : IDbCommandBuilderFactory
  {
    private readonly ISqlDialect _sqlDialect;
    private readonly IValueConverter _valueConverter;
    private readonly StorageProviderDefinition _storageProviderDefinition;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDbCommandBuilderFactory"/> class.
    /// </summary>
    /// <param name="sqlDialect">The SQL dialect.</param>
    /// <param name="valueConverter">The value converter.</param>
    /// <param name="storageProviderDefinition">The storage provider definition of the relational database.</param>
    public SqlDbCommandBuilderFactory (ISqlDialect sqlDialect, IValueConverter valueConverter, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);
      ArgumentUtility.CheckNotNull ("valueConverter", valueConverter);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      _sqlDialect = sqlDialect;
      _valueConverter = valueConverter;
      _storageProviderDefinition = storageProviderDefinition;
    }

    public IDbCommandBuilder CreateForSingleIDLookupFromTable (
        TableDefinition table, IEnumerable<ColumnDefinition> selectedColumns, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var columnValue = new ColumnValue (table.IDColumn, objectID.Value);
      return new SelectDbCommandBuilder (
          table,
          new SelectedColumnsSpecification (selectedColumns),
          new ComparedColumnsSpecification (new[] { columnValue }),
          EmptyOrderedColumnsSpecification.Instance,
          _sqlDialect,
          _valueConverter);
    }

    public IDbCommandBuilder CreateForMultiIDLookupFromTable (
        TableDefinition table, IEnumerable<ColumnDefinition> selectedColumns, ObjectID[] objectIDs)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      return new SelectDbCommandBuilder (
          table,
          new SelectedColumnsSpecification (selectedColumns),
          new SqlXmlSetComparedColumnSpecification (table.IDColumn, GetObjectIDValues(objectIDs)),
          EmptyOrderedColumnsSpecification.Instance,
          _sqlDialect,
          _valueConverter);
    }

    public IDbCommandBuilder CreateForRelationLookupFromTable (
        TableDefinition table,
        IEnumerable<ColumnDefinition> selectedColumns,
        IRdbmsStoragePropertyDefinition foreignKeyStorageProperty,
        ObjectID foreignKeyValue,
        IEnumerable<OrderedColumn> orderedColumns)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("foreignKeyStorageProperty", foreignKeyStorageProperty);
      ArgumentUtility.CheckNotNull ("foreignKeyValue", foreignKeyValue);
      ArgumentUtility.CheckNotNull ("orderedColumns", orderedColumns);

      var orderedColumnsSpecification = orderedColumns.Any()
                                            ? (IOrderedColumnsSpecification) new OrderedColumnsSpecification (orderedColumns)
                                            : EmptyOrderedColumnsSpecification.Instance;
      return new SelectDbCommandBuilder (
          table,
          new SelectedColumnsSpecification (selectedColumns),
          new ComparedColumnsSpecification (foreignKeyStorageProperty.SplitValueForComparison (foreignKeyValue)),
          orderedColumnsSpecification,
          _sqlDialect,
          _valueConverter);
    }

    public IDbCommandBuilder CreateForRelationLookupFromUnionView (
        UnionViewDefinition view,
        IEnumerable<ColumnDefinition> selectedColumns,
        IRdbmsStoragePropertyDefinition foreignKeyStorageProperty,
        ObjectID foreignKeyValue,
        IEnumerable<OrderedColumn> orderedColumns)
    {
      ArgumentUtility.CheckNotNull ("view", view);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("foreignKeyStorageProperty", foreignKeyStorageProperty);
      ArgumentUtility.CheckNotNull ("foreignKeyValue", foreignKeyValue);
      ArgumentUtility.CheckNotNull ("orderedColumns", orderedColumns);

      var orderedColumnsSpecification = orderedColumns.Any ()
                                            ? (IOrderedColumnsSpecification) new OrderedColumnsSpecification (orderedColumns)
                                            : EmptyOrderedColumnsSpecification.Instance;
      return new UnionRelationLookupSelectDbCommandBuilder (
          view,
          new SelectedColumnsSpecification (selectedColumns),
          foreignKeyStorageProperty,
          foreignKeyValue,
          orderedColumnsSpecification,
          _sqlDialect,
          _valueConverter);
    }

    public IDbCommandBuilder CreateForQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return new QueryDbCommandBuilder (query, _sqlDialect, _valueConverter);
    }

    public IDbCommandBuilder CreateForInsert (TableDefinition tableDefinition, IEnumerable<ColumnValue> insertedColumns)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("insertedColumns", insertedColumns);

      return new InsertDbCommandBuilder (tableDefinition, new InsertedColumnsSpecification (insertedColumns), _sqlDialect, _valueConverter);
    }

    public IDbCommandBuilder CreateForUpdate (
        TableDefinition tableDefinition,
        IEnumerable<ColumnValue> updatedColumns,
        IEnumerable<ColumnValue> comparedColumns)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("updatedColumns", updatedColumns);
      ArgumentUtility.CheckNotNull ("comparedColumns", comparedColumns);

      return new UpdateDbCommandBuilder (
          tableDefinition,
          new UpdatedColumnsSpecification (updatedColumns),
          new ComparedColumnsSpecification (comparedColumns),
          _sqlDialect,
          _valueConverter);
    }

    public IDbCommandBuilder CreateForDelete (TableDefinition tableDefinition, IEnumerable<ColumnValue> comparedColumns)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("comparedColumns", comparedColumns);

      return new DeleteDbCommandBuilder (tableDefinition, new ComparedColumnsSpecification (comparedColumns), _sqlDialect, _valueConverter);
    }

    
    private IEnumerable<object > GetObjectIDValues (IEnumerable<ObjectID> objectIDs)
    {
      foreach (var t in objectIDs)
      {
        if (t.StorageProviderDefinition!=_storageProviderDefinition)
          throw new ArgumentException ("Multi-ID lookups can only be performed for ObjectIDs from this storage provider.", "objectIDs");
        yield return t.Value;
      }
    }
  }
}