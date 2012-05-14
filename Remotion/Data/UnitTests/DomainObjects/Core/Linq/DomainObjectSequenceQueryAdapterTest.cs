// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class DomainObjectSequenceQueryAdapterTest : StandardMappingTest
  {
    private IQuery _queryStub;

    [SetUp]
    public new void SetUp ()
    {
      base.SetUp();

      _queryStub = MockRepository.GenerateStub<IQuery> ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Only collection queries can be used to load data containers.", MatchType = MessageMatch.Contains)]
    public void Execute_QueryTypeNotCollection ()
    {
      _queryStub.Stub (stub => stub.QueryType).Return (QueryType.Scalar);

      new DomainObjectSequenceQueryAdapter<string> (_queryStub);
    }

    [Test]
    public void Execute ()
    {
      _queryStub.Stub (stub => stub.QueryType).Return (QueryType.Collection);

      var order1 = DomainObjectMother.CreateFakeObject<Order>();
      var order2 = DomainObjectMother.CreateFakeObject<Order>();
      var fakeResult = new QueryResult<DomainObject> (_queryStub, new[] {  order1, order2 });

      var queryAdapter = new DomainObjectSequenceQueryAdapter<object> (_queryStub);
      var queryManagerMock = MockRepository.GenerateStrictMock<IQueryManager>();
      queryManagerMock.Expect (mock => mock.GetCollection (queryAdapter)).Return (fakeResult);

      var result = queryAdapter.Execute (queryManagerMock);

      CollectionAssert.AreEquivalent (new[] { order1, order2 }, result);
    }

  }
}