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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Linq;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public class UnitTestStorageObjectFactoryStub : IStorageObjectFactory
  {
    public UnitTestStorageObjectFactoryStub ()
    {
    }

    public StorageProvider CreateStorageProvider (IPersistenceListener persistenceListener, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("persistenceListener", persistenceListener);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var providerDefiniton = ArgumentUtility.CheckNotNullAndType<UnitTestStorageProviderStubDefinition> (
          "storageProviderDefinition", storageProviderDefinition);
      var storageNameProvider = new ReflectionBasedStorageNameProvider();

      return new UnitTestStorageProviderStub (providerDefiniton, storageNameProvider, persistenceListener);
    }

    public TypeConversionProvider CreateTypeConversionProvider ()
    {
      return TypeConversionProvider.Create();
    }

    public TypeProvider CreateTypeProvider ()
    {
      return new TypeProvider();
    }

    public IPersistenceModelLoader CreatePersistenceModelLoader (
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      var storageNameProvider = new ReflectionBasedStorageNameProvider();
      var columnDefinitionFactory = new ColumnDefinitionFactory (
          new SqlStorageTypeCalculator (storageProviderDefinitionFinder), storageNameProvider, storageProviderDefinitionFinder);
      var columnDefinitionResolver = new ColumnDefinitionResolver();
      var foreignKeyConstraintDefinitionFactory = new ForeignKeyConstraintDefinitionFactory (
          storageNameProvider, columnDefinitionResolver, columnDefinitionFactory, storageProviderDefinitionFinder);
      var entityDefinitionFactory = new EntityDefinitionFactory (
          columnDefinitionFactory,
          foreignKeyConstraintDefinitionFactory,
          columnDefinitionResolver,
          storageNameProvider,
          storageProviderDefinition);

      return new RdbmsPersistenceModelLoader (
          entityDefinitionFactory, columnDefinitionFactory, storageProviderDefinition, storageNameProvider, new RdbmsPersistenceModelProvider());
    }

    public IQueryExecutor CreateLinqQueryExecutor (
        ClassDefinition startingClassDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      ArgumentUtility.CheckNotNull ("startingClassDefinition", startingClassDefinition);
      ArgumentUtility.CheckNotNull ("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull ("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);

      var generator = new UniqueIdentifierGenerator();
      var storageNameProvider = new ReflectionBasedStorageNameProvider();
      var resolver = new MappingResolver (
          new StorageSpecificExpressionResolver (storageNameProvider, new RdbmsPersistenceModelProvider()), storageNameProvider);
      var sqlPreparationStage = ObjectFactory.Create<DefaultSqlPreparationStage> (
          ParamList.Create (methodCallTransformerProvider, resultOperatorHandlerRegistry, generator));
      var mappingResolutionStage = ObjectFactory.Create<DefaultMappingResolutionStage> (ParamList.Create (resolver, generator));
      var sqlGenerationStage = ObjectFactory.Create<DefaultSqlGenerationStage> (ParamList.Empty);
      return new DomainObjectQueryExecutor (startingClassDefinition, sqlPreparationStage, mappingResolutionStage, sqlGenerationStage);
    }
  }
}