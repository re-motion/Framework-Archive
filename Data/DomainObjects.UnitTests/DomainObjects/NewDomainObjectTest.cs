using System;
using NUnit.Framework;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class NewDomainObjectTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public NewDomainObjectTest ()
  {
  }

  // methods and properties

  [Test]
  public void Creation ()
  {
    Order order = new Order ();

    Assert.IsNotNull (order.ID);
    Assert.AreEqual (StateType.New, order.State);
    Assert.AreSame (order, order.DataContainer.DomainObject);
  }

  [Test]
  public void GetObject ()
  {
    Order order = new Order ();
    Order sameOrder = Order.GetObject (order.ID);

    Assert.AreSame (order, sameOrder);
  }

  [Test]
  public void GetRelatedObject ()
  {
    Order order = new Order ();

    Assert.IsNull (order.OrderTicket);
  }

  [Test]
  public void SetRelatedObject ()
  {
    Partner partner = new Partner ();
    Ceo ceo = new Ceo ();

    Assert.IsNull (partner.Ceo);
    Assert.IsNull (ceo.Company);

    partner.Ceo = ceo;

    Assert.AreSame (partner, ceo.Company);
    Assert.AreSame (ceo, partner.Ceo);
  }

  [Test]
  public void GetRelatedObjects ()
  {
    Order order = new Order ();

    Assert.IsNotNull (order.OrderItems);
    Assert.AreEqual (0, order.OrderItems.Count);
  }

  [Test]
  public void SetRelatedObjects ()
  {
    Order order = new Order ();
    OrderItem orderItem = new OrderItem ();

    order.OrderItems.Add (orderItem);

    Assert.AreSame (order, orderItem.Order);
    Assert.AreEqual (1, order.OrderItems.Count);
    Assert.IsNotNull (order.OrderItems[orderItem.ID]);
  }

  [Test]
  public void StateForPropertyChange ()
  {
    Customer customer = new Customer ();
    customer.Name = "Arthur Dent";

    Assert.AreEqual ("Arthur Dent", customer.Name);
    Assert.AreEqual (string.Empty, customer.DataContainer.PropertyValues["Name"].OriginalValue);
    Assert.AreEqual (StateType.New, customer.State);
  }

  [Test]
  public void StateForOneToOneRelationChange ()
  {
    Partner partner = new Partner ();
    Ceo ceo = new Ceo ();

    partner.Ceo = ceo;

    Assert.AreEqual (StateType.New, partner.State);
    Assert.AreEqual (StateType.New, ceo.State);
  }

  [Test]
  public void StateForOneToManyRelationChange ()
  {
    Order order = new Order ();
    OrderItem orderItem = new OrderItem ();

    order.OrderItems.Add (orderItem);

    Assert.AreEqual (StateType.New, order.State);
    Assert.AreEqual (StateType.New, orderItem.State);
  }

  [Test]
  public void Events ()
  {
    Order order = new Order ();
    OrderItem orderItem = new OrderItem ();

    DomainObjectEventReceiver orderEventReceiver = new DomainObjectEventReceiver (order);
    DomainObjectEventReceiver orderItemEventReceiver = new DomainObjectEventReceiver (orderItem);
    
    DomainObjectCollectionEventReceiver collectionEventReceiver = new DomainObjectCollectionEventReceiver (
        order.OrderItems);
 
    order.DeliveryDate = new DateTime (2010, 1, 1);
    order.OrderItems.Add (orderItem);

    Assert.IsTrue (orderEventReceiver.HasChangingEventBeenCalled);
    Assert.IsTrue (orderEventReceiver.HasChangedEventBeenCalled);
    Assert.AreEqual ("DeliveryDate", orderEventReceiver.ChangingPropertyValue.Name);
    Assert.AreEqual ("DeliveryDate", orderEventReceiver.ChangedPropertyValue.Name);

    Assert.IsTrue (orderEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsTrue (orderEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("OrderItems", orderEventReceiver.ChangingRelationPropertyName);
    Assert.AreEqual ("OrderItems", orderEventReceiver.ChangedRelationPropertyName);

    Assert.IsTrue (orderItemEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsTrue (orderItemEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Order", orderItemEventReceiver.ChangingRelationPropertyName);
    Assert.AreEqual ("Order", orderItemEventReceiver.ChangedRelationPropertyName);

    Assert.IsTrue (collectionEventReceiver.HasAddingEventBeenCalled);
    Assert.IsTrue (collectionEventReceiver.HasAddedEventBeenCalled);
    Assert.AreSame (orderItem, collectionEventReceiver.AddingDomainObject);
    Assert.AreSame (orderItem, collectionEventReceiver.AddedDomainObject);
  }

  [Test]
  public void GetOriginalRelatedObject ()
  {
    Partner partner = new Partner ();
    Ceo ceo = new Ceo ();

    partner.Ceo = ceo;

    Assert.IsNull (partner.GetOriginalRelatedObject ("Ceo"));
    Assert.IsNull (ceo.GetOriginalRelatedObject ("Company"));
  }

  [Test]
  public void GetOriginalRelatedObjects ()
  {
    Order order = new Order ();
    OrderItem orderItem = new OrderItem ();

    order.OrderItems.Add (orderItem);

    DomainObjectCollection originalOrderItems = order.GetOriginalRelatedObjects ("OrderItems");

    Assert.IsNotNull (originalOrderItems);
    Assert.AreEqual (0, originalOrderItems.Count);
    Assert.IsNull (orderItem.GetOriginalRelatedObject ("Order"));
  }

  [Test]
  public void SaveNewRelatedObjects ()
  {
    Ceo ceo = new Ceo ();
    Customer customer = new Customer ();
    Order order = new Order ();
    OrderTicket orderTicket = new OrderTicket (order);
    OrderItem orderItem = new OrderItem ();

    ObjectID ceoID = ceo.ID;
    ObjectID customerID = customer.ID;
    ObjectID orderID = order.ID;
    ObjectID orderTicketID = orderTicket.ID;
    ObjectID orderItemID = orderItem.ID;

    ceo.Name = "Ford Prefect";
    
    customer.CustomerSince = new NaDateTime (new DateTime (2000, 1, 1));
    customer.Name = "Arthur Dent";
    customer.Ceo = ceo;

    orderItem.Position = 1;
    orderItem.Product = "Sternenkarte";

    orderTicket.FileName = @"C:\home\arthur_dent\maporder.png";

    order.OrderNumber = 42;
    order.DeliveryDate = new DateTime (2005, 2, 1);
    order.Official = Official.GetObject (DomainObjectIDs.Official1);
    order.Customer = customer;
    order.OrderItems.Add (orderItem);

    Assert.IsNull (ceo.DataContainer.Timestamp);
    Assert.IsNull (customer.DataContainer.Timestamp);
    Assert.IsNull (order.DataContainer.Timestamp);
    Assert.IsNull (orderTicket.DataContainer.Timestamp);
    Assert.IsNull (orderItem.DataContainer.Timestamp);

    ClientTransaction.Current.Commit ();
    ClientTransaction.SetCurrent (null);

    ceo = Ceo.GetObject (ceoID);
    customer = Customer.GetObject (customerID);
    order = Order.GetObject (orderID);
    orderTicket = OrderTicket.GetObject (orderTicketID);
    orderItem = OrderItem.GetObject (orderItemID);
    Official official = Official.GetObject (DomainObjectIDs.Official1);

    Assert.IsNotNull (ceo);
    Assert.IsNotNull (customer);
    Assert.IsNotNull (order);
    Assert.IsNotNull (orderTicket);
    Assert.IsNotNull (orderItem);

    Assert.AreSame (customer, ceo.Company);
    Assert.AreSame (ceo, customer.Ceo);
    Assert.AreSame (customer, order.Customer);
    Assert.AreEqual (1, customer.Orders.Count);
    Assert.AreSame (order, customer.Orders[0]);
    Assert.AreSame (order, orderTicket.Order);
    Assert.AreSame (orderTicket, order.OrderTicket);
    Assert.AreSame (order, orderItem.Order);
    Assert.AreEqual (1, order.OrderItems.Count);
    Assert.AreSame (orderItem, order.OrderItems[0]);
    Assert.AreSame (official, order.Official);
    Assert.AreEqual (4, official.Orders.Count);
    Assert.IsNotNull (official.Orders[orderID]);

    Assert.AreEqual ("Ford Prefect", ceo.Name);
    Assert.AreEqual (new NaDateTime (new DateTime (2000, 1, 1)), customer.CustomerSince);
    Assert.AreEqual ("Arthur Dent", customer.Name);
    Assert.AreEqual (1, orderItem.Position);
    Assert.AreEqual ("Sternenkarte", orderItem.Product);
    Assert.AreEqual (@"C:\home\arthur_dent\maporder.png", orderTicket.FileName);
    Assert.AreEqual (42, order.OrderNumber);
    Assert.AreEqual (new DateTime (2005, 2, 1), order.DeliveryDate);

    Assert.IsNotNull (ceo.DataContainer.Timestamp);
    Assert.IsNotNull (customer.DataContainer.Timestamp);
    Assert.IsNotNull (order.DataContainer.Timestamp);
    Assert.IsNotNull (orderTicket.DataContainer.Timestamp);
    Assert.IsNotNull (orderItem.DataContainer.Timestamp);
  }

  [Test]
  public void SaveHierarchy ()
  {
    Employee supervisor = new Employee ();
    Employee subordinate = new Employee ();

    ObjectID supervisorID = supervisor.ID;
    ObjectID subordinateID = subordinate.ID;

    supervisor.Name = "Slartibartfast";
    subordinate.Name = "Zarniwoop";
    supervisor.Subordinates.Add (subordinate);

    ClientTransaction.Current.Commit ();
    ClientTransaction.SetCurrent (null);

    supervisor = Employee.GetObject (supervisorID);
    subordinate = Employee.GetObject (subordinateID);

    Assert.IsNotNull (supervisor);
    Assert.IsNotNull (subordinate);

    Assert.AreEqual (supervisorID, supervisor.ID);
    Assert.AreEqual (subordinateID, subordinate.ID);

    Assert.AreEqual ("Slartibartfast", supervisor.Name);
    Assert.AreEqual ("Zarniwoop", subordinate.Name);
  }

  [Test]
  [ExpectedException (typeof (MandatoryRelationNotSetException))]
  public void CheckMandatoryRelation ()
  {
    OrderItem orderItem = new OrderItem ();
    ClientTransaction.Current.Commit ();
  }

  [Test]
  public void SaveExistingObjectWithRelatedNew ()
  {
    Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);     
    Employee newEmployee = new Employee ();
    ObjectID newEmployeeID = newEmployee.ID;

    newEmployee.Computer = computer;
    newEmployee.Name = "Arthur Dent";
    
    ClientTransaction.Current.Commit ();
    ClientTransaction.SetCurrent (null);

    computer = Computer.GetObject (DomainObjectIDs.Computer4);
    newEmployee = Employee.GetObject (newEmployeeID);
    
    Assert.IsNotNull (newEmployee);
    Assert.AreEqual ("Arthur Dent", newEmployee.Name);
    Assert.AreSame (computer, newEmployee.Computer);
  }

  [Test]
  public void DataContainerStateAfterCommit ()
  {
    Computer computer = new Computer ();
    
    ClientTransaction.Current.Commit ();

    Assert.AreEqual (StateType.Unchanged, computer.State);
  }

  [Test]
  public void PropertyValueHasChangedAfterCommit ()
  {
    Employee employee = new Employee ();
    employee.Name = "Mr. Prosser";

    Assert.IsTrue (employee.DataContainer.PropertyValues["Name"].HasChanged);

    ClientTransaction.Current.Commit ();

    Assert.IsFalse (employee.DataContainer.PropertyValues["Name"].HasChanged);
  }

  [Test]
  public void OneToOneRelationHasChangedAfterCommit ()
  {
    Employee employee = new Employee ();
    employee.Name = "Jeltz";

    Computer computer = new Computer ();
    computer.SerialNumber = "42";

    employee.Computer = computer;

    Assert.IsNull (employee.GetOriginalRelatedObject ("Computer"));
    Assert.IsNull (computer.GetOriginalRelatedObject ("Employee"));

    ClientTransaction.Current.Commit ();

    Assert.AreSame (computer, employee.GetOriginalRelatedObject ("Computer"));
    Assert.AreSame (employee, computer.GetOriginalRelatedObject ("Employee"));
  }

  [Test]
  public void OneToManyRelationHasChangedAfterCommit ()
  {
    Employee supervisor = new Employee ();
    Employee subordinate = new Employee ();

    supervisor.Name = "Slartibartfast";
    subordinate.Name = "Zarniwoop";
    supervisor.Subordinates.Add (subordinate);

    Assert.AreEqual (0, supervisor.GetOriginalRelatedObjects ("Subordinates").Count);
    Assert.IsNull (subordinate.GetOriginalRelatedObject ("Supervisor"));

    ClientTransaction.Current.Commit ();

    DomainObjectCollection originalSubordinates = supervisor.GetOriginalRelatedObjects ("Subordinates");
    Assert.AreEqual (1, originalSubordinates.Count);
    Assert.AreSame (subordinate, originalSubordinates[subordinate.ID]);
    Assert.AreSame (supervisor, subordinate.GetOriginalRelatedObject ("Supervisor"));
  }
}
}
