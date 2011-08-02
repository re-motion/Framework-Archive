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
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// The <see cref="SaveCommandFactory"/> is responsible to reate save commands for a relational database.
  /// </summary>
  public class SaveCommandFactory
  {
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
    private readonly ITableDefinitionFinder _tableDefinitionFinder;

    public SaveCommandFactory (
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider,
        ITableDefinitionFinder tableDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilderFactory", dbCommandBuilderFactory);
      ArgumentUtility.CheckNotNull ("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);
      ArgumentUtility.CheckNotNull ("tableDefinitionFinder", tableDefinitionFinder);

      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
      _tableDefinitionFinder = tableDefinitionFinder;
    }

    public IStorageProviderCommand<IRdbmsProviderCommandExecutionContext> CreateForSave (IEnumerable<DataContainer> dataContainers)
    {
      ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

      return new MultiDataContainerSaveCommand (CreateDbCommandsForSave (dataContainers));
    }

    private IEnumerable<Tuple<ObjectID, IDbCommandBuilder>> CreateDbCommandsForSave (IEnumerable<DataContainer> dataContainers)
    {
      var insertCommands = new List<Tuple<ObjectID, IDbCommandBuilder>>();
      var updateCommands = new List<Tuple<ObjectID, IDbCommandBuilder>>();
      var deleteCommands = new List<Tuple<ObjectID, IDbCommandBuilder>>();

      foreach (var dataContainer in dataContainers)
      {
        var tableDefinition = _tableDefinitionFinder.GetTableDefinition (dataContainer.ID);

        if (dataContainer.State == StateType.New)
          insertCommands.Add (Tuple.Create (dataContainer.ID, CreateDbCommandForInsert (dataContainer, tableDefinition)));
        if (dataContainer.State == StateType.Deleted)
          deleteCommands.Add (Tuple.Create (dataContainer.ID, CreateDbCommandForDelete (dataContainer, tableDefinition)));

        var updatedColumnValues = GetUpdatedColumnValues (dataContainer, tableDefinition);
        if (updatedColumnValues.Any())
        {
          var dbCommandForUpdate = CreateDbCommandForUpdate (dataContainer, tableDefinition, updatedColumnValues);
          updateCommands.Add (Tuple.Create (dataContainer.ID, dbCommandForUpdate));
        }
      }

      return insertCommands.Concat (updateCommands).Concat (deleteCommands);
    }

    private IDbCommandBuilder CreateDbCommandForUpdate (
        DataContainer dataContainer,
        TableDefinition tableDefinition,
        IEnumerable<ColumnValue> updatedColumnValues)
    {
      var updatedColumnsSpecification = new UpdatedColumnsSpecification (updatedColumnValues);
      var comparedColumnSpecification = new ComparedColumnsSpecification (GetComparedColumnValuesForUpdate (dataContainer, tableDefinition));

      return _dbCommandBuilderFactory.CreateForUpdate (tableDefinition, updatedColumnsSpecification, comparedColumnSpecification);
    }

    private IEnumerable<ColumnValue> GetComparedColumnValuesForUpdate (DataContainer dataContainer, TableDefinition tableDefinition)
    {
      yield return new ColumnValue (tableDefinition.IDColumn, dataContainer.ID.Value);
      if (dataContainer.State != StateType.New)
        yield return new ColumnValue (tableDefinition.TimestampColumn, dataContainer.Timestamp);
    }

    private ColumnValue[] GetUpdatedColumnValues (DataContainer dataContainer, TableDefinition tableDefinition)
    {
      var propertyFilter = GetUpdatedPropertyFilter (dataContainer);

      var dataStorageColumnValues = dataContainer.PropertyValues.Cast<PropertyValue>()
          .Where (pv => pv.Definition.StorageClass == StorageClass.Persistent && propertyFilter (pv))
          .SelectMany (
              pv =>
              {
                var storageProperty = _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition (pv.Definition);
                var columnValues = storageProperty.SplitValue (pv.GetValueWithoutEvents (ValueAccess.Current));

                return columnValues;
              }
          )
          .ToArray();

      if (!dataStorageColumnValues.Any() && dataContainer.HasBeenMarkedChanged)
      {
        //dummy column value for the case that the data container should only change its timestamp
        return new[] { new ColumnValue (tableDefinition.ClassIDColumn, dataContainer.ID.ClassID) };
      }
      else
        return dataStorageColumnValues;
    }

    private Func<PropertyValue, bool> GetUpdatedPropertyFilter (DataContainer dataContainer)
    {
      if (dataContainer.State == StateType.New || dataContainer.State == StateType.Deleted)
        return pv => pv.Definition.IsObjectID;
      else if (dataContainer.State == StateType.Changed)
        return pv => pv.HasChanged;
      else
        return pv => false;
    }

    private IDbCommandBuilder CreateDbCommandForInsert (DataContainer dataContainer, TableDefinition tableDefinition)
    {
      var columnValues = GetInsertedColumnValues (dataContainer, tableDefinition);

      return _dbCommandBuilderFactory.CreateForInsert (tableDefinition, new InsertedColumnsSpecification (columnValues));
    }

    private IDbCommandBuilder CreateDbCommandForDelete (DataContainer dataContainer, TableDefinition tableDefinition)
    {
      var columnValues = GetComparedColumnValuesForDelete (dataContainer, tableDefinition);

      return _dbCommandBuilderFactory.CreateForDelete (tableDefinition, new ComparedColumnsSpecification (columnValues));
    }

    private IEnumerable<ColumnValue> GetComparedColumnValuesForDelete (DataContainer dataContainer, TableDefinition tableDefinition)
    {
      yield return new ColumnValue (tableDefinition.IDColumn, dataContainer.ID.Value);
      var mustAddTimestamp = !dataContainer.PropertyValues.Cast<PropertyValue>().Any (propertyValue => propertyValue.Definition.IsObjectID);
      if (mustAddTimestamp)
        yield return new ColumnValue (tableDefinition.TimestampColumn, dataContainer.Timestamp);
    }

    private IEnumerable<ColumnValue> GetInsertedColumnValues (DataContainer dataContainer, TableDefinition tableDefinition)
    {
      var objectIDStorageProperty =
          new
          {
              StorageProperty =
                  (IRdbmsStoragePropertyDefinition)
                  InfrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition (tableDefinition),
              Value = (object) dataContainer.ID
          };


      var dataStorageProperties = dataContainer.PropertyValues.Cast<PropertyValue>()
          .Where (pv => pv.Definition.StorageClass == StorageClass.Persistent && !pv.Definition.IsObjectID)
          .Select (
              pv => new
                    {
                        StorageProperty = _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition (pv.Definition),
                        Value = pv.GetValueWithoutEvents (ValueAccess.Current)
                    }
          );
      var allStorageProperties = new[] { objectIDStorageProperty }.Concat (dataStorageProperties);

      return allStorageProperties.SelectMany (storageProperty => storageProperty.StorageProperty.SplitValue (storageProperty.Value));
    }
  }
}