using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class RelationEndPointMapTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private RelationEndPointMap _map;

  // construction and disposing

  public RelationEndPointMapTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _map = ClientTransactionMock.DataManager.RelationEndPointMap;
  }

  [Test]
  public void DeleteNew ()
  {
    Order newOrder = new Order ();
    Assert.IsTrue (_map.Count > 0);

    _map.PerformDelete (newOrder);
    Assert.AreEqual (0, _map.Count);
  }

  [Test]
  public void CommitForDeletedObject ()
  {
    Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
    Assert.IsTrue (_map.Count > 0);

    computer.Delete ();

    DomainObjectCollection deletedDomainObjects = new DomainObjectCollection ();
    deletedDomainObjects.Add (computer);

    _map.Commit (deletedDomainObjects);

    Assert.AreEqual (0, _map.Count);
  }

  [Test]
  public void GetOriginalRelatedObjectsWithLazyLoad ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    RelationEndPointID endPointID = new RelationEndPointID (order.ID, "OrderItems");
    DomainObjectCollection originalOrderItems = _map.GetOriginalRelatedObjects (endPointID);
    DomainObjectCollection orderItems = _map.GetRelatedObjects (endPointID);

    Assert.IsFalse (object.ReferenceEquals (originalOrderItems, orderItems));
  }

  [Test]
  public void GetOriginalRelatedObjectWithLazyLoad ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    RelationEndPointID endPointID = new RelationEndPointID (order.ID, "OrderTicket");
    DomainObject originalOrderTicket = _map.GetOriginalRelatedObject (endPointID);
    DomainObject orderTicket = _map.GetRelatedObject (endPointID);

    Assert.IsTrue (object.ReferenceEquals (originalOrderTicket, orderTicket));
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), 
      "GetRelatedObject can only be called for end points with a cardinality of 'One'.\r\nParameter name: endPointID")]
  public void GetRelatedObjectWithEndPointIDOfWrongCardinality ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    _map.GetRelatedObject (new RelationEndPointID (order.ID, "OrderItems"));
  }

  [Test]
  [ExpectedException (typeof (ArgumentException),
      "GetOriginalRelatedObject can only be called for end points with a cardinality of 'One'.\r\nParameter name: endPointID")]
  public void GetOriginalRelatedObjectWithEndPointIDOfWrongCardinality ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    _map.GetOriginalRelatedObject (new RelationEndPointID (order.ID, "OrderItems"));
  }

  [Test]
  [ExpectedException (typeof (ArgumentException),
      "SetRelatedObject can only be called for end points with a cardinality of 'One'.\r\nParameter name: endPointID")]
  public void SetRelatedObjectWithEndPointIDOfWrongCardinality ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    _map.SetRelatedObject (new RelationEndPointID (order.ID, "OrderItems"), new OrderItem ());
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), 
      "GetRelatedObjects can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
  public void GetRelatedObjectsWithEndPointIDOfWrongCardinality ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    _map.GetRelatedObjects (new RelationEndPointID (order.ID, "OrderTicket"));
  }

  [Test]
  [ExpectedException (typeof (ArgumentException),
      "GetOriginalRelatedObjects can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
  public void GetOriginalRelatedObjectsWithEndPointIDOfWrongCardinality ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    _map.GetOriginalRelatedObjects (new RelationEndPointID (order.ID, "OrderTicket"));
  }

  [Test]
  [ExpectedException (typeof (ClientTransactionsDifferException), 
      "Property 'OrderTicket' of DomainObject 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'"
      + " cannot be set to DomainObject 'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid',"
      + " because the objects do not belong to the same ClientTransaction.")]
  public void SetRelatedObjectWithOtherClientTransaction ()
  {
    Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

    ClientTransaction clientTransaction = new ClientTransaction ();
    OrderTicket orderTicket2 = (OrderTicket) clientTransaction.GetObject (DomainObjectIDs.OrderTicket2);

    _map.SetRelatedObject (new RelationEndPointID (order1.ID, "OrderTicket"), orderTicket2);
  }

  [Test]
  [ExpectedException (typeof (ClientTransactionsDifferException), 
      "Cannot insert DomainObject 'OrderItem|0d7196a5-8161-4048-820d-b1bbdabe3293|System.Guid'"
      + " at position 2 into collection of property 'OrderItems' of DomainObject 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid',"
      + " because the objects do not belong to the same ClientTransaction.")]
  public void PerformCollectionAddWithOtherClientTransaction ()
  {
    Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

    ClientTransaction clientTransaction = new ClientTransaction ();
    OrderItem orderItem3 = (OrderItem) clientTransaction.GetObject (DomainObjectIDs.OrderItem3);

    order1.OrderItems.Add (orderItem3);
  }

  [Test]
  [ExpectedException (typeof (ClientTransactionsDifferException), 
      "Cannot insert DomainObject 'OrderItem|0d7196a5-8161-4048-820d-b1bbdabe3293|System.Guid'"
      + " at position 0 into collection of property 'OrderItems' of DomainObject 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid',"
      + " because the objects do not belong to the same ClientTransaction.")]
  public void PerformCollectionInsertWithOtherClientTransaction ()
  {
    Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

    ClientTransaction clientTransaction = new ClientTransaction ();
    OrderItem orderItem3 = (OrderItem) clientTransaction.GetObject (DomainObjectIDs.OrderItem3);

    order1.OrderItems.Insert (0, orderItem3);
  }

  [Test]
  [ExpectedException (typeof (ClientTransactionsDifferException), 
      "Cannot remove DomainObject 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid'"
      + " from collection of property 'OrderItems' of DomainObject 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid',"
      + " because the objects do not belong to the same ClientTransaction.")]
  public void PerformCollectionRemoveWithOtherClientTransaction ()
  {
    Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

    ClientTransaction clientTransaction = new ClientTransaction ();
    OrderItem orderItem1 = (OrderItem) clientTransaction.GetObject (DomainObjectIDs.OrderItem1);

    order1.OrderItems.Remove (orderItem1);
  }

  [Test]
  public void PerformCollectionReplaceWithOtherClientTransaction ()
  {
    Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

    ClientTransaction clientTransaction = new ClientTransaction ();
    OrderItem orderItem3 = (OrderItem) clientTransaction.GetObject (DomainObjectIDs.OrderItem3);

    int index = order1.OrderItems.IndexOf (DomainObjectIDs.OrderItem1);

    try
    {
      order1.OrderItems[index] = orderItem3;
    }
    catch (ClientTransactionsDifferException ex)
    {
      string actualMessage = string.Format (
          "Cannot replace DomainObject at position {0} with DomainObject 'OrderItem|0d7196a5-8161-4048-820d-b1bbdabe3293|System.Guid'"
          + " in collection of property 'OrderItems' of DomainObject 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid',"
          + " because the objects do not belong to the same ClientTransaction.",
          index);

      Assert.AreEqual (actualMessage, ex.Message);
      return;
    }

    Assert.Fail ("This test expects a ClientTransactionsDifferException.");
  }

  [Test]
  [ExpectedException (typeof (ClientTransactionsDifferException), 
      "Cannot remove DomainObject 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' from RelationEndPointMap, because it belongs to a different ClientTransaction.")]
  public void PerformDeletionWithOtherClientTransaction ()
  {
    ClientTransaction clientTransaction = new ClientTransaction ();
    Order order1 = (Order) clientTransaction.GetObject (DomainObjectIDs.Order1);

    _map.PerformDelete (order1);
  }
}
}