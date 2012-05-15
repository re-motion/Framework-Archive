// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Linq;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public class UnitTestStorageObjectFactoryStub : IStorageObjectFactory
  {
    public StorageProvider CreateStorageProvider (IPersistenceExtension persistenceExtension, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("persistenceExtension", persistenceExtension);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var providerDefiniton = ArgumentUtility.CheckNotNullAndType<UnitTestStorageProviderStubDefinition> (
          "storageProviderDefinition", storageProviderDefinition);
      return new UnitTestStorageProviderStub (providerDefiniton, persistenceExtension);
    }

    public IPersistenceModelLoader CreatePersistenceModelLoader (
        StorageProviderDefinition storageProviderDefinition, 
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      return new SqlStorageObjectFactory().CreatePersistenceModelLoader (storageProviderDefinition, storageProviderDefinitionFinder);
    }

    public IQueryExecutor CreateLinqQueryExecutor (
        ClassDefinition startingClassDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      throw new NotSupportedException ("Linq queries are not supported by the stub storage provider.");
    }
  }
}