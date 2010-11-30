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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration.SqlServer;
using Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.SchemaGeneration.SqlServer
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
          + "  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithRelationToOtherStorageProvider ()
    {
      _constraintBuilder.AddConstraint (OrderClass);

      string expectedScript =
          "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_Order_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintMultipleTimes ()
    {
      _constraintBuilder.AddConstraint (OrderItemClass);
      _constraintBuilder.AddConstraint (OrderClass);

      string expectedScript =
          "ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n\r\n"
          + "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_Order_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithTwoConstraints ()
    {
      var storageProviderDefinition = new RdbmsProviderDefinition ("DefaultStorageProvider", typeof (SqlStorageObjectFactory), "dummy");
      var firstClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Company));
      var properties = new List<PropertyDefinition>();
      properties.Add (CreatePropertyDefinition(firstClass, "SecondClass", "SecondClassID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (CreatePropertyDefinition (firstClass, "ThirdClass", "ThirdClassID", typeof (ObjectID), true, null, StorageClass.Persistent));
      firstClass.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      var secondClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Address));
      var thirdClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Employee));
      
      RelationDefinition relationDefinition1 = new RelationDefinition (
          "FirstClassToSecondClass",
          new RelationEndPointDefinition (firstClass, "SecondClass", false),
          new ReflectionBasedVirtualRelationEndPointDefinition (secondClass, "FirstClass", false, CardinalityType.Many,typeof (DomainObjectCollection),"sort", typeof(Employee).GetProperty("Name")));
      
      RelationDefinition relationDefinition2 = new RelationDefinition (
          "FirstClassToThirdClass",
          new RelationEndPointDefinition (firstClass, "ThirdClass", false),
          new ReflectionBasedVirtualRelationEndPointDefinition (thirdClass, "FirstClass", false, CardinalityType.Many, typeof (DomainObjectCollection), "sort", typeof (Employee).GetProperty ("Name")));

      firstClass.SetRelationDefinitions (new RelationDefinitionCollection (new[] { relationDefinition1, relationDefinition2 }, true));
      secondClass.SetRelationDefinitions (new RelationDefinitionCollection (new[] { relationDefinition1 }, true));
      thirdClass.SetRelationDefinitions (new RelationDefinitionCollection (new[] { relationDefinition2 }, true));

      firstClass.SetStorageEntity (new TableDefinition (storageProviderDefinition, "FirstEntity", "FirstEntityView", new ColumnDefinition[0]));
      secondClass.SetStorageEntity (new TableDefinition (storageProviderDefinition, "SecondEntity", "SecondEntityView", new ColumnDefinition[0]));
      thirdClass.SetStorageEntity (new TableDefinition (storageProviderDefinition, "ThirdEntity", "ThirdEntityView", new ColumnDefinition[0]));

      _constraintBuilder.AddConstraint (firstClass);

      string expectedScript =
          "ALTER TABLE [dbo].[FirstEntity] ADD\r\n"
          + "  CONSTRAINT [FK_FirstEntity_SecondClassID] FOREIGN KEY ([SecondClassID]) REFERENCES [dbo].[SecondEntity] ([ID]),\r\n"
          + "  CONSTRAINT [FK_FirstEntity_ThirdClassID] FOREIGN KEY ([ThirdClassID]) REFERENCES [dbo].[ThirdEntity] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    private PropertyDefinition CreatePropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName,
        Type propertyType, bool? isNullable, int? maxLength, StorageClass storageClass)
    {
      PropertyInfo dummyPropertyInfo = typeof (Order).GetProperty ("Number");
      var propertyDefinition = new ReflectionBasedPropertyDefinition (
          classDefinition,
          dummyPropertyInfo,
          propertyName,
          propertyType,
          isNullable,
          maxLength,
          storageClass);
      propertyDefinition.SetStorageProperty (new ColumnDefinition (columnName, propertyType, "dummyStorageType", isNullable.HasValue?isNullable.Value:true));
      return propertyDefinition;
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
      var storageProviderDefinition = new RdbmsProviderDefinition ("DefaultStorageProvider", typeof (SqlStorageObjectFactory), "dummy");
      var baseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Company));
      baseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
      var derivedClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Customer), baseClass);
      derivedClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] {CreatePropertyDefinition (derivedClass, "OtherClass", "OtherClassID", typeof (ObjectID), true, null, StorageClass.Persistent)}, true));

      ReflectionBasedClassDefinition otherClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (DevelopmentPartner));
      RelationDefinition relationDefinition1 = new RelationDefinition (
          "OtherClassToDerivedClass",
          new RelationEndPointDefinition (derivedClass, "OtherClass", false),
          new ReflectionBasedVirtualRelationEndPointDefinition (otherClass, "DerivedClass", false, CardinalityType.Many, typeof (DomainObjectCollection), "sort", typeof (Employee).GetProperty ("Name")));

      baseClass.SetRelationDefinitions (new RelationDefinitionCollection (new RelationDefinition[0], true));
      derivedClass.SetRelationDefinitions (new RelationDefinitionCollection (new[]{ relationDefinition1}, true));
      otherClass.SetRelationDefinitions (new RelationDefinitionCollection (new[]{ relationDefinition1}, true));

      baseClass.SetStorageEntity (new TableDefinition (storageProviderDefinition, "BaseClassEntity", "BaseClassEntityView", new ColumnDefinition[0]));
      derivedClass.SetStorageEntity (new TableDefinition (storageProviderDefinition, "DerivedClassEntity", "DerivedClassEntityView", new ColumnDefinition[0]));
      otherClass.SetStorageEntity (new TableDefinition (storageProviderDefinition, "OtherClassEntity", "OtherClassEntityView", new ColumnDefinition[0]));

      _constraintBuilder.AddConstraint (baseClass);

      string expectedScript =
          "ALTER TABLE [dbo].[BaseClassEntity] ADD\r\n"
          + "  CONSTRAINT [FK_BaseClassEntity_OtherClassID] FOREIGN KEY ([OtherClassID]) REFERENCES [dbo].[OtherClassEntity] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithRelationToDerivedOfConcreteClass ()
    {
      _constraintBuilder.AddConstraint (ClassWithRelations);

      string expectedScript =
          "ALTER TABLE [dbo].[TableWithRelations] ADD\r\n"
          + "  CONSTRAINT [FK_TableWithRelations_DerivedClassID] FOREIGN KEY ([DerivedClassID]) REFERENCES [dbo].[ConcreteClass] ([ID])\r\n";

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
          + "  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n\r\n"
          + "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_Order_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraints_WithoutClassDefinitions ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);

      _constraintBuilder.AddConstraints (classes);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript ());
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
          + "    WHERE fk.xtype = 'F' AND t.name IN ('TableWithRelations')\r\n"
          + "    ORDER BY t.name, fk.name\r\n"
          + "exec sp_executesql @statement\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void GetDropConstraintsScript_WithoutClasses ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);

      _constraintBuilder.AddConstraints (classes);

      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript ());
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
          + "    WHERE fk.xtype = 'F' AND t.name IN ('TableWithRelations', 'ConcreteClass')\r\n"
          + "    ORDER BY t.name, fk.name\r\n"
          + "exec sp_executesql @statement\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropConstraintScript());
    }
  }
}
