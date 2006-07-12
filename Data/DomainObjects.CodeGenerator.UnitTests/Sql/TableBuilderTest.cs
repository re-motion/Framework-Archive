using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.CodeGenerator.Sql;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.Sql
{
  [TestFixture]
  public class TableBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private TableBuilder _tableBuilder;
    private ClassDefinition _ceoClass;
    private ClassDefinition _customerClass;
    private ClassDefinition _orderItemClass;

    // construction and disposing

    public TableBuilderTest ()
    {
    }

    // methods and properties
    
    public override void SetUp ()
    {
      base.SetUp ();

      _tableBuilder = new TableBuilder ();
      _ceoClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Ceo");
      _customerClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Customer");
      _orderItemClass = MappingConfiguration.ClassDefinitions.GetMandatory ("OrderItem");
    }

    [Test]
    public void GetSqlDataType ()
    {
      Assert.AreEqual ("bit", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "boolean", false, false, NaInt32.Null)));
      Assert.AreEqual ("tinyint", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "byte", false, false, NaInt32.Null)));
      Assert.AreEqual ("datetime", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "date", false, false, NaInt32.Null)));
      Assert.AreEqual ("datetime", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "dateTime", false, false, NaInt32.Null)));
      Assert.AreEqual ("decimal (38, 3)", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "decimal", false, false, NaInt32.Null)));
      Assert.AreEqual ("float", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "double", false, false, NaInt32.Null)));
      Assert.AreEqual ("uniqueidentifier", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "guid", false, false, NaInt32.Null)));
      Assert.AreEqual ("smallint", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "int16", false, false, NaInt32.Null)));
      Assert.AreEqual ("int", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "int32", false, false, NaInt32.Null)));
      Assert.AreEqual ("bigint", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "int64", false, false, NaInt32.Null)));
      Assert.AreEqual ("real", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "single", false, false, NaInt32.Null)));
      Assert.AreEqual ("nvarchar (100)", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "string", false, false, 100)));
      
      Assert.AreEqual ("text", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "string")));

      Assert.AreEqual ("image", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "binary", false, false, NaInt32.Null)));
      Assert.AreEqual ("uniqueidentifier", TableBuilder.GetSqlDataType (_orderItemClass.GetMandatoryPropertyDefinition ("Order")));
      Assert.AreEqual ("varchar (255)", TableBuilder.GetSqlDataType (_customerClass.GetMandatoryPropertyDefinition ("PrimaryOfficial")));
    }

    [Test]
    public void GetSqlDataTypeWithDotNetType ()
    {
      string mappingTypeName = "Namespace.TypeName, AssemblyName";
      Assert.AreEqual ("int", TableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", mappingTypeName, false, false, NaInt32.Null)));
    }

    [Test]
    public void AddTable ()
    {
      _tableBuilder.AddTable (_ceoClass);

      string expectedStatement = "CREATE TABLE [dbo].[Ceo]\n"
          + "(\n"
          + "  [ID] uniqueidentifier NOT NULL,\n"
          + "  [ClassID] varchar (100) NOT NULL,\n"
          + "  [Timestamp] rowversion NOT NULL,\n\n"
          + "  -- Ceo columns\n"
          + "  [Name] nvarchar (100) NOT NULL,\n"
          + "  [CompanyID] uniqueidentifier NULL,\n"
          + "  [CompanyIDClassID] varchar (100) NULL,\n\n"
          + "  CONSTRAINT [PK_Ceo] PRIMARY KEY CLUSTERED ([ID])\n"
          + ")\n";

      Assert.AreEqual (expectedStatement, _tableBuilder.GetCreateTableScript ());
    }

    [Test]
    public void AddTableWithConcreteClass ()
    {
      _tableBuilder.AddTable (_customerClass);

      string expectedStatement = "CREATE TABLE [dbo].[Customer]\n"
          + "(\n"
          + "  [ID] uniqueidentifier NOT NULL,\n"
          + "  [ClassID] varchar (100) NOT NULL,\n"
          + "  [Timestamp] rowversion NOT NULL,\n\n"
          + "  -- Company columns\n"
          + "  [Name] nvarchar (100) NOT NULL,\n"
          + "  [PhoneNumber] nvarchar (100) NULL,\n"
          + "  [AddressID] uniqueidentifier NULL,\n\n"
          + "  -- Customer columns\n"
          + "  [CustomerType] int NOT NULL,\n"
          + "  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,\n"
          + "  [PrimaryOfficialID] varchar (255) NULL,\n\n"
          + "  CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID])\n"
          + ")\n";

      Assert.AreEqual (expectedStatement, _tableBuilder.GetCreateTableScript ());
    }

    [Test]
    public void AddTableForWithTwoAbstractBaseClasses ()
    {
      ClassDefinition abstractClass = new ClassDefinition ("AbstractClass", null, "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);
      abstractClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("PropertyInAbstractClass", "PropertyInAbstractClass", "string", false, true, 100));

      ClassDefinition derivedAbstractClass = new ClassDefinition (
          "DerivedAbstractClass", null, "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false, abstractClass);

      derivedAbstractClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("PropertyInAbstractDerivedClass", "PropertyInAbstractDerivedClass", "string", false, false, 101));

      ClassDefinition derivedConcreteClass = new ClassDefinition (
          "DerivedConcreteClass", "EntityName", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false, derivedAbstractClass);

      derivedConcreteClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("PropertyInDerivedConcreteClass", "PropertyInDerivedConcreteClass", "string", false, true, 102));

      _tableBuilder.AddTable (derivedConcreteClass);

      string expectedStatement = "CREATE TABLE [dbo].[EntityName]\n"
          + "(\n"
          + "  [ID] uniqueidentifier NOT NULL,\n"
          + "  [ClassID] varchar (100) NOT NULL,\n"
          + "  [Timestamp] rowversion NOT NULL,\n\n"
          + "  -- AbstractClass columns\n"
          + "  [PropertyInAbstractClass] nvarchar (100) NULL,\n\n"
          + "  -- DerivedAbstractClass columns\n"
          + "  [PropertyInAbstractDerivedClass] nvarchar (101) NOT NULL,\n\n"
          + "  -- DerivedConcreteClass columns\n"
          + "  [PropertyInDerivedConcreteClass] nvarchar (102) NULL,\n\n"
          + "  CONSTRAINT [PK_EntityName] PRIMARY KEY CLUSTERED ([ID])\n"
          + ")\n";

      Assert.AreEqual (expectedStatement, _tableBuilder.GetCreateTableScript ());
    }

    [Test]
    public void AddTableWithDerivedClasses ()
    {
      _tableBuilder.AddTable (MappingConfiguration.ClassDefinitions.GetMandatory ("ConcreteClass"));

      string expectedStatement = "CREATE TABLE [dbo].[ConcreteClass]\n"
          + "(\n"
          + "  [ID] uniqueidentifier NOT NULL,\n"
          + "  [ClassID] varchar (100) NOT NULL,\n"
          + "  [Timestamp] rowversion NOT NULL,\n\n"
          + "  -- ConcreteClass columns\n"
          + "  [PropertyInConcreteClass] nvarchar (100) NOT NULL,\n\n"
          + "  -- DerivedClass columns\n"
          + "  [PropertyInDerivedClass] nvarchar (100) NULL,\n\n"
          + "  -- DerivedOfDerivedClass columns\n"
          + "  [PropertyInDerivedOfDerivedClass] nvarchar (100) NULL,\n"
          + "  [ClassWithRelationsInDerivedOfDerivedClassID] uniqueidentifier NULL,\n\n"
          + "  -- SecondDerivedClass columns\n"
          + "  [PropertyInSecondDerivedClass] nvarchar (100) NULL,\n"
          + "  [ClassWithRelationsInSecondDerivedClassID] uniqueidentifier NULL,\n\n"
          + "  CONSTRAINT [PK_ConcreteClass] PRIMARY KEY CLUSTERED ([ID])\n"
          + ")\n";

      Assert.AreEqual (expectedStatement, _tableBuilder.GetCreateTableScript ());
    }

    [Test]
    public void AddTableWithRelationToClassWithoutInheritance ()
    {
      _tableBuilder.AddTable (OrderItemClass);

      string expectedStatement = "CREATE TABLE [dbo].[OrderItem]\n"
          + "(\n"
          + "  [ID] uniqueidentifier NOT NULL,\n"
          + "  [ClassID] varchar (100) NOT NULL,\n"
          + "  [Timestamp] rowversion NOT NULL,\n\n"
          + "  -- OrderItem columns\n"
          + "  [Position] int NOT NULL,\n"
          + "  [Product] nvarchar (100) NOT NULL,\n"
          + "  [OrderID] uniqueidentifier NULL,\n\n"
          + "  CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED ([ID])\n"
          + ")\n";

      Assert.AreEqual (expectedStatement, _tableBuilder.GetCreateTableScript ());
    }

    [Test]
    public void AddTableTwice ()
    {
      _tableBuilder.AddTable (OrderItemClass);
      _tableBuilder.AddTable (OrderItemClass);

      string expectedStatement = "CREATE TABLE [dbo].[OrderItem]\n"
          + "(\n"
          + "  [ID] uniqueidentifier NOT NULL,\n"
          + "  [ClassID] varchar (100) NOT NULL,\n"
          + "  [Timestamp] rowversion NOT NULL,\n\n"
          + "  -- OrderItem columns\n"
          + "  [Position] int NOT NULL,\n"
          + "  [Product] nvarchar (100) NOT NULL,\n"
          + "  [OrderID] uniqueidentifier NULL,\n\n"
          + "  CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED ([ID])\n"
          + ")\n\n"
          + "CREATE TABLE [dbo].[OrderItem]\n"
          + "(\n"
          + "  [ID] uniqueidentifier NOT NULL,\n"
          + "  [ClassID] varchar (100) NOT NULL,\n"
          + "  [Timestamp] rowversion NOT NULL,\n\n"
          + "  -- OrderItem columns\n"
          + "  [Position] int NOT NULL,\n"
          + "  [Product] nvarchar (100) NOT NULL,\n"
          + "  [OrderID] uniqueidentifier NULL,\n\n"
          + "  CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED ([ID])\n"
          + ")\n";

      Assert.AreEqual (expectedStatement, _tableBuilder.GetCreateTableScript ());
    }

    [Test]
    public void GetDropTableScript ()
    {
      _tableBuilder.AddTable (CustomerClass);

      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer')\n"
          + "  DROP TABLE [dbo].[Customer]\n";

      Assert.AreEqual (expectedScript, _tableBuilder.GetDropTableScript ());
    }

    [Test]
    public void GetDropTableScriptWithMultipleTables ()
    {
      _tableBuilder.AddTable (CustomerClass);
      _tableBuilder.AddTable (OrderClass);

      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer')\n"
          + "  DROP TABLE [dbo].[Customer]\n\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order')\n"
          + "  DROP TABLE [dbo].[Order]\n";

      Assert.AreEqual (expectedScript, _tableBuilder.GetDropTableScript ());
    }

    [Test]
    public void AddTables ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (CustomerClass);
      classes.Add (OrderClass);

      _tableBuilder.AddTables (classes);

      string expectedCreateTableScript = "CREATE TABLE [dbo].[Customer]\n"
          + "(\n"
          + "  [ID] uniqueidentifier NOT NULL,\n"
          + "  [ClassID] varchar (100) NOT NULL,\n"
          + "  [Timestamp] rowversion NOT NULL,\n\n"
          + "  -- Company columns\n"
          + "  [Name] nvarchar (100) NOT NULL,\n"
          + "  [PhoneNumber] nvarchar (100) NULL,\n"
          + "  [AddressID] uniqueidentifier NULL,\n\n"
          + "  -- Customer columns\n"
          + "  [CustomerType] int NOT NULL,\n"
          + "  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,\n"
          + "  [PrimaryOfficialID] varchar (255) NULL,\n\n"
          + "  CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID])\n"
          + ")\n\n"
          + "CREATE TABLE [dbo].[Order]\n"
          + "(\n"
          + "  [ID] uniqueidentifier NOT NULL,\n"
          + "  [ClassID] varchar (100) NOT NULL,\n"
          + "  [Timestamp] rowversion NOT NULL,\n\n"
          + "  -- Order columns\n"
          + "  [Number] int NOT NULL,\n"
          + "  [Priority] int NOT NULL,\n"
          + "  [CustomerID] uniqueidentifier NULL,\n"
          + "  [CustomerIDClassID] varchar (100) NULL,\n"
          + "  [OfficialID] varchar (255) NULL,\n\n"
          + "  CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([ID])\n"
          + ")\n";

      Assert.AreEqual (expectedCreateTableScript, _tableBuilder.GetCreateTableScript ());

      string expectedDropTableScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer')\n"
          + "  DROP TABLE [dbo].[Customer]\n\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order')\n"
          + "  DROP TABLE [dbo].[Order]\n";

      Assert.AreEqual (expectedDropTableScript, _tableBuilder.GetDropTableScript ());
    }

    [Test]
    public void AddTableWithAbstractClass ()
    {
      _tableBuilder.AddTable (CompanyClass);

      Assert.IsEmpty (_tableBuilder.GetCreateTableScript ());
      Assert.IsEmpty (_tableBuilder.GetDropTableScript ());
    }

    [Test]
    public void AddTableWithDerivedClassWithoutEntityName ()
    {
      _tableBuilder.AddTable (DerivedClass);

      Assert.IsEmpty (_tableBuilder.GetCreateTableScript ());
      Assert.IsEmpty (_tableBuilder.GetDropTableScript ());
    }

    [Test]
    public void AddTableWithDerivedClassWithEntityName ()
    {
      _tableBuilder.AddTable (SecondDerivedClass);

      Assert.IsEmpty (_tableBuilder.GetCreateTableScript ());
      Assert.IsEmpty (_tableBuilder.GetDropTableScript ());
    }

    [Test]
    public void AddTableWithDerivedOfDerivedClassWithEntityName ()
    {
      _tableBuilder.AddTable (DerivedOfDerivedClass);

      Assert.IsEmpty (_tableBuilder.GetCreateTableScript ());
      Assert.IsEmpty (_tableBuilder.GetDropTableScript ());
    }
  }
}
