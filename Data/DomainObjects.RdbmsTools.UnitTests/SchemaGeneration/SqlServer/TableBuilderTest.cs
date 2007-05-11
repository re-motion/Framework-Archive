using System;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.RdbmsTools.SchemaGeneration.SqlServer;
using Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.SchemaGeneration.SqlServer
{
  [TestFixture]
  public class TableBuilderTest : StandardMappingTest
  {
    private enum Int32Enum : int
    {
    }

    private enum Int16Enum : short
    {
    }

    [FirstStorageGroup]
    [DBTable]
    private abstract class AbstractClass : DomainObject
    {
    }

    private abstract class DerivedAbstractClass : AbstractClass
    {
    }

    private class DerivedConcreteClass : DerivedAbstractClass
    {
    }

    private TableBuilder _tableBuilder;
    private ReflectionBasedClassDefinition _classDefintion;

    public override void SetUp ()
    {
      base.SetUp();

      _tableBuilder = new TableBuilder();
      _classDefintion = new ReflectionBasedClassDefinition ("ClassID", "Table", "StorageProvider", typeof (Order), false);
    }

    [Test]
    public void GetSqlDataType ()
    {
      Assert.AreEqual ("bit", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Boolean), null, null)));
      Assert.AreEqual ("tinyint", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Byte), null, null)));
      Assert.AreEqual ("datetime", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (DateTime), null, null)));
      Assert.AreEqual ("decimal (38, 3)", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Decimal), null, null)));
      Assert.AreEqual ("float", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Double), null, null)));
      Assert.AreEqual ("uniqueidentifier", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Guid), null, null)));
      Assert.AreEqual ("smallint", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Int16), null, null)));
      Assert.AreEqual ("int", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Int32), null, null)));
      Assert.AreEqual ("bigint", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Int64), null, null)));
      Assert.AreEqual ("real", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Single), null, null)));

      Assert.AreEqual ("int", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Int32Enum), null, null)));
      Assert.AreEqual ("smallint", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Int16Enum), null, null)));

      Assert.AreEqual ("nvarchar (100)", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (String), false, 100)));
      Assert.AreEqual ("ntext", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (String), false, null)));

      Assert.AreEqual ("image", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Byte[]), false, null)));
    }

    //TODO: Copy to TableBuilderBaseTest
    [Test]
    public void GetSqlDataTypeForSpecialCulumns ()
    {
      Assert.AreEqual (
          "uniqueidentifier",
          _tableBuilder.GetSqlDataType (
              OrderItemClass.GetMandatoryPropertyDefinition ("Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain.OrderItem.Order")));
      Assert.AreEqual (
          "varchar (255)",
          _tableBuilder.GetSqlDataType (
              CustomerClass.GetMandatoryPropertyDefinition ("Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain.Customer.PrimaryOfficial")));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "Data type 'System.Char' is not supported.\r\n  Class: ClassID, property: Name")]
    public void GetSqlDataType_WithNotSupportedType ()
    {
      _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Char), null, null));
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
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- Ceo columns\r\n"
          + "  [CompanyID] uniqueidentifier NULL,\r\n"
          + "  [CompanyIDClassID] varchar (100) NULL,\r\n"
          + "  [Name] nvarchar (100) NOT NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_Ceo] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (CeoClass, stringBuilder);

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
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- Company columns\r\n"
          + "  [AddressID] uniqueidentifier NULL,\r\n"
          + "  [Name] nvarchar (100) NOT NULL,\r\n"
          + "  [PhoneNumber] nvarchar (100) NULL,\r\n\r\n"
          + "  -- Customer columns\r\n"
          + "  [PrimaryOfficialID] varchar (255) NULL,\r\n"
          + "  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,\r\n"
          + "  [CustomerType] int NOT NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (CustomerClass, stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString());
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScriptWithTwoAbstractBaseClasses ()
    {
      ReflectionBasedClassDefinition abstractClass =
          new ReflectionBasedClassDefinition ("AbstractClass", null, "FirstStorageProvider", typeof (AbstractClass), false);
      abstractClass.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (abstractClass, "PropertyInAbstractClass", "PropertyInAbstractClass", typeof (string), true, 100));

      ReflectionBasedClassDefinition derivedAbstractClass =
          new ReflectionBasedClassDefinition ("DerivedAbstractClass", null, "FirstStorageProvider", typeof (DerivedAbstractClass), false, abstractClass);
      derivedAbstractClass.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              derivedAbstractClass, "PropertyInAbstractDerivedClass", "PropertyInAbstractDerivedClass", typeof (string), false, 101));

      ReflectionBasedClassDefinition derivedConcreteClass = new ReflectionBasedClassDefinition (
          "DerivedConcreteClass", "EntityName", "FirstStorageProvider", typeof (DerivedConcreteClass), false, derivedAbstractClass);
      derivedConcreteClass.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              derivedConcreteClass, "PropertyInDerivedConcreteClass", "PropertyInDerivedConcreteClass", typeof (string), true, 102));

      string expectedStatement =
          "CREATE TABLE [dbo].[EntityName]\r\n"
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
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (derivedConcreteClass, stringBuilder);

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
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- ConcreteClass columns\r\n"
          + "  [PropertyInConcreteClass] nvarchar (100) NOT NULL,\r\n\r\n"
          + "  -- DerivedClass columns\r\n"
          + "  [PropertyInDerivedClass] nvarchar (100) NULL,\r\n\r\n"
          + "  -- DerivedOfDerivedClass columns\r\n"
          + "  [ClassWithRelationsInDerivedOfDerivedClassID] uniqueidentifier NULL,\r\n"
          + "  [PropertyInDerivedOfDerivedClass] nvarchar (100) NULL,\r\n\r\n"
          + "  -- SecondDerivedClass columns\r\n"
          + "  [ClassWithRelationsInSecondDerivedClassID] uniqueidentifier NULL,\r\n"
          + "  [PropertyInSecondDerivedClass] nvarchar (100) NULL,\r\n\r\n"
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
          + "  [OrderID] uniqueidentifier NULL,\r\n"
          + "  [Position] int NOT NULL,\r\n"
          + "  [Product] nvarchar (100) NOT NULL,\r\n\r\n"
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
          + "  [AddressID] uniqueidentifier NULL,\r\n"
          + "  [Name] nvarchar (100) NOT NULL,\r\n"
          + "  [PhoneNumber] nvarchar (100) NULL,\r\n\r\n"
          + "  -- Customer columns\r\n"
          + "  [PrimaryOfficialID] varchar (255) NULL,\r\n"
          + "  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,\r\n"
          + "  [CustomerType] int NOT NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n\r\n"
          + "CREATE TABLE [dbo].[Order]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- Order columns\r\n"
          + "  [CustomerID] uniqueidentifier NULL,\r\n"
          + "  [CustomerIDClassID] varchar (100) NULL,\r\n"
          + "  [Number] int NOT NULL,\r\n"
          + "  [OfficialID] varchar (255) NULL,\r\n"
          + "  [Priority] int NOT NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";

      Assert.AreEqual (expectedCreateTableScript, _tableBuilder.GetCreateTableScript ());

      string expectedDropTableScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP TABLE [dbo].[Customer]\r\n\r\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP TABLE [dbo].[Order]\r\n";

      Assert.AreEqual (expectedDropTableScript, _tableBuilder.GetDropTableScript ());
    }

    private PropertyDefinition CreatePropertyDefinition (Type propertyType, bool? isNullable, int? maxLength)
    {
      return new ReflectionBasedPropertyDefinition (_classDefintion, "Name", "ColumnName", propertyType, isNullable, maxLength, true);
    }
  }
}