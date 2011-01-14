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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Rhino.Mocks;
using ClassNotInMapping =
    Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.RelationReflector.RelatedPropertyTypeIsNotInMapping.ClassNotInMapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ColumnDefinitionFactoryTest : StandardMappingTest
  {
    private StorageTypeCalculator _storageTypeCalculatorStub;
    private ColumnDefinitionFactory _columnDefinitionFactory;
    private ReflectionBasedClassDefinition _classWithAllDataTypesDefinition;
    private ReflectionBasedClassDefinition _fileSystemItemClassDefinition;
    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;
    private ReflectionBasedClassDefinition _classAboveDbTableAttribute;
    private ReflectionBasedClassDefinition _classWithDbTableAttribute;
    private ReflectionBasedClassDefinition _classBelowDbTableAttribute;
    private ReflectionBasedClassDefinition _classBelowBelowDbTableAttribute;
    private IStorageNameCalculator _storageNameCalculatorStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
      _storageTypeCalculatorStub = MockRepository.GenerateStub<StorageTypeCalculator>(_storageProviderDefinitionFinder);
      _storageTypeCalculatorStub.Stub (stub => stub.SqlDataTypeClassID).Return ("varchar(100)");
      _storageTypeCalculatorStub.Stub (stub => stub.SqlDataTypeObjectID).Return ("guid");
      _storageTypeCalculatorStub.Stub (stub => stub.SqlDataTypeTimestamp).Return ("rowversion");
      _storageNameCalculatorStub = MockRepository.GenerateStub<IStorageNameCalculator>();
      _storageNameCalculatorStub.Stub (stub => stub.IDColumnName).Return ("ID");
      _storageNameCalculatorStub.Stub (stub => stub.ClassIDColumnName).Return ("ClassID");
      _storageNameCalculatorStub.Stub (stub => stub.TimestampColumnName).Return ("Timestamp");
      _columnDefinitionFactory = new ColumnDefinitionFactory (_storageTypeCalculatorStub, _storageNameCalculatorStub, _storageProviderDefinitionFinder);
      _classWithAllDataTypesDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithAllDataTypes));
      _classWithAllDataTypesDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _fileSystemItemClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (FileSystemItem));
      _fileSystemItemClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classAboveDbTableAttribute = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassNotInMapping));
      _classAboveDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classWithDbTableAttribute = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Company), _classAboveDbTableAttribute);
      _classWithDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classBelowDbTableAttribute = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Partner), _classWithDbTableAttribute);
      _classBelowDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classBelowBelowDbTableAttribute = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          typeof (Distributor), _classBelowDbTableAttribute);
      _classBelowBelowDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
    }

    [Test]
    public void CreateColumnDefinition_RelationProperty ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (_fileSystemItemClassDefinition, "ParentFolder", typeof (ObjectID));
      StubStorageCalculators (propertyDefinition);

      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateColumnDefinition (propertyDefinition);

      Assert.That (result.Name, Is.EqualTo ("FakeColumnName"));
      Assert.That (result.PropertyType, Is.SameAs (typeof (ObjectID)));
    }

    [Test]
    public void CreateColumnDefinition_PropertyType ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));
      StubStorageCalculators (propertyDefinition);

      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateColumnDefinition (propertyDefinition);

      Assert.That (result.Name, Is.EqualTo ("FakeColumnName"));
      Assert.That (result.PropertyType, Is.SameAs (typeof (bool)));
    }

    [Test]
    public void CreateColumnDefinition_StorageType ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));
      StubStorageCalculators (propertyDefinition);

      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateColumnDefinition (propertyDefinition);

      Assert.That (result.Name, Is.EqualTo ("FakeColumnName"));
      Assert.That (result.StorageType, Is.EqualTo ("storage type"));
    }

    [Test]
    public void CreateColumnDefinition_IsNullable ()
    {
      var nullablePropertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, typeof (ClassWithAllDataTypes), "StringProperty", "StringProperty", typeof (string), true);
      var nonNullablePropertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, typeof (ClassWithAllDataTypes), "StringProperty", "StringProperty", typeof (string), false);
      StubStorageCalculators (nullablePropertyDefinition);
      StubStorageCalculators (nonNullablePropertyDefinition);

      var nullableResult =
          (SimpleColumnDefinition) _columnDefinitionFactory.CreateColumnDefinition (nullablePropertyDefinition);
      var nonNullableResult =
          (SimpleColumnDefinition) _columnDefinitionFactory.CreateColumnDefinition (nonNullablePropertyDefinition);

      Assert.That (nullableResult.IsNullable, Is.True);
      Assert.That (nonNullableResult.IsNullable, Is.False);
    }

    [Test]
    public void CreateColumnDefinition_NotSupportedType ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition)).Return (null);

      var result = _columnDefinitionFactory.CreateColumnDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (UnsupportedStorageTypeColumnDefinition)));
    }

    [Test]
    public void CreateColumnDefinition_ClassAboveDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = ReflectionBasedPropertyDefinitionFactory.Create (
          _classAboveDbTableAttribute, StorageClass.Persistent, typeof (ClassNotInMapping).GetProperty ("RelationProperty"), false);
      var propertyDefinitionNullable = ReflectionBasedPropertyDefinitionFactory.Create (
          _classAboveDbTableAttribute, StorageClass.Persistent, typeof (ClassNotInMapping).GetProperty ("RelationProperty"), true);

      StubStorageCalculators (propertyDefinitionNotNullable);
      StubStorageCalculators (propertyDefinitionNullable);

      Assert.That (_classAboveDbTableAttribute.BaseClass, Is.Null);

      var resultNotNullable =
          (SimpleColumnDefinition) _columnDefinitionFactory.CreateColumnDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleColumnDefinition) _columnDefinitionFactory.CreateColumnDefinition (propertyDefinitionNullable);

      Assert.That (resultNotNullable.IsNullable, Is.False);
      Assert.That (resultNullable.IsNullable, Is.True);
    }

    [Test]
    public void CreateColumnDefinition_ClassWithDbTableAttribute()
    {
      var propertyDefinitionNotNullable = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithDbTableAttribute, StorageClass.Persistent, typeof (Company).GetProperty ("Name"), false);
      var propertyDefinitionNullable = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithDbTableAttribute, StorageClass.Persistent, typeof (Company).GetProperty ("Name"), true);

      StubStorageCalculators (propertyDefinitionNotNullable);
      StubStorageCalculators (propertyDefinitionNullable);

      var resultNotNullable =
           (SimpleColumnDefinition) _columnDefinitionFactory.CreateColumnDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleColumnDefinition) _columnDefinitionFactory.CreateColumnDefinition (propertyDefinitionNullable);

      Assert.That (resultNotNullable.IsNullable, Is.False);
      Assert.That (resultNullable.IsNullable, Is.True);
    }

    [Test]
    public void CreateColumnDefinition_ClassBelowDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = ReflectionBasedPropertyDefinitionFactory.Create (
          _classBelowDbTableAttribute, StorageClass.Persistent, typeof (Partner).GetProperty ("ContactPerson"), false);
      var propertyDefinitionNullable = ReflectionBasedPropertyDefinitionFactory.Create (
          _classBelowDbTableAttribute, StorageClass.Persistent, typeof (Partner).GetProperty ("ContactPerson"), true);

      StubStorageCalculators (propertyDefinitionNotNullable);
      StubStorageCalculators (propertyDefinitionNullable);

      var resultNotNullable =
           (SimpleColumnDefinition) _columnDefinitionFactory.CreateColumnDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleColumnDefinition) _columnDefinitionFactory.CreateColumnDefinition (propertyDefinitionNullable);

      Assert.That (resultNotNullable.IsNullable, Is.True);
      Assert.That (resultNullable.IsNullable, Is.True);
    }

    [Test]
    public void CreateColumnDefinition_ClassBelowBelowDbTableAttribute_And_PropertyDefinitionIsNotNullabe ()
    {
      var propertyDefinitionNotNullable = ReflectionBasedPropertyDefinitionFactory.Create (
          _classBelowBelowDbTableAttribute,
          StorageClass.Persistent,
          typeof (Distributor).GetProperty ("ClassWithoutRelatedClassIDColumn", BindingFlags.NonPublic | BindingFlags.Instance),
          false);
      var propertyDefinitionNullable = ReflectionBasedPropertyDefinitionFactory.Create (
         _classBelowBelowDbTableAttribute,
         StorageClass.Persistent,
         typeof (Distributor).GetProperty ("ClassWithoutRelatedClassIDColumn", BindingFlags.NonPublic | BindingFlags.Instance),
         true);

      StubStorageCalculators (propertyDefinitionNotNullable);
      StubStorageCalculators (propertyDefinitionNullable);

      var resultNotNullable =
           (SimpleColumnDefinition) _columnDefinitionFactory.CreateColumnDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleColumnDefinition) _columnDefinitionFactory.CreateColumnDefinition (propertyDefinitionNullable);

      Assert.That (resultNotNullable.IsNullable, Is.True);
      Assert.That (resultNullable.IsNullable, Is.True);
    }

    [Test]
    public void CreaeStoragePropertyDefinition_NonRelationProperty_GetsNoClassIdColumn ()
    {
      var classDefinition = Configuration.ClassDefinitions[typeof (Order)];
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (Order).FullName + ".OrderNumber"];
      StubStorageCalculators (propertyDefinition);

      var result = _columnDefinitionFactory.CreateColumnDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (SimpleColumnDefinition)));
    }

    [Test]
    public void CreaeStoragePropertyDefinition_RelationPropertyToAClassDefinitionThatISNotPartOfTheMapping_GetsNotClassIDColumn ()
    {
      var classDefinition = Configuration.ClassDefinitions[typeof (ClassWithManySideRelationProperties)];
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (ClassWithManySideRelationProperties).FullName + ".BidirectionalOneToOne"];
      StubStorageCalculators (propertyDefinition);
      
      var result = _columnDefinitionFactory.CreateColumnDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (IDColumnDefinition)));
      Assert.That (((IDColumnDefinition) result).HasClassIDColumn, Is.False);
    }

    [Test]
    public void CreaeStoragePropertyDefinition_RelationPropertyToAClassDefinitionWithBaseClassAndNoDerivedClasses_GetsClassIDColumn ()
    {
      var classDefinition = Configuration.ClassDefinitions[typeof (ClassWithoutRelatedClassIDColumn)];
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (ClassWithoutRelatedClassIDColumn).FullName + ".Distributor"];
      StubStorageCalculators (propertyDefinition);
      
      var result = _columnDefinitionFactory.CreateColumnDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (IDColumnDefinition)));
      Assert.That (((IDColumnDefinition) result).HasClassIDColumn, Is.True);
      var objectIDColumn = (SimpleColumnDefinition) ((IDColumnDefinition) result).ObjectIDColumn;
      var classIDColumn = (SimpleColumnDefinition) ((IDColumnDefinition) result).ClassIDColumn;

      Assert.That (objectIDColumn.Name, Is.EqualTo ("FakeColumnName"));
      Assert.That (objectIDColumn.IsNullable, Is.True);
      Assert.That (objectIDColumn.PropertyType, Is.SameAs (typeof (ObjectID)));
      Assert.That (objectIDColumn.StorageType, Is.EqualTo ("storage type"));
      Assert.That (objectIDColumn.IsPartOfPrimaryKey, Is.False);
      Assert.That (classIDColumn.Name, Is.EqualTo ("FakeRelationClassID"));
      Assert.That (classIDColumn.IsNullable, Is.True);
      Assert.That (classIDColumn.PropertyType, Is.SameAs (typeof (string)));
      Assert.That (classIDColumn.StorageType, Is.EqualTo ("varchar(100)"));
      Assert.That (classIDColumn.IsPartOfPrimaryKey, Is.False);
    }

    [Test]
    public void CreaeStoragePropertyDefinition_RelationPropertyToAClassDefinitionWithoutBaseClassAndWithDerivedClasses_GetsClassIDColumn ()
    {
      var classDefinition = Configuration.ClassDefinitions[typeof (Ceo)];
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (Ceo).FullName + ".Company"];
      StubStorageCalculators (propertyDefinition);

      var result = _columnDefinitionFactory.CreateColumnDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (IDColumnDefinition)));
    }

    [Test]
    public void CreateIDColumnDefinition ()
    {
      var result = _columnDefinitionFactory.CreateIDColumnDefinition();

      var objectIDColumn = (SimpleColumnDefinition) result.ObjectIDColumn;
      var classIDColumn = (SimpleColumnDefinition) result.ClassIDColumn;

      Assert.That (objectIDColumn.Name, Is.EqualTo ("ID"));
      Assert.That (objectIDColumn.IsNullable, Is.False);
      Assert.That (objectIDColumn.IsPartOfPrimaryKey, Is.True);
      Assert.That (objectIDColumn.PropertyType, Is.SameAs (typeof (ObjectID)));
      Assert.That (objectIDColumn.StorageType, Is.EqualTo ("guid"));
      Assert.That (classIDColumn.Name, Is.EqualTo ("ClassID"));
      Assert.That (classIDColumn.IsNullable, Is.False);
      Assert.That (classIDColumn.PropertyType, Is.SameAs (typeof (string)));
      Assert.That (classIDColumn.StorageType, Is.EqualTo ("varchar(100)"));
      Assert.That (classIDColumn.IsPartOfPrimaryKey, Is.False);
    }

    [Test]
    public void CreateTimestampColumnDefinition ()
    {
      var result = _columnDefinitionFactory.CreateTimestampColumnDefinition();

      Assert.That (result.Name, Is.EqualTo ("Timestamp"));
      Assert.That (result.IsNullable, Is.False);
      Assert.That (result.PropertyType, Is.SameAs (typeof (object)));
      Assert.That (result.StorageType, Is.EqualTo ("rowversion"));
      Assert.That (result.IsPartOfPrimaryKey, Is.False);
    }

    private void StubStorageCalculators (PropertyDefinition propertyDefinition)
    {
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition)).Return ("storage type");
      _storageNameCalculatorStub.Stub (stub => stub.GetColumnName (propertyDefinition)).Return ("FakeColumnName");
      _storageNameCalculatorStub.Stub (stub => stub.GetRelationClassIDColumnName (propertyDefinition)).Return ("FakeRelationClassID");
    }
  }
}