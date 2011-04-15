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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SecondaryXmlIndexDefinitionTest
  {
    private EntityNameDefinition _objectName;
    private SimpleColumnDefinition _xmlColumn;
    private SecondaryXmlIndexDefinition _indexDefinition;

    [SetUp]
    public void SetUp ()
    {
      _objectName = new EntityNameDefinition ("_objectSchema", "objectName");
      _xmlColumn = new SimpleColumnDefinition ("xmlColumn", typeof (string), "xml", false, false);
      
      _indexDefinition = new SecondaryXmlIndexDefinition ("IndexName", _objectName, _xmlColumn, "PrimaryIndexName", SecondaryXmlIndexKind.Property);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_indexDefinition.IndexName, Is.EqualTo("IndexName"));
      Assert.That (_indexDefinition.ObjectName, Is.SameAs (_objectName));
      Assert.That (_indexDefinition.PrimaryIndexName, Is.EqualTo("PrimaryIndexName"));
      Assert.That (_indexDefinition.XmlColumn, Is.SameAs (_xmlColumn));
      Assert.That (_indexDefinition.Kind, Is.EqualTo (SecondaryXmlIndexKind.Property));
    }

    [Test]
    public void Accept_IndexDefinitionVisitor ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IIndexDefinitionVisitor> ();
      visitorMock.Replay ();

      _indexDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }

    [Test]
    public void Accept_SqlIndexDefinitionVisitor ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ISqlIndexDefinitionVisitor> ();
      visitorMock.Expect (mock => mock.VisitSecondaryXmlIndexDefinition (_indexDefinition));
      visitorMock.Replay ();

      _indexDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }
  }
}