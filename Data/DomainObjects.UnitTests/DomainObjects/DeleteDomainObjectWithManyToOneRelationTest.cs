using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.EventSequence;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class DeleteDomainObjectWithManyToOneRelationTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private OrderItem _orderItem;
  private Order _order;
  private SequenceEventReceiver _eventReceiver;

  // construction and disposing

  public DeleteDomainObjectWithManyToOneRelationTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
    _order = _orderItem.Order;

    _eventReceiver = CreateEventReceiver ();
  }

  [Test]
  public void DeleteOrderItemEvents ()
  {
    _orderItem.Delete ();

    ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem"),
      new CollectionChangeState (_order.OrderItems, _orderItem, "2. Removing event of order.OrderItems"),
      new RelationChangeState (_order, "OrderItems", _orderItem, null, "3. Relation changing event of order"),
      new ObjectDeletionState (_orderItem, "4. Deleted event of orderItem"),
      new CollectionChangeState (_order.OrderItems, _orderItem, "5. Removed event of order.OrderItems"),
      new RelationChangeState (_order, "OrderItems", null, null, "6. Relation changed event of order"),
    };

    _eventReceiver.Compare (expectedStates);
  }

  [Test]
  public void OrderItemCancelsDeleteEvent ()
  {
    _eventReceiver.CancelEventNumber = 1;
    
    _orderItem.Delete ();

    ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem")
    };

    _eventReceiver.Compare (expectedStates);
  }

  [Test]
  public void OrderItemCollectionCancelsRemoveEvent ()
  {
    _eventReceiver.CancelEventNumber = 2;
    
    _orderItem.Delete ();

    ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem"),
      new CollectionChangeState (_order.OrderItems, _orderItem, "2. Removing event of order.OrderItems")
    };

    _eventReceiver.Compare (expectedStates);

  }

  [Test]
  public void OrderCancelsRelationChangeEvent ()
  {
    _eventReceiver.CancelEventNumber = 3;
    
    _orderItem.Delete ();

    ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem"),
      new CollectionChangeState (_order.OrderItems, _orderItem, "2. Removing event of order.OrderItems"),
      new RelationChangeState (_order, "OrderItems", _orderItem, null, "3. Relation changing event of order")
    };

    _eventReceiver.Compare (expectedStates);

  }

  [Test]
  public void Relations ()
  {
    int numberOfOrderItemsBeforeDelete = _order.OrderItems.Count;
    _orderItem.Delete ();

    Assert.IsNull (_orderItem.Order);
    Assert.AreEqual (numberOfOrderItemsBeforeDelete - 1, _order.OrderItems.Count);
    Assert.IsFalse (_order.OrderItems.Contains (_orderItem.ID));
    Assert.IsNull (_orderItem.DataContainer["Order"]);
    Assert.AreEqual (StateType.Changed, _order.State);
    Assert.AreEqual (StateType.Unchanged, _order.DataContainer.State);
  }

  [Test]
  public void ChangePropertyBeforeDeletion ()
  {
    _orderItem.Order = null;
    _eventReceiver = CreateEventReceiver ();

    _orderItem.Delete ();

    ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem"),
      new ObjectDeletionState (_orderItem, "2. Deleted event of orderItem"),
    };

    _eventReceiver.Compare (expectedStates);
  }

  [Test]
  public void GetOriginalRelatedObjects ()
  {
    _orderItem.Delete ();

    DomainObjectCollection originalOrderItems = _order.GetOriginalRelatedObjects ("OrderItems");

    Assert.IsNotNull (originalOrderItems);
    Assert.AreEqual (2, originalOrderItems.Count);
    Assert.AreSame (_orderItem, originalOrderItems[_orderItem.ID]);
  }

  [Test]
  [ExpectedException (typeof (ObjectDeletedException))]
  public void SetRelatedObjectOfDeletedObject ()
  {
    _orderItem.Delete ();

    _orderItem.Order = _order;
  }

  [Test]
  [ExpectedException (typeof (ObjectDeletedException))]
  public void ReassignDeletedObject ()
  {
    _orderItem.Delete ();
    
    _order.OrderItems.Add (_orderItem);
  }

  private SequenceEventReceiver CreateEventReceiver ()
  {
    return new SequenceEventReceiver (
        new DomainObject[] {_orderItem, _order},
        new DomainObjectCollection[] {_order.OrderItems});
  }
}
}
