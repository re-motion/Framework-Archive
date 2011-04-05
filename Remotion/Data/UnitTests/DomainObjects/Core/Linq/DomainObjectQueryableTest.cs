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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;


namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class DomainObjectQueryableTest : ClientTransactionBaseTest
  {
    private IQueryParser _queryParserStub;
    private IQueryExecutor _executorStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _queryParserStub = MockRepository.GenerateStub<IQueryParser> ();
      _executorStub = MockRepository.GenerateStub<IQueryExecutor> ();
    }

    [Test]
    public void Provider_AutoInitialized ()
    {
      var queryableWithOrder = new DomainObjectQueryable<Order> (_queryParserStub, _executorStub);

      Assert.That (queryableWithOrder.Provider, Is.Not.Null);
      Assert.That (queryableWithOrder.Provider, Is.InstanceOf (typeof (DefaultQueryProvider)));
      Assert.That (((DefaultQueryProvider) queryableWithOrder.Provider).QueryableType, Is.SameAs (typeof (DomainObjectQueryable<>)));
      Assert.That (queryableWithOrder.Provider.Executor, Is.SameAs (_executorStub));
      Assert.That (queryableWithOrder.Provider.QueryParser, Is.SameAs (_queryParserStub));
    }
    
    [Test]
    public void Provider_PassedIn ()
    {
      var expectedProvider = new DefaultQueryProvider (
          typeof (DomainObjectQueryable<>),
          _queryParserStub, _executorStub);

      var queryable = new DomainObjectQueryable<Order> (expectedProvider, Expression.Constant (null, typeof (DomainObjectQueryable<Order>)));
      
      Assert.That (queryable.Provider, Is.Not.Null);
      Assert.That (queryable.Provider, Is.SameAs (expectedProvider));
    }

    
  }
}