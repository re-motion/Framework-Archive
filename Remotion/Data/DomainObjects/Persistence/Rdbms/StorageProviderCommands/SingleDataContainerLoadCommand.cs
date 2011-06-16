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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  public class SingleDataContainerLoadCommand : IStorageProviderCommand<DataContainer>
  {
    private readonly IDbCommandBuilder _dbCommandBuilder;
    private readonly IDbCommandFactory _dbCommandFactory;
    private readonly IDbCommandExecutor _dbCommandExecutor;
    private readonly IDataContainerReader _dataContainerReader;

    public SingleDataContainerLoadCommand (
        IDbCommandBuilder dbCommandBuilder,
        IDbCommandFactory dbCommandFactory,
        IDbCommandExecutor dbCommandExecutor,
        IDataContainerReader dataContainerReader)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilder", dbCommandBuilder);
      ArgumentUtility.CheckNotNull ("dbCommandFactory", dbCommandFactory);
      ArgumentUtility.CheckNotNull ("dbCommandExecutor", dbCommandExecutor);
      ArgumentUtility.CheckNotNull ("dataContainerReader", dataContainerReader);

      _dbCommandBuilder = dbCommandBuilder;
      _dbCommandFactory = dbCommandFactory;
      _dbCommandExecutor = dbCommandExecutor;
      _dataContainerReader = dataContainerReader;
    }

    public IDbCommandBuilder DbCommandBuilder
    {
      get { return _dbCommandBuilder; }
    }

    public IDbCommandFactory DbCommandFactory
    {
      get { return _dbCommandFactory; }
    }

    public IDbCommandExecutor DbCommandExecutor
    {
      get { return _dbCommandExecutor; }
    }

    public IDataContainerReader DataContainerReader
    {
      get { return _dataContainerReader; }
    }

    public DataContainer Execute ()
    {
      using (var command = _dbCommandBuilder.Create (_dbCommandFactory))
      {
        using (var reader = _dbCommandExecutor.ExecuteReader (command, CommandBehavior.SingleRow))
        {
          return _dataContainerReader.CreateDataContainer(reader);
        }
      }
    }
  }
}