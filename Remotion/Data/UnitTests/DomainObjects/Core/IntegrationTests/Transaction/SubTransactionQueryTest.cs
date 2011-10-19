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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
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
    public void AccessObjectInFilterQueryResult ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var extensionMock = MockRepository.GenerateMock<IClientTransactionExtension> ();

        Order.GetObject (DomainObjectIDs.Order1);
        extensionMock.Stub (stub => stub.Key).Return ("stub");
        extensionMock.Replay();
        ClientTransactionMock.Extensions.Add (extensionMock);
        try
        {
          extensionMock.BackToRecord ();

          var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
          query.Parameters.Add ("@customerID", DomainObjectIDs.Customer3);

          var newQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject> ();

          extensionMock
              .Expect (mock => mock.FilterQueryResult (Arg<ClientTransaction>.Is.Anything, Arg<QueryResult<DomainObject>>.Is.Anything))
              .WhenCalled (mi => Order.GetObject (DomainObjectIDs.Order1))
              .Return (newQueryResult);

          extensionMock.Replay ();
          ClientTransaction.Current.QueryManager.GetCollection (query);
          extensionMock.VerifyAllExpectations ();
        }
        finally
        {
          ClientTransactionMock.Extensions.Remove ("stub");
        }
      }
    }

    [Test]
    public void QueryInSubtransaction_CausesObjectsInSubtransactionToBeLoaded ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var finalResult = ClientTransaction.Current.QueryManager.GetCollection (query);
        var loadedObjects = finalResult.ToArray ();

        Assert.That (loadedObjects.Length, Is.EqualTo (2));
        Assert.That (loadedObjects[0].State, Is.EqualTo (StateType.Unchanged));
        Assert.That (loadedObjects[1].State, Is.EqualTo (StateType.Unchanged));
      }
    }

    [Test]
    public void QueryInSubtransaction_CausesObjectsInSubtransactionToBeLoaded_WhenKnownInParent ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);

      var result = ClientTransaction.Current.QueryManager.GetCollection (query).ToArray(); // preload query result in parent transaction
      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0].State, Is.EqualTo (StateType.Unchanged));
      Assert.That (result[1].State, Is.EqualTo (StateType.Unchanged));

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var finalResult = ClientTransaction.Current.QueryManager.GetCollection (query);
        var loadedObjects = finalResult.ToArray ();

        Assert.That (loadedObjects[0].State, Is.EqualTo (StateType.Unchanged));
        Assert.That (loadedObjects[1].State, Is.EqualTo (StateType.Unchanged));
      }
    }

    [Test]
    [Ignore ("TODO 4242")]
    public void QueryInSubtransaction_ReturningObjectDeletedInParentTransaction ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer3);

      var outerResult = ClientTransaction.Current.QueryManager.GetCollection<Order> (query).ToArray ();
      Assert.That (outerResult.Length, Is.EqualTo (1));

      outerResult[0].Delete();
      Assert.That (outerResult[0].State, Is.EqualTo (StateType.Deleted));

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var finalResult = ClientTransaction.Current.QueryManager.GetCollection (query);
        var loadedObjects = finalResult.ToArray ();

        Assert.That (loadedObjects[0], Is.SameAs (outerResult[0]));
        Assert.That (loadedObjects[0].State, Is.EqualTo (StateType.Invalid));
      }
    }

    [Test]
    [Ignore ("TODO 4242")]
    public void QueryInSubtransaction_ReturningObjectInvalidInParentTransaction ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer3);

      var outerResult = ClientTransaction.Current.QueryManager.GetCollection<Order> (query).ToArray ();
      Assert.That (outerResult.Length, Is.EqualTo (1));

      outerResult[0].Delete ();
      Assert.That (outerResult[0].State, Is.EqualTo (StateType.Deleted));

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (outerResult[0].State, Is.EqualTo (StateType.Invalid));

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          var finalResult = ClientTransaction.Current.QueryManager.GetCollection (query);
          var loadedObjects = finalResult.ToArray ();

          Assert.That (loadedObjects[0], Is.SameAs (outerResult[0]));
          Assert.That (loadedObjects[0].State, Is.EqualTo (StateType.Invalid));
        }
      }
    }
  }
}
