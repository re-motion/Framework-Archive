using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class OneToOneRelationChangeTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields
  
  private Order _order;
  private OrderTicket _oldOrderTicket;
  private OrderTicket _newOrderTicket;
  private Order _oldOrderOfNewOrderTicket;

  private DomainObjectEventReceiver _orderEventReceiver;
  private DomainObjectEventReceiver _oldOrderTicketEventReceiver;
  private DomainObjectEventReceiver _newOrderTicketEventReceiver;
  private DomainObjectEventReceiver _oldOrderOfNewOrderTicketEventReceiver;

  // construction and disposing

  public OneToOneRelationChangeTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _order = Order.GetObject (DomainObjectIDs.Order1);
    _oldOrderTicket = _order.OrderTicket;
    _newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
    _oldOrderOfNewOrderTicket = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

    _orderEventReceiver = new DomainObjectEventReceiver (_order);
    _oldOrderTicketEventReceiver = new DomainObjectEventReceiver (_oldOrderTicket);
    _newOrderTicketEventReceiver = new DomainObjectEventReceiver (_newOrderTicket);
    _oldOrderOfNewOrderTicketEventReceiver = new DomainObjectEventReceiver (_oldOrderOfNewOrderTicket);
  }

  [Test]
  public void RelationChangeEvents ()
  {
    _orderEventReceiver.Cancel = false;
    _oldOrderTicketEventReceiver.Cancel = false;
    _newOrderTicketEventReceiver.Cancel = false;
    _oldOrderOfNewOrderTicketEventReceiver.Cancel = false;

    _order.OrderTicket = _newOrderTicket;

    Assert.AreEqual (true, _orderEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (true, _orderEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
    Assert.AreEqual ("OrderTicket", _orderEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_oldOrderTicket, _orderEventReceiver.OldRelatedObject);
    Assert.AreSame (_newOrderTicket, _orderEventReceiver.NewRelatedObject);

    Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Order", _oldOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.AreEqual ("Order", _oldOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_order, _oldOrderTicketEventReceiver.OldRelatedObject);
    Assert.AreSame (null, _oldOrderTicketEventReceiver.NewRelatedObject);

    Assert.AreEqual (true, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (true, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Order", _newOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.AreEqual ("Order", _newOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicketEventReceiver.OldRelatedObject);
    Assert.AreSame (_order, _newOrderTicketEventReceiver.NewRelatedObject);

    Assert.AreEqual (true, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (true, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("OrderTicket", _oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.AreEqual ("OrderTicket", _oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicketEventReceiver.OldRelatedObject);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.NewRelatedObject);

    Assert.AreEqual (StateType.Changed, _order.State);
    Assert.AreEqual (StateType.Changed, _newOrderTicket.State);
    Assert.AreEqual (StateType.Changed, _oldOrderTicket.State);
    Assert.AreEqual (StateType.Changed, _oldOrderOfNewOrderTicket.State);

    Assert.AreSame (_newOrderTicket, _order.OrderTicket);
    Assert.AreSame (_order, _newOrderTicket.Order);
    Assert.IsNull (_oldOrderTicket.Order);
    Assert.IsNull (_oldOrderOfNewOrderTicket.OrderTicket);
  }

  [Test]
  public void OrderCancelsRelationChangeEvent ()
  {
    _orderEventReceiver.Cancel = true;
    _oldOrderTicketEventReceiver.Cancel = false;
    _newOrderTicketEventReceiver.Cancel = false;
    _oldOrderOfNewOrderTicketEventReceiver.Cancel = false;

    _order.OrderTicket = _newOrderTicket;

    Assert.AreEqual (true, _orderEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _orderEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_orderEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_oldOrderTicket, _orderEventReceiver.OldRelatedObject);
    Assert.AreSame (_newOrderTicket, _orderEventReceiver.NewRelatedObject);

    Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.IsNull (_oldOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_oldOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_oldOrderTicketEventReceiver.OldRelatedObject);
    Assert.IsNull (_oldOrderTicketEventReceiver.NewRelatedObject);

    Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.IsNull (_newOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_newOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_newOrderTicketEventReceiver.OldRelatedObject);
    Assert.IsNull (_newOrderTicketEventReceiver.NewRelatedObject);

    Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.OldRelatedObject);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.NewRelatedObject);

    Assert.AreEqual (StateType.Unchanged, _order.State);
    Assert.AreEqual (StateType.Unchanged, _newOrderTicket.State);
    Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);
    Assert.AreEqual (StateType.Unchanged, _oldOrderOfNewOrderTicket.State);

    Assert.AreSame (_oldOrderTicket, _order.OrderTicket);
    Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicket.Order);
    Assert.AreSame (_order, _oldOrderTicket.Order);
    Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicket.OrderTicket);
  }

  [Test]
  public void OldRelatedObjectCancelsRelationChange ()
  {
    _orderEventReceiver.Cancel = false;
    _oldOrderTicketEventReceiver.Cancel = true;
    _newOrderTicketEventReceiver.Cancel = false;
    _oldOrderOfNewOrderTicketEventReceiver.Cancel = false;

    _order.OrderTicket = _newOrderTicket;

    Assert.AreEqual (true, _orderEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _orderEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_orderEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_oldOrderTicket, _orderEventReceiver.OldRelatedObject);
    Assert.AreSame (_newOrderTicket, _orderEventReceiver.NewRelatedObject);

    Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Order", _oldOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_oldOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_order, _oldOrderTicketEventReceiver.OldRelatedObject);
    Assert.AreSame (null, _oldOrderTicketEventReceiver.NewRelatedObject);

    Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.IsNull (_newOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_newOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_newOrderTicketEventReceiver.OldRelatedObject);
    Assert.IsNull (_newOrderTicketEventReceiver.NewRelatedObject);

    Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.OldRelatedObject);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.NewRelatedObject);

    Assert.AreEqual (StateType.Unchanged, _order.State);
    Assert.AreEqual (StateType.Unchanged, _newOrderTicket.State);
    Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);
    Assert.AreEqual (StateType.Unchanged, _oldOrderOfNewOrderTicket.State);

    Assert.AreSame (_oldOrderTicket, _order.OrderTicket);
    Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicket.Order);
    Assert.AreSame (_order, _oldOrderTicket.Order);
    Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicket.OrderTicket);
  }

  [Test]
  public void NewRelatedObjectCancelsRelationChange ()
  {
    _orderEventReceiver.Cancel = false;
    _oldOrderTicketEventReceiver.Cancel = false;
    _newOrderTicketEventReceiver.Cancel = true;
    _oldOrderOfNewOrderTicketEventReceiver.Cancel = false;

    _order.OrderTicket = _newOrderTicket;

    Assert.AreEqual (true, _orderEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _orderEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_orderEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_oldOrderTicket, _orderEventReceiver.OldRelatedObject);
    Assert.AreSame (_newOrderTicket, _orderEventReceiver.NewRelatedObject);

    Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Order", _oldOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_oldOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_order, _oldOrderTicketEventReceiver.OldRelatedObject);
    Assert.AreSame (null, _oldOrderTicketEventReceiver.NewRelatedObject);

    Assert.AreEqual (true, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Order", _newOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_newOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicketEventReceiver.OldRelatedObject);
    Assert.AreSame (_order, _newOrderTicketEventReceiver.NewRelatedObject);

    Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.OldRelatedObject);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.NewRelatedObject);
  
    Assert.AreEqual (StateType.Unchanged, _order.State);
    Assert.AreEqual (StateType.Unchanged, _newOrderTicket.State);
    Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);
    Assert.AreEqual (StateType.Unchanged, _oldOrderOfNewOrderTicket.State);

    Assert.AreSame (_oldOrderTicket, _order.OrderTicket);
    Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicket.Order);
    Assert.AreSame (_order, _oldOrderTicket.Order);
    Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicket.OrderTicket);
  }

  [Test]
  public void OldRelatedObjectOfNewRelatedObjectCancelsRelationChange ()
  {
    _orderEventReceiver.Cancel = false;
    _oldOrderTicketEventReceiver.Cancel = false;
    _newOrderTicketEventReceiver.Cancel = false;
    _oldOrderOfNewOrderTicketEventReceiver.Cancel = true;

    _order.OrderTicket = _newOrderTicket;

    Assert.AreEqual (true, _orderEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _orderEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_orderEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_oldOrderTicket, _orderEventReceiver.OldRelatedObject);
    Assert.AreSame (_newOrderTicket, _orderEventReceiver.NewRelatedObject);

    Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Order", _oldOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_oldOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_order, _oldOrderTicketEventReceiver.OldRelatedObject);
    Assert.AreSame (null, _oldOrderTicketEventReceiver.NewRelatedObject);

    Assert.AreEqual (true, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Order", _newOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_newOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicketEventReceiver.OldRelatedObject);
    Assert.AreSame (_order, _newOrderTicketEventReceiver.NewRelatedObject);

    Assert.AreEqual (true, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("OrderTicket", _oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicketEventReceiver.OldRelatedObject);
    Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.NewRelatedObject);
  
    Assert.AreEqual (StateType.Unchanged, _order.State);
    Assert.AreEqual (StateType.Unchanged, _newOrderTicket.State);
    Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);
    Assert.AreEqual (StateType.Unchanged, _oldOrderOfNewOrderTicket.State);

    Assert.AreSame (_oldOrderTicket, _order.OrderTicket);
    Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicket.Order);
    Assert.AreSame (_order, _oldOrderTicket.Order);
    Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicket.OrderTicket);
  }

  [Test]
  public void StateTracking ()
  {
    Assert.AreEqual (StateType.Unchanged, _order.State);
    Assert.AreEqual (StateType.Unchanged, _newOrderTicket.State);
    Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);
    Assert.AreEqual (StateType.Unchanged, _oldOrderOfNewOrderTicket.State);

    _order.OrderTicket = _newOrderTicket;

    Assert.AreEqual (StateType.Changed, _order.State);
    Assert.AreEqual (StateType.Changed, _newOrderTicket.State);
    Assert.AreEqual (StateType.Changed, _oldOrderTicket.State);
    Assert.AreEqual (StateType.Changed, _oldOrderOfNewOrderTicket.State);
  }

  [Test]
  public void OldObjectAndNewObjectAreSame ()
  {
    Assert.AreEqual (StateType.Unchanged, _order.State);
    Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);

    _order.OrderTicket = _oldOrderTicket;

    Assert.AreEqual (StateType.Unchanged, _order.State);
    Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);
  }

  [Test]
  public void ChangeRelationOverVirtualEndPoint ()
  {
    _order.OrderTicket = _newOrderTicket;
    
    Assert.IsNull (_oldOrderTicket.DataContainer.GetObjectID ("Order"));
    Assert.AreEqual (_order.ID, _newOrderTicket.DataContainer.GetObjectID ("Order"));

    Assert.AreSame (_newOrderTicket, _order.OrderTicket);
    Assert.AreSame (_order, _newOrderTicket.Order);
    Assert.IsNull (_oldOrderTicket.Order);
    Assert.IsNull (_oldOrderOfNewOrderTicket.OrderTicket);
  }

  [Test]
  public void ChangeRelation ()
  {
    _newOrderTicket.Order = _order;
    
    Assert.IsNull (_oldOrderTicket.DataContainer.GetObjectID ("Order"));
    Assert.AreEqual (_order.ID, _newOrderTicket.DataContainer.GetObjectID ("Order"));

    Assert.AreSame (_order, _newOrderTicket.Order);
    Assert.AreSame (_newOrderTicket, _order.OrderTicket);
    Assert.IsNull (_oldOrderTicket.Order);
    Assert.IsNull (_oldOrderOfNewOrderTicket.OrderTicket);
  }

  [Test]
  public void ChangeRelationWithInheritance ()
  {
    Person person = Person.GetObject (DomainObjectIDs.Person1);
    Distributor distributor = Distributor.GetObject (DomainObjectIDs.Distributor1);

    person.AssociatedPartnerCompany = distributor;

    Assert.AreSame (distributor, person.AssociatedPartnerCompany);
    Assert.AreSame (person, distributor.ContactPerson);
  }

  [Test]
  public void NoPropertyValueEventForRelationChange ()
  {
    PropertyValueEventReceiver oldOrderTicketPropertyEventReceiver = new PropertyValueEventReceiver (
        _oldOrderTicket.DataContainer.PropertyValues["Order"]);

    PropertyValueEventReceiver newOrderTicketPropertyEventReceiver = new PropertyValueEventReceiver (
        _newOrderTicket.DataContainer.PropertyValues["Order"]);

    _order.OrderTicket = _newOrderTicket;

    Assert.IsFalse (oldOrderTicketPropertyEventReceiver.HasChangingEventBeenCalled);
    Assert.IsFalse (oldOrderTicketPropertyEventReceiver.HasChangedEventBeenCalled);
    Assert.IsFalse (newOrderTicketPropertyEventReceiver.HasChangingEventBeenCalled);
    Assert.IsFalse (newOrderTicketPropertyEventReceiver.HasChangedEventBeenCalled);
  }

  [Test]
  public void ChangeRelationBackToOriginalValue ()
  {
    _order.OrderTicket = _newOrderTicket;
    Assert.AreEqual (StateType.Changed, _order.State);

    _order.OrderTicket = _oldOrderTicket;
    Assert.AreEqual (StateType.Unchanged, _order.State);
  }

  [Test]
  public void GetOriginalRelatedObject ()
  {
    Assert.AreSame (_oldOrderTicket, _order.GetOriginalRelatedObject ("OrderTicket"));

    _order.OrderTicket = _newOrderTicket;

    Assert.AreSame (_newOrderTicket, _order.OrderTicket);
    Assert.AreSame (_oldOrderTicket, _order.GetOriginalRelatedObject ("OrderTicket"));
  }

  [Test]
  public void GetOriginalRelatedObjectWithLazyLoad ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order2);

    Assert.AreEqual (DomainObjectIDs.OrderTicket3, order.GetOriginalRelatedObject("OrderTicket").ID);
  }

  [Test]
  public void GetNullOriginalRelatedObject ()
  {
    Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
    Assert.IsNull (computer.GetOriginalRelatedObject ("Employee"));
  }

  [Test]
  public void OldObjectAndNewObjectAreSameRelationInherited ()
  {
    // TODO: Events überprüfen
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer4);
    Ceo ceo = customer.Ceo;

    Assert.AreEqual (StateType.Unchanged, customer.State);
    Assert.AreEqual (StateType.Unchanged, ceo.State);

    customer.Ceo = ceo;

    Assert.AreEqual (StateType.Unchanged, customer.State);
    Assert.AreEqual (StateType.Unchanged, ceo.State);
  }
}
}