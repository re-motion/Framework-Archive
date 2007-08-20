using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Resources;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.DomainObjects
{
  [TestFixture]
  public class DomainObjectTest: ClientTransactionBaseTest
  {
    public override void TestFixtureSetUp()
    {
      base.TestFixtureSetUp();
      SetDatabaseModifyable();
    }

    [Test]
    public void PublicTypeWorks ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = new ClassWithAllDataTypes ();
      Assert.AreEqual (typeof (ClassWithAllDataTypes), classWithAllDataTypes.GetPublicDomainObjectType());
    }

    private bool ShouldUseFactoryForInstantiation (Type type)
    {
      return (bool) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DomainObject), "ShouldUseFactoryForInstantiation", type);
    }

    [Test]
    public void ShouldUseFactoryForInstantiation ()
    {
      Assert.IsFalse (ShouldUseFactoryForInstantiation (typeof (ClassWithAllDataTypes)));
      Assert.IsFalse (ShouldUseFactoryForInstantiation (typeof (ClassWithAllDataTypes)));
    }

    private bool WasCreatedByFactory (object o)
    {
      return DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.WasCreatedByFactory (o.GetType ());
    }

    [Test]
    public void NewObject ()
    {
      Order order = Order.NewObject ();
      Assert.IsNotNull (order);
      order.OrderNumber = 7;
      Assert.AreEqual (7, order.OrderNumber);
    }

    [Test]
    public void PropertyAccess ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.IsFalse (WasCreatedByFactory (classWithAllDataTypes));

      Assert.AreEqual (DomainObjectIDs.ClassWithAllDataTypes1.Value, classWithAllDataTypes.ID.Value, "ID.Value");
      Assert.AreEqual (DomainObjectIDs.ClassWithAllDataTypes1.ClassID, classWithAllDataTypes.ID.ClassID, "ID.ClassID");
      Assert.AreEqual (DomainObjectIDs.ClassWithAllDataTypes1.StorageProviderID, classWithAllDataTypes.ID.StorageProviderID, "ID.StorageProviderID");

      Assert.AreEqual (false, classWithAllDataTypes.BooleanProperty, "BooleanProperty");
      Assert.AreEqual (85, classWithAllDataTypes.ByteProperty, "ByteProperty");
      Assert.AreEqual (new DateTime (2005, 1, 1), classWithAllDataTypes.DateProperty, "DateProperty");
      Assert.AreEqual (new DateTime (2005, 1, 1, 17, 0, 0), classWithAllDataTypes.DateTimeProperty, "DateTimeProperty");
      Assert.AreEqual (123456.789, classWithAllDataTypes.DecimalProperty, "DecimalProperty");
      Assert.AreEqual (987654.321, classWithAllDataTypes.DoubleProperty, "DoubleProperty");
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value1, classWithAllDataTypes.EnumProperty, "EnumProperty");
      Assert.AreEqual (new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"), classWithAllDataTypes.GuidProperty, "GuidProperty");
      Assert.AreEqual (32767, classWithAllDataTypes.Int16Property, "Int16Property");
      Assert.AreEqual (2147483647, classWithAllDataTypes.Int32Property, "Int32Property");
      Assert.AreEqual (9223372036854775807, classWithAllDataTypes.Int64Property, "Int64Property");
      Assert.AreEqual (6789.321, classWithAllDataTypes.SingleProperty, "SingleProperty");
      Assert.AreEqual ("abcdeföäü", classWithAllDataTypes.StringProperty, "StringProperty");
      Assert.AreEqual ("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", classWithAllDataTypes.StringPropertyWithoutMaxLength, "StringPropertyWithoutMaxLength");
      ResourceManager.IsEqualToImage1 (classWithAllDataTypes.BinaryProperty, "BinaryProperty");

      Assert.AreEqual (new NaBoolean (true), classWithAllDataTypes.NaBooleanProperty, "NaBooleanProperty");
      Assert.AreEqual (new NaByte (78), classWithAllDataTypes.NaByteProperty, "NaByteProperty");
      Assert.AreEqual (new NaDateTime (new DateTime (2005, 2, 1)), classWithAllDataTypes.NaDateProperty, "NaDateProperty");
      Assert.AreEqual (new NaDateTime (new DateTime (2005, 2, 1, 5, 0, 0)), classWithAllDataTypes.NaDateTimeProperty, "NaDateTimeProperty");
      Assert.AreEqual (new NaDecimal (new decimal (765.098)), classWithAllDataTypes.NaDecimalProperty, "NaDecimalProperty");
      Assert.AreEqual (new NaDouble (654321.789), classWithAllDataTypes.NaDoubleProperty, "NaDoubleProperty");
      Assert.AreEqual (new NaGuid (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}")), classWithAllDataTypes.NaGuidProperty, "NaGuidProperty");
      Assert.AreEqual (new NaInt16 (12000), classWithAllDataTypes.NaInt16Property, "NaInt16Property");
      Assert.AreEqual (new NaInt32 (-2147483647), classWithAllDataTypes.NaInt32Property, "NaInt32Property");
      Assert.AreEqual (new NaInt64 (3147483647), classWithAllDataTypes.NaInt64Property, "NaInt64Property");
      Assert.AreEqual (new NaSingle (12.456F), classWithAllDataTypes.NaSingleProperty, "NaSingleProperty");

      Assert.AreEqual (NaBoolean.Null, classWithAllDataTypes.NaBooleanWithNullValueProperty, "NaBooleanWithNullValueProperty");
      Assert.AreEqual (NaByte.Null, classWithAllDataTypes.NaByteWithNullValueProperty, "NaByteWithNullValueProperty");
      Assert.AreEqual (NaDecimal.Null, classWithAllDataTypes.NaDecimalWithNullValueProperty, "NaDecimalWithNullValueProperty");
      Assert.AreEqual (NaDateTime.Null, classWithAllDataTypes.NaDateWithNullValueProperty, "NaDateWithNullValueProperty");
      Assert.AreEqual (NaDateTime.Null, classWithAllDataTypes.NaDateTimeWithNullValueProperty, "NaDateTimeWithNullValueProperty");
      Assert.AreEqual (NaDouble.Null, classWithAllDataTypes.NaDoubleWithNullValueProperty, "NaDoubleWithNullValueProperty");
      Assert.AreEqual (NaGuid.Null, classWithAllDataTypes.NaGuidWithNullValueProperty, "NaGuidWithNullValueProperty");
      Assert.AreEqual (NaInt16.Null, classWithAllDataTypes.NaInt16WithNullValueProperty, "NaInt16WithNullValueProperty");
      Assert.AreEqual (NaInt32.Null, classWithAllDataTypes.NaInt32WithNullValueProperty, "NaInt32WithNullValueProperty");
      Assert.AreEqual (NaInt64.Null, classWithAllDataTypes.NaInt64WithNullValueProperty, "NaInt64WithNullValueProperty");
      Assert.AreEqual (NaSingle.Null, classWithAllDataTypes.NaSingleWithNullValueProperty, "NaSingleWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.StringWithNullValueProperty, "StringWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NullableBinaryProperty, "NullableBinaryProperty");
    }

    [Test]
    public void LoadingOfSimpleObject()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.AreEqual (DomainObjectIDs.ClassWithAllDataTypes1.Value, classWithAllDataTypes.ID.Value, "ID.Value");
      Assert.AreEqual (DomainObjectIDs.ClassWithAllDataTypes1.ClassID, classWithAllDataTypes.ID.ClassID, "ID.ClassID");
      Assert.AreEqual (DomainObjectIDs.ClassWithAllDataTypes1.StorageProviderID, classWithAllDataTypes.ID.StorageProviderID, "ID.StorageProviderID");

      Assert.AreEqual (false, classWithAllDataTypes.BooleanProperty, "BooleanProperty");
      Assert.AreEqual (85, classWithAllDataTypes.ByteProperty, "ByteProperty");
      Assert.AreEqual (new DateTime (2005, 1, 1), classWithAllDataTypes.DateProperty, "DateProperty");
      Assert.AreEqual (new DateTime (2005, 1, 1, 17, 0, 0), classWithAllDataTypes.DateTimeProperty, "DateTimeProperty");
      Assert.AreEqual (123456.789, classWithAllDataTypes.DecimalProperty, "DecimalProperty");
      Assert.AreEqual (987654.321, classWithAllDataTypes.DoubleProperty, "DoubleProperty");
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value1, classWithAllDataTypes.EnumProperty, "EnumProperty");
      Assert.AreEqual (new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"), classWithAllDataTypes.GuidProperty, "GuidProperty");
      Assert.AreEqual (32767, classWithAllDataTypes.Int16Property, "Int16Property");
      Assert.AreEqual (2147483647, classWithAllDataTypes.Int32Property, "Int32Property");
      Assert.AreEqual (9223372036854775807, classWithAllDataTypes.Int64Property, "Int64Property");
      Assert.AreEqual (6789.321, classWithAllDataTypes.SingleProperty, "SingleProperty");
      Assert.AreEqual ("abcdeföäü", classWithAllDataTypes.StringProperty, "StringProperty");
      Assert.AreEqual (
          "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890",
          classWithAllDataTypes.StringPropertyWithoutMaxLength,
          "StringPropertyWithoutMaxLength");
      ResourceManager.IsEqualToImage1 (classWithAllDataTypes.BinaryProperty, "BinaryProperty");

      Assert.AreEqual (new NaBoolean (true), classWithAllDataTypes.NaBooleanProperty, "NaBooleanProperty");
      Assert.AreEqual (new NaByte (78), classWithAllDataTypes.NaByteProperty, "NaByteProperty");
      Assert.AreEqual (new NaDateTime (new DateTime (2005, 2, 1)), classWithAllDataTypes.NaDateProperty, "NaDateProperty");
      Assert.AreEqual (new NaDateTime (new DateTime (2005, 2, 1, 5, 0, 0)), classWithAllDataTypes.NaDateTimeProperty, "NaDateTimeProperty");
      Assert.AreEqual (new NaDecimal (new decimal (765.098)), classWithAllDataTypes.NaDecimalProperty, "NaDecimalProperty");
      Assert.AreEqual (new NaDouble (654321.789), classWithAllDataTypes.NaDoubleProperty, "NaDoubleProperty");
      Assert.AreEqual (new NaGuid (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}")), classWithAllDataTypes.NaGuidProperty, "NaGuidProperty");
      Assert.AreEqual (new NaInt16 (12000), classWithAllDataTypes.NaInt16Property, "NaInt16Property");
      Assert.AreEqual (new NaInt32 (-2147483647), classWithAllDataTypes.NaInt32Property, "NaInt32Property");
      Assert.AreEqual (new NaInt64 (3147483647), classWithAllDataTypes.NaInt64Property, "NaInt64Property");
      Assert.AreEqual (new NaSingle (12.456F), classWithAllDataTypes.NaSingleProperty, "NaSingleProperty");

      Assert.AreEqual (NaBoolean.Null, classWithAllDataTypes.NaBooleanWithNullValueProperty, "NaBooleanWithNullValueProperty");
      Assert.AreEqual (NaByte.Null, classWithAllDataTypes.NaByteWithNullValueProperty, "NaByteWithNullValueProperty");
      Assert.AreEqual (NaDecimal.Null, classWithAllDataTypes.NaDecimalWithNullValueProperty, "NaDecimalWithNullValueProperty");
      Assert.AreEqual (NaDateTime.Null, classWithAllDataTypes.NaDateWithNullValueProperty, "NaDateWithNullValueProperty");
      Assert.AreEqual (NaDateTime.Null, classWithAllDataTypes.NaDateTimeWithNullValueProperty, "NaDateTimeWithNullValueProperty");
      Assert.AreEqual (NaDouble.Null, classWithAllDataTypes.NaDoubleWithNullValueProperty, "NaDoubleWithNullValueProperty");
      Assert.AreEqual (NaGuid.Null, classWithAllDataTypes.NaGuidWithNullValueProperty, "NaGuidWithNullValueProperty");
      Assert.AreEqual (NaInt16.Null, classWithAllDataTypes.NaInt16WithNullValueProperty, "NaInt16WithNullValueProperty");
      Assert.AreEqual (NaInt32.Null, classWithAllDataTypes.NaInt32WithNullValueProperty, "NaInt32WithNullValueProperty");
      Assert.AreEqual (NaInt64.Null, classWithAllDataTypes.NaInt64WithNullValueProperty, "NaInt64WithNullValueProperty");
      Assert.AreEqual (NaSingle.Null, classWithAllDataTypes.NaSingleWithNullValueProperty, "NaSingleWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.StringWithNullValueProperty, "StringWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NullableBinaryProperty, "NullableBinaryProperty");
    }

    [Test]
    public void LoadingOfDerivedObject ()
    {
      Company company = Company.GetObject (DomainObjectIDs.Partner2);
      Assert.IsNotNull (company);
      Assert.IsFalse (WasCreatedByFactory (company));

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
      Assert.IsFalse (WasCreatedByFactory (company));

      Supplier supplier = company as Supplier;
      Assert.IsNotNull (supplier);

      Assert.AreEqual (DomainObjectIDs.Supplier1, supplier.ID);
      Assert.AreEqual ("Lieferant 1", supplier.Name, "Name");
      Assert.AreEqual (DomainObjectIDs.Person3, supplier.ContactPerson.ID, "ContactPerson");
      Assert.AreEqual (1, supplier.SupplierQuality, "SupplierQuality");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Property 'BooleanProperty' does not allow null values.")]
    public void GetNullFromNonNullableValueType()
    {
      ClassWithAllDataTypes classWithAllDataTypes = new ClassWithAllDataTypes();
      classWithAllDataTypes.DataContainer["BooleanProperty"] = null;
      Dev.Null = classWithAllDataTypes.BooleanProperty;
    }

    [Test]
    public void OnLoaded ()
    {
      ObjectID id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (id);
      Assert.IsFalse (WasCreatedByFactory (classWithAllDataTypes));

      Assert.IsTrue (classWithAllDataTypes.OnLoadedHasBeenCalled);
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.IsFalse (WasCreatedByFactory (order));

      Assert.IsNotNull (order.OrderTicket);
      Assert.AreEqual (DomainObjectIDs.OrderTicket1, order.OrderTicket.ID);
      Assert.IsFalse (WasCreatedByFactory (order.OrderTicket));
    }

    [Test]
    public void GetRelatedObjectByInheritedRelationTwice ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer4);
      Assert.IsFalse (WasCreatedByFactory (customer));

      Ceo ceoReference1 = customer.Ceo;

      Ceo ceoReference2 = customer.Ceo;

      Assert.AreSame (ceoReference1, ceoReference2);
      Assert.IsFalse (WasCreatedByFactory (ceoReference1));
    }

    [Test]
    public void GetDerivedRelatedObject ()
    {
      Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo10);
      Assert.IsFalse (WasCreatedByFactory (ceo));

      Company company = ceo.Company;
      Assert.IsNotNull (company);
      Assert.IsFalse (WasCreatedByFactory (company));


      Distributor distributor = company as Distributor;
      Assert.IsNotNull (distributor);
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      Assert.IsFalse (WasCreatedByFactory (customer));

      Assert.IsNotNull (customer.Orders);
      Assert.AreEqual (2, customer.Orders.Count);

      Assert.AreEqual (DomainObjectIDs.Order1, customer.Orders[DomainObjectIDs.Order1].ID);
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, customer.Orders[DomainObjectIDs.OrderWithoutOrderItem].ID);

      Assert.IsFalse (WasCreatedByFactory (customer.Orders[DomainObjectIDs.Order1]));
      Assert.IsFalse (WasCreatedByFactory (customer.Orders[DomainObjectIDs.OrderWithoutOrderItem]));
    }

    [Test]
    public void GetRelatedObjectsWithDerivation ()
    {
      IndustrialSector industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector2);
      DomainObjectCollection collection = industrialSector.Companies;

      Assert.AreEqual (7, collection.Count);
      foreach (object o in collection)
      {
        Assert.IsFalse (WasCreatedByFactory (o));
      }

      Assert.IsTrue (collection[DomainObjectIDs.Company1] is Company);
      Assert.IsTrue (collection[DomainObjectIDs.Company2] is Company);
      Assert.IsTrue (collection[DomainObjectIDs.Customer2] is Customer);
      Assert.IsTrue (collection[DomainObjectIDs.Customer3] is Customer);
      Assert.IsTrue (collection[DomainObjectIDs.Partner2] is Partner);
      Assert.IsTrue (collection[DomainObjectIDs.Supplier2] is Supplier);
      Assert.IsTrue (collection[DomainObjectIDs.Distributor1] is Distributor);
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
      Assert.AreEqual ("Kunde 1", eventReceiver.ChangingOldValue);
      Assert.AreEqual ("New name", eventReceiver.ChangingNewValue);
      Assert.AreEqual ("Kunde 1", eventReceiver.ChangedOldValue);
      Assert.AreEqual ("New name", eventReceiver.ChangedNewValue);
    }

    [Test]
    public void CancelChangeTrackingEvents ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      DomainObjectEventReceiver eventReceiver = new DomainObjectEventReceiver (customer, true);

      try
      {
        customer.Name = "New name";
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreEqual (true, eventReceiver.HasChangingEventBeenCalled);
        Assert.AreEqual (false, eventReceiver.HasChangedEventBeenCalled);
        Assert.AreEqual ("Kunde 1", customer.Name);
        Assert.AreEqual ("Kunde 1", eventReceiver.ChangingOldValue);
        Assert.AreEqual ("New name", eventReceiver.ChangingNewValue);
      }
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
      Dev.Null = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
    }

    [Test]
    public void ProtectedConstructor ()
    {
      Dev.Null = Company.GetObject (DomainObjectIDs.Company1);
    }

    [Test]
    public void PublicConstructor ()
    {
      Dev.Null = Customer.GetObject (DomainObjectIDs.Customer1);
    }

    [Test]
    public void InternalConstructor ()
    {
      Dev.Null = Ceo.GetObject (DomainObjectIDs.Ceo1);
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
      ObjectID newOrderID = newOrder.ID;
      newOrder.DeliveryDate = DateTime.Now;
      newOrder.Official = official1;
      customer1.Orders.Add (newOrder);

      newOrder.OrderTicket = orderTicket1;
      orderTicket1.FileName = @"C:\NewFile.tif";

      OrderItem newOrderItem1 = new OrderItem ();
      ObjectID newOrderItem1ID = newOrderItem1.ID;

      newOrderItem1.Position = 1;
      newOrder.OrderItems.Add (newOrderItem1);

      OrderItem newOrderItem2 = new OrderItem ();
      ObjectID newOrderItem2ID = newOrderItem2.ID;
      newOrderItem2.Position = 2;
      order3.OrderItems.Add (newOrderItem2);

      Customer newCustomer = new Customer ();
      ObjectID newCustomerID = newCustomer.ID;

      Ceo newCeo = new Ceo ();
      ObjectID newCeoID = newCeo.ID;
      newCustomer.Ceo = newCeo;
      order2.Customer = newCustomer;

      orderTicket3.FileName = @"C:\NewFile.gif";

      Order deletedNewOrder = new Order ();
      deletedNewOrder.Delete ();

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

      CheckIfObjectIsDeleted (DomainObjectIDs.Order1);
      CheckIfObjectIsDeleted (DomainObjectIDs.OrderItem1);
      CheckIfObjectIsDeleted (DomainObjectIDs.OrderItem2);

      order3 = Order.GetObject (DomainObjectIDs.Order3);
      Assert.AreEqual (7, order3.OrderNumber);

      newOrder = Order.GetObject (newOrderID);
      Assert.IsNotNull (newOrder);

      official1 = Official.GetObject (DomainObjectIDs.Official1);
      Assert.IsNotNull (official1.Orders[newOrderID]);
      Assert.AreSame (official1, newOrder.Official);
      Assert.IsNull (official1.Orders[DomainObjectIDs.Order1]);

      orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      Assert.AreEqual (@"C:\NewFile.tif", orderTicket1.FileName);
      Assert.AreSame (newOrder, orderTicket1.Order);
      Assert.AreSame (orderTicket1, newOrder.OrderTicket);

      newOrderItem1 = OrderItem.GetObject (newOrderItem1ID);
      Assert.IsNotNull (newOrderItem1);
      Assert.AreEqual (1, newOrderItem1.Position);
      Assert.AreSame (newOrder, newOrderItem1.Order);
      Assert.IsNotNull (newOrder.OrderItems[newOrderItem1ID]);

      newOrderItem2 = OrderItem.GetObject (newOrderItem2ID);
      Assert.IsNotNull (newOrderItem2);
      Assert.AreEqual (2, newOrderItem2.Position);
      Assert.AreSame (order3, newOrderItem2.Order);
      Assert.IsNotNull (order3.OrderItems[newOrderItem2ID]);

      newCustomer = Customer.GetObject (newCustomerID);
      newCeo = Ceo.GetObject (newCeoID);

      Assert.AreSame (newCustomer, newCeo.Company);
      Assert.AreSame (newCeo, newCustomer.Ceo);
      Assert.IsTrue (newCustomer.Orders.Contains (DomainObjectIDs.Order2));
      Assert.AreSame (newCustomer, ((Order) newCustomer.Orders[DomainObjectIDs.Order2]).Customer);

      orderTicket3 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket3);
      Assert.AreEqual (@"C:\NewFile.gif", orderTicket3.FileName);
    }

    [Test]
    public void TestAllOperationsWithHierarchy ()
    {
      Employee newSupervisor1 = new Employee ();
      ObjectID newSupervisor1ID = newSupervisor1.ID;

      Employee newSubordinate1 = new Employee ();
      ObjectID newSubordinate1ID = newSubordinate1.ID;
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
      ObjectID newSubordinate2ID = newSubordinate2.ID;
      Employee newSubordinate3 = new Employee ();
      ObjectID newSubordinate3ID = newSubordinate3.ID;

      newSupervisor1.Supervisor = supervisor2;
      newSubordinate2.Supervisor = supervisor1;
      newSubordinate3.Supervisor = supervisor6;

      supervisor1.Delete ();
      subordinate4.Delete ();

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

      newSupervisor1 = Employee.GetObject (newSupervisor1ID);
      newSubordinate1 = Employee.GetObject (newSubordinate1ID);

      Assert.AreSame (newSupervisor1, newSubordinate1.Supervisor);
      Assert.IsTrue (newSupervisor1.Subordinates.Contains (newSubordinate1ID));

      supervisor2 = Employee.GetObject (DomainObjectIDs.Employee2);

      Assert.IsNull (supervisor2.Supervisor);
      Assert.AreEqual ("New name of supervisor", supervisor2.Name);

      subordinate3 = Employee.GetObject (DomainObjectIDs.Employee3);

      Assert.AreSame (supervisor2, subordinate3.Supervisor);
      Assert.IsTrue (supervisor2.Subordinates.Contains (DomainObjectIDs.Employee3));
      Assert.AreEqual ("New name of subordinate", subordinate3.Name);

      Assert.AreSame (supervisor2, newSupervisor1.Supervisor);
      Assert.IsTrue (supervisor2.Subordinates.Contains (newSupervisor1ID));

      newSubordinate2 = Employee.GetObject (newSubordinate2ID);

      Assert.IsNull (newSubordinate2.Supervisor);

      supervisor6 = Employee.GetObject (DomainObjectIDs.Employee6);
      newSubordinate3 = Employee.GetObject (newSubordinate3ID);

      Assert.AreSame (supervisor6, newSubordinate3.Supervisor);
      Assert.IsTrue (supervisor6.Subordinates.Contains (newSubordinate3ID));

      CheckIfObjectIsDeleted (DomainObjectIDs.Employee1);
      CheckIfObjectIsDeleted (DomainObjectIDs.Employee4);
    }

    [Test]
    public void DeleteNewObjectWithExistingRelated ()
    {
      Computer computer4 = Computer.GetObject (DomainObjectIDs.Computer4);

      Employee newDeletedEmployee = new Employee ();
      computer4.Employee = newDeletedEmployee;

      newDeletedEmployee.Delete ();

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

      computer4 = Computer.GetObject (DomainObjectIDs.Computer4);
      Assert.IsNull (computer4.Employee);
    }

    [Test]
    public void ExistingObjectRelatesToNewAndDeleted ()
    {
      Partner partner = Partner.GetObject (DomainObjectIDs.Partner2);

      Person newPerson = new Person ();
      partner.ContactPerson = newPerson;
      partner.IndustrialSector.Delete ();

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

      partner = Partner.GetObject (DomainObjectIDs.Partner2);
      Assert.AreEqual (newPerson.ID, partner.ContactPerson.ID);
      Assert.IsNull (partner.IndustrialSector);
    }

    [Test]
    public void GetObjectWithTransaction ()
    {
      ClientTransactionMock clientTransactionMock = new ClientTransactionMock ();
      using (new ClientTransactionScope (clientTransactionMock))
      {
        Order order = (Order) TestDomainBase.GetObject (DomainObjectIDs.Order1);

        Assert.AreSame (clientTransactionMock, order.DataContainer.ClientTransaction);
        Assert.IsFalse (object.ReferenceEquals (this.ClientTransactionMock, order.DataContainer.ClientTransaction));
      }
    }

    [Test]
    public void GetDeletedObjectWithTransaction ()
    {
      ClientTransactionMock clientTransactionMock = new ClientTransactionMock ();
      using (new ClientTransactionScope (clientTransactionMock))
      {
        Order order = (Order) TestDomainBase.GetObject (DomainObjectIDs.Order1);

        order.Delete ();

        order = (Order) TestDomainBase.GetObject (DomainObjectIDs.Order1, true);

        Assert.AreEqual (StateType.Deleted, order.State);
        Assert.AreSame (clientTransactionMock, order.DataContainer.ClientTransaction);
        Assert.IsFalse (object.ReferenceEquals (this.ClientTransactionMock, order.DataContainer.ClientTransaction));
      }
    }

    [Test]
    public void CreateNewObjectWithTransaction ()
    {
      ClientTransactionMock clientTransactionMock = new ClientTransactionMock ();
      using (new ClientTransactionScope (clientTransactionMock))
      {
        Order order = new Order ();

        Assert.AreSame (clientTransactionMock, order.DataContainer.ClientTransaction);
        Assert.IsFalse (object.ReferenceEquals (this.ClientTransactionMock, order.DataContainer.ClientTransaction));
      }
    }

    [Test]
    public void GetRelatedObjectsWithCorrectOrder ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      Assert.AreEqual (DomainObjectIDs.Order1, customer.Orders[0].ID);
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, customer.Orders[1].ID);
    }

    [Test]
    public void GetRelatedObjectsWithCorrectOrderWithLazyLoad ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      Order orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

      Assert.AreEqual (DomainObjectIDs.Order1, customer.Orders[0].ID);
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, customer.Orders[1].ID);
    }
  }
}