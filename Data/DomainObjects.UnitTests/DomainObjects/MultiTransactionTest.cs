using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class MultiTransactionTest : ClientTransactionBaseTest
  {
    [Test]
    public void LoadIntoTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (new ClientTransactionScope())
      {
        Order order2 = DomainObject.LoadIntoTransaction (order, ClientTransactionScope.CurrentTransaction);
        Assert.IsNotNull (order2);
        Assert.AreNotSame (order, order2);
        Assert.AreEqual (order.ID, order2.ID);
        Assert.IsTrue (order2.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      }
    }

    [Test]
    public void LoadIntoTransactionSameReferenceIfSameTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Order order2 = DomainObject.LoadIntoTransaction (order, ClientTransactionScope.CurrentTransaction);
      Assert.AreSame (order, order2);

      using (new ClientTransactionScope ())
      {
        Order order3 = DomainObject.LoadIntoTransaction (order, ClientTransactionScope.CurrentTransaction);
        Order order4 = DomainObject.LoadIntoTransaction (order, ClientTransactionScope.CurrentTransaction);
        Assert.AreSame (order3, order4);
        Assert.AreNotSame (order, order3);
      }
    }

    [Test]
    public void LoadIntoTransactionDoesNotCopyValues ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      --order.OrderNumber;
      order.OrderTicket = null;
      order.OrderItems.Add (OrderItem.NewObject ());

      int outerNumber = order.OrderNumber;
      int outerNumberOfItems = order.OrderItems.Count;
      using (new ClientTransactionScope ())
      {
        Order order2 = DomainObject.LoadIntoTransaction (order, ClientTransactionScope.CurrentTransaction);
        Assert.AreEqual (outerNumber + 1, order2.OrderNumber);
        Assert.IsNotNull (order2.OrderTicket);
        Assert.AreEqual (outerNumberOfItems - 1, order2.OrderItems.Count);
      }
    }

    [Test]
    public void LoadIntoTransactionWorksWithCommitedNewObject ()
    {
      SetDatabaseModifyable ();

      Order order = Order.NewObject ();
      order.OrderNumber = 5;
      order.Official = Official.GetObject (DomainObjectIDs.Official1);
      order.Customer = Customer.GetObject (DomainObjectIDs.Customer1);
      order.OrderTicket = OrderTicket.NewObject (order);
      order.OrderItems.Add (OrderItem.NewObject ());
      order.DeliveryDate = DateTime.Now;
      ClientTransactionScope.CurrentTransaction.Commit ();

      using (new ClientTransactionScope ())
      {
        Order order2 = DomainObject.LoadIntoTransaction (order, ClientTransactionScope.CurrentTransaction);
        Assert.AreEqual (order.ID, order2.ID);
        Assert.AreEqual (5, order2.OrderNumber);
        Assert.IsNotNull (order2.OrderTicket);
      }

      order.OrderTicket.Delete ();
      order.OrderItems[0].Delete ();
      order.Delete ();
      ClientTransactionScope.CurrentTransaction.Commit ();
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException))]
    public void LoadIntoTransactionThrowsWithUncommitedNewObjects ()
    {
      Order order = Order.NewObject ();
      using (new ClientTransactionScope ())
      {
        DomainObject.LoadIntoTransaction (order, ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    public void CanBeUsedInTransaction ()
    {
      Order order = Order.NewObject ();
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransaction.NewTransaction()));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Domain object '.*' cannot be used in the current transaction "
        + "as it was loaded or created in another transaction. Use a ClientTransactionScope to set the right transaction, or call "
        + "EnlistInTransaction to enlist the object in the current transaction.", MatchType = MessageMatch.Regex)]
    public void ThrowsWhenCannotBeUsedInTransaction ()
    {
      Order order = Order.NewObject ();
      using (new ClientTransactionScope ())
      {
        Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        int i = order.OrderNumber;
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Domain object '.*' cannot be used in the current transaction "
        + "as it was loaded or created in another transaction. Use a ClientTransactionScope to set the right transaction, or call "
        + "EnlistInTransaction to enlist the object in the current transaction.", MatchType = MessageMatch.Regex)]
    public void ThrowsOnDeleteWhenCannotBeUsedInTransaction ()
    {
      Order order = Order.NewObject ();
      using (new ClientTransactionScope ())
      {
        Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        order.Delete();
      }
    }

    [Test]
    public void LoadedObjectCanBeEnlistedInTransaction ()
    {
      ClientTransaction newTransaction = ClientTransaction.NewTransaction();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      Assert.IsFalse (order.CanBeUsedInTransaction (newTransaction));

      order.EnlistInTransaction (newTransaction);
      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object '.*' cannot be enlisted in the given transaction because it "
          + "does not exist in that transaction. Maybe it was newly created and has not yet been committed, or it was deleted.",
          MatchType = MessageMatch.Regex)]
    public void NewObjectCannotBeEnlistedInTransaction ()
    {
      ClientTransaction newTransaction = ClientTransaction.NewTransaction();
      Order order = Order.NewObject ();
      order.EnlistInTransaction (newTransaction);
    }

    [Test]
    public void NewObjectCanBeEnlistedInTransactionWhenCommitted ()
    {
      SetDatabaseModifyable ();
      ClientTransaction newTransaction = ClientTransaction.NewTransaction();
      Order order = Order.NewObject ();
      order.OrderNumber = 5;
      order.DeliveryDate = DateTime.Now;
      order.OrderItems.Add (OrderItem.NewObject ());
      order.OrderTicket = OrderTicket.NewObject ();
      order.Official = Official.GetObject (DomainObjectIDs.Official1);
      order.Customer = Customer.GetObject (DomainObjectIDs.Customer1);
      ClientTransactionScope.CurrentTransaction.Commit ();

      order.EnlistInTransaction (newTransaction);
      using (newTransaction.EnterScope ())
      {
        Assert.AreEqual (5, order.OrderNumber);
      }
      order.OrderTicket.Delete ();
      order.OrderItems[0].Delete ();
      order.Delete ();
      ClientTransactionScope.CurrentTransaction.Commit ();
    }

    [Test]
    public void EnlistedObjectCanBeUsedInTwoTransactions ()
    {
      ClientTransaction newTransaction = ClientTransaction.NewTransaction();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.EnlistInTransaction (newTransaction);
      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));

      Assert.AreNotEqual (5, order.OrderNumber);
      order.OrderNumber = 5;
      Assert.AreEqual (5, order.OrderNumber);

      using (newTransaction.EnterScope ())
      {
        Assert.AreNotEqual (5, order.OrderNumber);
        order.OrderNumber = 6;
        Assert.AreEqual (6, order.OrderNumber);
      }

      Assert.AreEqual (5, order.OrderNumber);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object '.*' cannot be enlisted in the given transaction because it "
          + "does not exist in that transaction. Maybe it was newly created and has not yet been committed, or it was deleted.",
          MatchType = MessageMatch.Regex)]
    public void DeletedObjectCannotBeEnlistedInTransaction ()
    {
      SetDatabaseModifyable ();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      for (int i = order.OrderItems.Count - 1; i >= 0; --i)
        order.OrderItems[i].Delete ();

      order.OrderTicket.Delete ();
      order.Delete ();
      ClientTransactionScope.CurrentTransaction.Commit ();

      order.EnlistInTransaction (ClientTransaction.NewTransaction());
    }

    [Test]
    public void DeletedObjectCanBeEnlistedInTransactionIfNotCommitted ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int orderNumber = order.OrderNumber;
      order.Delete ();
      Assert.AreEqual (StateType.Deleted, order.State);
      
      ClientTransaction newTransaction = ClientTransaction.NewTransaction();
      order.EnlistInTransaction (newTransaction);
      using (newTransaction.EnterScope ())
      {
        Assert.AreNotEqual (StateType.Deleted, order.State);
      }
    }

    [Test]
    public void CommitOnlyInfluencesTransactionsEnlistedAfterCommit ()
    {
      SetDatabaseModifyable ();

      ClientTransaction newTransaction1 = ClientTransaction.NewTransaction();
      ClientTransaction newTransaction2 = ClientTransaction.NewTransaction();
      
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.EnlistInTransaction (newTransaction1);

      int oldOrderNumber = order.OrderNumber;
      order.OrderNumber = 5;
      ClientTransactionScope.CurrentTransaction.Commit ();

      order.EnlistInTransaction (newTransaction2);

      using (newTransaction1.EnterScope ())
      {
        Assert.AreNotEqual (5, order.OrderNumber);
        Assert.AreEqual (oldOrderNumber, order.OrderNumber);
      }

      using (newTransaction2.EnterScope ())
      {
        Assert.AreEqual (5, order.OrderNumber);
        Assert.AreNotEqual (oldOrderNumber, order.OrderNumber);
      }

      order.OrderNumber = oldOrderNumber;
      ClientTransactionScope.CurrentTransaction.Commit ();
    }
  }
}
