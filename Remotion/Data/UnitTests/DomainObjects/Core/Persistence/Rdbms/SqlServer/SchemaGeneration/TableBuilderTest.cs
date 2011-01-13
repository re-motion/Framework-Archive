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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGenerationTestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class TableBuilderTest : SchemaGenerationTestBase
  {
    private TableBuilder _tableBuilder;

    public override void SetUp ()
    {
      base.SetUp();

      _tableBuilder = new TableBuilder();
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScript ()
    {
      string expectedStatement =
          "CREATE TABLE [dbo].[Ceo]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n"
          + "  [Name] nvarchar (100) NOT NULL,\r\n"
          + "  [CompanyID] uniqueidentifier NULL,\r\n"
          + "  [CompanyIDClassID] varchar (100) NULL,\r\n"
          + "  CONSTRAINT [PK_Ceo] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (
          (TableDefinition) MappingConfiguration.ClassDefinitions[typeof (Ceo)].StorageEntityDefinition, stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString());
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScriptWithConcreteClass ()
    {
      string expectedStatement =
          "CREATE TABLE [dbo].[Customer]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n"
          + "  [Name] nvarchar (100) NOT NULL,\r\n"
          + "  [PhoneNumber] nvarchar (100) NULL,\r\n"
          + "  [AddressID] uniqueidentifier NULL,\r\n"
          + "  [CustomerType] int NOT NULL,\r\n"
          + "  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,\r\n"
          + "  [PrimaryOfficialID] varchar (255) NULL,\r\n"
          + "  [LicenseCode] nvarchar (max) NULL,\r\n"
          + "  CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (
          (TableDefinition) MappingConfiguration.ClassDefinitions[typeof (Customer)].StorageEntityDefinition, stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString());
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScriptWithDerivedClasses ()
    {
      string expectedStatement = "CREATE TABLE [dbo].[ConcreteClass]\r\n"
                                 + "(\r\n"
                                 + "  [ID] uniqueidentifier NOT NULL,\r\n"
                                 + "  [ClassID] varchar (100) NOT NULL,\r\n"
                                 + "  [Timestamp] rowversion NOT NULL,\r\n"
                                 + "  [PropertyInConcreteClass] nvarchar (100) NOT NULL,\r\n"
                                 + "  [PropertyInDerivedClass] nvarchar (100) NULL,\r\n"
                                 + "  [PersistentProperty] nvarchar (max) NULL,\r\n"
                                 + "  [PropertyInDerivedOfDerivedClass] nvarchar (100) NULL,\r\n"
                                 + "  [ClassWithRelationsInDerivedOfDerivedClassID] uniqueidentifier NULL,\r\n"
                                 + "  [ClassWithRelationsInDerivedOfDerivedClassIDClassID] varchar (100) NULL,\r\n"
                                 + "  [PropertyInSecondDerivedClass] nvarchar (100) NULL,\r\n"
                                 + "  [ClassWithRelationsInSecondDerivedClassID] uniqueidentifier NULL,\r\n"
                                 + "  [ClassWithRelationsInSecondDerivedClassIDClassID] varchar (100) NULL,\r\n"
                                 + "  CONSTRAINT [PK_ConcreteClass] PRIMARY KEY CLUSTERED ([ID])\r\n"
                                 + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (
          (TableDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("ConcreteClass").StorageEntityDefinition, stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString());
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScriptWithRelationToClassWithoutInheritance ()
    {
      string expectedStatement = "CREATE TABLE [dbo].[OrderItem]\r\n"
                                 + "(\r\n"
                                 + "  [ID] uniqueidentifier NOT NULL,\r\n"
                                 + "  [ClassID] varchar (100) NOT NULL,\r\n"
                                 + "  [Timestamp] rowversion NOT NULL,\r\n"
                                 + "  [Position] int NOT NULL,\r\n"
                                 + "  [Product] nvarchar (100) NOT NULL,\r\n"
                                 + "  [OrderID] uniqueidentifier NULL,\r\n"
                                 + "  CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED ([ID])\r\n"
                                 + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (
          (TableDefinition) MappingConfiguration.ClassDefinitions[typeof (OrderItem)].StorageEntityDefinition, stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString());
    }

    [Test]
    public void AddToCreateTableScriptWithClassWithoutProperties ()
    {
      string expectedStatement =
          "CREATE TABLE [dbo].[TableWithoutProperties]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n"
          + "  CONSTRAINT [PK_TableWithoutProperties] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (
          (TableDefinition) MappingConfiguration.ClassDefinitions[typeof (ClassWithoutProperties)].StorageEntityDefinition, stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString());
    }

    [Test]
    public void AddToDropTableScript ()
    {
      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer' AND TABLE_SCHEMA = 'dbo')\r\n"
                              + "  DROP TABLE [dbo].[Customer]\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToDropTableScript (
          (TableDefinition) MappingConfiguration.ClassDefinitions[typeof (Customer)].StorageEntityDefinition, stringBuilder);

      Assert.AreEqual (expectedScript, stringBuilder.ToString());
    }

    [Test]
    public void IntegrationTest ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (MappingConfiguration.ClassDefinitions[typeof (Customer)]);
      classes.Add (MappingConfiguration.ClassDefinitions[typeof (Order)]);

      _tableBuilder.AddTables (classes);

      string expectedCreateTableScript = "CREATE TABLE [dbo].[Customer]\r\n"
                                         + "(\r\n"
                                         + "  [ID] uniqueidentifier NOT NULL,\r\n"
                                         + "  [ClassID] varchar (100) NOT NULL,\r\n"
                                         + "  [Timestamp] rowversion NOT NULL,\r\n"
                                         + "  [Name] nvarchar (100) NOT NULL,\r\n"
                                         + "  [PhoneNumber] nvarchar (100) NULL,\r\n"
                                         + "  [AddressID] uniqueidentifier NULL,\r\n"
                                         + "  [CustomerType] int NOT NULL,\r\n"
                                         + "  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,\r\n"
                                         + "  [PrimaryOfficialID] varchar (255) NULL,\r\n"
                                         + "  [LicenseCode] nvarchar (max) NULL,\r\n"
                                         + "  CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID])\r\n"
                                         + ")\r\n\r\n"
                                         + "CREATE TABLE [dbo].[Order]\r\n"
                                         + "(\r\n"
                                         + "  [ID] uniqueidentifier NOT NULL,\r\n"
                                         + "  [ClassID] varchar (100) NOT NULL,\r\n"
                                         + "  [Timestamp] rowversion NOT NULL,\r\n"
                                         + "  [Number] int NOT NULL,\r\n"
                                         + "  [Priority] int NOT NULL,\r\n"
                                         + "  [CustomerID] uniqueidentifier NULL,\r\n"
                                         + "  [CustomerIDClassID] varchar (100) NULL,\r\n"
                                         + "  [OfficialID] varchar (255) NULL,\r\n"
                                         + "  CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([ID])\r\n"
                                         + ")\r\n";

      Assert.AreEqual (expectedCreateTableScript, _tableBuilder.GetCreateTableScript());

      string expectedDropTableScript =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP TABLE [dbo].[Customer]\r\n\r\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP TABLE [dbo].[Order]\r\n";

      Assert.AreEqual (expectedDropTableScript, _tableBuilder.GetDropTableScript());
    }

    private PropertyDefinition CreatePropertyDefinition (
        ReflectionBasedClassDefinition classDefinition,
        string propertyName,
        string columnName,
        Type propertyType,
        PropertyInfo propertyInfo,
        bool? isNullable,
        int? maxLength,
        StorageClass storageClass)
    {
      var propertyDefinition = new ReflectionBasedPropertyDefinition (
          classDefinition,
          propertyInfo,
          propertyName,
          propertyType,
          isNullable,
          maxLength,
          storageClass);
      propertyDefinition.SetStorageProperty (
          new SimpleColumnDefinition (columnName, propertyType, "dummyStorageType", isNullable.HasValue ? isNullable.Value : true, false));
      return propertyDefinition;
    }

    private void ApplyPersistenceModel (ClassDefinition classDefinition)
    {
      var providerDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
      var columnDefinitionFactory = new ColumnDefinitionFactory (new SqlStorageTypeCalculator (providerDefinitionFinder), providerDefinitionFinder);
      var columnDefinitionResolver = new ColumnDefinitionResolver ();
      var storageNameCalculator = new StorageNameCalculator ();
      var foreignKeyConstraintDefinitionFactory = new ForeignKeyConstraintDefinitionFactory (storageNameCalculator, columnDefinitionResolver, columnDefinitionFactory, providerDefinitionFinder);
      var entityDefinitionFactory = new EntityDefinitionFactory (columnDefinitionFactory, foreignKeyConstraintDefinitionFactory, columnDefinitionResolver, SchemaGenerationFirstStorageProviderDefinition);

      var persistenceModelLoader = new RdbmsPersistenceModelLoader (
          entityDefinitionFactory,
          columnDefinitionFactory,
          SchemaGenerationFirstStorageProviderDefinition);
      persistenceModelLoader.ApplyPersistenceModelToHierarchy (classDefinition);
    }

  }
}