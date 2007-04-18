using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class DeleteDomainObjectWithOneToOneRelationTest : ClientTransactionBaseTest
  {
    private OrderTicket _orderTicket;
    private Order _order;
    private SequenceEventReceiver _eventReceiver;

    public override void SetUp ()
    {
      base.SetUp ();

      _orderTicket = DomainObject.GetObject<OrderTicket> (DomainObjectIDs.OrderTicket1);
      _order = _orderTicket.Order;

      _eventReceiver = CreateEventReceiver ();
    }

    [Test]
    public void DeleteOrderTicketEvents ()
    {
      _orderTicket.Delete ();

      ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_orderTicket, "1. Deleting event of orderTicket"),
      new RelationChangeState (_order, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _orderTicket, null, "2. Relation changing event of order"),
      new RelationChangeState (_order, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", null, null, "3. Relation changed event of order"),
      new ObjectDeletionState (_orderTicket, "4. Deleted event of orderTicket")
    };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void DeleteComputerWithoutEmployeeEvents ()
    {
      Computer computer = DomainObject.GetObject<Computer> (DomainObjectIDs.Computer4);

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { computer }, new DomainObjectCollection[0]);

      computer.Delete ();

      ChangeState[] expectedStates = new ChangeState[] 
    {
      new ObjectDeletionState (computer, "1. Deleting of computer"),
      new ObjectDeletionState (computer, "2. Deleted of computer")
    };

      eventReceiver.Check (expectedStates);
    }

    [Test]
    public void OrderTicketCancelsDeleteEvent ()
    {
      _eventReceiver.CancelEventNumber = 1;

      try
      {
        _orderTicket.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[] { new ObjectDeletionState (_orderTicket, "1. Deleting event of orderTicket") };

        _eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void OrderCancelsRelationChangeEvent ()
    {
      _eventReceiver.CancelEventNumber = 2;

      try
      {
        _orderTicket.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[]
            {
              new ObjectDeletionState (_orderTicket, "1. Deleting event of orderTicket"),
              new RelationChangeState (_order, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _orderTicket, null, "2. Relation changing event of order")
            };

        _eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void NonVirtualProperty ()
    {
      _orderTicket.Delete ();

      Assert.IsNull (_orderTicket.Order);
      Assert.IsNull (_order.OrderTicket);
      Assert.IsNull (_orderTicket.DataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"]);
      Assert.AreEqual (StateType.Changed, _order.State);
      Assert.AreEqual (StateType.Unchanged, _order.DataContainer.State);
    }

    [Test]
    public void VirtualProperty ()
    {
      _order.Delete ();

      Assert.IsNull (_orderTicket.Order);
      Assert.IsNull (_order.OrderTicket);
      Assert.IsNull (_orderTicket.DataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"]);
      Assert.AreEqual (StateType.Changed, _orderTicket.DataContainer.State);
    }

    [Test]
    public void ChangeNonVirtualPropertyBeforeDeletion ()
    {
      _orderTicket.Order = null;
      _eventReceiver = CreateEventReceiver ();

      _orderTicket.Delete ();

      ChangeState[] expectedStates = new ChangeState[]
          {
            new ObjectDeletionState (_orderTicket, "1. Deleting event of orderTicket"),
            new ObjectDeletionState (_orderTicket, "2. Deleted event of orderTicket"),
          };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void ChangeVirtualPropertyBeforeDeletion ()
    {
      _order.OrderTicket = null;
      _eventReceiver = CreateEventReceiver ();

      _order.Delete ();

      ChangeState[] expectedStates = new ChangeState[]
          {
            new ObjectDeletionState (_order, "1. Deleting event of ordert"),
            new ObjectDeletionState (_order, "2. Deleted event of order"),
          };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void GetOriginalRelatedObjectFromDeletedObject ()
    {
      _orderTicket.Delete ();

      Order originalOrder = (Order) _orderTicket.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");

      Assert.IsNotNull (originalOrder);
      Assert.AreEqual (DomainObjectIDs.Order1, originalOrder.ID);
    }

    [Test]
    public void GetOriginalRelatedObjectFromOppositeObject ()
    {
      Order oldRelatedOrder = _orderTicket.Order;
      _orderTicket.Delete ();

      OrderTicket deletedOrderTicket = (OrderTicket) oldRelatedOrder.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      Assert.IsNotNull (deletedOrderTicket);
      Assert.AreEqual (_orderTicket.ID, deletedOrderTicket.ID);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void SetRelatedObjectOfDeletedObject ()
    {
      _orderTicket.Delete ();

      _orderTicket.Order = _order;
    }

    [Test]
    public void GetOriginalRelatedObjectForBothDeleted ()
    {
      OrderTicket orderTicket = _order.OrderTicket;
      _order.Delete ();
      orderTicket.Delete ();

      Assert.IsNotNull (_order.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
      Assert.IsNotNull (orderTicket.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void ReassignDeletedObject ()
    {
      _orderTicket.Delete ();

      _order.OrderTicket = _orderTicket;
    }

    private SequenceEventReceiver CreateEventReceiver ()
    {
      return new SequenceEventReceiver (
          new DomainObject[] { _orderTicket, _order },
          new DomainObjectCollection[] { _order.OrderItems });
    }
  }
}
