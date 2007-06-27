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
      Assert.IsFalse (order.CanBeUsedInTransaction (new ClientTransaction()));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Domain object '.*' cannot be used in the current transaction "
        + "as it was loaded or created in another transaction. Use a ClientTransactionScope to set the right transaction, or call "
        + "EnlistInCurrentTransaction to enlist the object with the current transaction.", MatchType = MessageMatch.Regex)]
    public void ThrowsWhenCannotBeUsedInTransaction ()
    {
      Order order = Order.NewObject ();
      using (new ClientTransactionScope ())
      {
        Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        int i = order.OrderNumber;
      }
    }
  }
}
