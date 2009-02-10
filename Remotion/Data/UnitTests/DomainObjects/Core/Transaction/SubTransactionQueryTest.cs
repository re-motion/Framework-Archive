// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class SubTransactionQueryTest : ClientTransactionBaseTest
  {
    [Test]
    public void ScalarQueryInSubTransaction ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var query = QueryFactory.CreateQueryFromConfiguration ("QueryWithoutParameter");

        Assert.AreEqual (42, ClientTransactionScope.CurrentTransaction.QueryManager.GetScalar (query));
      }
    }

    [Test]
    public void ObjectQueryInSubTransaction ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var query = QueryFactory.CreateQueryFromConfiguration ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        var queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
        var array = queriedObjects.ToArray ();
        Customer queriedObject = (Customer) array[0];

        Assert.IsNotNull (queriedObjects);
        Assert.AreEqual (1, queriedObjects.Count);
        Assert.AreEqual (DomainObjectIDs.Customer1, array[0].ID);

        Assert.AreEqual (new DateTime(2000, 1, 1), queriedObject.CustomerSince);
        Assert.AreSame (Order.GetObject (DomainObjectIDs.Order1), queriedObject.Orders[0]);
      }
    }

    [Test]
    public void ObjectQueryWithObjectListInSubTransaction ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var query = QueryFactory.CreateQueryFromConfiguration ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        var queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection<Customer> (query);
        var array = queriedObjects.ToArray ();
        Customer queriedObject = array[0];

        Assert.IsNotNull (queriedObjects);
        Assert.AreEqual (1, queriedObjects.Count);
        Assert.AreEqual (DomainObjectIDs.Customer1, array[0].ID);

        Assert.AreEqual (new DateTime (2000, 1, 1), queriedObject.CustomerSince);
        Assert.AreSame (Order.GetObject (DomainObjectIDs.Order1), queriedObject.Orders[0]);
      }
    }

    [Test]
    public void ObjectQueryInSubAndRootTransaction ()
    {
      IQueryResult queriedObjectsInSub;
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var query = QueryFactory.CreateQueryFromConfiguration ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        queriedObjectsInSub = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      }

      var queryInRoot = QueryFactory.CreateQueryFromConfiguration ("CustomerTypeQuery");
      queryInRoot.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

      IQueryResult queriedObjectsInRoot = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (queryInRoot);
      Assert.That (queriedObjectsInRoot.ToArray(), Is.EqualTo (queriedObjectsInSub.ToArray()));
    }

    [Test]
    public void QueriedObjectsCanBeUsedInParentTransaction ()
    {
      IQueryResult queriedObjects;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var query = QueryFactory.CreateQueryFromConfiguration ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      }

      Customer queriedObject = (Customer) queriedObjects.ToArray()[0];

      Assert.IsNotNull (queriedObjects);
      Assert.AreEqual (1, queriedObjects.Count);
      
      Assert.AreEqual (DomainObjectIDs.Customer1, queriedObject.ID);
      Assert.AreEqual (new DateTime (2000, 1, 1), queriedObject.CustomerSince);
      Assert.AreSame (Order.GetObject (DomainObjectIDs.Order1), queriedObject.Orders[0]);
    }

    [Test]
    public void ChangedComittedQueriedObjectsCanBeUsedInParentTransaction ()
    {
      IQueryResult queriedObjects;
      Customer queriedObject;

      Order newOrder;
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var query = QueryFactory.CreateQueryFromConfiguration ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
        queriedObject = (Customer) queriedObjects.ToArray() [0];

        newOrder = Order.NewObject ();
        newOrder.Official = Official.NewObject ();
        newOrder.OrderTicket = OrderTicket.NewObject ();
        newOrder.OrderItems.Add (OrderItem.NewObject ());
        queriedObject.Orders.Insert (0, newOrder);
        queriedObject.CustomerSince = null;

        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      Assert.IsNotNull (queriedObjects);
      Assert.AreEqual (1, queriedObjects.Count);
      Assert.AreEqual (DomainObjectIDs.Customer1, queriedObjects.ToArray()[0].ID);

      Assert.IsNull (queriedObject.CustomerSince);
      Assert.AreSame (newOrder, queriedObject.Orders[0]);
    }

    [Test]
    public void FilterQueryResultCalledInCorrectScope ()
    {
      MockRepository mockRepository = new MockRepository ();
      IClientTransactionExtension extensionMock = mockRepository.Stub<IClientTransactionExtension> ();

      ClientTransactionMock.Extensions.Add ("mock", extensionMock);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
        query.Parameters.Add ("@customerID", DomainObjectIDs.Customer3);

        extensionMock.FilterQueryResult (null, null, null); // expectation
        LastCall.Constraints (Mocks_Is.Same (ClientTransactionMock), Mocks_Is.Anything (), Mocks_Is.Anything ());
        LastCall.Do ((Action<ClientTransaction, DomainObjectCollection, IQuery>) delegate
        {
          Assert.AreSame (ClientTransactionMock, ClientTransaction.Current);
        });

        mockRepository.ReplayAll ();
        ClientTransaction.Current.QueryManager.GetCollection (query);
        mockRepository.VerifyAll ();
      }
    }

    [Test]
    public void AccessObjectInFilterQueryResult ()
    {
      MockRepository mockRepository = new MockRepository ();
      IClientTransactionExtension extensionMock = mockRepository.Stub<IClientTransactionExtension> ();

      Order.GetObject (DomainObjectIDs.Order1);

      ClientTransactionMock.Extensions.Add ("mock", extensionMock);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
        query.Parameters.Add ("@customerID", DomainObjectIDs.Customer3);

        extensionMock.FilterQueryResult (null, null, null); // expectation
        LastCall.IgnoreArguments();
        LastCall.Do ((Action<ClientTransaction, DomainObjectCollection, IQuery>) delegate
        {
          Order.GetObject (DomainObjectIDs.Order1);
        });

        mockRepository.ReplayAll ();
        ClientTransaction.Current.QueryManager.GetCollection (query);
        mockRepository.VerifyAll ();
      }
    }
  }
}
