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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  public static class UnionViewDefinitionObjectMother
  {
    public static UnionViewDefinition Create (StorageProviderDefinition storageProviderDefinition)
    {
      return Create (storageProviderDefinition, new EntityNameDefinition ("TestSchema", "TestUnion"));
    }

    public static UnionViewDefinition Create (
    StorageProviderDefinition storageProviderDefinition, EntityNameDefinition viewName)
    {
      return Create (storageProviderDefinition, viewName, TableDefinitionObjectMother.Create (storageProviderDefinition));
    }

    public static UnionViewDefinition Create (
        StorageProviderDefinition storageProviderDefinition, EntityNameDefinition viewName, params IEntityDefinition[] unionedEntities)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("unionedEntities", unionedEntities);

      return Create (
          storageProviderDefinition, 
          viewName, 
          unionedEntities, 
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty, 
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty, 
          new IRdbmsStoragePropertyDefinition[0]);
    }

    public static UnionViewDefinition Create (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition viewName,
        IEnumerable<IEntityDefinition> unionedEntities,
        ObjectIDStoragePropertyDefinition objectIDProperty,
        SimpleStoragePropertyDefinition timestampProperty,
        params IRdbmsStoragePropertyDefinition[] dataProperties)
    {
      return new UnionViewDefinition (
          storageProviderDefinition,
          viewName,
          unionedEntities,
          ObjectIDStoragePropertyDefinitionTestHelper.GetIDColumnDefinition (objectIDProperty),
          ObjectIDStoragePropertyDefinitionTestHelper.GetClassIDColumnDefinition (objectIDProperty),
          timestampProperty.ColumnDefinition,
          dataProperties.SelectMany (p => p.GetColumns()),
          objectIDProperty,
          timestampProperty,
          dataProperties,
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    public static UnionViewDefinition CreateWithIndexes (StorageProviderDefinition storageProviderDefinition, IEnumerable<IIndexDefinition> indexDefinitions)
    {
      return new UnionViewDefinition (
          storageProviderDefinition,
          new EntityNameDefinition ("TestSchema", "TestUnionView"),
          new[] { TableDefinitionObjectMother.Create (storageProviderDefinition) },
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new ColumnDefinition[0],
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          new IRdbmsStoragePropertyDefinition[0],
          indexDefinitions,
          new EntityNameDefinition[0]);
    }

    public static UnionViewDefinition CreateWithSynonyms (StorageProviderDefinition storageProviderDefinition, IEnumerable<EntityNameDefinition> synonyms)
    {
      return new UnionViewDefinition (
          storageProviderDefinition,
          new EntityNameDefinition ("TestSchema", "TestUnionView"),
          new[] { TableDefinitionObjectMother.Create (storageProviderDefinition) },
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new ColumnDefinition[0],
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          new IRdbmsStoragePropertyDefinition[0],
          new IIndexDefinition[0],
          synonyms);
    }
  }
}