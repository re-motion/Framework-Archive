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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  public class TestableRdbmsProvider : RdbmsProvider
  {
    public interface IConnectionCreator
    {
      IDbConnection CreateConnection ();
    }

    public TestableRdbmsProvider (
        RdbmsProviderDefinition definition,
        IStorageNameProvider storageNameProvider,
        ISqlDialect dialect,
        IPersistenceListener persistenceListener,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> commandFactory,
        IConnectionCreator connectionCreator,
        IStorageTypeInformationProvider storageTypeInformationProvider)
      : base (definition, storageNameProvider, dialect, persistenceListener, commandFactory, storageTypeInformationProvider, connectionCreator.CreateConnection)
    {

    }
    
  }
}