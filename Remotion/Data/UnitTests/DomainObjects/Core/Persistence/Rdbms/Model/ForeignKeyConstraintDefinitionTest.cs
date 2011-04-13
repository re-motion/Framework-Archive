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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ForeignKeyConstraintDefinitionTest
  {
    private ForeignKeyConstraintDefinition _constraint;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private string _referencedTableName;
    private SimpleColumnDefinition _referencingColumn;
    private SimpleColumnDefinition _referencedColumn;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider");
      _referencingColumn = new SimpleColumnDefinition ("COL1", typeof (string), "varchar", false, false);
      _referencedColumn = new SimpleColumnDefinition ("COL2", typeof (string), "varchar", false, false);

      _referencedTableName = "TableName";
      _constraint = new ForeignKeyConstraintDefinition (
          "Test", new EntityNameDefinition (null, _referencedTableName), new[] { _referencingColumn }, new[] { _referencedColumn });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_constraint.ConstraintName, Is.EqualTo ("Test"));
      Assert.That (_constraint.ReferencedTableName.EntityName, Is.SameAs (_referencedTableName));
      Assert.That (_constraint.ReferencedTableName.SchemaName, Is.Null);
      Assert.That (_constraint.ReferencingColumns, Is.EqualTo (new[] { _referencingColumn }));
      Assert.That (_constraint.ReferencedColumns, Is.EqualTo (new[] { _referencedColumn }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The referencing and referenced column sets must have the same number of items.\r\nParameter name: referencingColumns")]
    public void Initialization_InvalidColumns ()
    {
      new ForeignKeyConstraintDefinition (
          "Test", new EntityNameDefinition (null, _referencedTableName), new[] { _referencingColumn }, new SimpleColumnDefinition[0]);
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ITableConstraintDefinitionVisitor>();

      visitorMock.Expect (mock => mock.VisitForeignKeyConstraintDefinition (_constraint));
      visitorMock.Replay();

      _constraint.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }
  }
}