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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class StorageSpecificExpressionResolverTest : StandardMappingTest
  {
    private StorageSpecificExpressionResolver _storageSpecificExpressionResolver;
    private ClassDefinition _classDefinition;
    private IStorageNameProvider _storageNameProviderStub;
    private IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProviderStub;
    private IRdbmsStoragePropertyDefinition _rdbmsStoragePropertyDefinitionStub;
    private IStorageTypeInformationProvider _storageTypeInformationProviderStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _storageNameProviderStub = MockRepository.GenerateStub<IStorageNameProvider>();
      _storageNameProviderStub.Stub (stub => stub.GetIDColumnName()).Return ("ID");
      _storageNameProviderStub.Stub (stub => stub.GetClassIDColumnName()).Return ("ClassID");

      _rdbmsPersistenceModelProviderStub = MockRepository.GenerateStub<IRdbmsPersistenceModelProvider>();
      _rdbmsStoragePropertyDefinitionStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _storageTypeInformationProviderStub = MockRepository.GenerateStub<IStorageTypeInformationProvider>();

      _storageSpecificExpressionResolver = new StorageSpecificExpressionResolver (
          _rdbmsPersistenceModelProviderStub, _storageNameProviderStub, _storageTypeInformationProviderStub);

      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Order));
      _classDefinition.SetStorageEntity (
          TableDefinitionObjectMother.Create (
              TestDomainStorageProviderDefinition,
              new EntityNameDefinition (null, "Order"),
              new EntityNameDefinition (null, "OrderView")));
    }

    [Test]
    public void ResolveEntity ()
    {
      var objectIDProperty = ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      var timestampProperty = SimpleStoragePropertyDefinitionObjectMother.TimestampProperty;

      var foreignKeyProperty = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("ForeignKey");
      var simpleProperty = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column1");
      var tableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          null,
          objectIDProperty,
          timestampProperty,
          foreignKeyProperty,
          simpleProperty);
      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (_classDefinition))
          .Return (tableDefinition);

      var result = _storageSpecificExpressionResolver.ResolveEntity (_classDefinition, "o");

      var expectedIdColumn = new SqlColumnDefinitionExpression (objectIDProperty.PropertyType, "o", "ID", true);
      var expectedClassIdColumn = new SqlColumnDefinitionExpression (typeof (string), "o", "ClassID", false);
      var expectedTimestampColumn = new SqlColumnDefinitionExpression (timestampProperty.PropertyType, "o", "Timestamp", false);
      var expectedForeignKeyColumn = new SqlColumnDefinitionExpression (foreignKeyProperty.PropertyType, "o", "ForeignKey", false);
      var expectedColumn = new SqlColumnDefinitionExpression (simpleProperty.PropertyType, "o", "Column1", false);

      Assert.That (result.Type, Is.SameAs (typeof (Order)));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.Name, Is.Null);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedIdColumn, result.GetIdentityExpression());
      Assert.That (result.Columns, Has.Count.EqualTo (5));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedIdColumn, result.Columns[0]);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedClassIdColumn, result.Columns[1]);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedTimestampColumn, result.Columns[2]);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedForeignKeyColumn, result.Columns[3]);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedColumn, result.Columns[4]);
    }

    [Test]
    public void ResolveColumn_NoPrimaryKeyColumn ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      var entityExpression = CreateEntityDefinition (typeof (Order), "o");

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (propertyDefinition))
          .Return (_rdbmsStoragePropertyDefinitionStub);

      var columnDefinition = ColumnDefinitionObjectMother.CreateColumn ("OrderNumber");
      _rdbmsStoragePropertyDefinitionStub.Stub (stub => stub.PropertyType).Return (typeof (string));
      _rdbmsStoragePropertyDefinitionStub.Stub (stub => stub.GetColumns()).Return (new[] { columnDefinition });

      var result = (SqlColumnDefinitionExpression) _storageSpecificExpressionResolver.ResolveColumn (entityExpression, propertyDefinition);

      Assert.That (result.ColumnName, Is.EqualTo ("OrderNumber"));
      Assert.That (result.OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (result.Type, Is.SameAs (typeof (string)));
      Assert.That (result.IsPrimaryKey, Is.False);
    }

    [Test]
    public void ResolveColumn_PrimaryKeyColumn ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      var entityExpression = CreateEntityDefinition (typeof (Order), "o");

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (propertyDefinition))
          .Return (_rdbmsStoragePropertyDefinitionStub);

      var columnDefinition = ColumnDefinitionObjectMother.IDColumn;
      _rdbmsStoragePropertyDefinitionStub.Stub (stub => stub.PropertyType).Return (typeof (string));
      _rdbmsStoragePropertyDefinitionStub.Stub (stub => stub.GetColumns()).Return (new[] { columnDefinition });

      var result = (SqlColumnDefinitionExpression) _storageSpecificExpressionResolver.ResolveColumn (entityExpression, propertyDefinition);

      Assert.That (result.ColumnName, Is.EqualTo ("ID"));
      Assert.That (result.OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (result.Type, Is.SameAs (typeof (string)));
      Assert.That (result.IsPrimaryKey, Is.True);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Compound-column properties are not supported by this LINQ provider.")]
    public void ResolveColumn_CompoundColumn ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      var entityExpression = CreateEntityDefinition (typeof (Order), "o");

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (propertyDefinition))
          .Return (_rdbmsStoragePropertyDefinitionStub);
      _rdbmsStoragePropertyDefinitionStub
          .Stub (stub => stub.GetColumns())
          .Return (new[] { ColumnDefinitionObjectMother.IDColumn, ColumnDefinitionObjectMother.ClassIDColumn });

      _storageSpecificExpressionResolver.ResolveColumn (entityExpression, propertyDefinition);
    }

    [Test]
    public void ResoveIDColumn ()
    {
      var entityExpression = CreateEntityDefinition (typeof (Order), "o");
      var entityDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Test"));

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (_classDefinition))
          .Return (entityDefinition);

      var result = _storageSpecificExpressionResolver.ResolveIDColumn (entityExpression, _classDefinition);

      ExpressionTreeComparer.CheckAreEqualTrees (entityExpression.GetIdentityExpression (), result);
    }

    [Test]
    public void ResolveValueColumn ()
    {
      var storageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();
      storageTypeInformationStub.Stub (stub => stub.DotNetType).Return (typeof (Guid));
      
      _storageTypeInformationProviderStub.Stub (stub => stub.GetStorageTypeForID (true)).Return (storageTypeInformationStub);

      var columnExpression = new SqlColumnDefinitionExpression (typeof (string), "o", "columnName", true);

      var result = _storageSpecificExpressionResolver.ResolveValueColumn (columnExpression);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.Type, Is.SameAs (typeof (object)));
      Assert.That (result, Is.TypeOf (typeof (UnaryExpression)));

      var innerExpression = ((UnaryExpression) result).Operand;
      Assert.That (innerExpression, Is.TypeOf (typeof (SqlColumnDefinitionExpression)));
      var columnDefinitionExpression = ((SqlColumnDefinitionExpression) innerExpression);
      Assert.That (columnDefinitionExpression.Type, Is.SameAs (typeof (Guid)));
      Assert.That (columnDefinitionExpression.OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (columnDefinitionExpression.ColumnName, Is.EqualTo ("columnName"));
      Assert.That (columnDefinitionExpression.IsPrimaryKey, Is.True);
    }

    [Test]
    public void ResolveClassIDColumn ()
    {
      var columnExpression = new SqlColumnDefinitionExpression (typeof (string), "o", "columnName", false);

      var result = _storageSpecificExpressionResolver.ResolveClassIDColumn (columnExpression);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.Type, Is.SameAs (typeof (string)));

      Assert.That (result, Is.TypeOf (typeof (SqlColumnDefinitionExpression)));
      var columnDefinitionExpression = ((SqlColumnDefinitionExpression) result);
      Assert.That (columnDefinitionExpression.ColumnName, Is.EqualTo ("ClassID"));
      Assert.That (columnDefinitionExpression.IsPrimaryKey, Is.False);
    }

    [Test]
    public void ResolveClassIDColumn_OnColumnReference_WithClassIDProperty ()
    {
      var referencedEntity = CreateEntityDefinition (typeof (Order), "o");

      var sqlColumnReferenceExpression = new SqlColumnReferenceExpression (typeof (string), "c", "Name", false, referencedEntity);

      var result = _storageSpecificExpressionResolver.ResolveClassIDColumn (sqlColumnReferenceExpression);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.Type, Is.SameAs (typeof (string)));

      Assert.That (result, Is.TypeOf (typeof (SqlColumnReferenceExpression)));
      var columnDefinitionExpression = (SqlColumnReferenceExpression) result;
      Assert.That (columnDefinitionExpression.ColumnName, Is.EqualTo ("ClassID"));
      Assert.That (columnDefinitionExpression.OwningTableAlias, Is.EqualTo (sqlColumnReferenceExpression.OwningTableAlias));
      Assert.That (columnDefinitionExpression.IsPrimaryKey, Is.False);
    }

    [Test]
    public void ResolveTable_TableDefinitionWithNoSchemaName ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"), new EntityNameDefinition (null, "TableView"));
      _classDefinition.SetStorageEntity (tableDefinition);

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (_classDefinition))
          .Return (_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTable (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("TableView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void ResolveTable_TableDefinitionWithSchemaName ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"), new EntityNameDefinition ("schemaName", "TableView"));
      _classDefinition.SetStorageEntity (tableDefinition);

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (_classDefinition))
          .Return (_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTable (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("schemaName.TableView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void ResolveTable_FilterViewDefinitionWithNoSchemaName ()
    {
      var filterViewDefinition = FilterViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "FilterView"),
          (IRdbmsStorageEntityDefinition) _classDefinition.StorageEntityDefinition);
      _classDefinition.SetStorageEntity (filterViewDefinition);

      _rdbmsPersistenceModelProviderStub.Stub (stub => stub.GetEntityDefinition (_classDefinition)).Return (
          _classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTable (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("FilterView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void ResolveTable_FilterViewDefinitionWithSchemaName ()
    {
      var filterViewDefinition = FilterViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition ("schemaName", "FilterView"),
          (IRdbmsStorageEntityDefinition) _classDefinition.StorageEntityDefinition);
      _classDefinition.SetStorageEntity (filterViewDefinition);

      _rdbmsPersistenceModelProviderStub.Stub (stub => stub.GetEntityDefinition (_classDefinition)).Return (
          _classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTable (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("schemaName.FilterView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void ResolveTable_UnionViewDefinitionWithNoSchemaName ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "UnionView"),
          new[] { (IRdbmsStorageEntityDefinition) _classDefinition.StorageEntityDefinition });
      _classDefinition.SetStorageEntity (unionViewDefinition);

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (_classDefinition))
          .Return (_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTable (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("UnionView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void ResolveTable_UnionViewDefinitionWithSchemaName ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition ("schemaName", "UnionView"),
          new[] { (IRdbmsStorageEntityDefinition) _classDefinition.StorageEntityDefinition });
      _classDefinition.SetStorageEntity (unionViewDefinition);

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (_classDefinition))
          .Return (_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTable (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("schemaName.UnionView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void ResolveTable_EmptyViewDefinitionWithNoSchemaName ()
    {
      var emptyViewDefinition = EmptyViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "EmptyView"));
      _classDefinition.SetStorageEntity (emptyViewDefinition);

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (_classDefinition))
          .Return (_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTable (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("EmptyView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void ResolveTable_EmptyViewDefinitionWithSchemaName ()
    {
      var emptyViewDefinition = EmptyViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition ("schemaName", "EmptyView"));
      _classDefinition.SetStorageEntity (emptyViewDefinition);

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (_classDefinition))
          .Return (_classDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);

      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTable (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("schemaName.EmptyView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void ResolveJoin_LeftSideHoldsForeignKey ()
    {
      // Order.Customer
      var propertyDefinition = CreatePropertyDefinition (_classDefinition, "Customer", "Customer");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      var columnDefinition = ColumnDefinitionObjectMother.CreateColumn ("Customer");
      _rdbmsStoragePropertyDefinitionStub.Stub (stub => stub.GetColumnsForComparison ()).Return (new[] { columnDefinition });
      _rdbmsStoragePropertyDefinitionStub.Stub (stub => stub.PropertyType).Return (typeof (ObjectID));

      var leftEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);
      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (leftEndPointDefinition.PropertyDefinition))
          .Return (_rdbmsStoragePropertyDefinitionStub);

      // Customer.Order
      var customerClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Customer));
      var customerTableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, 
          new EntityNameDefinition (null, "CustomerTable"), 
          new EntityNameDefinition (null, "CustomerView"));
      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (customerClassDefinition))
          .Return (customerTableDefinition);

      var rightEndPointDefinition = new AnonymousRelationEndPointDefinition (customerClassDefinition);

      var originatingEntity = CreateEntityDefinition (typeof (Order), "o");

      var result = _storageSpecificExpressionResolver.ResolveJoin (originatingEntity, leftEndPointDefinition, rightEndPointDefinition, "c");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.ItemType, Is.EqualTo (typeof (Customer)));
      Assert.That (result.ForeignTableInfo, Is.TypeOf (typeof (ResolvedSimpleTableInfo)));
      Assert.That (((ResolvedSimpleTableInfo) result.ForeignTableInfo).TableName, Is.EqualTo ("CustomerView"));
      Assert.That (((ResolvedSimpleTableInfo) result.ForeignTableInfo).TableAlias, Is.EqualTo ("c"));

      var expected = Expression.Equal (
          new SqlColumnDefinitionExpression (typeof (ObjectID), "o", "Customer", false),
          new SqlColumnDefinitionExpression (typeof (ObjectID), "c", "ID", true));
      ExpressionTreeComparer.CheckAreEqualTrees (expected, result.JoinCondition);
    }

    [Test]
    public void ResolveJoin_LeftSideHoldsNoForeignKey ()
    {
      var propertyDefinition = CreatePropertyDefinition (_classDefinition, "Customer", "Customer");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      var leftEndPointDefinition = new AnonymousRelationEndPointDefinition (_classDefinition);
      var rightEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);

      var entityExpression = CreateEntityDefinition (typeof (Customer), "c");

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (rightEndPointDefinition.ClassDefinition))
          .Return (rightEndPointDefinition.ClassDefinition.StorageEntityDefinition as IRdbmsStorageEntityDefinition);
      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (rightEndPointDefinition.PropertyDefinition))
          .Return (_rdbmsStoragePropertyDefinitionStub);

      _rdbmsStoragePropertyDefinitionStub
          .Stub (stub => stub.GetColumnsForComparison())
          .Return (new[] { ColumnDefinitionObjectMother.CreateColumn ("Customer") });
      _rdbmsStoragePropertyDefinitionStub.Stub (stub => stub.PropertyType).Return (typeof (ObjectID));

      var result = _storageSpecificExpressionResolver.ResolveJoin (entityExpression, leftEndPointDefinition, rightEndPointDefinition, "o");

      ExpressionTreeComparer.CheckAreEqualTrees (
          Expression.Equal (
            entityExpression.GetIdentityExpression(), // c.ID
            new SqlColumnDefinitionExpression (typeof (ObjectID), "o", "Customer", false)),
          result.JoinCondition);
    }

    [Test]
    public void ResolveEntityIdentityViaForeignKey()
    {
      // Order.Customer
      var propertyDefinition = CreatePropertyDefinition (_classDefinition, "Customer", "Customer");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      var columnDefinition = ColumnDefinitionObjectMother.CreateColumn ("Customer");
      _rdbmsStoragePropertyDefinitionStub.Stub (stub => stub.GetColumnsForComparison ()).Return (new[] { columnDefinition });
      _rdbmsStoragePropertyDefinitionStub.Stub (stub => stub.PropertyType).Return (typeof (ObjectID));

      var foreignKeyEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);
      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (foreignKeyEndPointDefinition.PropertyDefinition))
          .Return (_rdbmsStoragePropertyDefinitionStub);

      var originatingEntity = CreateEntityDefinition (typeof (Order), "o");

      var result = _storageSpecificExpressionResolver.ResolveEntityIdentityViaForeignKey (originatingEntity, foreignKeyEndPointDefinition);

      var expected = new SqlColumnDefinitionExpression (typeof (ObjectID), "o", "Customer", false);
      ExpressionTreeComparer.CheckAreEqualTrees (expected, result);
    }

    private PropertyDefinition CreatePropertyDefinition (ClassDefinition classDefinition, string propertyName, string columnName)
    {
      var propertyDefinition = new PropertyDefinition (
          classDefinition,
          MockRepository.GenerateStub<IPropertyInformation>(),
          propertyName,
          true,
          true,
          null,
          StorageClass.Persistent);
      propertyDefinition.SetStorageProperty (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty (columnName));
      return propertyDefinition;
    }

    private SqlEntityDefinitionExpression CreateEntityDefinition (Type itemType, string tableAlias)
    {
      return new SqlEntityDefinitionExpression (itemType, tableAlias, null, e => e.GetColumn (typeof (ObjectID), "ID", true));
    }
  }
}