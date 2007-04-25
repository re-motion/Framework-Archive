using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.CodeGenerator.Sql.SqlServer;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using System.Collections;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.Sql.SqlServer
{
  [TestFixture]
  public class ConstraintBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private ConstraintBuilder _constraintBuilder;

    // construction and disposing

    public ConstraintBuilderTest ()
    {
    }

    // methods and properties


    public override void SetUp ()
    {
      base.SetUp ();

      _constraintBuilder = new ConstraintBuilder ();
    }

    [Test]
    public void AddConstraintWithRelationToSameStorageProvider ()
    {
      _constraintBuilder.AddConstraint (OrderItemClass);

      string expectedScript = "ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderToOrderItem] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintWithRelationToOtherStorageProvider ()
    {
      _constraintBuilder.AddConstraint (OrderClass);

      string expectedScript = "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_CustomerToOrder] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintMultipleTimes ()
    {
      _constraintBuilder.AddConstraint (OrderItemClass);
      _constraintBuilder.AddConstraint (OrderClass);

      string expectedScript = "ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderToOrderItem] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n\r\n"
          + "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_CustomerToOrder] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());

    }

    [Test]
    public void AddConstraintWithTwoConstraints ()
    {
      XmlBasedClassDefinition firstClass = new XmlBasedClassDefinition (
          "FirstClass", "FirstEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      firstClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("SecondClass", "SecondClassID", TypeInfo.ObjectIDMappingTypeName, false, true, null));

      firstClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("ThirdClass", "ThirdClassID", TypeInfo.ObjectIDMappingTypeName, false, true, null));

      XmlBasedClassDefinition secondClass = new XmlBasedClassDefinition (
          "SecondClass", "SecondEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      XmlBasedClassDefinition thirdClass = new XmlBasedClassDefinition (
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

      _constraintBuilder.AddConstraint (firstClass);

      string expectedScript = "ALTER TABLE [dbo].[FirstEntity] ADD\r\n"
          + "  CONSTRAINT [FK_FirstClassToSecondClass] FOREIGN KEY ([SecondClassID]) REFERENCES [dbo].[SecondEntity] ([ID]),\r\n"
          + "  CONSTRAINT [FK_FirstClassToThirdClass] FOREIGN KEY ([ThirdClassID]) REFERENCES [dbo].[ThirdEntity] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintWithNoConstraintNecessary ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions.GetMandatory ("Official"));
      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintWithRelationInDerivedClass ()
    {
      XmlBasedClassDefinition baseClass = new XmlBasedClassDefinition (
          "BaseClass", "BaseClassEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      XmlBasedClassDefinition derivedClass = new XmlBasedClassDefinition (
          "DerivedClass", "BaseClassEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false, baseClass);

      derivedClass.MyPropertyDefinitions.Add (
          new PropertyDefinition ("OtherClass", "OtherClassID", TypeInfo.ObjectIDMappingTypeName, false, true, null));

      XmlBasedClassDefinition otherClass = new XmlBasedClassDefinition (
          "OtherClass", "OtherClassEntity", "FirstStorageProvider", "Namespace.TypeName, AssemblyName", false);

      RelationDefinition relationDefinition1 = new RelationDefinition (
          "OtherClassToDerivedClass",
          new RelationEndPointDefinition (derivedClass, "OtherClass", false),
          new VirtualRelationEndPointDefinition (otherClass, "DerivedClass", false, CardinalityType.Many, typeof (DomainObjectCollection)));

      derivedClass.MyRelationDefinitions.Add (relationDefinition1);
      otherClass.MyRelationDefinitions.Add (relationDefinition1);

      _constraintBuilder.AddConstraint (baseClass);

      string expectedScript = "ALTER TABLE [dbo].[BaseClassEntity] ADD\r\n"
          + "  CONSTRAINT [FK_OtherClassToDerivedClass] FOREIGN KEY ([OtherClassID]) REFERENCES [dbo].[OtherClassEntity] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintWithRelationToDerivedOfConcreteClass ()
    {
      _constraintBuilder.AddConstraint (ClassWithRelations);

      string expectedScript = "ALTER TABLE [dbo].[ClassWithRelations] ADD\r\n"
          + "  CONSTRAINT [FK_DerivedClassToClassWithRelations] FOREIGN KEY ([DerivedClassID]) REFERENCES [dbo].[ConcreteClass] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintWithRelationToAbstractClass ()
    {
      _constraintBuilder.AddConstraint (CeoClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void AddConstraintWithAbstractClass ()
    {
      _constraintBuilder.AddConstraint (CompanyClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript ());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript ());
    }

    [Test]
    public void AddConstraintWithDerivedClassWithEntityName ()
    {
      _constraintBuilder.AddConstraint (SecondDerivedClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript ());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript ());
    }

    [Test]
    public void AddConstraintWithDerivedOfDerivedClassWithEntityName ()
    {
      _constraintBuilder.AddConstraint (DerivedOfDerivedClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript ());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript ());
    }

    [Test]
    public void AddConstraints ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (OrderItemClass);
      classes.Add (OrderClass);

      _constraintBuilder.AddConstraints (classes);

      string expectedScript = "ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderToOrderItem] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n\r\n"
          + "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_CustomerToOrder] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void GetDropConstraintsScript ()
    {
      _constraintBuilder.AddConstraint (ClassWithRelations);

      string expectedScript = "DECLARE @statement nvarchar (4000)\r\n"
          + "SET @statement = ''\r\n"
          + "SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \r\n"
          + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id \r\n"
          + "    WHERE fk.xtype = 'F' AND t.name IN ('ClassWithRelations')\r\n"
          + "    ORDER BY t.name, fk.name\r\n"
          + "exec sp_executesql @statement\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropConstraintScript ());
    }

    [Test]
    public void GetDropConstraintsScriptWithMultipleEntities ()
    {
      _constraintBuilder.AddConstraint (ClassWithRelations);
      _constraintBuilder.AddConstraint (ConcreteClass);

      string expectedScript = "DECLARE @statement nvarchar (4000)\r\n"
          + "SET @statement = ''\r\n"
          + "SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \r\n"
          + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id \r\n"
          + "    WHERE fk.xtype = 'F' AND t.name IN ('ClassWithRelations', 'ConcreteClass')\r\n"
          + "    ORDER BY t.name, fk.name\r\n"
          + "exec sp_executesql @statement\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropConstraintScript ());
    }
  }
}
