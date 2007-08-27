using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Queries;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class SubTransactionQueryTest : ClientTransactionBaseTest
  {
    [Test]
    public void ScalarQueryInSubTransaction ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Query query = new Query ("QueryWithoutParameter");

        Assert.AreEqual (42, ClientTransactionScope.CurrentTransaction.QueryManager.GetScalar (query));
      }
    }

    [Test]
    public void ObjectQueryInSubTransaction ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Query query = new Query ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        DomainObjectCollection queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
        Customer queriedObject = (Customer) queriedObjects[0];

        Assert.IsNotNull (queriedObjects);
        Assert.AreEqual (1, queriedObjects.Count);
        Assert.AreEqual (DomainObjectIDs.Customer1, queriedObjects[0].ID);

        Assert.AreEqual (new DateTime(2000, 1, 1), queriedObject.CustomerSince);
        Assert.AreSame (Order.GetObject (DomainObjectIDs.Order1), queriedObject.Orders[0]);
      }
    }

    [Test]
    public void ObjectQueryInSubAndRootTransaction ()
    {
      DomainObjectCollection queriedObjectsInSub;
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Query query = new Query ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        queriedObjectsInSub = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      }

      Query queryInRoot = new Query ("CustomerTypeQuery");
      queryInRoot.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

      DomainObjectCollection queriedObjectsInRoot = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (queryInRoot);
      Assert.That (queriedObjectsInRoot, Is.EqualTo (queriedObjectsInSub));
    }

    [Test]
    public void QueriedObjectsCanBeUsedInParentTransaction ()
    {
      DomainObjectCollection queriedObjects;

      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Query query = new Query ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      }

      Customer queriedObject = (Customer) queriedObjects[0];

      Assert.IsNotNull (queriedObjects);
      Assert.AreEqual (1, queriedObjects.Count);
      Assert.AreEqual (DomainObjectIDs.Customer1, queriedObjects[0].ID);
    
      Assert.AreEqual (new DateTime (2000, 1, 1), queriedObject.CustomerSince);
      Assert.AreSame (Order.GetObject (DomainObjectIDs.Order1), queriedObject.Orders[0]);
    }

    [Test]
    public void ChangedComittedQueriedObjectsCanBeUsedInParentTransaction ()
    {
      DomainObjectCollection queriedObjects;
      Customer queriedObject;

      Order newOrder;
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Query query = new Query ("CustomerTypeQuery");
        query.Parameters.Add ("@customerType", Customer.CustomerType.Standard);

        queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
        queriedObject = (Customer) queriedObjects[0];

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
      Assert.AreEqual (DomainObjectIDs.Customer1, queriedObjects[0].ID);

      Assert.IsNull (queriedObject.CustomerSince);
      Assert.AreSame (newOrder, queriedObject.Orders[0]);
    }
  }
}