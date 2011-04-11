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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class ConstraintBuilderTest : SchemaGenerationTestBase
  {
    private ConstraintBuilder _constraintBuilder;
    private SimpleColumnDefinition _referencedColumn1;
    private SimpleColumnDefinition _referencedColumn2;
    private SimpleColumnDefinition _referencingColumn;
    private ForeignKeyConstraintDefinition _foreignKeyConstraintDefinition1;
    private ForeignKeyConstraintDefinition _foreignKeyConstraintDefinition2;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _constraintBuilder = new ConstraintBuilder();

      _referencedColumn1 = new SimpleColumnDefinition ("OrderID", typeof (int), "integer", true, false);
      _referencedColumn2 = new SimpleColumnDefinition ("CustomerID", typeof (int), "integer", true, false);

      _referencingColumn = new SimpleColumnDefinition ("ID", typeof (int), "integer", false, true);
      _foreignKeyConstraintDefinition1 = new ForeignKeyConstraintDefinition (
          "FK_OrderItem_OrderID", "Order", new[] { _referencingColumn }, new[] { _referencedColumn1 });
      _foreignKeyConstraintDefinition2 = new ForeignKeyConstraintDefinition (
          "FK_OrderItem_CustomerID", "Customer", new[] { _referencingColumn }, new[] { _referencedColumn2 });
      _tableDefinition1 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition, "OrderItem", null, new[] { _referencingColumn }, new[] { _foreignKeyConstraintDefinition1 });
      _tableDefinition2 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          "Customer",
          null,
          new[] { _referencingColumn },
          new[] { _foreignKeyConstraintDefinition1, _foreignKeyConstraintDefinition2 });
    }

    [Test]
    public void GetAddConstraintScript_OneConstraint ()
    {
      _constraintBuilder.AddConstraint (_tableDefinition1);

      var expectedScript =
          "ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void GetAddConstraintScript_Constraints ()
    {
      _constraintBuilder.AddConstraint (_tableDefinition2);
  
      var expectedScript =
          "ALTER TABLE [dbo].[Customer] ADD\r\n"
          + "  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID]),\r\n"
          + "  CONSTRAINT [FK_OrderItem_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void GetAddConstraintScript_NoConstraint ()
    {
      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void GetAddConstraintScript_NoConstraintsAdded ()
    {
      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void GetDropConstraintScript ()
    {
      _constraintBuilder.AddConstraint (_tableDefinition1);

      string expectedScript =
          "DECLARE @statement nvarchar (max)\r\n"
          + "SET @statement = ''\r\n"
          + "SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \r\n"
          + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id \r\n"
          + "    WHERE fk.xtype = 'F' AND t.name IN ('OrderItem')\r\n"
          + "    ORDER BY t.name, fk.name\r\n"
          + "exec sp_executesql @statement\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void GetDropConstraintsScriptWithMultipleEntities ()
    {
      _constraintBuilder.AddConstraint (_tableDefinition1);
      _constraintBuilder.AddConstraint (_tableDefinition2);

      string expectedScript =
          "DECLARE @statement nvarchar (max)\r\n"
          + "SET @statement = ''\r\n"
          + "SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \r\n"
          + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id \r\n"
          + "    WHERE fk.xtype = 'F' AND t.name IN ('OrderItem', 'Customer')\r\n"
          + "    ORDER BY t.name, fk.name\r\n"
          + "exec sp_executesql @statement\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropConstraintScript());
    }
  }
}