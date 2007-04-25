using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.CodeGenerator.Sql.SqlServer;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.Sql.SqlServer
{
  [TestFixture]
  public class TableBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private TableBuilder _tableBuilder;

    // construction and disposing

    public TableBuilderTest ()
    {
    }

    // methods and properties
    
    public override void SetUp ()
    {
      base.SetUp ();

      _tableBuilder = new TableBuilder ();
    }

    [Test]
    public void GetSqlDataType ()
    {
      Assert.AreEqual ("bit", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "boolean", false, false, null)));
      Assert.AreEqual ("tinyint", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "byte", false, false, null)));
      Assert.AreEqual ("datetime", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "date", false, false, null)));
      Assert.AreEqual ("datetime", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "dateTime", false, false, null)));
      Assert.AreEqual ("decimal (38, 3)", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "decimal", false, false, null)));
      Assert.AreEqual ("float", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "double", false, false, null)));
      Assert.AreEqual ("uniqueidentifier", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "guid", false, false, null)));
      Assert.AreEqual ("smallint", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "int16", false, false, null)));
      Assert.AreEqual ("int", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "int32", false, false, null)));
      Assert.AreEqual ("bigint", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "int64", false, false, null)));
      Assert.AreEqual ("real", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "single", false, false, null)));
      Assert.AreEqual ("nvarchar (100)", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "string", false, false, 100)));

      Assert.AreEqual ("ntext", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "string")));

      Assert.AreEqual ("image", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "binary", false, false, null)));
      Assert.AreEqual ("uniqueidentifier", _tableBuilder.GetSqlDataType (OrderItemClass.GetMandatoryPropertyDefinition ("Order")));
      Assert.AreEqual ("varchar (255)", _tableBuilder.GetSqlDataType (CustomerClass.GetMandatoryPropertyDefinition ("PrimaryOfficial")));
    }

    [Test]
    public void GetSqlDataTypeWithDotNetType ()
    {
      string mappingTypeName = "Namespace.TypeName, AssemblyName";
      Assert.AreEqual ("int", _tableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", mappingTypeName, false, false, null)));
    }

    [Test]
    public void AddToCreateTableScript ()
    {
      string expectedStatement = "CREATE TABLE [dbo].[Ceo]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- Ceo columns\r\n"
          + "  [Name] nvarchar (100) NOT NULL,\r\n"
          + "  [CompanyID] uniqueidentifier NULL,\r\n"
          + "  [CompanyIDClassID] varchar (100) NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_Ceo] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder ();
 
      _tableBuilder.AddToCreateTableScript (CeoClass, stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString ());
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScriptWithConcreteClass ()
    {
      string expectedStatement = "CREATE TABLE [dbo].[Customer]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- Company columns\r\n"
          + "  [Name] nvarchar (100) NOT NULL,\r\n"
          + "  [PhoneNumber] nvarchar (100) NULL,\r\n"
          + "  [AddressID] uniqueidentifier NULL,\r\n\r\n"
          + "  -- Customer columns\r\n"
          + "  [CustomerType] int NOT NULL,\r\n"
          + "  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,\r\n"
          + "  [PrimaryOfficialID] varchar (255) NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();
   
      _tableBuilder.AddToCreateTableScript (CustomerClass, stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString ());
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScriptWithTwoAbstractBaseClasses ()
    {
      XmlBasedClassDefinition abstractClass = new XmlBasedClassDefinition ("AbstractClass", null, "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);
      abstractClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("PropertyInAbstractClass", "PropertyInAbstractClass", "string", false, true, 100));

      XmlBasedClassDefinition derivedAbstractClass = new XmlBasedClassDefinition (
          "DerivedAbstractClass", null, "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false, abstractClass);

      derivedAbstractClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("PropertyInAbstractDerivedClass", "PropertyInAbstractDerivedClass", "string", false, false, 101));

      XmlBasedClassDefinition derivedConcreteClass = new XmlBasedClassDefinition (
          "DerivedConcreteClass", "EntityName", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false, derivedAbstractClass);

      derivedConcreteClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("PropertyInDerivedConcreteClass", "PropertyInDerivedConcreteClass", "string", false, true, 102));

      string expectedStatement = "CREATE TABLE [dbo].[EntityName]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- AbstractClass columns\r\n"
          + "  [PropertyInAbstractClass] nvarchar (100) NULL,\r\n\r\n"
          + "  -- DerivedAbstractClass columns\r\n"
          + "  [PropertyInAbstractDerivedClass] nvarchar (101) NOT NULL,\r\n\r\n"
          + "  -- DerivedConcreteClass columns\r\n"
          + "  [PropertyInDerivedConcreteClass] nvarchar (102) NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_EntityName] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder ();

      _tableBuilder.AddToCreateTableScript (derivedConcreteClass, stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString ());
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScriptWithDerivedClasses ()
    {
      string expectedStatement = "CREATE TABLE [dbo].[ConcreteClass]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- ConcreteClass columns\r\n"
          + "  [PropertyInConcreteClass] nvarchar (100) NOT NULL,\r\n\r\n"
          + "  -- DerivedClass columns\r\n"
          + "  [PropertyInDerivedClass] nvarchar (100) NULL,\r\n\r\n"
          + "  -- DerivedOfDerivedClass columns\r\n"
          + "  [PropertyInDerivedOfDerivedClass] nvarchar (100) NULL,\r\n"
          + "  [ClassWithRelationsInDerivedOfDerivedClassID] uniqueidentifier NULL,\r\n\r\n"
          + "  -- SecondDerivedClass columns\r\n"
          + "  [PropertyInSecondDerivedClass] nvarchar (100) NULL,\r\n"
          + "  [ClassWithRelationsInSecondDerivedClassID] uniqueidentifier NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_ConcreteClass] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder ();

      _tableBuilder.AddToCreateTableScript (MappingConfiguration.ClassDefinitions.GetMandatory ("ConcreteClass"), stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString ());
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScriptWithRelationToClassWithoutInheritance ()
    {
      string expectedStatement = "CREATE TABLE [dbo].[OrderItem]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- OrderItem columns\r\n"
          + "  [Position] int NOT NULL,\r\n"
          + "  [Product] nvarchar (100) NOT NULL,\r\n"
          + "  [OrderID] uniqueidentifier NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder ();

      _tableBuilder.AddToCreateTableScript (OrderItemClass, stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString ());
    }

    [Test]
    public void AddToDropTableScript ()
    {
      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP TABLE [dbo].[Customer]\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToDropTableScript (CustomerClass, stringBuilder);
      
      Assert.AreEqual (expectedScript, stringBuilder.ToString());
    }

    [Test]
    public void IntegrationTest ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (CustomerClass);
      classes.Add (OrderClass);

      _tableBuilder.AddTables (classes);

      string expectedCreateTableScript = "CREATE TABLE [dbo].[Customer]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- Company columns\r\n"
          + "  [Name] nvarchar (100) NOT NULL,\r\n"
          + "  [PhoneNumber] nvarchar (100) NULL,\r\n"
          + "  [AddressID] uniqueidentifier NULL,\r\n\r\n"
          + "  -- Customer columns\r\n"
          + "  [CustomerType] int NOT NULL,\r\n"
          + "  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,\r\n"
          + "  [PrimaryOfficialID] varchar (255) NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n\r\n"
          + "CREATE TABLE [dbo].[Order]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- Order columns\r\n"
          + "  [Number] int NOT NULL,\r\n"
          + "  [Priority] int NOT NULL,\r\n"
          + "  [CustomerID] uniqueidentifier NULL,\r\n"
          + "  [CustomerIDClassID] varchar (100) NULL,\r\n"
          + "  [OfficialID] varchar (255) NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";

      Assert.AreEqual (expectedCreateTableScript, _tableBuilder.GetCreateTableScript ());

      string expectedDropTableScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP TABLE [dbo].[Customer]\r\n\r\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP TABLE [dbo].[Order]\r\n";

      Assert.AreEqual (expectedDropTableScript, _tableBuilder.GetDropTableScript ());
    }
  }
}
