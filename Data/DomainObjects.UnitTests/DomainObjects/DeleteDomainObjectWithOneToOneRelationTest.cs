using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.EventSequence;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class DeleteDomainObjectWithOneToOneRelationTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private OrderTicket _orderTicket;
  private Order _order;
  private SequenceEventReceiver _eventReceiver;

  // construction and disposing

  public DeleteDomainObjectWithOneToOneRelationTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
    _order = _orderTicket.Order;

    _eventReceiver = new SequenceEventReceiver (
        new DomainObject[] {_orderTicket, _order},
        new DomainObjectCollection[] {_order.OrderItems});
  }

  [Test]
  public void DeleteOrderTicketEvents ()
  {
    _orderTicket.Delete ();

    ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_orderTicket, "1. Deleting event of orderTicket"),
      new RelationChangeState (_order, "OrderTicket", _orderTicket, null, "2. Relation changing event of order"),
      new ObjectDeletionState (_orderTicket, "3. Deleted event of orderItem"),
      new RelationChangeState (_order, "OrderTicket", null, null, "4. Relation changed event of order"),
    };

    _eventReceiver.Compare (expectedStates);
  }

  [Test]
  public void DeleteComputerWithoutEmployeeEvents ()
  {
    Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);

    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (
        new DomainObject[] {computer}, new DomainObjectCollection[0]);

    computer.Delete ();

    ChangeState[] expectedStates = new ChangeState [] 
    {
      new ObjectDeletionState (computer, "1. Deleting of computer"),
      new ObjectDeletionState (computer, "2. Deleted of computer")
    };

    eventReceiver.Compare (expectedStates); 
  }

  [Test]
  public void OrderTicketCancelsDeleteEvent ()
  {
    _eventReceiver.CancelEventNumber = 1;
    
    _orderTicket.Delete ();

    ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_orderTicket, "1. Deleting event of orderTicket")
    };

    _eventReceiver.Compare (expectedStates);
  }

  [Test]
  public void OrderCancelsRelationChangeEvent ()
  {
    _eventReceiver.CancelEventNumber = 2;
    
    _orderTicket.Delete ();

    ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_orderTicket, "1. Deleting event of orderTicket"),
      new RelationChangeState (_order, "OrderTicket", _orderTicket, null, "2. Relation changing event of order")
    };

    _eventReceiver.Compare (expectedStates);

  }

  [Test]
  public void DomainObjectWithNonVirtualProperty ()
  {
    _orderTicket.Delete ();

    Assert.IsNull (_orderTicket.Order);
    Assert.IsNull (_order.OrderTicket);
    Assert.IsNull (_orderTicket.DataContainer["Order"]);
    Assert.AreEqual (StateType.Changed, _order.State);
    Assert.AreEqual (StateType.Original, _order.DataContainer.State);
  }

// TODO: Implement this test:
//  [Test]
//  public void DomainObjectWithVirtualProperty ()
//  {
//    _order.Delete ();
//
//    Assert.IsNull (_orderTicket.Order);
//    Assert.IsNull (_order.OrderTicket);
//    Assert.IsNull (_orderTicket.DataContainer["Order"]);
//    Assert.AreEqual (StateType.Changed, _orderTicket.DataContainer.State);
//  }
}
}
