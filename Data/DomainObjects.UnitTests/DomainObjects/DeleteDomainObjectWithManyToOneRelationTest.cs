using System;
using NUnit.Framework;

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

    _eventReceiver = new SequenceEventReceiver (
        new DomainObject[] {_orderItem, _order},
        new DomainObjectCollection[] {_order.OrderItems});
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

// TODO: Reactivate code below
//  [Test]
//  public void Relations ()
//  {
//    int numberOfOrderItemsBeforeDelete = _order.OrderItems.Count;
//    _orderItem.Delete ();
//
//    Assert.IsNull (_orderItem.Order);
//    Assert.AreEqual (numberOfOrderItemsBeforeDelete - 1, _order.OrderItems.Count);
//    Assert.IsFalse (_order.OrderItems.Contains (_orderItem.ID));
//  }
}
}
