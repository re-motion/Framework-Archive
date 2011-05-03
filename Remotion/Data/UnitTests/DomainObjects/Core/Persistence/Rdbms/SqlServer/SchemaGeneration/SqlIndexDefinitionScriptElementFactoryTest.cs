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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlIndexDefinitionScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private SqlIndexDefinitionScriptElementFactory _factory;
    private SqlIndexDefinition _indexDefinitionWithDefaultSchema;
    private SqlIndexDefinition _indexDefinitionWithCustomSchema;
    private SqlIndexedColumnDefinition _column1;
    private SqlIndexedColumnDefinition _column2;
    private SimpleColumnDefinition _includedColumn1;
    private SimpleColumnDefinition _includedColumn2;

    public override void SetUp ()
    {
      base.SetUp();

      _factory = new SqlIndexDefinitionScriptElementFactory();

      _column1 = new SqlIndexedColumnDefinition (
          new SimpleColumnDefinition ("IndexColumn1", typeof (int), "integer", false, false), IndexOrder.Desc);
      _column2 = new SqlIndexedColumnDefinition (
          new SimpleColumnDefinition ("IndexColumn2", typeof (string), "varchar", false, false), IndexOrder.Asc);
      _includedColumn1 = new SimpleColumnDefinition ("IncludedColumn1", typeof (bool), "bit", true, false);
      _includedColumn2 = new SimpleColumnDefinition ("IncludedColumn2", typeof (bool), "bit", true, false);
      
      _indexDefinitionWithCustomSchema = new SqlIndexDefinition (
          "Index1", new EntityNameDefinition ("SchemaName", "TableName1"), new[] { _column1 });
      _indexDefinitionWithDefaultSchema = new SqlIndexDefinition (
          "Index2", new EntityNameDefinition (null, "TableName2"), new[] { _column2 });
    }

    [Test]
    public void GetCreateElement_CustomSchema ()
    {
      var result = _factory.GetCreateElement (_indexDefinitionWithCustomSchema);

      var expectedResult =
          "CREATE NONCLUSTERED INDEX [Index1]\r\n"
          + "  ON [SchemaName].[TableName1] ([IndexColumn1] DESC)";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_DefaultSchema ()
    {
      var result = _factory.GetCreateElement (_indexDefinitionWithDefaultSchema);

      var expectedResult =
          "CREATE NONCLUSTERED INDEX [Index2]\r\n"
          + "  ON [dbo].[TableName2] ([IndexColumn2] ASC)";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_WithIncludedColumnsAndSomeOptions ()
    {
      var indexDefinition = new SqlIndexDefinition (
          "Index1", new EntityNameDefinition (null, "TableName"), new[] { _column1, _column2 }, new[] { _includedColumn1 }, false, false, true, true);

      var result = _factory.GetCreateElement (indexDefinition);

      var expectedResult =
          "CREATE NONCLUSTERED INDEX [Index1]\r\n"
          + "  ON [dbo].[TableName] ([IndexColumn1] DESC, [IndexColumn2] ASC)\r\n"
          + "  INCLUDE ([IncludedColumn1])\r\n"
          + "  WITH (IGNORE_DUP_KEY = ON, ONLINE = ON)";
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_WithAllOptionsOn ()
    {
      var indexDefinition = new SqlIndexDefinition (
          "Index1",
          new EntityNameDefinition(null, "TableName"),
          new[] { _column1, _column2 },
          new[] { _includedColumn1, _includedColumn2 },
          true,
          true,
          true,
          true,
          true,
          5,
          true,
          true,
          true,
          true,
          true,
          2);

      var result = _factory.GetCreateElement (indexDefinition);

      var expectedResult =
          "CREATE UNIQUE CLUSTERED INDEX [Index1]\r\n"
          + "  ON [dbo].[TableName] ([IndexColumn1] DESC, [IndexColumn2] ASC)\r\n"
          + "  INCLUDE ([IncludedColumn1], [IncludedColumn2])\r\n"
          + "  WITH (IGNORE_DUP_KEY = ON, ONLINE = ON, PAD_INDEX = ON, FILLFACTOR = 5, SORT_IN_TEMPDB = ON, STATISTICS_NORECOMPUTE = ON, "
          + "DROP_EXISTING = ON, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, MAXDOP = 2)";
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_WithAllOptionsOff ()
    {
      var indexDefinition = new SqlIndexDefinition (
          "Index1",
          new EntityNameDefinition(null, "TableName"),
          new[] { _column1, _column2 },
          new[] { _includedColumn1 },
          false,
          false,
          false,
          false,
          false,
          0,
          false,
          false,
          false,
          false,
          false,
          0);

      var result = _factory.GetCreateElement (indexDefinition);

      var expectedResult =
          "CREATE NONCLUSTERED INDEX [Index1]\r\n"
          + "  ON [dbo].[TableName] ([IndexColumn1] DESC, [IndexColumn2] ASC)\r\n"
          + "  INCLUDE ([IncludedColumn1])\r\n"
          + "  WITH (IGNORE_DUP_KEY = OFF, ONLINE = OFF, PAD_INDEX = OFF, FILLFACTOR = 0, SORT_IN_TEMPDB = OFF, STATISTICS_NORECOMPUTE = OFF, "
          + "DROP_EXISTING = OFF, ALLOW_ROW_LOCKS = OFF, ALLOW_PAGE_LOCKS = OFF, MAXDOP = 0)";
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_CustomSchema ()
    {
      var result = _factory.GetDropElement (_indexDefinitionWithCustomSchema);

      var expectedResult =
          "IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'TableName1' AND "
          + "schema_name (so.schema_id)='SchemaName' AND si.[name] = 'Index1')\r\n"
          + "  DROP INDEX [Index1] ON [SchemaName].[TableName1]";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_DefaultSchema ()
    {
      var result = _factory.GetDropElement (_indexDefinitionWithDefaultSchema);

      var expectedResult =
          "IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'TableName2' AND "
          + "schema_name (so.schema_id)='dbo' AND si.[name] = 'Index2')\r\n"
          + "  DROP INDEX [Index2] ON [dbo].[TableName2]";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }
  }
}