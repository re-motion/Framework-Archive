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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// <see cref="UnionViewDefinition"/> defines a union view in a relational database.
  /// </summary>
  public class UnionViewDefinition : IEntityDefinition
  {
    private class ColumnDefinitionFinder : IColumnDefinitionVisitor
    {
      private readonly HashSet<IColumnDefinition> _availableColumns;

      private IColumnDefinition _foundColumn;

      public ColumnDefinitionFinder (IEnumerable<IColumnDefinition> availableColumns)
      {
        _availableColumns = new HashSet<IColumnDefinition> (availableColumns);
      }

      public IColumnDefinition FindColumn (IColumnDefinition columnDefinition)
      {
        _foundColumn = null;
        columnDefinition.Accept (this);
        return _foundColumn;
      }

      void IColumnDefinitionVisitor.VisitSimpleColumnDefinition (SimpleColumnDefinition simpleColumnDefinition)
      {
        if (_availableColumns.Contains (simpleColumnDefinition))
          _foundColumn = simpleColumnDefinition;
        else
          _foundColumn = new NullColumnDefinition ();
      }

      void IColumnDefinitionVisitor.VisitObjectIDWithClassIDColumnDefinition (ObjectIDWithClassIDColumnDefinition objectIDWithClassIDColumnDefinition)
      {
        if (_availableColumns.Contains (objectIDWithClassIDColumnDefinition))
        {
          _foundColumn = objectIDWithClassIDColumnDefinition;
        }
        else
        {
          var objectIDColumn = FindColumn (objectIDWithClassIDColumnDefinition.ObjectIDColumn);
          var classIDColumn = FindColumn (objectIDWithClassIDColumnDefinition.ClassIDColumn);
          _foundColumn = new ObjectIDWithClassIDColumnDefinition (objectIDColumn, classIDColumn);
        }
      }

      public void VisitNullColumnDefinition (NullColumnDefinition nullColumnDefinition)
      {
        _foundColumn = new NullColumnDefinition ();
      }
    }

    private readonly string _viewName;
    private readonly ReadOnlyCollection<IEntityDefinition> _unionedEntities;
    private readonly ReadOnlyCollection<IColumnDefinition> _columns;
    private readonly StorageProviderDefinition _storageProviderDefinition;

    public UnionViewDefinition (
        StorageProviderDefinition storageProviderDefinition,
        string viewName,
        IEnumerable<IEntityDefinition> unionedEntities,
        IEnumerable<IColumnDefinition> columns)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotEmpty ("viewName", viewName);
      ArgumentUtility.CheckNotNullOrEmpty ("unionedEntities", unionedEntities);
      ArgumentUtility.CheckNotNull ("columns", columns);

      var unionedEntitiesList = unionedEntities.ToList().AsReadOnly();
      for (int i = 0; i < unionedEntitiesList.Count; ++i)
      {
        var unionedEntity = unionedEntitiesList[i];
        if (!(unionedEntity is TableDefinition || unionedEntity is UnionViewDefinition))
        {
          throw new ArgumentItemTypeException (
              "unionedEntities",
              i,
              null,
              unionedEntity.GetType(),
              "The unioned entities must either be a TableDefinitions or UnionViewDefinitions.");
        }
      }

      _storageProviderDefinition = storageProviderDefinition;
      _viewName = viewName;
      _unionedEntities = unionedEntitiesList;
      _columns = columns.ToList().AsReadOnly();
    }

    public string StorageProviderID
    {
      get { return _storageProviderDefinition.Name; }
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public string ViewName
    {
      get { return _viewName; }
    }

    public ReadOnlyCollection<IEntityDefinition> UnionedEntities
    {
      get { return _unionedEntities; }
    }

    public string LegacyEntityName
    {
      get { return null; }
    }

    public string LegacyViewName
    {
      get { return _viewName; }
    }

    public ReadOnlyCollection<IColumnDefinition> GetColumns ()
    {
      return _columns;
    }

    public IColumnDefinition[] CreateFullColumnList (IEnumerable<IColumnDefinition> availableColumns)
    {
      ArgumentUtility.CheckNotNull ("availableColumns", availableColumns);

      var finder = new ColumnDefinitionFinder (availableColumns);
      return _columns.Select (c => finder.FindColumn (c)).ToArray ();
    }

    // Always returns at least one table
    public IEnumerable<TableDefinition> GetAllTables ()
    {
      foreach (var entityDefinition in _unionedEntities)
      {
        if (entityDefinition is TableDefinition)
          yield return (TableDefinition) entityDefinition;
        else
        {
          foreach (var derivedTable in ((UnionViewDefinition) entityDefinition).GetAllTables())
            yield return derivedTable;
        }
      }
    }

    public void Accept (IEntityDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitUnionViewDefinition (this);
    }
  }
}