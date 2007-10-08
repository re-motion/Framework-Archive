using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class NewDomainObjectTest : ClientTransactionBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    [Test]
    public void Creation ()
    {
      Order order = Order.NewObject ();

      Assert.IsNotNull (order.ID);
      Assert.AreEqual (StateType.New, order.State);
			Assert.AreSame (order, order.InternalDataContainer.DomainObject);
    }

    [Test]
    public void Creation_NonGenericWithDefaultConstructor ()
    {
      Order order = (Order) DomainObject.NewObject (typeof (Order));

      Assert.IsNotNull (order.ID);
      Assert.AreEqual (StateType.New, order.State);
			Assert.AreSame (order, order.InternalDataContainer.DomainObject);
    }

    [Test]
    public void GetObject_Generic ()
    {
      Order order = Order.NewObject ();
      Order sameOrder = Order.GetObject (order.ID);

      Assert.AreSame (order, sameOrder);
    }

    [Test]
    public void GetObject_NonGeneric ()
    {
      Order order = (Order) DomainObject.NewObject (typeof (Order));
      Order sameOrder = (Order) DomainObject.GetObject (order.ID);

      Assert.AreSame (order, sameOrder);
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = Order.NewObject ();

      Assert.IsNull (order.OrderTicket);
    }

    [Test]
    public void SetRelatedObject ()
    {
      Partner partner = Partner.NewObject ();
      Ceo ceo = Ceo.NewObject ();

      Assert.IsNull (partner.Ceo);
      Assert.IsNull (ceo.Company);

      partner.Ceo = ceo;

      Assert.AreSame (partner, ceo.Company);
      Assert.AreSame (ceo, partner.Ceo);
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Order order = Order.NewObject ();

      Assert.IsNotNull (order.OrderItems);
      Assert.AreEqual (0, order.OrderItems.Count);
    }

    [Test]
    public void SetRelatedObjects ()
    {
      Order order = Order.NewObject ();
      OrderItem orderItem = OrderItem.NewObject ();

      order.OrderItems.Add (orderItem);

      Assert.AreSame (order, orderItem.Order);
      Assert.AreEqual (1, order.OrderItems.Count);
      Assert.IsNotNull (order.OrderItems[orderItem.ID]);
    }

    [Test]
    public void StateForPropertyChange ()
    {
      Customer customer = Customer.NewObject ();
      customer.Name = "Arthur Dent";

      Assert.AreEqual ("Arthur Dent", customer.Name);
			Assert.AreEqual (string.Empty, customer.Properties["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Name"].GetOriginalValue<string>());
      Assert.AreEqual (StateType.New, customer.State);
    }

    [Test]
    public void StateForOneToOneRelationChange ()
    {
      Partner partner = Partner.NewObject();
      Ceo ceo = Ceo.NewObject ();

      partner.Ceo = ceo;

      Assert.AreEqual (StateType.New, partner.State);
      Assert.AreEqual (StateType.New, ceo.State);
    }

    [Test]
    public void StateForOneToManyRelationChange ()
    {
      Order order = Order.NewObject ();
      OrderItem orderItem = OrderItem.NewObject ();

      order.OrderItems.Add (orderItem);

      Assert.AreEqual (StateType.New, order.State);
      Assert.AreEqual (StateType.New, orderItem.State);
    }

    [Test]
    public void Events ()
    {
      Order order = Order.NewObject ();
      OrderItem orderItem = OrderItem.NewObject ();

      DomainObjectEventReceiver orderEventReceiver = new DomainObjectEventReceiver (order);
      DomainObjectEventReceiver orderItemEventReceiver = new DomainObjectEventReceiver (orderItem);

      DomainObjectCollectionEventReceiver collectionEventReceiver = new DomainObjectCollectionEventReceiver (
          order.OrderItems);

      order.DeliveryDate = new DateTime (2010, 1, 1);
      order.OrderItems.Add (orderItem);

      Assert.IsTrue (orderEventReceiver.HasChangingEventBeenCalled);
      Assert.IsTrue (orderEventReceiver.HasChangedEventBeenCalled);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate", orderEventReceiver.ChangingPropertyValue.Name);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate", orderEventReceiver.ChangedPropertyValue.Name);

      Assert.IsTrue (orderEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (orderEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", orderEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", orderEventReceiver.ChangedRelationPropertyName);

      Assert.IsTrue (orderItemEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (orderItemEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", orderItemEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", orderItemEventReceiver.ChangedRelationPropertyName);

      Assert.IsTrue (collectionEventReceiver.HasAddingEventBeenCalled);
      Assert.IsTrue (collectionEventReceiver.HasAddedEventBeenCalled);
      Assert.AreSame (orderItem, collectionEventReceiver.AddingDomainObject);
      Assert.AreSame (orderItem, collectionEventReceiver.AddedDomainObject);
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      Partner partner = Partner.NewObject();
      Ceo ceo = Ceo.NewObject ();

      partner.Ceo = ceo;

      Assert.IsNull (partner.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo"));
      Assert.IsNull (ceo.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company"));
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      Order order = Order.NewObject ();
      OrderItem orderItem = OrderItem.NewObject ();

      order.OrderItems.Add (orderItem);

      DomainObjectCollection originalOrderItems = order.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      Assert.IsNotNull (originalOrderItems);
      Assert.AreEqual (0, originalOrderItems.Count);
      Assert.IsNull (orderItem.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"));
    }

    [Test]
    public void SaveNewRelatedObjects ()
    {
      Ceo ceo = Ceo.NewObject ();
      Customer customer = Customer.NewObject ();
      Order order = Order.NewObject ();
      OrderTicket orderTicket = OrderTicket.NewObject (order);
      OrderItem orderItem = OrderItem.NewObject ();

      ObjectID ceoID = ceo.ID;
      ObjectID customerID = customer.ID;
      ObjectID orderID = order.ID;
      ObjectID orderTicketID = orderTicket.ID;
      ObjectID orderItemID = orderItem.ID;

      ceo.Name = "Ford Prefect";

      customer.CustomerSince = new DateTime (2000, 1, 1);
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

			Assert.IsNull (ceo.InternalDataContainer.Timestamp);
			Assert.IsNull (customer.InternalDataContainer.Timestamp);
			Assert.IsNull (order.InternalDataContainer.Timestamp);
			Assert.IsNull (orderTicket.InternalDataContainer.Timestamp);
			Assert.IsNull (orderItem.InternalDataContainer.Timestamp);

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

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
      Assert.AreEqual (6, official.Orders.Count);
      Assert.IsNotNull (official.Orders[orderID]);

      Assert.AreEqual ("Ford Prefect", ceo.Name);
      Assert.AreEqual (new DateTime (2000, 1, 1), customer.CustomerSince);
      Assert.AreEqual ("Arthur Dent", customer.Name);
      Assert.AreEqual (1, orderItem.Position);
      Assert.AreEqual ("Sternenkarte", orderItem.Product);
      Assert.AreEqual (@"C:\home\arthur_dent\maporder.png", orderTicket.FileName);
      Assert.AreEqual (42, order.OrderNumber);
      Assert.AreEqual (new DateTime (2005, 2, 1), order.DeliveryDate);

			Assert.IsNotNull (ceo.InternalDataContainer.Timestamp);
			Assert.IsNotNull (customer.InternalDataContainer.Timestamp);
			Assert.IsNotNull (order.InternalDataContainer.Timestamp);
			Assert.IsNotNull (orderTicket.InternalDataContainer.Timestamp);
			Assert.IsNotNull (orderItem.InternalDataContainer.Timestamp);
    }

    [Test]
    public void SaveHierarchy ()
    {
      Employee supervisor = Employee.NewObject ();
      Employee subordinate = Employee.NewObject ();

      ObjectID supervisorID = supervisor.ID;
      ObjectID subordinateID = subordinate.ID;

      supervisor.Name = "Slartibartfast";
      subordinate.Name = "Zarniwoop";
      supervisor.Subordinates.Add (subordinate);

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

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
      OrderItem orderItem = OrderItem.NewObject ();
      ClientTransactionMock.Commit ();
    }

    [Test]
    public void SaveExistingObjectWithRelatedNew ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      Employee newEmployee = Employee.NewObject ();
      ObjectID newEmployeeID = newEmployee.ID;

      newEmployee.Computer = computer;
      newEmployee.Name = "Arthur Dent";

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

      computer = Computer.GetObject (DomainObjectIDs.Computer4);
      newEmployee = Employee.GetObject (newEmployeeID);

      Assert.IsNotNull (newEmployee);
      Assert.AreEqual ("Arthur Dent", newEmployee.Name);
      Assert.AreSame (computer, newEmployee.Computer);
    }

    [Test]
    public void DataContainerStateAfterCommit ()
    {
      Computer computer = Computer.NewObject ();

      ClientTransactionMock.Commit ();

      Assert.AreEqual (StateType.Unchanged, computer.State);
    }

    [Test]
    public void PropertyValueHasChangedAfterCommit ()
    {
      Employee employee = Employee.NewObject ();
      employee.Name = "Mr. Prosser";

      Assert.IsTrue (employee.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Name"].HasChanged);

      ClientTransactionMock.Commit ();

      Assert.IsFalse (employee.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Name"].HasChanged);
    }

    [Test]
    public void PropertyValueHasBeenTouchedAfterCommit ()
    {
      Employee employee = Employee.NewObject ();
      employee.Name = "Mr. Prosser";

      Assert.IsTrue (employee.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Name"].HasBeenTouched);

      ClientTransactionMock.Commit ();

      Assert.IsFalse (employee.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Name"].HasBeenTouched);
    }

    [Test]
    public void OneToOneRelationHasChangedAfterCommit ()
    {
      Employee employee = Employee.NewObject ();
      employee.Name = "Jeltz";

      Computer computer = Computer.NewObject ();
      computer.SerialNumber = "42";

      employee.Computer = computer;

      Assert.IsNull (employee.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"));
      Assert.IsNull (computer.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"));

      ClientTransactionMock.Commit ();

      Assert.AreSame (computer, employee.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"));
      Assert.AreSame (employee, computer.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"));
    }

    [Test]
    public void OneToManyRelationHasChangedAfterCommit ()
    {
      Employee supervisor = Employee.NewObject ();
      Employee subordinate = Employee.NewObject ();

      supervisor.Name = "Slartibartfast";
      subordinate.Name = "Zarniwoop";
      supervisor.Subordinates.Add (subordinate);

      Assert.AreEqual (0, supervisor.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates").Count);
      Assert.IsNull (subordinate.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));

      ClientTransactionMock.Commit ();

      DomainObjectCollection originalSubordinates = supervisor.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates");
      Assert.AreEqual (1, originalSubordinates.Count);
      Assert.AreSame (subordinate, originalSubordinates[subordinate.ID]);
      Assert.AreSame (supervisor, subordinate.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
    }
  }
}
