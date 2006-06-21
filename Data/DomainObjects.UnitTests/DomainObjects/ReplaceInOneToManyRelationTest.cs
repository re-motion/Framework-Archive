using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class ReplaceInOneToManyRelationTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    private Customer _customer;
    private Customer _oldCustomerOfNewOrder;
    private Order _oldOrder;
    private Order _newOrder;

    // construction and disposing

    public ReplaceInOneToManyRelationTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _customer = Customer.GetObject (DomainObjectIDs.Customer1);
      _oldCustomerOfNewOrder = Customer.GetObject (DomainObjectIDs.Customer3);
      _oldOrder = Order.GetObject (DomainObjectIDs.Order1);
      _newOrder = Order.GetObject (DomainObjectIDs.Order2);
    }

    [Test]
    public void Events ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);
      _customer.Orders[replaceIndex] = _newOrder;

      ChangeState[] expectedChangeStates = new ChangeState[]
    {
      new RelationChangeState (_oldOrder, "Customer", _customer, null, "1. Changing event of old order from old customer to null"),
      new RelationChangeState (_newOrder, "Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer"),
      new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders"),
      new CollectionChangeState (_customer.Orders, _newOrder, "4. Adding event of new order to customer.Orders"),
      new RelationChangeState (_customer, "Orders", _oldOrder, _newOrder, "5. Changing event of customer"),

      new CollectionChangeState (_oldCustomerOfNewOrder.Orders, _newOrder, "6. Removing event of new order from oldCustomerOfNewOrder.Orders"),
      new RelationChangeState (_oldCustomerOfNewOrder, "Orders", _newOrder, null, "7. Changing event of oldCustomerOfNewOrder"),

      new RelationChangeState (_oldOrder, "Customer", null, null, "8. Changed event of old order from old customer to null"),
      new RelationChangeState (_newOrder, "Customer", null, null, "9. Changed event of new order from null to new customer"),
      new CollectionChangeState (_customer.Orders, _oldOrder, "10. Removed event of old order from orders"),
      new CollectionChangeState (_customer.Orders, _newOrder, "11. Added event of new order to orders"),
      new RelationChangeState (_customer, "Orders", null, null, "12. Changed event of customer"),

      new CollectionChangeState (_oldCustomerOfNewOrder.Orders, _newOrder, "13. Removed event of new order from oldCustomerOfNewOrder.Orders"),
      new RelationChangeState (_oldCustomerOfNewOrder, "Orders", null, null, "14. Changed event of oldCustomerOfNewOrder")
    };

      eventReceiver.Check (expectedChangeStates);

      Assert.AreEqual (StateType.Changed, _customer.State);
      Assert.AreEqual (StateType.Changed, _oldCustomerOfNewOrder.State);
      Assert.AreEqual (StateType.Changed, _oldOrder.State);
      Assert.AreEqual (StateType.Changed, _newOrder.State);

      Assert.AreSame (_newOrder, _customer.Orders[replaceIndex]);
      Assert.AreSame (_customer, _newOrder.Customer);

      Assert.IsFalse (_customer.Orders.ContainsObject (_oldOrder));
      Assert.IsNull (_oldOrder.Customer);

      Assert.IsFalse (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder));
    }

    [Test]
    public void EventsWithoutOldCustomerOfNewOrder ()
    {
      Order newOrder = new Order ();
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);
      _customer.Orders[replaceIndex] = newOrder;

      ChangeState[] expectedChangeStates = new ChangeState[]
    {
      new RelationChangeState (_oldOrder, "Customer", _customer, null, "1. Changing event of old order from old customer to null"),
      new RelationChangeState (newOrder, "Customer", null, _customer, "2. Changing event of new order from null to new customer"),
      new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders"),
      new CollectionChangeState (_customer.Orders, newOrder, "4. Adding event of new order to customer.Orders"),
      new RelationChangeState (_customer, "Orders", _oldOrder, newOrder, "5. Changing event of customer"),

      new RelationChangeState (_oldOrder, "Customer", null, null, "6. Changed event of old order from old customer to null"),
      new RelationChangeState (newOrder, "Customer", null, null, "7. Changed event of new order from null to new customer"),
      new CollectionChangeState (_customer.Orders, _oldOrder, "8. Removed event of old order from orders"),
      new CollectionChangeState (_customer.Orders, newOrder, "9. Added event of new order to orders"),
      new RelationChangeState (_customer, "Orders", null, null, "10. Changed event of customer"),
    };

      eventReceiver.Check (expectedChangeStates);

      Assert.AreEqual (StateType.Changed, _customer.State);
      Assert.AreEqual (StateType.Unchanged, _oldCustomerOfNewOrder.State);
      Assert.AreEqual (StateType.Changed, _oldOrder.State);
      Assert.AreEqual (StateType.New, newOrder.State);
      Assert.AreEqual (_customer.ID, newOrder.DataContainer.GetObjectID ("Customer"));

      Assert.AreSame (newOrder, _customer.Orders[replaceIndex]);
      Assert.AreSame (_customer, newOrder.Customer);

      Assert.IsFalse (_customer.Orders.ContainsObject (_oldOrder));
      Assert.IsNull (_oldOrder.Customer);
    }

    [Test]
    public void OldOrderCancelsReplace ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 1);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            { new RelationChangeState (_oldOrder, "Customer", _customer, null, "1. Changing event of old order from old customer to null")};

        eventReceiver.Check (expectedChangeStates);

        Assert.AreEqual (StateType.Unchanged, _customer.State);
        Assert.AreEqual (StateType.Unchanged, _oldCustomerOfNewOrder.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrder.State);
        Assert.AreEqual (StateType.Unchanged, _newOrder.State);

        Assert.AreSame (_oldOrder, _customer.Orders[replaceIndex]);
        Assert.AreSame (_customer, _oldOrder.Customer);

        Assert.IsTrue (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder));
        Assert.AreSame (_oldCustomerOfNewOrder, _newOrder.Customer);
      }
    }

    [Test]
    public void NewOrderCancelsReplace ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 2);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_oldOrder, "Customer", _customer, null, "1. Changing event of old order from old customer to null"),
              new RelationChangeState (_newOrder, "Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.AreEqual (StateType.Unchanged, _customer.State);
        Assert.AreEqual (StateType.Unchanged, _oldCustomerOfNewOrder.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrder.State);
        Assert.AreEqual (StateType.Unchanged, _newOrder.State);

        Assert.AreSame (_oldOrder, _customer.Orders[replaceIndex]);
        Assert.AreSame (_customer, _oldOrder.Customer);

        Assert.IsTrue (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder));
        Assert.AreSame (_oldCustomerOfNewOrder, _newOrder.Customer);
      }
    }

    [Test]
    public void NewOrderCollectionCancelsRemove ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 3);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_oldOrder, "Customer", _customer, null, "1. Changing event of old order from old customer to null"),
              new RelationChangeState (_newOrder, "Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer"),
              new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.AreEqual (StateType.Unchanged, _customer.State);
        Assert.AreEqual (StateType.Unchanged, _oldCustomerOfNewOrder.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrder.State);
        Assert.AreEqual (StateType.Unchanged, _newOrder.State);

        Assert.AreSame (_oldOrder, _customer.Orders[replaceIndex]);
        Assert.AreSame (_customer, _oldOrder.Customer);

        Assert.IsTrue (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder));
        Assert.AreSame (_oldCustomerOfNewOrder, _newOrder.Customer);
      }
    }

    [Test]
    public void NewOrderCollectionCancelsAdd ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 4);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_oldOrder, "Customer", _customer, null, "1. Changing event of old order from old customer to null"),
              new RelationChangeState (_newOrder, "Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer"),
              new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders"),
              new CollectionChangeState (_customer.Orders, _newOrder, "4. Adding event of new order to customer.Orders")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.AreEqual (StateType.Unchanged, _customer.State);
        Assert.AreEqual (StateType.Unchanged, _oldCustomerOfNewOrder.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrder.State);
        Assert.AreEqual (StateType.Unchanged, _newOrder.State);

        Assert.AreSame (_oldOrder, _customer.Orders[replaceIndex]);
        Assert.AreSame (_customer, _oldOrder.Customer);

        Assert.IsTrue (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder));
        Assert.AreSame (_oldCustomerOfNewOrder, _newOrder.Customer);
      }
    }

    [Test]
    public void NewCustomerCancelsReplace ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 5);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_oldOrder, "Customer", _customer, null, "1. Changing event of old order from old customer to null"),
              new RelationChangeState (_newOrder, "Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer"),
              new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders"),
              new CollectionChangeState (_customer.Orders, _newOrder, "4. Adding event of new order to customer.Orders"),
              new RelationChangeState (_customer, "Orders", _oldOrder, _newOrder, "5. Changing event of customer")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.AreEqual (StateType.Unchanged, _customer.State);
        Assert.AreEqual (StateType.Unchanged, _oldCustomerOfNewOrder.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrder.State);
        Assert.AreEqual (StateType.Unchanged, _newOrder.State);

        Assert.AreSame (_oldOrder, _customer.Orders[replaceIndex]);
        Assert.AreSame (_customer, _oldOrder.Customer);

        Assert.IsTrue (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder));
        Assert.AreSame (_oldCustomerOfNewOrder, _newOrder.Customer);
      }
    }

    [Test]
    public void OldOrderCollectionCancelsRemove ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 6);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_oldOrder, "Customer", _customer, null, "1. Changing event of old order from old customer to null"),
              new RelationChangeState (_newOrder, "Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer"),
              new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders"),
              new CollectionChangeState (_customer.Orders, _newOrder, "4. Adding event of new order to customer.Orders"),
              new RelationChangeState (_customer, "Orders", _oldOrder, _newOrder, "5. Changing event of customer"),
              new CollectionChangeState (_oldCustomerOfNewOrder.Orders, _newOrder, "6. Removing event of new order from oldCustomerOfNewOrder.Orders")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.AreEqual (StateType.Unchanged, _customer.State);
        Assert.AreEqual (StateType.Unchanged, _oldCustomerOfNewOrder.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrder.State);
        Assert.AreEqual (StateType.Unchanged, _newOrder.State);

        Assert.AreSame (_oldOrder, _customer.Orders[replaceIndex]);
        Assert.AreSame (_customer, _oldOrder.Customer);

        Assert.IsTrue (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder));
        Assert.AreSame (_oldCustomerOfNewOrder, _newOrder.Customer);
      }
    }

    [Test]
    public void OldCustomerCancelsRemove ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver =
          new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 7);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);

      try
      {
        _customer.Orders[replaceIndex] = _newOrder;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_oldOrder, "Customer", _customer, null, "1. Changing event of old order from old customer to null"),
              new RelationChangeState (_newOrder, "Customer", _oldCustomerOfNewOrder, _customer, "2. Changing event of new order from null to new customer"),
              new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from customer.Orders"),
              new CollectionChangeState (_customer.Orders, _newOrder, "4. Adding event of new order to customer.Orders"),
              new RelationChangeState (_customer, "Orders", _oldOrder, _newOrder, "5. Changing event of customer"),
              new CollectionChangeState (_oldCustomerOfNewOrder.Orders, _newOrder, "6. Removing event of new order from oldCustomerOfNewOrder.Orders"),
              new RelationChangeState (_oldCustomerOfNewOrder, "Orders", _newOrder, null, "7. Changing event of oldCustomerOfNewOrder")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.AreEqual (StateType.Unchanged, _customer.State);
        Assert.AreEqual (StateType.Unchanged, _oldCustomerOfNewOrder.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrder.State);
        Assert.AreEqual (StateType.Unchanged, _newOrder.State);

        Assert.AreSame (_oldOrder, _customer.Orders[replaceIndex]);
        Assert.AreSame (_customer, _oldOrder.Customer);

        Assert.IsTrue (_oldCustomerOfNewOrder.Orders.ContainsObject (_newOrder));
        Assert.AreSame (_oldCustomerOfNewOrder, _newOrder.Customer);
      }
    }

    [Test]
    public void ReplaceWithSameObject ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] { _customer, _oldCustomerOfNewOrder, _oldOrder, _newOrder };
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] { _customer.Orders, _oldCustomerOfNewOrder.Orders };

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources);

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);
      _customer.Orders[replaceIndex] = _oldOrder;

      ChangeState[] expectedChangeStates = new ChangeState[0];
      eventReceiver.Check (expectedChangeStates);
    }

    [Test]
    public void ReplaceWithObjectAlreadyInCollection ()
    {
      try
      {
        _customer.Orders[0] = _customer.Orders[1];
        Assert.Fail ("Expected test to raise exception.");
      }
      catch (InvalidOperationException e)
      {
        string expectedMessage = string.Format ("The object '{0}' is already part of this collection.", _customer.Orders[1].ID);
        Assert.AreEqual (expectedMessage, e.Message);
      }
    }

    [Test]
    public void ChangeEventsWithOldRelatedObjectNotLoaded ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order3);

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { _oldOrder, newOrder, _customer },
          new DomainObjectCollection[] { _customer.Orders });

      int replaceIndex = _customer.Orders.IndexOf (_oldOrder);
      _customer.Orders[replaceIndex] = newOrder;

      Assert.AreSame (_customer, newOrder.Customer);
      Assert.IsTrue (_customer.Orders.ContainsObject (newOrder));

      Customer oldCustomerOfNewOrder = Customer.GetObject (DomainObjectIDs.Customer4);

      Assert.IsFalse (oldCustomerOfNewOrder.Orders.ContainsObject (newOrder));

      ChangeState[] expectedStates = new ChangeState[]
    {
      new RelationChangeState (_oldOrder, "Customer", _customer, null, "1. Changing event of old order from new customer to null"),
      new RelationChangeState (newOrder, "Customer", oldCustomerOfNewOrder, _customer, "2. Changing event of new order from old to new customer"),
      new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of new customer's order collection"),
      new CollectionChangeState (_customer.Orders, newOrder, "4. Adding event of new customer's order collection"),
      new RelationChangeState (_customer, "Orders", _oldOrder, newOrder, "5. Changing event of new customer from old order to new order"),

      new RelationChangeState (_oldOrder, "Customer", null, null, "6. Changed event of old order from new customer to null"),
      new RelationChangeState (newOrder, "Customer", null, null, "7. Changed event of new order from old to new customer"),
      new CollectionChangeState (_customer.Orders, _oldOrder, "8. Removed event of new customer's order collection"),
      new CollectionChangeState (_customer.Orders, newOrder, "9. Added event of new customer's order collection"),
      new RelationChangeState (_customer, "Orders", null, null, "10. Changed event of new customer from old order to new order"),
    };

      eventReceiver.Check (expectedStates);

    }
  }
}
