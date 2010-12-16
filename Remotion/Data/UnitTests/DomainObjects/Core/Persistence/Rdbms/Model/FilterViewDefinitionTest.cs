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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class FilterViewDefinitionTest
  {
    private SimpleColumnDefinition _column1;
    private SimpleColumnDefinition _column2;
    private SimpleColumnDefinition _column3;
    private TableDefinition _entityDefinition;
    private FilterViewDefinition _filterViewDefinition;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID", typeof (UnitTestStorageObjectFactoryStub));
      _column1 = new SimpleColumnDefinition ("Column1", typeof (string), "varchar", true);
      _column2 = new SimpleColumnDefinition ("Column2", typeof (string), "varchar", true);
      _column3 = new SimpleColumnDefinition ("Column3", typeof (string), "varchar", true);
      _entityDefinition = new TableDefinition (_storageProviderDefinition, "Table", "View", new[] { _column1, _column2, _column3 });

      _filterViewDefinition = new FilterViewDefinition (
          _storageProviderDefinition,
          "Test",
          _entityDefinition,
          "CLASSID",
          new[] { _column1, _column3 });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_filterViewDefinition.ClassID, Is.EqualTo ("CLASSID"));
      Assert.That (_filterViewDefinition.BaseEntity, Is.SameAs (_entityDefinition));
      Assert.That (_filterViewDefinition.ViewName, Is.EqualTo ("Test"));
      Assert.That (_filterViewDefinition.StorageProviderID, Is.EqualTo ("SPID"));
      Assert.That (_filterViewDefinition.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var filterViewDefinition = new FilterViewDefinition (_storageProviderDefinition, null, _entityDefinition, "CLASSID", new IColumnDefinition[0]);
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
    public void GetColumns ()
    {
      var result = _filterViewDefinition.GetColumns();

      Assert.That (result, Is.EqualTo (new[] { _column1, _column3 }));
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