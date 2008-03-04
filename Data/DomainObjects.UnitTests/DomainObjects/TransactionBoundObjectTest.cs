using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class TransactionBoundObjectTest : StandardMappingTest
  {
    private ClientTransaction _bindingTransaction;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();
      _bindingTransaction = ClientTransaction.NewBindingTransaction ();
    }

    private T NewBound<T> (params object[] args)
        where T: DomainObject
    {
      using (_bindingTransaction.EnterNonDiscardingScope())
      {
        return (T) RepositoryAccessor.NewObject (typeof (T)).Invoke (args);
      }
    }

    private T GetBound<T> (ObjectID id)
        where T: DomainObject
    {
      using (_bindingTransaction.EnterNonDiscardingScope())
      {
        return (T) RepositoryAccessor.GetObject (id, true);
      }
    }

    [Test]
    public void UnboundObjects ()
    {
      using (ClientTransaction.NewTransaction().EnterNonDiscardingScope())
      {
        Order newOrder = Order.NewObject();
        Assert.IsFalse (newOrder.IsBoundToSpecificTransaction);
        Assert.AreSame (ClientTransaction.Current, newOrder.ClientTransaction);

        Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
        Assert.IsFalse (loadedOrder.IsBoundToSpecificTransaction);
        Assert.AreSame (ClientTransaction.Current, loadedOrder.ClientTransaction);
      }
    }

    [Test]
    public void UnboundObjects_NoCurrentTransaction ()
    {
      Order newOrder;
      using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
      {
        newOrder = Order.NewObject ();
      }
      Assert.IsNull (newOrder.ClientTransaction);
    }

    [Test]
    public void NewBoundObject ()
    {
      Order order = NewBound<Order>();
      Assert.IsTrue (order.IsBoundToSpecificTransaction);
      Assert.AreSame (_bindingTransaction, order.ClientTransaction);
    }

    [Test]
    public void GetBoundObject ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      Assert.IsTrue (order.IsBoundToSpecificTransaction);
      Assert.AreSame (_bindingTransaction, order.ClientTransaction);
    }

    [Test]
    public void GetBoundObject_WithCurrentTransaction ()
    {
      using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
      {
        Order order = GetBound<Order> (DomainObjectIDs.Order1);
        Assert.IsTrue (order.IsBoundToSpecificTransaction);
        Assert.AreSame (_bindingTransaction, order.ClientTransaction);
        Assert.AreNotSame (order, Order.GetObject (DomainObjectIDs.Order1));
      }
    }

    [Test]
    public void CanBeUsedInTransaction ()
    {
      Order order = GetBound<Order>(DomainObjectIDs.Order1);
      Assert.IsTrue (order.CanBeUsedInTransaction (_bindingTransaction));
    }

    [Test]
    public void CanBeUsedInTransaction_WithCurrentTransaction ()
    {
      using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
      {
        Order order = GetBound<Order> (DomainObjectIDs.Order1);
        Assert.IsTrue (order.CanBeUsedInTransaction (_bindingTransaction));
        Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransaction.Current));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot enlist the domain object 'Order|5682f032-2f0b-494b-a31c-"
        + "c97f02b89c36|System.Guid' in this transaction, because it is already bound to another transaction.")]
    public void Enlist_InDifferentTransaction ()
    {
      ClientTransaction newTransaction = ClientTransaction.NewTransaction ();
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      newTransaction.EnlistDomainObject (order);
    }

    [Test]
    public void Enlist_InBindingTransaction ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      _bindingTransaction.EnlistDomainObject (order);
    }

    [Test]
    public void GetSetValue ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      order.OrderNumber = 12;
      Assert.AreEqual (12, order.OrderNumber);
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      Assert.IsNotNull (order.OrderTicket);
      Assert.IsNotNull (order.OrderItems[0]);
    }

    [Test]
    public void LoadedRelatedObjects_AreBound ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      Assert.IsTrue (order.OrderTicket.IsBoundToSpecificTransaction);
      Assert.AreSame (_bindingTransaction, order.OrderTicket.ClientTransaction);
      Assert.IsTrue (order.OrderItems[0].IsBoundToSpecificTransaction);
      Assert.AreSame (_bindingTransaction, order.OrderItems[0].ClientTransaction);
    }

    [Test]
    public void SetRelatedObject ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);

      OrderTicket orderTicket = NewBound<OrderTicket> ();
      order.OrderTicket = orderTicket;
      Assert.AreSame (orderTicket, order.OrderTicket);
    }

    [Test]
    public void InsertRelatedObject ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);

      OrderItem orderItem = NewBound<OrderItem> ();
      order.OrderItems.Add (orderItem);
      Assert.AreSame (orderItem, order.OrderItems[order.OrderItems.Count - 1]);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void SetRelatedObject_UnboundValue ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      using (ClientTransaction.NewTransaction().EnterNonDiscardingScope())
      {
        order.OrderTicket = OrderTicket.NewObject();
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void InsertRelatedObject_UnboundValue ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      using (ClientTransaction.NewTransaction ().EnterNonDiscardingScope ())
      {
        order.OrderItems.Add (OrderItem.NewObject ());
      }
    }

    [Test]
    public void State ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      Assert.AreEqual (StateType.Unchanged, order.State);
      ++order.OrderNumber;
      Assert.AreEqual (StateType.Changed, order.State);
    }

    [Test]
    public void MarkAsChanged ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      Assert.AreEqual (StateType.Unchanged, order.State);
      order.MarkAsChanged();
      Assert.AreEqual (StateType.Changed, order.State);
    }

    [Test]
    public void IsDiscarded ()
    {
      Order order = NewBound<Order> ();
      Assert.AreEqual (StateType.New, order.State);
      Assert.IsFalse (order.IsDiscarded);
      order.Delete ();
      Assert.AreEqual (StateType.Discarded, order.State);
      Assert.IsTrue (order.IsDiscarded);
    }

    [Test]
    public void DataContainer ()
    {
      Order order = NewBound<Order> ();
      DataContainer dc = order.InternalDataContainer;
      Assert.AreSame (_bindingTransaction, dc.ClientTransaction);
    }

    [Test]
    public void Delete ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      order.Delete();
      Assert.AreEqual (StateType.Deleted, order.State);
    }
  }
}