using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.EventSequence;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class DeleteDomainObjectTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  Order _order;
  OrderTicket _orderTicket;

  // construction and disposing

  public DeleteDomainObjectTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _order = Order.GetObject (DomainObjectIDs.Order2);
    _orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
  }

  [Test]
  public void Delete ()
  {
    _orderTicket.Delete ();

    Assert.AreEqual (StateType.Deleted, _orderTicket.State);
    Assert.AreEqual (StateType.Deleted, _orderTicket.DataContainer.State);
  }

  [Test]
  public void DeleteTwice ()
  {
    _orderTicket.Delete ();

    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (_orderTicket);
    _orderTicket.Delete ();

    Assert.AreEqual (0, eventReceiver.Count);
  }

  [Test]
  [ExpectedException (typeof (ObjectDeletedException))]
  public void GetObject ()
  {
    _orderTicket.Delete ();

    OrderTicket.GetObject (_orderTicket.ID);
  }

  [Test]
  public void GetObjectAndIncludeDeleted ()
  {
    _orderTicket.Delete ();

    Assert.IsNotNull (OrderTicket.GetObject (_orderTicket.ID, true));
  }

  [Test]
  public void GetOriginalRelatedObjectFromDeletedObject ()
  {
    _orderTicket.Delete ();

    Order originalOrder = (Order) _orderTicket.GetOriginalRelatedObject ("Order");
    
    Assert.IsNotNull (originalOrder);
    Assert.AreEqual (DomainObjectIDs.Order1, originalOrder.ID);
  }

  [Test]
  public void GetOriginalRelatedObjectsFromDeletedObject ()
  {
    _order.Delete ();
    DomainObjectCollection originalOrderItems = _order.GetOriginalRelatedObjects ("OrderItems");

    Assert.IsNotNull (originalOrderItems);
    Assert.AreEqual (1, originalOrderItems.Count);
    Assert.AreEqual (DomainObjectIDs.OrderItem3, originalOrderItems[0].ID);
  }

  [Test]
  [ExpectedException (typeof (ObjectDeletedException))]
  public void SetRelatedObjectOfDeletedObject ()
  {
    _orderTicket.Delete ();

    _orderTicket.Order = _order;
  }

  [Test]
  [ExpectedException (typeof (ObjectDeletedException))]
  public void AddToRelatedObjectsOfDeletedObject ()
  {
    _order.Delete ();

    _order.OrderItems.Add (OrderItem.GetObject (DomainObjectIDs.OrderItem1));
  }

  [Test]
  public void GetOriginalRelatedObjectFromOppositeObject ()
  {
    Order oldRelatedOrder = _orderTicket.Order;
    _orderTicket.Delete ();

    OrderTicket deletedOrderTicket = (OrderTicket) oldRelatedOrder.GetOriginalRelatedObject ("OrderTicket");

    Assert.IsNotNull (deletedOrderTicket);
    Assert.AreEqual (_orderTicket.ID, deletedOrderTicket.ID);
  }

  [Test]
  public void GetOriginalRelatedObjectsFromOppositeObject ()
  {
    Customer oldRelatedCustomer = _order.Customer;
    _order.Delete ();

    OrderCollection originalOrders = (OrderCollection) oldRelatedCustomer.GetOriginalRelatedObjects ("Orders");

    Assert.IsNotNull (originalOrders);
    Assert.AreEqual (1, originalOrders.Count);
    Assert.AreSame (_order, originalOrders[0]);
  }

  [Test]
  public void GetOriginalRelatedObjectForBothDeleted ()
  {
    OrderTicket orderTicket = _order.OrderTicket;
    _order.Delete ();
    orderTicket.Delete ();

    Assert.IsNotNull (_order.GetOriginalRelatedObject ("OrderTicket"));
    Assert.IsNotNull (orderTicket.GetOriginalRelatedObject ("Order"));
  }
}
}
