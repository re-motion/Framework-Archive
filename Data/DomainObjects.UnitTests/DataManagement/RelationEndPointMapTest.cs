using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class RelationEndPointMapTest : ClientTransactionBaseTest
  {
    private RelationEndPointMap _map;

    public override void SetUp ()
    {
      base.SetUp ();

      _map = ClientTransactionMock.DataManager.RelationEndPointMap;
    }

    [Test]
    public void DeleteNew ()
    {
      Order newOrder = Order.NewObject ();
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
      RelationEndPointID endPointID = new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      DomainObjectCollection originalOrderItems = _map.GetOriginalRelatedObjects (endPointID);
      DomainObjectCollection orderItems = _map.GetRelatedObjects (endPointID);

      Assert.IsFalse (ReferenceEquals (originalOrderItems, orderItems));
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      RelationEndPointID endPointID = new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      DomainObject originalOrderTicket = _map.GetOriginalRelatedObject (endPointID);
      DomainObject orderTicket = _map.GetRelatedObject (endPointID);

      Assert.IsTrue (ReferenceEquals (originalOrderTicket, orderTicket));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetRelatedObject can only be called for end points with a cardinality of 'One'.\r\nParameter name: endPointID")]
    public void GetRelatedObjectWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetRelatedObject (new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetOriginalRelatedObject can only be called for end points with a cardinality of 'One'.\r\nParameter name: endPointID")]
    public void GetOriginalRelatedObjectWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetOriginalRelatedObject (new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "SetRelatedObject can only be called for end points with a cardinality of 'One'.\r\nParameter name: endPointID")]
    public void SetRelatedObjectWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.SetRelatedObject (new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"), OrderItem.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetRelatedObjects can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
    public void GetRelatedObjectsWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetRelatedObjects (new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetOriginalRelatedObjects can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
    public void GetOriginalRelatedObjectsWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetOriginalRelatedObjects (new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        "Property 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' of DomainObject "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be set to DomainObject "
        + "'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid', because the objects do not belong to the same ClientTransaction.")]
    public void SetRelatedObjectWithOtherClientTransaction ()
    {
      Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

      ClientTransaction clientTransaction = new ClientTransaction ();
      OrderTicket orderTicket2 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2, clientTransaction);

      _map.SetRelatedObject (new RelationEndPointID (order1.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"), orderTicket2);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        "Cannot insert DomainObject 'OrderItem|0d7196a5-8161-4048-820d-b1bbdabe3293|System.Guid' at position 2 into collection of property "
        + "'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' of DomainObject "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', because the objects do not belong to the same ClientTransaction.")]
    public void PerformCollectionAddWithOtherClientTransaction ()
    {
      Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

      ClientTransaction clientTransaction = new ClientTransaction ();
      OrderItem orderItem3 = OrderItem.GetObject (DomainObjectIDs.OrderItem3, clientTransaction);

      order1.OrderItems.Add (orderItem3);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        "Cannot insert DomainObject 'OrderItem|0d7196a5-8161-4048-820d-b1bbdabe3293|System.Guid' at position 0 into collection of property "
        + "'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' of DomainObject "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', because the objects do not belong to the same ClientTransaction.")]
    public void PerformCollectionInsertWithOtherClientTransaction ()
    {
      Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

      ClientTransaction clientTransaction = new ClientTransaction ();
      OrderItem orderItem3 = OrderItem.GetObject (DomainObjectIDs.OrderItem3, clientTransaction);

      order1.OrderItems.Insert (0, orderItem3);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        "Cannot remove DomainObject 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' from collection of property "
        + "'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' of DomainObject "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', because the objects do not belong to the same ClientTransaction.")]
    public void PerformCollectionRemoveWithOtherClientTransaction ()
    {
      Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

      ClientTransaction clientTransaction = new ClientTransaction ();
      OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1, clientTransaction);

      order1.OrderItems.Remove (orderItem1);
    }

    [Test]
    public void PerformCollectionReplaceWithOtherClientTransaction ()
    {
      Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

      ClientTransaction clientTransaction = new ClientTransaction ();
      OrderItem orderItem3 = OrderItem.GetObject (DomainObjectIDs.OrderItem3, clientTransaction);

      int index = order1.OrderItems.IndexOf (DomainObjectIDs.OrderItem1);

      try
      {
        order1.OrderItems[index] = orderItem3;
        Assert.Fail ("This test expects a ClientTransactionsDifferException.");
      }
      catch (ClientTransactionsDifferException ex)
      {
        string actualMessage = string.Format (
            "Cannot replace DomainObject at position {0} with DomainObject 'OrderItem|0d7196a5-8161-4048-820d-b1bbdabe3293|System.Guid'"
            + " in collection of property 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' of DomainObject "
            + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', because the objects do not belong to the same ClientTransaction.",
            index);

        Assert.AreEqual (actualMessage, ex.Message);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        "Cannot remove DomainObject 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' from RelationEndPointMap, "
        + "because it belongs to a different ClientTransaction.")]
    public void PerformDeletionWithOtherClientTransaction ()
    {
      ClientTransaction clientTransaction = new ClientTransaction ();
      Order order1 = Order.GetObject (DomainObjectIDs.Order1, clientTransaction);

      _map.PerformDelete (order1);
    }
  }
}