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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="EntityDefinitionFactory"/> provides factory methods to create <see cref="IEntityDefinition"/>s.
  /// </summary>
  public class EntityDefinitionFactory : IEntityDefinitionFactory
  {
    private readonly IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly IStoragePropertyDefinitionResolver _storagePropertyDefinitionResolver;
    private readonly IForeignKeyConstraintDefinitionFactory _foreignKeyConstraintDefinitionFactory;
    private readonly IStorageNameProvider _storageNameProvider;

    public EntityDefinitionFactory (
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
        IForeignKeyConstraintDefinitionFactory foreignKeyConstraintDefinitionFactory,
        IStoragePropertyDefinitionResolver storagePropertyDefinitionResolver,
        IStorageNameProvider storageNameProvider,
        StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);
      ArgumentUtility.CheckNotNull ("foreignKeyConstraintDefinitionFactory", foreignKeyConstraintDefinitionFactory);
      ArgumentUtility.CheckNotNull ("storagePropertyDefinitionResolver", storagePropertyDefinitionResolver);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      _infrastructureStoragePropertyDefinitionProvider = infrastructureStoragePropertyDefinitionProvider;
      _foreignKeyConstraintDefinitionFactory = foreignKeyConstraintDefinitionFactory;
      _storagePropertyDefinitionResolver = storagePropertyDefinitionResolver;
      _storageNameProvider = storageNameProvider;
      _storageProviderDefinition = storageProviderDefinition;
    }

    public virtual IEntityDefinition CreateTableDefinition (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var tableName = _storageNameProvider.GetTableName (classDefinition);
      if (string.IsNullOrEmpty (tableName))
        throw new MappingException (string.Format ("Class '{0}' has no table name defined.", classDefinition.ID));

      var objectIDColumn = _infrastructureStoragePropertyDefinitionProvider.GetIDColumnDefinition();
      var classIDColumn = _infrastructureStoragePropertyDefinitionProvider.GetClassIDColumnDefinition ();
      var timestampColumn = _infrastructureStoragePropertyDefinitionProvider.GetTimestampColumnDefinition ();
      var columns = GetSimpleColumnDefinitions (_storagePropertyDefinitionResolver.GetStoragePropertiesForHierarchy (classDefinition));
      var allColumns = new[] { objectIDColumn, classIDColumn, timestampColumn }.Concat (columns).ToList();

      var clusteredPrimaryKeyConstraint = new PrimaryKeyConstraintDefinition (
          _storageNameProvider.GetPrimaryKeyConstraintName (classDefinition),
          true,
          allColumns.Where (c => c.IsPartOfPrimaryKey).ToArray());

      var foreignKeyConstraints =
          _foreignKeyConstraintDefinitionFactory.CreateForeignKeyConstraints (classDefinition).Cast<ITableConstraintDefinition>();

      return new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, tableName),
          new EntityNameDefinition (null, _storageNameProvider.GetViewName (classDefinition)),
          objectIDColumn,
          classIDColumn,
          timestampColumn,
          columns,
          new ITableConstraintDefinition[] { clusteredPrimaryKeyConstraint }.Concat (foreignKeyConstraints),
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    public virtual IEntityDefinition CreateFilterViewDefinition (ClassDefinition classDefinition, IEntityDefinition baseEntity)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("baseEntity", baseEntity);

      return new FilterViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, _storageNameProvider.GetViewName (classDefinition)),
          baseEntity,
          GetClassIDsForBranch (classDefinition),
          _infrastructureStoragePropertyDefinitionProvider.GetIDColumnDefinition (),
          _infrastructureStoragePropertyDefinitionProvider.GetClassIDColumnDefinition (),
          _infrastructureStoragePropertyDefinitionProvider.GetTimestampColumnDefinition (),
          GetSimpleColumnDefinitions (_storagePropertyDefinitionResolver.GetStoragePropertiesForHierarchy (classDefinition)),
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    public virtual IEntityDefinition CreateUnionViewDefinition (ClassDefinition classDefinition, IEnumerable<IEntityDefinition> unionedEntities)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("unionedEntities", unionedEntities);

      return new UnionViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, _storageNameProvider.GetViewName (classDefinition)),
          unionedEntities,
          _infrastructureStoragePropertyDefinitionProvider.GetIDColumnDefinition (),
          _infrastructureStoragePropertyDefinitionProvider.GetClassIDColumnDefinition (),
          _infrastructureStoragePropertyDefinitionProvider.GetTimestampColumnDefinition (),
          GetSimpleColumnDefinitions (_storagePropertyDefinitionResolver.GetStoragePropertiesForHierarchy (classDefinition)),
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    protected IEnumerable<string> GetClassIDsForBranch (ClassDefinition classDefinition)
    {
      return new[] { classDefinition }.Concat (classDefinition.GetAllDerivedClasses()).Select (cd => cd.ID);
    }

    private IEnumerable<ColumnDefinition> GetSimpleColumnDefinitions (IEnumerable<IRdbmsStoragePropertyDefinition> columns)
    {
      return columns.SelectMany (c => c.GetColumns());
    }
  }
}