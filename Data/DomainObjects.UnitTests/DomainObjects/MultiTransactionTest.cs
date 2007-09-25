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
      using (ClientTransaction.NewTransaction ().EnterScope ())
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

      using (ClientTransaction.NewTransaction().EnterScope())
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
      using (ClientTransaction.NewTransaction().EnterScope())
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

      using (ClientTransaction.NewTransaction().EnterScope())
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
      using (ClientTransaction.NewTransaction().EnterScope())
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
      using (ClientTransaction.NewTransaction().EnterScope())
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
      using (ClientTransaction.NewTransaction().EnterScope())
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

      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
    }

    [Test][ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object '.*' cannot be enlisted because it does not exist in this "
          + "transaction. Maybe it was newly created and has not yet been committed, or it was deleted.",
          MatchType = MessageMatch.Regex)]
    public void NewObjectCannotBeEnlistedInTransaction ()
    {
      ClientTransaction newTransaction = ClientTransaction.NewTransaction();
      Order order = Order.NewObject ();
      newTransaction.EnlistDomainObject (order);
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

      newTransaction.EnlistDomainObject (order);
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
      newTransaction.EnlistDomainObject (order);
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
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object '.*' cannot be enlisted because it does not exist in this "
          + "transaction. Maybe it was newly created and has not yet been committed, or it was deleted.",
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

      ClientTransaction.NewTransaction().EnlistDomainObject (order);
    }

    [Test]
    public void DeletedObjectCanBeEnlistedInTransactionIfNotCommitted ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int orderNumber = order.OrderNumber;
      order.Delete ();
      Assert.AreEqual (StateType.Deleted, order.State);
      
      ClientTransaction newTransaction = ClientTransaction.NewTransaction();
      newTransaction.EnlistDomainObject (order);
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
      newTransaction1.EnlistDomainObject (order);

      int oldOrderNumber = order.OrderNumber;
      order.OrderNumber = 5;
      ClientTransactionScope.CurrentTransaction.Commit ();

      newTransaction2.EnlistDomainObject (order);

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

    [Test]
    public void GetObjectAfterEnlistingReturnsEnlistedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransaction.NewTransaction().EnterScope())
      {
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
        Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        Assert.AreSame (order, Order.GetObject (order.ID));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A domain object instance for object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' already exists in this transaction.")]
    public void EnlistingAlthoughObjectHasAlreadyBeenLoadedThrows ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransaction.NewTransaction().EnterScope())
      {
        Assert.AreNotSame (order, Order.GetObject (order.ID));
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
      }
    }

    [Test]
    public void MultipleEnlistingsAreIgnored()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
      using (ClientTransaction.NewTransaction().EnterScope())
      {
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);

        using (ClientTransactionScope.CurrentTransaction.CreateSubTransaction ().EnterScope ())
        {
          ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
          ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
        }
      }
    }

    [Test]
    public void EnlistDomainObjectInSubTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction newTransaction = ClientTransaction.NewTransaction ().CreateSubTransaction ();
      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
    }

    [Test]
    public void EnlistSameDomainObjects()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = order.OrderItems[0];
      
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionMock));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (ClientTransactionMock));

      ClientTransaction newTransaction = ClientTransaction.NewTransaction ();

      Assert.IsFalse (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsFalse (orderItem.CanBeUsedInTransaction (newTransaction));

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock);

      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (newTransaction));
    }

    [Test]
    public void EnlistSameDomainObjectsInSubTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = order.OrderItems[0];

      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionMock));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (ClientTransactionMock));

      ClientTransaction newTransaction = ClientTransaction.NewTransaction ().CreateSubTransaction();

      Assert.IsFalse (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsFalse (orderItem.CanBeUsedInTransaction (newTransaction));

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock);

      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (newTransaction));

      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction.ParentTransaction));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (newTransaction.ParentTransaction));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The source transaction contains domain object '.*', which "
        + "cannot be enlisted because its state type is 'New'. Delete the object or rollback or commit the transaction.",
        MatchType = MessageMatch.Regex)]
    public void EnlistSameDomainObjectsThrowsOnNewObjects ()
    {
      Order.NewObject ();
      ClientTransaction newTransaction = ClientTransaction.NewTransaction ();
      newTransaction.EnlistSameDomainObjects (ClientTransactionMock);
    }

    [Test]
    public void EnlistSameDomainObjectsIgnoresDiscardedObjects ()
    {
      Order order = Order.NewObject ();
      order.Delete ();
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.IsTrue (order.IsDiscarded);

      ClientTransaction newTransaction = ClientTransaction.NewTransaction ();

      using (newTransaction.EnterScope())
      {
        ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1).Delete ();
        SetDatabaseModifyable ();
        newTransaction.Commit ();
      }

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock);

      Assert.IsFalse (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsFalse (cwadt.CanBeUsedInTransaction (newTransaction));
    }

    [Test]
    public void EnlistSameDomainObjectsIgnoresObjectsAlreadyEnlisted ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction newTransaction = ClientTransaction.NewTransaction ();
      
      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock);

      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
    }

    [Test]
    public void EnlistSameDomainObjectsWorksWithObjectsDeletedInDatabase ()
    {
      SetDatabaseModifyable ();
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      
      using (ClientTransaction.NewTransaction ().EnterScope())
      {
        ClassWithAllDataTypes.GetObject (cwadt.ID).Delete ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      ClientTransaction newTransaction = ClientTransaction.NewTransaction ();

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock);

      Assert.IsTrue (cwadt.CanBeUsedInTransaction (newTransaction));
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid' could not be found.")]
    public void UsingEnlistedObjectsDeletedInDatabaseThrowsObjectNotFoundException ()
    {
      SetDatabaseModifyable ();
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      using (ClientTransaction.NewTransaction ().EnterScope ())
      {
        ClassWithAllDataTypes.GetObject (cwadt.ID).Delete ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      ClientTransaction newTransaction = ClientTransaction.NewTransaction ();

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock);

      Assert.IsTrue (cwadt.CanBeUsedInTransaction (newTransaction));
      using (newTransaction.EnterScope ())
      {
        cwadt.StringProperty = "FoO";
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A domain object instance for object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' already exists in this transaction.")]
    public void EnlistSameDomainObjectsThrowsOnObjectsAlreadyEnlistedWithDifferentReferences ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction newTransaction = ClientTransaction.NewTransaction ();

      using (newTransaction.EnterScope ())
      {
        Order order2 = Order.GetObject (DomainObjectIDs.Order1);
        Assert.AreNotSame (order, order2);
      }

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock);
    }

    [Test]
    public void OnLoadedCannAccessObjectPropertiesInEnlistDomainObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction newTransaction = ClientTransaction.NewTransaction ();
      order.ProtectedLoaded += delegate (object sender, EventArgs e) { Assert.AreEqual (1, ((Order) sender).OrderNumber); };

      newTransaction.EnlistDomainObject (order);
    }
  }
}
