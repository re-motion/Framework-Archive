using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.CodeGenerator.Sql;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.Sql
{
  [TestFixture]
  public class SqlFileBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private SqlFileBuilder _sqlFileBuilder;
    private ClassDefinition _ceoClass;
    private ClassDefinition _customerClass;
    private ClassDefinition _orderClass;
    private ClassDefinition _orderItemClass;


    // construction and disposing

    public SqlFileBuilderTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();
      _sqlFileBuilder = new SqlFileBuilder (MappingConfiguration, StorageProviderConfiguration, "FirstStorageProvider");
      _ceoClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Ceo");
      _customerClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Customer");
      _orderClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Order");
      _orderItemClass = MappingConfiguration.ClassDefinitions.GetMandatory ("OrderItem");
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (MappingConfiguration, _sqlFileBuilder.MappingConfiguration);
      Assert.AreSame (StorageProviderConfiguration, _sqlFileBuilder.StorageProviderConfiguration);
      Assert.AreEqual ("FirstStorageProvider", _sqlFileBuilder.StorageProviderID);
    }

    [Test]
    public void TableRootClasses ()
    {
      ClassDefinitionCollection tableRootClasses = _sqlFileBuilder.TableRootClasses;

      Assert.IsNotNull (tableRootClasses);
      Assert.AreEqual (7, tableRootClasses.Count);
      Assert.IsTrue (tableRootClasses.Contains (_ceoClass));
      Assert.IsTrue (tableRootClasses.Contains (_customerClass));
      Assert.IsTrue (tableRootClasses.Contains (MappingConfiguration.ClassDefinitions.GetMandatory ("DevelopmentPartner")));
      Assert.IsTrue (tableRootClasses.Contains (_orderClass));
      Assert.IsTrue (tableRootClasses.Contains (_orderItemClass));
      Assert.IsTrue (tableRootClasses.Contains (MappingConfiguration.ClassDefinitions.GetMandatory ("ClassWithAllDataTypes")));
      Assert.IsTrue (tableRootClasses.Contains (MappingConfiguration.ClassDefinitions.GetMandatory ("Employee")));
    }

    [Test]
    public void GetEntityNames ()
    {
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection (false);
      classDefinitions.Add (MappingConfiguration.ClassDefinitions.GetMandatory ("Ceo"));
      classDefinitions.Add (MappingConfiguration.ClassDefinitions.GetMandatory ("Customer"));

      List<string> entityNames = _sqlFileBuilder.GetEntityNames (classDefinitions);

      Assert.AreEqual (2, entityNames.Count);
      Assert.Contains ("Ceo", entityNames);
      Assert.Contains ("Customer", entityNames);
    }

    [Test]
    public void GetEntityNamesWithAbstractClasses ()
    {
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection (false);
      classDefinitions.Add (_ceoClass);
      classDefinitions.Add (_customerClass);
      classDefinitions.Add (MappingConfiguration.ClassDefinitions.GetMandatory ("Company"));

      List<string> entityNames = _sqlFileBuilder.GetEntityNames (classDefinitions);

      Assert.AreEqual (2, entityNames.Count);
      Assert.Contains ("Ceo", entityNames);
      Assert.Contains ("Customer", entityNames);
    }

    [Test]
    public void GetEntityNamesWithDuplicateEntities ()
    {
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection (false);
      classDefinitions.Add (_ceoClass);
      classDefinitions.Add (_customerClass);
      classDefinitions.Add (new ClassDefinition ("ClassWithCustomerEntity", "Customer", "FirstStorageProvider", "Type, Assembly", false));

      List<string> entityNames = _sqlFileBuilder.GetEntityNames (classDefinitions);

      Assert.AreEqual (2, entityNames.Count);
      Assert.Contains ("Ceo", entityNames);
      Assert.Contains ("Customer", entityNames);
    }

    [Test]
    public void GetDropForeignKeysScript ()
    {
      string actualScript = _sqlFileBuilder.GetDropForeignKeyScript ();

      string[] entityNames = _sqlFileBuilder.GetEntityNames (_sqlFileBuilder.TableRootClasses).ToArray ();

      string expectedScript = "DECLARE @statement nvarchar (4000)\n"
          + "SET @statement = ''\n"
          + "SELECT @statement = @statement + 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \n"
          + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id \n"
          + "    WHERE fk.xtype = 'F' AND t.name IN ('" + string.Join ("', '", entityNames) + "')\n"
          + "    ORDER BY t.name, fk.name\n"
          + "exec sp_executesql @statement\n";

      Assert.AreEqual (expectedScript, actualScript);
    }

    [Test]
    public void GetDatabaseName ()
    {
      Assert.AreEqual ("CodeGeneratorUnitTests1", _sqlFileBuilder.GetDatabaseName ());
    }

    [Test]
    public void GetDatabaseNameWithNonRdmbsProvider ()
    {
      SqlFileBuilder sqlFileBuilder = new SqlFileBuilder (MappingConfiguration, StorageProviderConfiguration, "NonRdbmsStorageProvider");
      Assert.IsNull (sqlFileBuilder.GetDatabaseName ());
    }

    [Test]
    public void GetAddConstraintScriptWithClassDefinition ()
    {
      string actualScript = _sqlFileBuilder.GetAddConstraintScript (_orderItemClass);

      string expectedScript = "ALTER TABLE [OrderItem] ADD\n"
          + "  CONSTRAINT [FK_OrderToOrderItem] FOREIGN KEY ([OrderID]) REFERENCES [Order] ([ID])\n";

      Assert.AreEqual (expectedScript, actualScript);
    }

    [Test]
    public void GetAddConstraintScriptWithRelationToOtherStorageProvider ()
    {
      string actualScript = _sqlFileBuilder.GetAddConstraintScript (_orderClass);

      string expectedScript = "ALTER TABLE [Order] ADD\n"
          + "  CONSTRAINT [FK_CustomerToOrder] FOREIGN KEY ([CustomerID]) REFERENCES [Customer] ([ID])\n";

      Assert.AreEqual (expectedScript, actualScript);
    }

    [Test]
    public void GetAddConstraintScriptWithTwoConstraints ()
    {
      ClassDefinition firstClass = new ClassDefinition (
          "FirstClass", "FirstEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      firstClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("SecondClass", "SecondClassID", TypeInfo.ObjectIDMappingTypeName, false, true, NaInt32.Null));

      firstClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("ThirdClass", "ThirdClassID", TypeInfo.ObjectIDMappingTypeName, false, true, NaInt32.Null));

      ClassDefinition secondClass = new ClassDefinition (
          "SecondClass", "SecondEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      ClassDefinition thirdClass = new ClassDefinition (
          "ThirdClass", "ThirdEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      RelationDefinition relationDefinition1 = new RelationDefinition (
          "FirstClassToSecondClass",
          new RelationEndPointDefinition (firstClass, "SecondClass", false),
          new VirtualRelationEndPointDefinition (secondClass, "FirstClass", false, CardinalityType.Many, typeof (DomainObjectCollection)));
      firstClass.MyRelationDefinitions.Add (relationDefinition1);
      secondClass.MyRelationDefinitions.Add (relationDefinition1);

      RelationDefinition relationDefinition2 = new RelationDefinition (
          "FirstClassToThirdClass",
          new RelationEndPointDefinition (firstClass, "ThirdClass", false),
          new VirtualRelationEndPointDefinition (thirdClass, "FirstClass", false, CardinalityType.Many, typeof (DomainObjectCollection)));
      firstClass.MyRelationDefinitions.Add (relationDefinition2);
      thirdClass.MyRelationDefinitions.Add (relationDefinition2);

      string actualScript = _sqlFileBuilder.GetAddConstraintScript (firstClass);

      string expectedScript = "ALTER TABLE [FirstEntity] ADD\n"
          + "  CONSTRAINT [FK_FirstClassToSecondClass] FOREIGN KEY ([SecondClassID]) REFERENCES [SecondEntity] ([ID]),\n"
          + "  CONSTRAINT [FK_FirstClassToThirdClass] FOREIGN KEY ([ThirdClassID]) REFERENCES [ThirdEntity] ([ID])\n";

      Assert.AreEqual (expectedScript, actualScript);
    }

    [Test]
    public void GetAddConstraintScriptWithNoForeignKeyPossible ()
    {
      Assert.IsEmpty (_sqlFileBuilder.GetAddConstraintScript (_customerClass));
    }

    [Test]
    public void GetAddConstraintScriptWithRelationInDerivedClass ()
    {
      ClassDefinition baseClass = new ClassDefinition (
          "BaseClass", "BaseClassEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      ClassDefinition derivedClass = new ClassDefinition (
          "DerivedClass", "BaseClassEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false, baseClass);

      derivedClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("OtherClass", "OtherClassID", TypeInfo.ObjectIDMappingTypeName, false, true, NaInt32.Null));

      ClassDefinition otherClass = new ClassDefinition (
          "OtherClass", "OtherClassEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      RelationDefinition relationDefinition1 = new RelationDefinition (
          "OtherClassToDerivedClass",
          new RelationEndPointDefinition (derivedClass, "OtherClass", false),
          new VirtualRelationEndPointDefinition (otherClass, "DerivedClass", false, CardinalityType.Many, typeof (DomainObjectCollection)));

      derivedClass.MyRelationDefinitions.Add (relationDefinition1);
      otherClass.MyRelationDefinitions.Add (relationDefinition1);

      string actualScript = _sqlFileBuilder.GetAddConstraintScript (baseClass);

      string expectedScript = "ALTER TABLE [BaseClassEntity] ADD\n"
          + "  CONSTRAINT [FK_OtherClassToDerivedClass] FOREIGN KEY ([OtherClassID]) REFERENCES [OtherClassEntity] ([ID])\n";

      Assert.AreEqual (expectedScript, actualScript);
    }

    [Test]
    public void GetConstraintWithRelationToDifferentStorageProvider ()
    {
      IRelationEndPointDefinition endPointDefinition = _customerClass.GetMandatoryRelationEndPointDefinition ("PrimaryOfficial");
      Assert.AreEqual (string.Empty, _sqlFileBuilder.GetConstraint (endPointDefinition));
    }

    [Test]
    public void GetConstraintWithRelationToAbstractClass ()
    {
      IRelationEndPointDefinition endPointDefinition = _ceoClass.GetMandatoryRelationEndPointDefinition ("Company");
      Assert.AreEqual (string.Empty, _sqlFileBuilder.GetConstraint (endPointDefinition));
    }

    [Test]
    public void GetAddConstraintScript ()
    {
      string actualScript = _sqlFileBuilder.GetAddConstraintScript ();

      string expectedScript = "ALTER TABLE [Order] ADD\n"
          + "  CONSTRAINT [FK_CustomerToOrder] FOREIGN KEY ([CustomerID]) REFERENCES [Customer] ([ID])\n\n"
          + "ALTER TABLE [OrderItem] ADD\n"
          + "  CONSTRAINT [FK_OrderToOrderItem] FOREIGN KEY ([OrderID]) REFERENCES [Order] ([ID])\n\n"
          +"ALTER TABLE [Employee] ADD\n"
          + "  CONSTRAINT [FK_EmployeeToEmployee] FOREIGN KEY ([SupervisorID]) REFERENCES [Employee] ([ID])\n";

      Assert.AreEqual (expectedScript, actualScript);
    }

    [Test]
    public void GetConstraint ()
    {
      IRelationEndPointDefinition anonymousEndPointDefinition = _customerClass.GetOppositeEndPointDefinition ("PrimaryOfficial");

      Assert.IsTrue (anonymousEndPointDefinition.IsNull);
      Assert.IsEmpty (_sqlFileBuilder.GetConstraint (anonymousEndPointDefinition));
    }


    [Test]
    public void GetDropTableScriptWithSingleClass ()
    {
      SqlFileBuilder sqlFileBuilder = new SqlFileBuilder (MappingConfiguration, StorageProviderConfiguration, "SecondStorageProvider");
      string actualScript = sqlFileBuilder.GetDropTableScript ();

      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Official')\n"
          + "  DROP TABLE [Official]\n";

      Assert.AreEqual (expectedScript, actualScript);
    }

    [Test]
    public void GetDropTableScript ()
    {
      string actualScript = _sqlFileBuilder.GetDropTableScript ();

      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer')\n"
          + "  DROP TABLE [Customer]\n\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DevelopmentPartner')\n"
          + "  DROP TABLE [DevelopmentPartner]\n\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order')\n"
          + "  DROP TABLE [Order]\n\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderItem')\n"
          + "  DROP TABLE [OrderItem]\n\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Ceo')\n"
          + "  DROP TABLE [Ceo]\n\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithAllDataTypes')\n"
          + "  DROP TABLE [TableWithAllDataTypes]\n\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Employee')\n"
          + "  DROP TABLE [Employee]\n";
      ;

      Assert.AreEqual (expectedScript, actualScript);
    }

    [Test]
    public void GetScriptWithSingeTable ()
    {
      SqlFileBuilder sqlFileBuilder = new SqlFileBuilder (MappingConfiguration, StorageProviderConfiguration, "SecondStorageProvider");
      string expectedScript = File.ReadAllText (@"..\..\Database\SetupDB_SecondStorageProvider.sql").Replace ("\r\n", "\n");

      Assert.AreEqual (expectedScript, sqlFileBuilder.GetScript ());
    }

    [Test]
    public void GetScript ()
    {
      string expectedScript = File.ReadAllText (@"..\..\Database\SetupDB_FirstStorageProvider.sql").Replace ("\r\n", "\n");

      Assert.AreEqual (expectedScript, _sqlFileBuilder.GetScript ());
    }
  }
}
