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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  public class LegacyInsertDbCommandBuilder : DbCommandBuilder
  {
    private readonly DataContainer _dataContainer;
    private readonly IStorageNameProvider _storageNameProvider;

    public LegacyInsertDbCommandBuilder (
        IStorageNameProvider storageNameProvider, DataContainer dataContainer, ISqlDialect sqlDialect, IValueConverter valueConverter)
        : base (sqlDialect, valueConverter)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      if (dataContainer.State != StateType.New)
      {
        throw new ArgumentException (
            string.Format ("State of provided DataContainer must be 'New', but is '{0}'.", dataContainer.State), "dataContainer");
      }

      _storageNameProvider = storageNameProvider;
      _dataContainer = dataContainer;
    }

    public IStorageNameProvider StorageNameProvider
    {
      get { return _storageNameProvider; }
    }

    public override IDbCommand Create (IRdbmsProviderCommandExecutionContext commandExecutionContext)
    {
      ArgumentUtility.CheckNotNull ("commandExecutionContext", commandExecutionContext);

      IDbCommand command = commandExecutionContext.CreateDbCommand ();

      var columnBuilder = new StringBuilder();
      var valueBuilder = new StringBuilder();

      string idColumn = StorageNameProvider.IDColumnName;
      string classIDColumn = StorageNameProvider.ClassIDColumnName;

      AppendColumn (columnBuilder, valueBuilder, idColumn, idColumn);
      AppendColumn (columnBuilder, valueBuilder, classIDColumn, classIDColumn);

      AddCommandParameter (command, idColumn, _dataContainer.ID);
      AddCommandParameter (command, classIDColumn, _dataContainer.ID.ClassID);

      foreach (PropertyValue propertyValue in _dataContainer.PropertyValues)
      {
        if (propertyValue.Definition.StorageClass == StorageClass.Persistent && !propertyValue.Definition.IsObjectID)
        {
          AppendColumn (
              columnBuilder,
              valueBuilder,
              propertyValue.Definition.StoragePropertyDefinition.Name,
              propertyValue.Definition.StoragePropertyDefinition.Name);
          AddCommandParameter (command, propertyValue.Definition.StoragePropertyDefinition.Name, propertyValue);
        }
      }

      command.CommandText = string.Format (
          "INSERT INTO {0} ({1}) VALUES ({2}){3}",
          SqlDialect.DelimitIdentifier (_dataContainer.ClassDefinition.GetEntityName()),
          columnBuilder,
          valueBuilder,
          SqlDialect.StatementDelimiter);

      return command;
    }

    protected virtual void AppendColumn (StringBuilder columnBuilder, StringBuilder valueBuilder, string columnName, string parameterName)
    {
      if (columnBuilder.Length > 0)
        columnBuilder.Append (", ");

      columnBuilder.Append (SqlDialect.DelimitIdentifier (columnName));

      if (valueBuilder.Length > 0)
        valueBuilder.Append (", ");

      valueBuilder.Append (SqlDialect.GetParameterName (parameterName));
    }
  }
}