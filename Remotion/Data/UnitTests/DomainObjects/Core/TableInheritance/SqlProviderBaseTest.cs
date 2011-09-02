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
using System.Data.SqlClient;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  public class SqlProviderBaseTest : TableInheritanceMappingTest
  {
    private RdbmsProvider _provider;

    public override void SetUp ()
    {
      base.SetUp ();

      var storageNameProvider = new ReflectionBasedStorageNameProvider();
      var storageTypeInformationProvider = new SqlStorageTypeInformationProvider ();

      var rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider ();
      var infrastructureStoragePropertyDefinitionProvider = new InfrastructureStoragePropertyDefinitionProvider (
          storageTypeInformationProvider, storageNameProvider);

      var commandFactory = new RdbmsProviderCommandFactory (
          new SqlDbCommandBuilderFactory (SqlDialect.Instance),
          rdbmsPersistenceModelProvider,
          new ObjectReaderFactory (rdbmsPersistenceModelProvider, infrastructureStoragePropertyDefinitionProvider),
          new TableDefinitionFinder (rdbmsPersistenceModelProvider),
          storageTypeInformationProvider,
          TableInheritanceTestDomainStorageProviderDefinition);

      _provider = new RdbmsProvider (
          TableInheritanceTestDomainStorageProviderDefinition,
          storageNameProvider,
          SqlDialect.Instance,
          NullPersistenceListener.Instance,
          commandFactory,
          () => new SqlConnection ());

      _provider.Connect ();
    }

    protected RdbmsProvider Provider
    {
      get { return _provider; }
    }
  }
}
