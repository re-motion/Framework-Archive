using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.RdbmsTools.SchemaGeneration.SqlServer;
using Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.SchemaGeneration.SqlServer
{
  [TestFixture]
  public class ConstraintBuilderTest : StandardMappingTest
  {
    private ConstraintBuilder _constraintBuilder;

    public override void SetUp ()
    {
      base.SetUp();

      _constraintBuilder = new ConstraintBuilder();
    }

    [Test]
    public void AddConstraintWithRelationToSameStorageProvider ()
    {
      _constraintBuilder.AddConstraint (OrderItemClass);

      string expectedScript =
          "ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithRelationToOtherStorageProvider ()
    {
      _constraintBuilder.AddConstraint (OrderClass);

      string expectedScript =
          "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintMultipleTimes ()
    {
      _constraintBuilder.AddConstraint (OrderItemClass);
      _constraintBuilder.AddConstraint (OrderClass);

      string expectedScript =
          "ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n\r\n"
          + "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithTwoConstraints ()
    {
      ReflectionBasedClassDefinition firstClass = new ReflectionBasedClassDefinition (
          "FirstClass", "FirstEntity", "FirstStorageProvider", typeof (Company), false);

      firstClass.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (firstClass, "SecondClass", "SecondClassID", typeof (ObjectID), true, null, true));

      firstClass.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (firstClass, "ThirdClass", "ThirdClassID", typeof (ObjectID), true, null, true));

      ReflectionBasedClassDefinition secondClass = new ReflectionBasedClassDefinition (
          "SecondClass", "SecondEntity", "FirstStorageProvider", typeof (Address), false);

      ReflectionBasedClassDefinition thirdClass = new ReflectionBasedClassDefinition (
          "ThirdClass", "ThirdEntity", "FirstStorageProvider", typeof (Employee), false);

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

      string expectedScript =
          "ALTER TABLE [dbo].[FirstEntity] ADD\r\n"
          + "  CONSTRAINT [FK_SecondClassID] FOREIGN KEY ([SecondClassID]) REFERENCES [dbo].[SecondEntity] ([ID]),\r\n"
          + "  CONSTRAINT [FK_ThirdClassID] FOREIGN KEY ([ThirdClassID]) REFERENCES [dbo].[ThirdEntity] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithNoConstraintNecessary ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions.GetMandatory ("Official"));
      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithRelationInDerivedClass ()
    {
      ReflectionBasedClassDefinition baseClass = new ReflectionBasedClassDefinition (
          "BaseClass", "BaseClassEntity", "FirstStorageProvider", typeof (Company), false);

      ReflectionBasedClassDefinition derivedClass = new ReflectionBasedClassDefinition (
          "DerivedClass", "BaseClassEntity", "FirstStorageProvider", typeof (Customer), false, baseClass);

      derivedClass.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (derivedClass, "OtherClass", "OtherClassID", typeof (ObjectID), true, null, true));

      ReflectionBasedClassDefinition otherClass = new ReflectionBasedClassDefinition (
          "OtherClass", "OtherClassEntity", "FirstStorageProvider", typeof (DevelopmentPartner), false);

      RelationDefinition relationDefinition1 = new RelationDefinition (
          "OtherClassToDerivedClass",
          new RelationEndPointDefinition (derivedClass, "OtherClass", false),
          new VirtualRelationEndPointDefinition (otherClass, "DerivedClass", false, CardinalityType.Many, typeof (DomainObjectCollection)));

      derivedClass.MyRelationDefinitions.Add (relationDefinition1);
      otherClass.MyRelationDefinitions.Add (relationDefinition1);

      _constraintBuilder.AddConstraint (baseClass);

      string expectedScript =
          "ALTER TABLE [dbo].[BaseClassEntity] ADD\r\n"
          + "  CONSTRAINT [FK_OtherClassID] FOREIGN KEY ([OtherClassID]) REFERENCES [dbo].[OtherClassEntity] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithRelationToDerivedOfConcreteClass ()
    {
      _constraintBuilder.AddConstraint (ClassWithRelations);

      string expectedScript =
          "ALTER TABLE [dbo].[ClassWithRelations] ADD\r\n"
          + "  CONSTRAINT [FK_DerivedClassID] FOREIGN KEY ([DerivedClassID]) REFERENCES [dbo].[ConcreteClass] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithRelationToAbstractClass ()
    {
      _constraintBuilder.AddConstraint (CeoClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithAbstractClass ()
    {
      _constraintBuilder.AddConstraint (CompanyClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void AddConstraintWithDerivedClassWithEntityName ()
    {
      _constraintBuilder.AddConstraint (SecondDerivedClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void AddConstraintWithDerivedOfDerivedClassWithEntityName ()
    {
      _constraintBuilder.AddConstraint (DerivedOfDerivedClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void AddConstraints ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (OrderItemClass);
      classes.Add (OrderClass);

      _constraintBuilder.AddConstraints (classes);

      string expectedScript =
          "ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n\r\n"
          + "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void GetDropConstraintsScript ()
    {
      _constraintBuilder.AddConstraint (ClassWithRelations);

      string expectedScript =
          "DECLARE @statement nvarchar (4000)\r\n"
          + "SET @statement = ''\r\n"
          + "SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \r\n"
          + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id \r\n"
          + "    WHERE fk.xtype = 'F' AND t.name IN ('ClassWithRelations')\r\n"
          + "    ORDER BY t.name, fk.name\r\n"
          + "exec sp_executesql @statement\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void GetDropConstraintsScriptWithMultipleEntities ()
    {
      _constraintBuilder.AddConstraint (ClassWithRelations);
      _constraintBuilder.AddConstraint (ConcreteClass);

      string expectedScript =
          "DECLARE @statement nvarchar (4000)\r\n"
          + "SET @statement = ''\r\n"
          + "SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \r\n"
          + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id \r\n"
          + "    WHERE fk.xtype = 'F' AND t.name IN ('ClassWithRelations', 'ConcreteClass')\r\n"
          + "    ORDER BY t.name, fk.name\r\n"
          + "exec sp_executesql @statement\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropConstraintScript());
    }
  }
}