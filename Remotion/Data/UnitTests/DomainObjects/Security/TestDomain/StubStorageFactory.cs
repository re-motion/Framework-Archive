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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Security.TestDomain
{
  public class StubStorageFactory : IRdbmsStorageObjectFactory
  {
    private readonly RdbmsProviderDefinition _storageProviderDefinition;

    public StubStorageFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      _storageProviderDefinition = storageProviderDefinition;
    }

    public StorageProvider CreateStorageProvider (IPersistenceListener persistenceListener)
    {
      ArgumentUtility.CheckNotNull ("persistenceListener", persistenceListener);

      return new StubStorageProvider (_storageProviderDefinition, persistenceListener);
    }

    public TypeConversionProvider CreateTypeConversionProvider ()
    {
      return TypeConversionProvider.Create();
    }

    public TypeProvider CreateTypeProvider ()
    {
      return new TypeProvider();
    }

    public IPersistenceModelLoader CreatePersistenceModelLoader (IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      var columnDefinitionFactory = new ColumnDefinitionFactory (new SqlStorageTypeCalculator(storageProviderDefinitionFinder), storageProviderDefinitionFinder);
      var columnDefinitonResolver = new ColumnDefinitionResolver();
      var entityDefinitionFactory = new EntityDefinitionFactory (columnDefinitionFactory, columnDefinitonResolver, _storageProviderDefinition);

      return new RdbmsPersistenceModelLoader (entityDefinitionFactory, columnDefinitionFactory, _storageProviderDefinition);
    }

    public FileBuilderBase CreateSchemaFileBuilder ()
    {
      return new FileBuilder (_storageProviderDefinition);
    }
  }
}