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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Development.Data.UnitTesting.DomainObjects.Linq;
using Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects.TestDomain;

namespace Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects.Linq
{
  [TestFixture]
  public class ExpressionTreeComparerTest
  {
    private ExpressionTreeComparer _expressionTreeComparer;

    [SetUp]
    public void SetUp ()
    {
      _expressionTreeComparer = new ExpressionTreeComparer ((actual, exptected) => Assert.That (actual, Is.EqualTo (exptected)));
    }

    [Test]
    public void Compare_Equal ()
    {
      IQueryable<SimpleDomainObject> expected = from d in QueryFactory.CreateLinqQuery<SimpleDomainObject>() where d.Value == 1 select d;
      IQueryable<SimpleDomainObject> actual = from d in QueryFactory.CreateLinqQuery<SimpleDomainObject>() where d.Value == 1 select d;

      _expressionTreeComparer.Compare (expected, actual);
    }

    [Test]
    [ExpectedException (typeof (AssertionException))]
    public void Compare_NotEqual ()
    {
      IQueryable<SimpleDomainObject> expected = from d in QueryFactory.CreateLinqQuery<SimpleDomainObject>() where d.Value == 1 select d;
      IQueryable<SimpleDomainObject> actual = from d in QueryFactory.CreateLinqQuery<SimpleDomainObject>() where d.Value == 0 select d;

      _expressionTreeComparer.Compare (expected, actual);
    }
  }
}
