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
  public class CreateTableBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private CreateTableBuilder _createTableBuilder;
    private ClassDefinition _ceoClass;
    private ClassDefinition _customerClass;
    private ClassDefinition _companyClass;
    private ClassDefinition _orderItemClass;

    // construction and disposing

    public CreateTableBuilderTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _createTableBuilder = new CreateTableBuilder ();
      _ceoClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Ceo");
      _customerClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Customer");
      _companyClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Company");
      _orderItemClass = MappingConfiguration.ClassDefinitions.GetMandatory ("OrderItem");
    }

    [Test]
    public void GetSqlDataType ()
    {
      Assert.AreEqual ("bit", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "boolean", false, false, NaInt32.Null)));
      Assert.AreEqual ("tinyint", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "byte", false, false, NaInt32.Null)));
      Assert.AreEqual ("datetime", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "date", false, false, NaInt32.Null)));
      Assert.AreEqual ("datetime", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "dateTime", false, false, NaInt32.Null)));
      Assert.AreEqual ("decimal (38, 3)", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "decimal", false, false, NaInt32.Null)));
      Assert.AreEqual ("float", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "double", false, false, NaInt32.Null)));
      Assert.AreEqual ("uniqueidentifier", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "guid", false, false, NaInt32.Null)));
      Assert.AreEqual ("smallint", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "int16", false, false, NaInt32.Null)));
      Assert.AreEqual ("int", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "int32", false, false, NaInt32.Null)));
      Assert.AreEqual ("bigint", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "int64", false, false, NaInt32.Null)));
      Assert.AreEqual ("real", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "single", false, false, NaInt32.Null)));
      Assert.AreEqual ("nvarchar (100)", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "string", false, false, 100)));
      Assert.AreEqual ("image", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", "binary", false, false, NaInt32.Null)));
      Assert.AreEqual ("uniqueidentifier", CreateTableBuilder.GetSqlDataType (_orderItemClass.GetMandatoryPropertyDefinition ("Order")));
      Assert.AreEqual ("varchar (255)", CreateTableBuilder.GetSqlDataType (_customerClass.GetMandatoryPropertyDefinition ("PrimaryOfficial")));
    }

    [Test]
    public void GetSqlDataTypeWithDotNetType ()
    {
      string mappingTypeName = "Namespace.TypeName, AssemblyName";
      Assert.AreEqual ("int", CreateTableBuilder.GetSqlDataType (new PropertyDefinition ("Name", "ColumnName", mappingTypeName, false, false, NaInt32.Null)));
    }

    [Test]
    public void GetColumns ()
    {
      PropertyDefinition nameProperty = _ceoClass.GetMandatoryPropertyDefinition ("Name");
      string statement = _createTableBuilder.GetColumn (nameProperty);

      Assert.AreEqual ("  [Name] nvarchar (100) NOT NULL,\n", statement);
    }

    [Test]
    public void GetColumnsForNullableProperty ()
    {

      PropertyDefinition phoneNumberProperty = _companyClass.GetMandatoryPropertyDefinition ("PhoneNumber");

      string statement = _createTableBuilder.GetColumn (phoneNumberProperty);

      Assert.AreEqual ("  [PhoneNumber] nvarchar (100) NULL,\n", statement);
    }

    [Test]
    public void GetColumnsWithForceNullable ()
    {
      PropertyDefinition nameProperty = _ceoClass.GetMandatoryPropertyDefinition ("Name");
      string statement = _createTableBuilder.GetColumn (nameProperty, true);

      Assert.AreEqual ("  [Name] nvarchar (100) NULL,\n", statement);
    }

    [Test]
    public void GetColumnsWithMandatoryRelationProperty ()
    {
      PropertyDefinition orderProperty = _orderItemClass.GetMandatoryPropertyDefinition ("Order");
      string statement = _createTableBuilder.GetColumn (orderProperty);

      Assert.AreEqual ("  [OrderID] uniqueidentifier NULL,\n", statement);
    }

    [Test]
    public void GetColumnsWithMandatoryRelationPropertyAndDerivation ()
    {
      ClassDefinition orderClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Order");
      PropertyDefinition customerProperty = orderClass.GetMandatoryPropertyDefinition ("Customer");
      string actualStatement = _createTableBuilder.GetColumn (customerProperty);

      string expectedStatement = "  [CustomerID] uniqueidentifier NULL,\n"
          + "  [CustomerIDClassID] varchar (100) NULL,\n";

      Assert.AreEqual (expectedStatement, actualStatement);
    }

    [Test]
    public void GetColumnsWithRelationToDifferentStorageProvider ()
    {
      PropertyDefinition primaryOfficialProperty = _customerClass.GetMandatoryPropertyDefinition ("PrimaryOfficial");
      string actualStatement = _createTableBuilder.GetColumn (primaryOfficialProperty, false);

      string expectedStatement = "  [PrimaryOfficialID] varchar (255) NULL,\n";

      Assert.AreEqual (expectedStatement, actualStatement);
    }


    [Test]
    public void GetCreateTableStatement ()
    {
      string actualStatement = _createTableBuilder.GetCreateTableStatement (_ceoClass);

      string expectedStatement = "CREATE TABLE [Ceo]\n"
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

      Assert.AreEqual (expectedStatement, actualStatement);
    }

    [Test]
    public void GetCreateTableStatementForConcreteClass ()
    {
      string actualStatement = _createTableBuilder.GetCreateTableStatement (_customerClass);

      string expectedStatement = "CREATE TABLE [Customer]\n"
          + "(\n"
          + "  [ID] uniqueidentifier NOT NULL,\n"
          + "  [ClassID] varchar (100) NOT NULL,\n"
          + "  [Timestamp] rowversion NOT NULL,\n\n"
          + "  -- Company columns\n"
          + "  [Name] nvarchar (100) NOT NULL,\n"
          + "  [PhoneNumber] nvarchar (100) NULL,\n\n"
          + "  -- Customer columns\n"
          + "  [CustomerType] int NOT NULL,\n"
          + "  [PrimaryOfficialID] varchar (255) NULL,\n\n"
          + "  CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID])\n"
          + ")\n";

      Assert.AreEqual (expectedStatement, actualStatement);
    }

    [Test]
    public void GetCreateTableStatementForWithTwoAbstractClasses ()
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

      string actualStatement = _createTableBuilder.GetCreateTableStatement (derivedConcreteClass);

      string expectedStatement = "CREATE TABLE [EntityName]\n"
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

      Assert.AreEqual (expectedStatement, actualStatement);
    }

    [Test]
    public void GetCreateTableStatementWithDerivedClasses ()
    {
      ClassDefinition baseClass = new ClassDefinition ("BaseClass", "EntityName", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);
      baseClass.MyPropertyDefinitions.Add (new PropertyDefinition ("PropertyInBaseClass", "PropertyInBaseClass", "string", false, false, 100));

      ClassDefinition firstDerivedClass = new ClassDefinition (
          "FirstDerivedClass", null, "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false, baseClass);

      firstDerivedClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("PropertyInFirstDerivedClass", "PropertyInFirstDerivedClass", "string", false, false, 101));

      ClassDefinition derivedOfDerivedClass = new ClassDefinition (
          "DerivedOfDerivedClass", null, "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false, firstDerivedClass);

      derivedOfDerivedClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("PropertyInDerivedOfDerivedClass", "PropertyInDerivedOfDerivedClass", "string", false, true, 102));

      ClassDefinition secondDerivedClass = new ClassDefinition (
          "SecondDerivedClass", null, "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false, baseClass);

      secondDerivedClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("PropertyInSecondDerivedClass", "PropertyInSecondDerivedClass", "string", false, false, 103));

      string actualStatement = _createTableBuilder.GetCreateTableStatement (baseClass);

      string expectedStatement = "CREATE TABLE [EntityName]\n"
          + "(\n"
          + "  [ID] uniqueidentifier NOT NULL,\n"
          + "  [ClassID] varchar (100) NOT NULL,\n"
          + "  [Timestamp] rowversion NOT NULL,\n\n"
          + "  -- BaseClass columns\n"
          + "  [PropertyInBaseClass] nvarchar (100) NOT NULL,\n\n"
          + "  -- FirstDerivedClass columns\n"
          + "  [PropertyInFirstDerivedClass] nvarchar (101) NULL,\n\n"
          + "  -- DerivedOfDerivedClass columns\n"
          + "  [PropertyInDerivedOfDerivedClass] nvarchar (102) NULL,\n\n"
          + "  -- SecondDerivedClass columns\n"
          + "  [PropertyInSecondDerivedClass] nvarchar (103) NULL,\n\n"
          + "  CONSTRAINT [PK_EntityName] PRIMARY KEY CLUSTERED ([ID])\n"
          + ")\n";

      Assert.AreEqual (expectedStatement, actualStatement);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The ClassDefinition must have an entity name.\r\nParameter name: classDefinition")]
    public void GetCreateTableStatementWithAbstractClass ()
    {
      _createTableBuilder.GetCreateTableStatement (_companyClass);
    }
  }
}
