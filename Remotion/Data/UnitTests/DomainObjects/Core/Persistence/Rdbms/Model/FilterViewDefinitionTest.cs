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
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class FilterViewDefinitionTest
  {
    private SimpleColumnDefinition _column1;
    private SimpleColumnDefinition _column2;
    private TableDefinition _entityDefinition;
    private FilterViewDefinition _filterViewDefinition;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private IIndexDefinition[] _indexes;
    private EntityNameDefinition[] _synonyms;
    private SimpleColumnDefinition _objectIDColunmn;
    private SimpleColumnDefinition _classIDCOlumn;
    private SimpleColumnDefinition _timestampColumn;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID");
      _objectIDColunmn = new SimpleColumnDefinition ("ObjectID", typeof (int), "integer", false, true);
      _classIDCOlumn = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false);
      _timestampColumn = new SimpleColumnDefinition ("Timestamp", typeof (DateTime), "datetime", true, false);
      _column1 = new SimpleColumnDefinition ("Column1", typeof (string), "varchar", true, false);
      _column2 = new SimpleColumnDefinition ("Column3", typeof (string), "varchar", true, false);
      _synonyms = new[] { new EntityNameDefinition (null, "Test") };

      _entityDefinition = TableDefinitionObjectMother.Create (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Table"),
          _objectIDColunmn,
          _classIDCOlumn,
          _timestampColumn);

      _indexes = new[] { MockRepository.GenerateStub<IIndexDefinition>() };
      _filterViewDefinition = new FilterViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          _entityDefinition,
          new[] { "ClassId1", "ClassId2" },
          _objectIDColunmn,
          _classIDCOlumn,
          _timestampColumn,
          new[] { _column1, _column2 },
          _indexes,
          _synonyms);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_filterViewDefinition.ClassIDs.Count, Is.EqualTo (2));
      Assert.That (_filterViewDefinition.ClassIDs[0], Is.EqualTo ("ClassId1"));
      Assert.That (_filterViewDefinition.ClassIDs[1], Is.EqualTo ("ClassId2"));
      Assert.That (_filterViewDefinition.BaseEntity, Is.SameAs (_entityDefinition));
      Assert.That (_filterViewDefinition.ViewName.EntityName, Is.EqualTo ("Test"));
      Assert.That (_filterViewDefinition.StorageProviderID, Is.EqualTo ("SPID"));
      Assert.That (_filterViewDefinition.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));
    }

    [Test]
    public void Initialization_WithBaseFilterViewEntity ()
    {
      new FilterViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          _filterViewDefinition,
          new[] { "x" },
          _objectIDColunmn,
          _classIDCOlumn,
          _timestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "The base entity must either be a TableDefinition or a FilterViewDefinition.\r\nParameter name: baseEntity")]
    public void Initialization_WithInvalidBaseEntity ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "TestUnion"),
          new IEntityDefinition[]
          {
              TableDefinitionObjectMother.Create (
                  _storageProviderDefinition, new EntityNameDefinition (null, "Test"), _objectIDColunmn, _classIDCOlumn, _timestampColumn)
          },
          _objectIDColunmn,
          _classIDCOlumn,
          _timestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      new FilterViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          unionViewDefinition,
          new[] { "x" },
          _objectIDColunmn,
          _classIDCOlumn,
          _timestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var filterViewDefinition = new FilterViewDefinition (
          _storageProviderDefinition,
          null,
          _entityDefinition,
          new[] { "ClassId" },
          _objectIDColunmn,
          _classIDCOlumn,
          _timestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      Assert.That (filterViewDefinition.ViewName, Is.Null);
    }

    [Test]
    public void LegacyEntityName ()
    {
      Assert.That (_filterViewDefinition.LegacyEntityName, Is.Null);
    }

    [Test]
    public void LegacyViewName ()
    {
      Assert.That (_filterViewDefinition.LegacyViewName, Is.EqualTo ("Test"));
    }

    [Test]
    public void ObjectIDColumn ()
    {
      Assert.That (_filterViewDefinition.ObjectIDColumn, Is.SameAs (_objectIDColunmn));
    }

    [Test]
    public void ClassIDColumn ()
    {
      Assert.That (_filterViewDefinition.ClassIDColumn, Is.SameAs (_classIDCOlumn));
    }

    [Test]
    public void TimestampColumn ()
    {
      Assert.That (_filterViewDefinition.TimestampColumn, Is.SameAs (_timestampColumn));
    }

    [Test]
    public void DataColumns ()
    {
      Assert.That (_filterViewDefinition.DataColumns, Is.EqualTo (new[] { _column1, _column2 }));
    }

    [Test]
    public void GetAllColumns ()
    {
      var result = _filterViewDefinition.GetAllColumns();

      Assert.That (result, Is.EqualTo (new[] { _objectIDColunmn, _classIDCOlumn, _timestampColumn, _column1, _column2 }));
    }

    [Test]
    public void Indexes ()
    {
      var result = _filterViewDefinition.Indexes;

      Assert.That (result, Is.EqualTo (_indexes));
    }

    [Test]
    public void Synonyms ()
    {
      var result = _filterViewDefinition.Synonyms;

      Assert.That (result, Is.EqualTo (_synonyms));
    }

    [Test]
    public void GetBaseTable ()
    {
      var table = _filterViewDefinition.GetBaseTable();

      Assert.That (table, Is.SameAs (_entityDefinition));
    }

    [Test]
    public void GetBaseTable_IndirectTable ()
    {
      var derivedFilterViewDefinition = new FilterViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          _filterViewDefinition,
          new[] { "x" },
          _objectIDColunmn,
          _classIDCOlumn,
          _timestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      var table = derivedFilterViewDefinition.GetBaseTable();

      Assert.That (table, Is.SameAs (_entityDefinition));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_filterViewDefinition.IsNull, Is.False);
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IEntityDefinitionVisitor>();

      visitorMock.Expect (mock => mock.VisitFilterViewDefinition (_filterViewDefinition));
      visitorMock.Replay();

      _filterViewDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }
  }
}