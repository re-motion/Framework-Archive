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
  /// <see cref="TableDefinition"/> defines a table in a relational database.
  /// </summary>
  public class TableDefinition : EntityDefinitionBase
  {
    private readonly EntityNameDefinition _tableName;
    private readonly ReadOnlyCollection<ITableConstraintDefinition> _constraints;

    public TableDefinition (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition tableName,
        EntityNameDefinition viewName,
        ColumnDefinition objectIDColumnDefinition,
        ColumnDefinition classIDColumnDefinition,
        ColumnDefinition timstampColumnDefinition,
        IEnumerable<ColumnDefinition> dataColumns,
        ObjectIDStoragePropertyDefinition objectIDProperty,
        IRdbmsStoragePropertyDefinition timestampProperty,
        IEnumerable<IRdbmsStoragePropertyDefinition> dataProperties,
        IEnumerable<ITableConstraintDefinition> constraints,
        IEnumerable<IIndexDefinition> indexes,
        IEnumerable<EntityNameDefinition> synonyms)
        : base (
            storageProviderDefinition,
            viewName,
            objectIDColumnDefinition,
            classIDColumnDefinition,
            timstampColumnDefinition,
            dataColumns,
            objectIDProperty,
            timestampProperty,
            dataProperties,
            indexes,
            synonyms)
    {
      ArgumentUtility.CheckNotNull ("tableName", tableName);
      ArgumentUtility.CheckNotNull ("constraints", constraints);

      _tableName = tableName;
      _constraints = constraints.ToList().AsReadOnly();
    }

    // TODO 4231: Remove
    public TableDefinition (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition tableName,
        EntityNameDefinition viewName,
        ColumnDefinition objectIDColumnDefinition,
        ColumnDefinition classIDColumnDefinition,
        ColumnDefinition timstampColumnDefinition,
        IEnumerable<ColumnDefinition> dataColumns,
        IEnumerable<ITableConstraintDefinition> constraints,
        IEnumerable<IIndexDefinition> indexes,
        IEnumerable<EntityNameDefinition> synonyms)
      : this (
          storageProviderDefinition,
          tableName,
          viewName,
          objectIDColumnDefinition,
          classIDColumnDefinition,
          timstampColumnDefinition,
          dataColumns,
          null,
          null,
          null,
          constraints,
          indexes,
          synonyms)
    {
    }

    public EntityNameDefinition TableName
    {
      get { return _tableName; }
    }

    public ReadOnlyCollection<ITableConstraintDefinition> Constraints
    {
      get { return _constraints; }
    }

    public override void Accept (IEntityDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitTableDefinition (this);
    }
  }
}