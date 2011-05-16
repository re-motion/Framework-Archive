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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.UnitTests.DomainObjects.Core.Linq;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Mixins;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer
{
  [TestFixture]
  public class SqlStorageObjectFactoryTest
  {
    private SqlStorageObjectFactory _sqlProviderFactory;
    private RdbmsProviderDefinition _rdbmsProviderDefinition;
    private IPersistenceListener _persistenceListenerStub;
    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    [SetUp]
    public void SetUp ()
    {
      _rdbmsProviderDefinition = new RdbmsProviderDefinition ("TestDomain", new SqlStorageObjectFactory(), "ConnectionString");
      _sqlProviderFactory = new SqlStorageObjectFactory();
      _persistenceListenerStub = MockRepository.GenerateStub<IPersistenceListener>();
      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
    }

    [Test]
    public void CreateStorageProvider ()
    {
      var result = _sqlProviderFactory.CreateStorageProvider (_persistenceListenerStub, _rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (SqlProvider)));
      Assert.That (result.PersistenceListener, Is.SameAs (_persistenceListenerStub));
      Assert.That (result.StorageProviderDefinition, Is.SameAs (_rdbmsProviderDefinition));
    }

    [Test]
    public void CreateStorageProviderWithMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (SqlProvider)).Clear().AddMixins (typeof (SqlProviderTestMixin)).EnterScope())
      {
        var result = _sqlProviderFactory.CreateStorageProvider (_persistenceListenerStub, _rdbmsProviderDefinition);

        Assert.That (Mixin.Get<SqlProviderTestMixin> (result), Is.Not.Null);
      }
    }

    [Test]
    public void GetTypeConversionProvider ()
    {
      var result = _sqlProviderFactory.CreateTypeConversionProvider();

      Assert.That (result, Is.TypeOf (typeof (TypeConversionProvider)));
    }

    [Test]
    public void GetTypeProvider ()
    {
      var result = _sqlProviderFactory.CreateTypeProvider();

      Assert.That (result, Is.TypeOf (typeof (TypeProvider)));
    }

    [Test]
    public void GetPersistenceModelLoader ()
    {
      var result = _sqlProviderFactory.CreatePersistenceModelLoader (_storageProviderDefinitionFinder, _rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (RdbmsPersistenceModelLoader)));
      Assert.That (((RdbmsPersistenceModelLoader) result).ColumnDefinitionFactory, Is.TypeOf (typeof (ColumnDefinitionFactory)));
      Assert.That (((RdbmsPersistenceModelLoader) result).StorageProviderID, Is.EqualTo ("TestDomain"));
    }

    [Test]
    public void CreateStorageNameResolver ()
    {
      var result = _sqlProviderFactory.CreateStorageNameProvider();

      Assert.That (result, Is.TypeOf (typeof (ReflectionBasedStorageNameProvider)));
    }

    [Test]
    public void CreateSchemaFileBuilder ()
    {
      var tableBuilderStub = MockRepository.GenerateStub<TableScriptBuilder> (
          MockRepository.GenerateStub<ITableScriptElementFactory>(), new SqlCommentScriptElementFactory());
      var viewBuilderStub = MockRepository.GenerateStub<ViewScriptBuilder> (
          MockRepository.GenerateStub<IViewScriptElementFactory<TableDefinition>>(),
          MockRepository.GenerateStub<IViewScriptElementFactory<UnionViewDefinition>>(),
          MockRepository.GenerateStub<IViewScriptElementFactory<FilterViewDefinition>>(),
          new SqlCommentScriptElementFactory());
      var constraintBuilderStub =
          MockRepository.GenerateStub<ForeignKeyConstraintScriptBuilder> (
              MockRepository.GenerateStub<IForeignKeyConstraintScriptElementFactory>(), new SqlCommentScriptElementFactory());
      var indexScriptElementFactoryStub = MockRepository.GenerateStub<SqlIndexScriptElementFactory> (
          MockRepository.GenerateStub<ISqlIndexDefinitionScriptElementFactory<SqlIndexDefinition>>(),
          MockRepository.GenerateStub<ISqlIndexDefinitionScriptElementFactory<SqlPrimaryXmlIndexDefinition>>(),
          MockRepository.GenerateStub<ISqlIndexDefinitionScriptElementFactory<SqlSecondaryXmlIndexDefinition>>());
      var indexBuilderStub = MockRepository.GenerateStub<IndexScriptBuilder> (indexScriptElementFactoryStub, new SqlCommentScriptElementFactory());
      var synonymBuilderStub =
          MockRepository.GenerateStub<SynonymScriptBuilder> (
              MockRepository.GenerateStub<ISynonymScriptElementFactory<TableDefinition>>(),
              MockRepository.GenerateStub<ISynonymScriptElementFactory<UnionViewDefinition>>(),
              MockRepository.GenerateStub<ISynonymScriptElementFactory<FilterViewDefinition>>(),
              new SqlCommentScriptElementFactory());

      var sqlProviderFactory = new TestableSqlStorageObjectFactory (
          tableBuilderStub, viewBuilderStub, constraintBuilderStub, indexBuilderStub, synonymBuilderStub);

      var result = sqlProviderFactory.CreateSchemaScriptBuilder (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (SqlDatabaseSelectionScriptElementBuilder)));
      Assert.That (((SqlDatabaseSelectionScriptElementBuilder) result).InnerScriptBuilder, Is.TypeOf (typeof (CompositeScriptBuilder)));
      Assert.That (
          ((CompositeScriptBuilder) ((SqlDatabaseSelectionScriptElementBuilder) result).InnerScriptBuilder).RdbmsProviderDefinition,
          Is.SameAs (_rdbmsProviderDefinition));
      Assert.That (
          ((CompositeScriptBuilder) ((SqlDatabaseSelectionScriptElementBuilder) result).InnerScriptBuilder).ScriptBuilders,
          Is.EqualTo (
              new IScriptBuilder[]
              {
                  tableBuilderStub,
                  constraintBuilderStub,
                  viewBuilderStub,
                  indexBuilderStub,
                  synonymBuilderStub
              }));
    }

    [Test]
    public void CreateLinqQueryExecutor ()
    {
      var classDefintion = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var methodCallTransformerProvider = MockRepository.GenerateStub<IMethodCallTransformerProvider>();

      var result = _sqlProviderFactory.CreateLinqQueryExecutor (
          classDefintion, methodCallTransformerProvider, ResultOperatorHandlerRegistry.CreateDefault());

      Assert.That (result, Is.TypeOf (typeof (DomainObjectQueryExecutor)));
    }

    [Test]
    public void CreateLinqQueryExecutor_CanBeMixed ()
    {
      var classDefintion = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var methodCallTransformerProvider = MockRepository.GenerateStub<IMethodCallTransformerProvider>();

      using (MixinConfiguration.BuildNew().ForClass (typeof (DomainObjectQueryExecutor)).AddMixin<TestQueryExecutorMixin>().EnterScope())
      {
        var executor = _sqlProviderFactory.CreateLinqQueryExecutor (
            classDefintion, methodCallTransformerProvider, ResultOperatorHandlerRegistry.CreateDefault());
        Assert.That (Mixin.Get<TestQueryExecutorMixin> (executor), Is.Not.Null);
      }
    }

    [Test]
    public void CreateLinqQueryExecutor_WithMixedStages ()
    {
      var classDefintion = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var methodCallTransformerProvider = MockRepository.GenerateStub<IMethodCallTransformerProvider>();

      using (MixinConfiguration.BuildNew()
          .ForClass<DefaultSqlPreparationStage>().AddMixin<TestSqlPreparationStageMixin>()
          .ForClass<DefaultMappingResolutionStage>().AddMixin<TestMappingResolutionStageMixin>()
          .ForClass<DefaultSqlGenerationStage>().AddMixin<TestSqlGenerationStageMixin>()
          .EnterScope())
      {
        var executor = (DomainObjectQueryExecutor) _sqlProviderFactory.CreateLinqQueryExecutor (
            classDefintion, methodCallTransformerProvider, ResultOperatorHandlerRegistry.CreateDefault());

        Assert.That (Mixin.Get<TestSqlPreparationStageMixin> (executor.PreparationStage), Is.Not.Null);
        Assert.That (Mixin.Get<TestMappingResolutionStageMixin> (executor.ResolutionStage), Is.Not.Null);
        Assert.That (Mixin.Get<TestSqlGenerationStageMixin> (executor.GenerationStage), Is.Not.Null);
      }
    }

    [Test]
    public void ExtendedSqlStorageObjectFactory ()
    {
      var storageObjectFactory = new ExtendedSqlStorageObjectFactory();

      var schemaScriptBuilder = storageObjectFactory.CreateSchemaScriptBuilder (_rdbmsProviderDefinition);

      Assert.That (schemaScriptBuilder, Is.TypeOf (typeof (ExtendedScriptElementBuilder)));
    }
  }
}