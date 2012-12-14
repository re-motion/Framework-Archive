using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class OneToManyRelationChangeTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private Customer _oldCustomer;
  private Customer _newCustomer;
  private Order _order;

  // construction and disposing

  public OneToManyRelationChangeTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _oldCustomer = Customer.GetObject (DomainObjectIDs.Customer1);
    _newCustomer = Customer.GetObject (DomainObjectIDs.Customer2);
    _order = Order.GetObject (DomainObjectIDs.Order1);
  }

  [Test]
  public void ChangeEvents ()
  {
    DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order};

    DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders}; 

    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources);
           
    _newCustomer.Orders.Add (_order);

    ChangeState[] expectedChangeStates = new ChangeState[]
    {
      new RelationChangeState (_order, "Customer", _oldCustomer, _newCustomer, "1. Changing event of order from old to new customer"),
      new CollectionChangeState (_oldCustomer.Orders, _order, "2. Removing of orders of old customer"),
      new RelationChangeState (_oldCustomer, "Orders", _order, null, "3. Changing event of old customer"),
      new CollectionChangeState (_newCustomer.Orders, _order, "4. Adding event of new customer's order collection"),
      new RelationChangeState (_newCustomer, "Orders", null, _order, "5. Changing event of new customer"),
      new RelationChangeState (_order, "Customer", null, null, "6. Changed event of order from old to new customer"),
      new CollectionChangeState (_oldCustomer.Orders, _order, "7. Removed event of old customer's order collection"),
      new RelationChangeState (_oldCustomer, "Orders", null, null, "8. Changed event of old customer"),
      new CollectionChangeState (_newCustomer.Orders, _order, "9. Added event of new customer's order collection"),
      new RelationChangeState (_newCustomer, "Orders", null, null, "10. Changed event of new customer")
    };      

    eventReceiver.Check (expectedChangeStates);

    Assert.AreEqual (StateType.Changed, _order.State);
    Assert.AreEqual (StateType.Changed, _oldCustomer.State);
    Assert.AreEqual (StateType.Changed, _newCustomer.State);

    Assert.AreSame (_newCustomer, _order.Customer);
    Assert.IsNull (_oldCustomer.Orders[_order.ID]);
    Assert.AreSame (_order, _newCustomer.Orders[_order.ID]);
  }

  [Test]
  public void OrderCancelsChangeEvent ()
  {
    DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order};

    DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders}; 

    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 1);

    try
    { 
      _newCustomer.Orders.Add (_order);
      Assert.Fail ("EventReceiverCancelException should be raised.");
    }
    catch (EventReceiverCancelException)
    {
      ChangeState[] expectedChangeStates = new ChangeState[]
      {
        new RelationChangeState (_order, "Customer", _oldCustomer, _newCustomer, "1. Changing event of order from old to new customer")
      };      

      eventReceiver.Check (expectedChangeStates);

      Assert.AreEqual (StateType.Unchanged, _order.State);
      Assert.AreEqual (StateType.Unchanged, _oldCustomer.State);
      Assert.AreEqual (StateType.Unchanged, _newCustomer.State);

      Assert.AreSame (_oldCustomer, _order.Customer);
      Assert.AreSame (_order, _oldCustomer.Orders[_order.ID]);
      Assert.IsNull (_newCustomer.Orders[_order.ID]);
    }
  }

  [Test]
  public void OldCustomerCollectionCancelsChangeEvent ()
  {
    DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order};

    DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders}; 

    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 2);

    try
    { 
      _newCustomer.Orders.Add (_order);
      Assert.Fail ("EventReceiverCancelException should be raised.");
    }
    catch (EventReceiverCancelException)
    {
      ChangeState[] expectedChangeStates = new ChangeState[]
      {
        new RelationChangeState (_order, "Customer", _oldCustomer, _newCustomer, "1. Changing event of order from old to new customer"),
        new CollectionChangeState (_oldCustomer.Orders, _order, "2. Removing of orders of old customer")
      };      

      eventReceiver.Check (expectedChangeStates);

      Assert.AreEqual (StateType.Unchanged, _order.State);
      Assert.AreEqual (StateType.Unchanged, _oldCustomer.State);
      Assert.AreEqual (StateType.Unchanged, _newCustomer.State);

      Assert.AreSame (_oldCustomer, _order.Customer);
      Assert.AreSame (_order, _oldCustomer.Orders[_order.ID]);
      Assert.IsNull (_newCustomer.Orders[_order.ID]);
    }
  }

  [Test]
  public void OldCustomerCancelsChangeEvent ()
  {
    DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order};

    DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders}; 

    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 3);
           
    try
    {
      _newCustomer.Orders.Add (_order);
      Assert.Fail ("EventReceiverCancelException should be raised.");
    }
    catch (EventReceiverCancelException)
    {
      ChangeState[] expectedChangeStates = new ChangeState[]
      {
        new RelationChangeState (_order, "Customer", _oldCustomer, _newCustomer, "1. Changing event of order from old to new customer"),
        new CollectionChangeState (_oldCustomer.Orders, _order, "2. Removing of orders of old customer"),
        new RelationChangeState (_oldCustomer, "Orders", _order, null, "3. Changing event of old customer"),
      };      

      eventReceiver.Check (expectedChangeStates);

      Assert.AreEqual (StateType.Unchanged, _order.State);
      Assert.AreEqual (StateType.Unchanged, _oldCustomer.State);
      Assert.AreEqual (StateType.Unchanged, _newCustomer.State);

      Assert.AreSame (_oldCustomer, _order.Customer);
      Assert.AreSame (_order, _oldCustomer.Orders[_order.ID]);
      Assert.IsNull (_newCustomer.Orders[_order.ID]);
    }
  }

  [Test]
  public void NewCustomerCollectionCancelsChangeEvent ()
  {
    DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order};

    DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders}; 

    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 4);
           
    try
    {
      _newCustomer.Orders.Add (_order);
      Assert.Fail ("EventReceiverCancelException should be raised.");
    }
    catch (EventReceiverCancelException)
    {
      ChangeState[] expectedChangeStates = new ChangeState[]
      {
        new RelationChangeState (_order, "Customer", _oldCustomer, _newCustomer, "1. Changing event of order from old to new customer"),
        new CollectionChangeState (_oldCustomer.Orders, _order, "2. Removing of orders of old customer"),
        new RelationChangeState (_oldCustomer, "Orders", _order, null, "3. Changing event of old customer"),
        new CollectionChangeState (_newCustomer.Orders, _order, "4. Adding event of new customer's order collection")
      };      

      eventReceiver.Check (expectedChangeStates);

      Assert.AreEqual (StateType.Unchanged, _order.State);
      Assert.AreEqual (StateType.Unchanged, _oldCustomer.State);
      Assert.AreEqual (StateType.Unchanged, _newCustomer.State);

      Assert.AreSame (_oldCustomer, _order.Customer);
      Assert.AreSame (_order, _oldCustomer.Orders[_order.ID]);
      Assert.IsNull (_newCustomer.Orders[_order.ID]);
    }
  }

  [Test]
  public void NewCustomerCancelsChangeEvent ()
  {
    DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order};

    DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders}; 

    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 5);
           
    try
    {
      _newCustomer.Orders.Add (_order);
      Assert.Fail ("EventReceiverCancelException should be raised.");
    }
    catch (EventReceiverCancelException)
    {
      ChangeState[] expectedChangeStates = new ChangeState[]
      {
        new RelationChangeState (_order, "Customer", _oldCustomer, _newCustomer, "1. Changing event of order from old to new customer"),
        new CollectionChangeState (_oldCustomer.Orders, _order, "2. Removing of orders of old customer"),
        new RelationChangeState (_oldCustomer, "Orders", _order, null, "3. Changing event of old customer"),
        new CollectionChangeState (_newCustomer.Orders, _order, "4. Adding event of new customer's order collection"),
        new RelationChangeState (_newCustomer, "Orders", null, _order, "5. Changing event of new customer")
      };      

      eventReceiver.Check (expectedChangeStates);

      Assert.AreEqual (StateType.Unchanged, _order.State);
      Assert.AreEqual (StateType.Unchanged, _oldCustomer.State);
      Assert.AreEqual (StateType.Unchanged, _newCustomer.State);

      Assert.AreSame (_oldCustomer, _order.Customer);
      Assert.AreSame (_order, _oldCustomer.Orders[_order.ID]);
      Assert.IsNull (_newCustomer.Orders[_order.ID]);
    }
  }

  [Test]
  public void StateTracking ()
  {
    _newCustomer.Orders.Add (_order);

    Assert.AreEqual (StateType.Changed, _order.State);
    Assert.AreEqual (StateType.Changed, _oldCustomer.State);
    Assert.AreEqual (StateType.Changed, _newCustomer.State);
  }

  [Test]
  public void ChangeWithInheritance ()
  {
    IndustrialSector industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
    Partner partner = Partner.GetObject (DomainObjectIDs.Partner2);

    Assert.IsNull (industrialSector.Companies[partner.ID]);
    Assert.IsFalse (object.ReferenceEquals (industrialSector, partner.IndustrialSector));

    industrialSector.Companies.Add (partner);

    Assert.IsNotNull (industrialSector.Companies[partner.ID]);
    Assert.AreSame (industrialSector, partner.IndustrialSector);
  }

  [Test]
  public void SetNewCustomerThroughOrder ()
  {
    _order.Customer = _newCustomer;

    Assert.AreEqual (StateType.Changed, _order.State);
    Assert.AreEqual (StateType.Changed, _oldCustomer.State);
    Assert.AreEqual (StateType.Changed, _newCustomer.State);

    Assert.AreSame (_newCustomer, _order.Customer);
    Assert.IsNull (_oldCustomer.Orders[_order.ID]);
    Assert.AreSame (_order, _newCustomer.Orders[_order.ID]);
  }

  [Test]
  public void ChangeRelationBackToOriginalValue ()
  {
    _order.Customer = _newCustomer;
    Assert.AreEqual (StateType.Changed, _order.State); 
    Assert.AreEqual (StateType.Changed, _oldCustomer.State);
    Assert.AreEqual (StateType.Changed, _newCustomer.State);

    _order.Customer = _oldCustomer;
    Assert.AreEqual (StateType.Unchanged, _order.State); 
    Assert.AreEqual (StateType.Unchanged, _oldCustomer.State);
    Assert.AreEqual (StateType.Unchanged, _newCustomer.State);
  }

  [Test]
  public void GetOriginalRelatedObject ()
  {
    _order.Customer = _newCustomer;

    Assert.AreSame (_newCustomer, _order.Customer);
    Assert.AreSame (_oldCustomer, _order.GetOriginalRelatedObject ("Customer")); 
  }

  [Test]
  public void GetOriginalRelatedObjects ()
  {
    Assert.IsNull (_newCustomer.Orders[_order.ID]);

    _newCustomer.Orders.Add (_order);

    DomainObjectCollection oldOrders = _newCustomer.GetOriginalRelatedObjects ("Orders");
    Assert.AreSame (_order, _newCustomer.Orders[_order.ID]);
    Assert.IsNull (oldOrders[_order.ID]);
  }

  [Test]
  public void GetOriginalRelatedObjectsWithLazyLoad ()
  {
    Employee supervisor = Employee.GetObject (DomainObjectIDs.Employee1);
    DomainObjectCollection subordinates = supervisor.GetOriginalRelatedObjects ("Subordinates");

    Assert.AreEqual (2, subordinates.Count);  
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void CheckRequiredItemTypeForExisting ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    DomainObjectCollection orderItems = order.OrderItems;

    orderItems.Add (new Customer ());
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void CheckRequiredItemTypeForNew ()
  {
    Order order = new Order ();
    DomainObjectCollection orderItems = order.OrderItems;

    orderItems.Add (new Customer ());
  }
}
}
