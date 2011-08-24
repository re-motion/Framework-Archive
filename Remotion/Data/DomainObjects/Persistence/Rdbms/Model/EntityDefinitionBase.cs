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
  /// <see cref="EntityDefinitionBase"/> is the base-class for all entity definitions.
  /// </summary>
  public abstract class EntityDefinitionBase : IEntityDefinition
  {
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly EntityNameDefinition _viewName;
    private readonly ReadOnlyCollection<EntityNameDefinition> _synonyms;
    private readonly ColumnDefinition _idColumn;
    private readonly ColumnDefinition _classIDColumn;
    private readonly ColumnDefinition _timestampColumn;
    private readonly ObjectIDStoragePropertyDefinition _objectIDProperty;
    private readonly IRdbmsStoragePropertyDefinition _timestampProperty;
    private readonly ReadOnlyCollection<IRdbmsStoragePropertyDefinition> _dataProperties;
    private readonly ReadOnlyCollection<ColumnDefinition> _dataColumns;
    private readonly ReadOnlyCollection<IIndexDefinition> _indexes;

    protected EntityDefinitionBase (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition viewName,
        ColumnDefinition idColumn,
        ColumnDefinition classIDColumn,
        ColumnDefinition timstampColumn,
        IEnumerable<ColumnDefinition> dataColumns,
        ObjectIDStoragePropertyDefinition objectIDProperty,
        IRdbmsStoragePropertyDefinition timestampProperty,
        IEnumerable<IRdbmsStoragePropertyDefinition> dataProperties,
        IEnumerable<IIndexDefinition> indexes,
        IEnumerable<EntityNameDefinition> synonyms)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("idColumn", idColumn);
      ArgumentUtility.CheckNotNull ("classIDColumn", classIDColumn);
      ArgumentUtility.CheckNotNull ("timstampColumn", timstampColumn);
      ArgumentUtility.CheckNotNull ("dataColumns", dataColumns);
      //ArgumentUtility.CheckNotNull ("objectIDProperty", objectIDProperty); // TODO 4231: Uncomment
      //ArgumentUtility.CheckNotNull ("timestampProperty", timestampProperty);
      //ArgumentUtility.CheckNotNull ("dataProperties", dataProperties);
      ArgumentUtility.CheckNotNull ("synonyms", synonyms);

      _storageProviderDefinition = storageProviderDefinition;
      _viewName = viewName;
      _idColumn = idColumn;
      _classIDColumn = classIDColumn;
      _timestampColumn = timstampColumn;
      _objectIDProperty = objectIDProperty;
      _timestampProperty = timestampProperty;
      _dataProperties = dataProperties == null ? null : dataProperties.ToList().AsReadOnly(); // TODO 4231: Remove null check
      _dataColumns = dataColumns.ToList().AsReadOnly();
      _indexes = indexes.ToList().AsReadOnly();
      _synonyms = synonyms.ToList().AsReadOnly();
    }

    public abstract void Accept (IEntityDefinitionVisitor visitor);

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public string StorageProviderID
    {
      get { return _storageProviderDefinition.Name; }
    }

    public EntityNameDefinition ViewName
    {
      get { return _viewName; }
    }

    public ColumnDefinition IDColumn
    {
      get { return _idColumn; }
    }

    public ColumnDefinition ClassIDColumn
    {
      get { return _classIDColumn; }
    }

    public ColumnDefinition TimestampColumn
    {
      get { return _timestampColumn; }
    }

    public IEnumerable<ColumnDefinition> DataColumns
    {
      get { return _dataColumns; }
    }

    public ObjectIDStoragePropertyDefinition ObjectIDProperty
    {
      get { return _objectIDProperty; }
    }

    public IRdbmsStoragePropertyDefinition TimestampProperty
    {
      get { return _timestampProperty; }
    }

    public IEnumerable<IRdbmsStoragePropertyDefinition> DataProperties
    {
      get { return _dataProperties; }
    }

    public IEnumerable<ColumnDefinition> GetAllColumns ()
    {
      yield return _idColumn;
      yield return _classIDColumn;
      yield return _timestampColumn;

      foreach (var column in _dataColumns)
        yield return column;
    }

    public ReadOnlyCollection<IIndexDefinition> Indexes
    {
      get { return _indexes; }
    }

    public ReadOnlyCollection<EntityNameDefinition> Synonyms
    {
      get { return _synonyms; }
    }

    bool INullObject.IsNull { get { return false; } }
  }
}