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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration.ScriptElements
{
  [TestFixture]
  public class BatchSeparatorStatementTest
  {
    private BatchSeparatorStatement _batchSeparatorStatement;
    private ISqlDialect _sqlDialectStub;
    private List<ScriptStatement> _script;

    [SetUp]
    public void SetUp ()
    {
      _batchSeparatorStatement = new BatchSeparatorStatement();
      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _sqlDialectStub.Stub (stub => stub.BatchSeparator).Return ("SEPARATOR");
      _script = new List<ScriptStatement> ();
    }

    [Test]
    public void AppendToScript_EmptyScript ()
    {
      _batchSeparatorStatement.AppendToScript (_script, _sqlDialectStub);

      Assert.That (_script, Is.Empty);
    }

    [Test]
    public void AppendToScript_NonEmptyScript ()
    {
      var statement1 = new ScriptStatement ("1");
      var statement2 = new ScriptStatement ("2");
      _script.Add (statement1);
      _script.Add (statement2);

      _batchSeparatorStatement.AppendToScript (_script, _sqlDialectStub);

      Assert.That (_script.Count, Is.EqualTo (3));
      Assert.That (_script[0], Is.SameAs(statement1));
      Assert.That (_script[1], Is.EqualTo (statement2));
      Assert.That (_script[2].Statement, Is.EqualTo ("SEPARATOR"));
    }

    [Test]
    public void AppendToScript_NonEmptyScript_LastStatementIsBatchStatement ()
    {
      var statement1 = new ScriptStatement ("1");
      var statement2 = new ScriptStatement ("SEPARATOR");
      _script.Add (statement1);
      _script.Add (statement2);

      _batchSeparatorStatement.AppendToScript (_script, _sqlDialectStub);

      Assert.That (_script.Count, Is.EqualTo (2));
      Assert.That (_script[0], Is.SameAs (statement1));
      Assert.That (_script[1], Is.EqualTo (statement2));
    }
  }
}