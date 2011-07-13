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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.Model.Building
{
  [TestFixture]
  public class SqlStorageTypeCalculatorTest : StandardMappingTest
  {
    private SqlStorageTypeCalculator _typeCalculator;
    private ClassDefinition _classDefinition;
    private ClassDefinition _orderItemClass;
    private ClassDefinition _orderClass;
    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    private enum Int32Enum : int
    {
    }

    private enum Int16Enum : short
    {
    }

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
      _typeCalculator = new SqlStorageTypeCalculator (_storageProviderDefinitionFinder);
      _classDefinition = ClassDefinitionFactory.CreateClassDefinition(typeof(Order), TestDomainStorageProviderDefinition);
      _orderItemClass = MappingConfiguration.Current.GetTypeDefinition (typeof (OrderItem));
      _orderClass = MappingConfiguration.Current.GetTypeDefinition (typeof (Order));
    }

    [Test]
    public void GetStorageType ()
    {
      Assert.AreEqual ("bit", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Boolean), false, null)).StorageType);
      Assert.AreEqual (DbType.Boolean, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Boolean), false, null)).DbType);

      Assert.AreEqual ("tinyint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte), false, null)).StorageType);
      Assert.AreEqual (DbType.Int16, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte), false, null)).DbType);

      Assert.AreEqual ("datetime", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (DateTime), false, null)).StorageType);
      Assert.AreEqual (DbType.DateTime, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (DateTime), false, null)).DbType);

      Assert.AreEqual ("decimal (38, 3)", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Decimal), false, null)).StorageType);
      Assert.AreEqual (DbType.Decimal, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Decimal), false, null)).DbType);

      Assert.AreEqual ("float", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Double), false, null)).StorageType);
      Assert.AreEqual (DbType.Double, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Double), false, null)).DbType);
      
      Assert.AreEqual ("uniqueidentifier", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Guid), false, null)).StorageType);
      Assert.AreEqual (DbType.Guid, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Guid), false, null)).DbType);

      Assert.AreEqual ("smallint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16), false, null)).StorageType);
      Assert.AreEqual (DbType.Int16, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16), false, null)).DbType);

      Assert.AreEqual ("int", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32), false, null)).StorageType);
      Assert.AreEqual (DbType.Int32, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32), false, null)).DbType);

      Assert.AreEqual ("bigint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int64), false, null)).StorageType);
      Assert.AreEqual (DbType.Int64, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int64), false, null)).DbType);

      Assert.AreEqual ("real", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Single), false, null)).StorageType);
      Assert.AreEqual (DbType.Single, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Single), false, null)).DbType);
      
      Assert.AreEqual ("int", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32Enum), false, null)).StorageType);
      Assert.AreEqual (DbType.Int32, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32Enum), false, null)).DbType);

      Assert.AreEqual ("smallint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16Enum), false, null)).StorageType);
      Assert.AreEqual (DbType.Int16, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16Enum), false, null)).DbType);

      Assert.AreEqual ("nvarchar (200)", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String), false, 200)).StorageType);
      Assert.AreEqual (DbType.String, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String), false, 200)).DbType);

      Assert.AreEqual ("nvarchar (max)", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String), false, null)).StorageType);
      Assert.AreEqual (DbType.String, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String), false, null)).DbType);
      
      Assert.AreEqual ("varbinary (200)", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]), false, 200)).StorageType);
      Assert.AreEqual (DbType.Binary, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]), false, 200)).DbType);

      Assert.AreEqual ("varbinary (max)", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]), false, null)).StorageType);
      Assert.AreEqual (DbType.Binary, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]), false, null)).DbType);
    }

    [Test]
    public void GetStorageType_ForNullableValueTypes ()
    {
      Assert.AreEqual ("bit", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Boolean>), true, null)).StorageType);
      Assert.AreEqual (DbType.Boolean, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Boolean>), true, null)).DbType);
      
      Assert.AreEqual ("tinyint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Byte>), true, null)).StorageType);
      Assert.AreEqual (DbType.Int16, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Byte>), true, null)).DbType);

      Assert.AreEqual ("datetime", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<DateTime>), true, null)).StorageType);
      Assert.AreEqual (DbType.DateTime, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<DateTime>), true, null)).DbType);

      Assert.AreEqual ("decimal (38, 3)", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Decimal>), true, null)).StorageType);
      Assert.AreEqual (DbType.Decimal, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Decimal>), true, null)).DbType);

      Assert.AreEqual ("float", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Double>), true, null)).StorageType);
      Assert.AreEqual (DbType.Double, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Double>), true, null)).DbType);

      Assert.AreEqual ("uniqueidentifier", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Guid>), true, null)).StorageType);
      Assert.AreEqual (DbType.Guid, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Guid>), true, null)).DbType);

      Assert.AreEqual ("smallint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int16>), true, null)).StorageType);
      Assert.AreEqual (DbType.Int16, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int16>), true, null)).DbType);

      Assert.AreEqual ("int", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int32>), true, null)).StorageType);
      Assert.AreEqual (DbType.Int32, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int32>), true, null)).DbType);

      Assert.AreEqual ("bigint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int64>), true, null)).StorageType);
      Assert.AreEqual (DbType.Int64, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int64>), true, null)).DbType);

      Assert.AreEqual ("real", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Single>), true, null)).StorageType);
      Assert.AreEqual (DbType.Single, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Single>), true, null)).DbType);

      Assert.AreEqual ("int", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int32Enum>), true, null)).StorageType);
      Assert.AreEqual (DbType.Int32, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int32Enum>), true, null)).DbType);

      Assert.AreEqual ("smallint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int16Enum>), true, null)).StorageType);
      Assert.AreEqual (DbType.Int16, _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int16Enum>), true, null)).DbType);
    }

    [Test]
    public void GetStorageTypeForSpecialCulumns ()
    {
      Assert.AreEqual (
          "uniqueidentifier",
          _typeCalculator.GetStorageType (
              _orderItemClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order")).StorageType);
      Assert.AreEqual (
          DbType.Guid,
          _typeCalculator.GetStorageType (
              _orderItemClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order")).DbType);
      Assert.AreEqual (
          "varchar (255)",
          _typeCalculator.GetStorageType (
          _orderClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official")).StorageType);
      Assert.AreEqual (
          DbType.String,
          _typeCalculator.GetStorageType (
          _orderClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official")).DbType);
    }

    [Test]
    public void GettorageType_WithNotSupportedType ()
    {
      var propertyDefinition = CreatePropertyDefinition (typeof (Char), false, null);

      var result = _typeCalculator.GetStorageType (propertyDefinition);

      Assert.That (result.StorageType, Is.Null);
    }

    private PropertyDefinition CreatePropertyDefinition (Type propertyType, bool isNullable, int? maxLength)
    {
      return PropertyDefinitionFactory.CreateForFakePropertyInfo (
          _classDefinition, "Name", "ColumnName", propertyType, isNullable, maxLength, StorageClass.Persistent);
    }
  }
}