using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.EventSequence;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class DomainObjectTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public DomainObjectTest ()
  {
  }

  // methods and properties

  [Test]
  public void LoadingOfSimpleObject ()
  {
    ObjectID id = new ObjectID (DatabaseTest.c_testDomainProviderID,
        "ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

    ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (id);
    
    Assert.AreEqual (id.Value, classWithAllDataTypes.ID.Value, "ID.Value");
    Assert.AreEqual (id.ClassID, classWithAllDataTypes.ID.ClassID, "ID.ClassID");
    Assert.AreEqual (id.StorageProviderID, classWithAllDataTypes.ID.StorageProviderID, "ID.StorageProviderID");

    Assert.AreEqual (false, classWithAllDataTypes.BooleanProperty, "BooleanProperty");
    Assert.AreEqual (85, classWithAllDataTypes.ByteProperty, "ByteProperty");
    Assert.AreEqual ('a', classWithAllDataTypes.CharProperty, "CharProperty");
    Assert.AreEqual (new DateTime (2005, 1, 1), classWithAllDataTypes.DateTimeProperty, "DateTimeProperty");
    Assert.AreEqual (123456.789, classWithAllDataTypes.DecimalProperty, "DecimalProperty");
    Assert.AreEqual (987654.321, classWithAllDataTypes.DoubleProperty, "DoubleProperty");
    Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value1, classWithAllDataTypes.EnumProperty, "EnumProperty");

    Assert.AreEqual (new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"), 
        classWithAllDataTypes.GuidProperty, "GuidProperty");
    
    Assert.AreEqual (32767, classWithAllDataTypes.Int16Property, "Int16Property");
    Assert.AreEqual (2147483647, classWithAllDataTypes.Int32Property, "Int32Property");
    Assert.AreEqual (9223372036854775807, classWithAllDataTypes.Int64Property, "Int64Property");
    Assert.AreEqual (6789.321, classWithAllDataTypes.SingleProperty, "SingleProperty");
    Assert.AreEqual ("abcdeföäü", classWithAllDataTypes.StringProperty, "StringProperty");
    Assert.AreEqual (new NaBoolean (true), classWithAllDataTypes.NaBooleanProperty, "NaBooleanProperty");

    Assert.AreEqual (new NaDateTime (new DateTime (2005, 2, 1)), 
        classWithAllDataTypes.NaDateTimeProperty, "NaDateTimeProperty");

    Assert.AreEqual (new NaDouble (654321.789), classWithAllDataTypes.NaDoubleProperty, "NaDoubleProperty");
    Assert.AreEqual (new NaInt32 (-2147483647), classWithAllDataTypes.NaInt32Property, "NaInt32Property");
    Assert.AreEqual (null, classWithAllDataTypes.StringWithNullValueProperty, "StringWithNullValueProperty");
    
    Assert.AreEqual (NaBoolean.Null, 
        classWithAllDataTypes.NaBooleanWithNullValueProperty, "NaBooleanWithNullValueProperty");
    
    Assert.AreEqual (NaDateTime.Null, 
        classWithAllDataTypes.NaDateTimeWithNullValueProperty, "NaDateTimeWithNullValueProperty");

    Assert.AreEqual (NaDouble.Null, 
        classWithAllDataTypes.NaDoubleWithNullValueProperty, "NaDoubleWithNullValueProperty");

    Assert.AreEqual (NaInt32.Null, classWithAllDataTypes.NaInt32WithNullValueProperty, "NaInt32WithNullValueProperty");
  }

  [Test]
  public void LoadingOfDerivedObject ()
  {
    Company company = Company.GetObject (DomainObjectIDs.Partner2);
    Assert.IsNotNull (company);

    Partner partner = company as Partner;
    Assert.IsNotNull (partner);

    Assert.AreEqual (DomainObjectIDs.Partner2, partner.ID, "ID");
    Assert.AreEqual ("Partner 2", partner.Name, "Name");

    Assert.AreEqual (DomainObjectIDs.Person2, partner.ContactPerson.ID, "ContactPerson");
  }

  [Test]
  public void LoadingOfTwiceDerivedObject ()
  {
    Company company = Company.GetObject (DomainObjectIDs.Supplier1);
    Assert.IsNotNull (company);

    Supplier supplier = company as Supplier;
    Assert.IsNotNull (supplier);

    Assert.AreEqual (DomainObjectIDs.Supplier1, supplier.ID);
    Assert.AreEqual ("Lieferant 1", supplier.Name, "Name");
    Assert.AreEqual (DomainObjectIDs.Person3, supplier.ContactPerson.ID, "ContactPerson");
    Assert.AreEqual (1, supplier.SupplierQuality, "SupplierQuality");
  }

  [Test]
  public void OnLoaded ()
  {
    ObjectID id = new ObjectID (
        c_testDomainProviderID, "ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

    ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (id);
    
    Assert.IsTrue (classWithAllDataTypes.OnLoadedHasBeenCalled);
  }

  [Test]
  public void GetRelatedObject ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    
    Assert.IsNotNull (order.OrderTicket);
    Assert.AreEqual (DomainObjectIDs.OrderTicket1, order.OrderTicket.ID);
  }

  [Test]
  public void GetDerivedRelatedObject ()
  {
    Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo10);

    Company company = ceo.Company;
    Assert.IsNotNull (company);

    Distributor distributor = company as Distributor;
    Assert.IsNotNull (distributor);
  }

  [Test]
  public void GetRelatedObjects ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    
    Assert.IsNotNull (customer.Orders);   
    Assert.AreEqual (2, customer.Orders.Count);
    Assert.AreEqual (DomainObjectIDs.Order1, customer.Orders[DomainObjectIDs.Order1].ID);
    Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, customer.Orders[DomainObjectIDs.OrderWithoutOrderItem].ID);
  }

  [Test]
  public void GetRelatedObjectsWithDerivation ()
  {
    IndustrialSector industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector2);
    DomainObjectCollection collection = industrialSector.Companies;

    Assert.AreEqual (7, collection.Count);
    Assert.AreEqual (typeof (Company), collection[DomainObjectIDs.Company1].GetType());
    Assert.AreEqual (typeof (Company), collection[DomainObjectIDs.Company2].GetType());
    Assert.AreEqual (typeof (Customer), collection[DomainObjectIDs.Customer2].GetType());
    Assert.AreEqual (typeof (Customer), collection[DomainObjectIDs.Customer3].GetType());
    Assert.AreEqual (typeof (Partner), collection[DomainObjectIDs.Partner2].GetType());
    Assert.AreEqual (typeof (Supplier), collection[DomainObjectIDs.Supplier2].GetType());
    Assert.AreEqual (typeof (Distributor), collection[DomainObjectIDs.Distributor1].GetType());
  }

  [Test]
  public void ChangeTrackingEvents ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    
    DomainObjectEventReceiver eventReceiver = new DomainObjectEventReceiver (customer, false);
    customer.Name = "New name";

    Assert.AreEqual (true, eventReceiver.HasChangingEventBeenCalled);
    Assert.AreEqual (true, eventReceiver.HasChangedEventBeenCalled);
    Assert.AreEqual ("New name", customer.Name);
    Assert.AreEqual ("Kunde 1", eventReceiver.OldValue);
    Assert.AreEqual ("New name", eventReceiver.NewValue);
  }

  [Test]
  public void CancelChangeTrackingEvents ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    
    DomainObjectEventReceiver eventReceiver = new DomainObjectEventReceiver (customer, true);
    customer.Name = "New name";

    Assert.AreEqual (true, eventReceiver.HasChangingEventBeenCalled);
    Assert.AreEqual (false, eventReceiver.HasChangedEventBeenCalled);
    Assert.AreEqual ("Kunde 1", customer.Name);
    Assert.AreEqual ("Kunde 1", eventReceiver.OldValue);
    Assert.AreEqual ("New name", eventReceiver.NewValue);
  }

  [Test]
  public void StateProperty ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    
    Assert.AreEqual (StateType.Unchanged, customer.State);    
    customer.Name = "New name";
    Assert.AreEqual (StateType.Changed, customer.State);
  }

  [Test]
  public void PrivateConstructor ()
  {
    OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1); 
  }

  [Test]
  public void ProtectedConstructor ()
  {
    Company company = Company.GetObject (DomainObjectIDs.Company1); 
  }

  [Test]
  public void PublicConstructor ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1); 
  }

  [Test]
  public void InternalConstructor ()
  {
    Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1); 
  }

  [Test]
  [ExpectedException (typeof (ValueTooLongException))]
  public void MaxLengthCheck ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1); 
    
    string tooLongName = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901";
    customer.Name = tooLongName;
  }

  [Test]
  [ExpectedException (typeof (InvalidTypeException))]
  public void TypeCheck ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1); 
    
    int invalidName = 123;
    customer.NamePropertyOfInvalidType = invalidName;
  }

  [Test]
  public void TestAllOperations ()
  {
    Order order1 = Order.GetObject (DomainObjectIDs.Order1);
    Order order2 = Order.GetObject (DomainObjectIDs.Order2);

    Customer customer1 = order1.Customer;
    Customer customer4 = Customer.GetObject (DomainObjectIDs.Customer4);

    Order order3 = customer4.Orders[DomainObjectIDs.Order3];
    Order order4 = customer4.Orders[DomainObjectIDs.Order4];

    OrderTicket orderTicket1 = order1.OrderTicket;
    OrderTicket orderTicket3 = order2.OrderTicket;

    Official official1 = order1.Official;

    OrderItem orderItem1 = (OrderItem) order1.OrderItems[DomainObjectIDs.OrderItem1];
    OrderItem orderItem2 = (OrderItem) order1.OrderItems[DomainObjectIDs.OrderItem2];
    OrderItem orderItem4 = (OrderItem) order3.OrderItems[DomainObjectIDs.OrderItem4];

    order1.Delete ();
    orderItem1.Delete ();
    orderItem2.Delete ();
  
    order3.OrderNumber = 7;

    Order newOrder = new Order ();
    newOrder.DeliveryDate = DateTime.Now;
    newOrder.Official = official1;
    customer1.Orders.Add (newOrder);

    newOrder.OrderTicket = orderTicket1;
    orderTicket1.FileName = @"C:\NewFile.tif";

    OrderItem newOrderItem1 = new OrderItem ();
    newOrderItem1.Position = 1;
    newOrder.OrderItems.Add (newOrderItem1);

    OrderItem newOrderItem2 = new OrderItem ();
    newOrderItem2.Position = 2;
    order3.OrderItems.Add (newOrderItem2);

    Customer newCustomer = new Customer ();
    newCustomer.Ceo = new Ceo ();
    order2.Customer = newCustomer;

    orderTicket3.FileName = @"C:\NewFile.gif";

    Order deletedNewOrder = new Order ();
    deletedNewOrder.Delete ();

    ClientTransactionMock.Commit ();

    // expectation: no exception
  }

  [Test]
  public void TestAllOperationsWithHirarchy ()
  {
    Employee newSupervisor1 = new Employee ();
    Employee newSubordinate1 = new Employee ();
    newSubordinate1.Supervisor = newSupervisor1;

    Employee supervisor1 = Employee.GetObject (DomainObjectIDs.Employee1);
    Employee subordinate4 = Employee.GetObject (DomainObjectIDs.Employee4);

    Employee supervisor2 = Employee.GetObject (DomainObjectIDs.Employee2);
    Employee subordinate3 = Employee.GetObject (DomainObjectIDs.Employee3);
    supervisor2.Supervisor = supervisor1;
    supervisor2.Name = "New name of supervisor";
    subordinate3.Name = "New name of subordinate";

    Employee supervisor6 = Employee.GetObject (DomainObjectIDs.Employee6);
    Employee subordinate7 = Employee.GetObject (DomainObjectIDs.Employee7);

    Employee newSubordinate2 = new Employee ();
    Employee newSubordinate3 = new Employee ();

    newSupervisor1.Supervisor = supervisor2;
    newSubordinate2.Supervisor = supervisor1;
    newSubordinate3.Supervisor = supervisor6;

    supervisor1.Delete ();
    subordinate4.Delete ();

    ClientTransactionMock.Commit ();

    // expectation: no exception
  }
}
}
