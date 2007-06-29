using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class ClientTransactionExtensionTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IClientTransactionExtension _extension;

    private Order _order1;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _mockRepository = new MockRepository ();
      _extension = _mockRepository.CreateMock<IClientTransactionExtension> ();

      ClientTransactionScope.CurrentTransaction.Extensions.Add ("Name", _extension);
    }

    [Test]
    public void Extensions ()
    {
      Assert.IsNotNull (ClientTransactionScope.CurrentTransaction.Extensions);
    }

    [Test]
    public void NewObjectCreation ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.NewObjectCreating (typeof (Order));
      }

      _mockRepository.ReplayAll ();

      Order.NewObject ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectLoading ()
    {
      ClientTransaction newTransaction = new ClientTransaction();
      newTransaction.Extensions.Add ("Name", _extension);
      _mockRepository.BackToRecordAll ();
      using (newTransaction.EnterScope())
      {
        using (_mockRepository.Ordered())
        {
          _extension.ObjectLoading (DomainObjectIDs.Order1);
          _extension.ObjectsLoaded (null);
          LastCall.Constraints (Mocks_Is.NotNull());
        }

        _mockRepository.ReplayAll();

        Order order = Order.GetObject (DomainObjectIDs.Order1);
        order = Order.GetObject (DomainObjectIDs.Order1);

        _mockRepository.VerifyAll();
      }
    }

    private void RecordObjectLoadingCalls (ObjectID expectedMainObjectID, bool expectingCollection, bool expectLoadedEvent,
        ObjectID[] expectedRelatedObjectIDs)
    {
      using (_mockRepository.Ordered ())
      {
        // loading of main object
        _extension.ObjectLoading (expectedMainObjectID);
        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Is.NotNull ());

        // accessing relation property

        _extension.RelationReading (null, null, ValueAccess.Current);
        LastCall.IgnoreArguments ();

        foreach (ObjectID relatedID in expectedRelatedObjectIDs)
          _extension.ObjectLoading (relatedID);

        if (expectLoadedEvent)
        {
          _extension.ObjectsLoaded (null);
          LastCall.IgnoreArguments();
        }

        if (expectingCollection)
          _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        else
          _extension.RelationRead (null, null, (DomainObject) null, ValueAccess.Current);

        LastCall.IgnoreArguments();

        // loading of main object a second time

        // accessing relation property a second time

        _extension.RelationReading (null, null, ValueAccess.Current);
        LastCall.IgnoreArguments();

        if (expectingCollection)
          _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        else
          _extension.RelationRead (null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.IgnoreArguments();
      }
    }

    private void TestObjectLoadingWithRelatedObjects (Proc accessCode, ObjectID expectedMainObjectID, bool expectCollection, bool expectLoadedEvent,
        ObjectID[] expectedRelatedIDs)
    {
      ClientTransaction newTransaction = new ClientTransaction ();
      newTransaction.Extensions.Add ("Name", _extension);
      _mockRepository.BackToRecordAll ();
      using (newTransaction.EnterScope ())
      {
        RecordObjectLoadingCalls (expectedMainObjectID, expectCollection, expectLoadedEvent, expectedRelatedIDs);

        _mockRepository.ReplayAll ();

        accessCode ();
        accessCode ();

        _mockRepository.VerifyAll ();
      }
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Order order = Order.GetObject (DomainObjectIDs.Order1);
            int orderItemCount = order.OrderItems.Count;
            Assert.AreEqual (2, orderItemCount);
          }, DomainObjectIDs.Order1, true, true, new ObjectID[] { DomainObjectIDs.OrderItem2, DomainObjectIDs.OrderItem1 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
            Order order = orderItem.Order;
            Assert.IsNotNull (order);
          }, DomainObjectIDs.OrderItem1, false, true, new ObjectID[] { DomainObjectIDs.Order1 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
            Employee employee = computer.Employee;
            Assert.IsNotNull (employee);
          }, DomainObjectIDs.Computer1, false, true, new ObjectID[] { DomainObjectIDs.Employee3 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
            Computer computer = employee.Computer;
            Assert.IsNotNull (computer);
          }, DomainObjectIDs.Employee3, false, true, new ObjectID[] { DomainObjectIDs.Computer1 });
    }

    [Test]
    public void EmptyObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Official official = Official.GetObject (DomainObjectIDs.Official2);
            int count = official.Orders.Count;
            Assert.AreEqual (0, count);
          }, DomainObjectIDs.Official2, true, false, new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Client client = Client.GetObject (DomainObjectIDs.Client1);
            Client parent = client.ParentClient;
            Assert.IsNull (parent);
          }, DomainObjectIDs.Client1, false, false, new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
            Employee employee = computer.Employee;
            Assert.IsNull (employee);
          }, DomainObjectIDs.Computer4, false, false, new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Employee employee = Employee.GetObject (DomainObjectIDs.Employee7);
            Computer computer = employee.Computer;
            Assert.IsNull (computer);
          }, DomainObjectIDs.Employee7, false, false, new ObjectID[] { });
    }

    [Test]
    public void ObjectsLoaded ()
    {
      _extension.ObjectLoading (DomainObjectIDs.ClassWithAllDataTypes1);
      _extension.ObjectsLoaded (null);
      LastCall.Constraints (Mocks_Property.Value ("Count", 1));

      _mockRepository.ReplayAll();

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectsLoadedWithRelations ()
    {
      _extension.ObjectLoading (DomainObjectIDs.Order2);
      _extension.ObjectsLoaded (null);
      LastCall.Constraints (Mocks_Property.Value ("Count", 1));

      _mockRepository.ReplayAll();

      Order order = Order.GetObject (DomainObjectIDs.Order2);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectsLoadedWithEvents ()
    {
      ClientTransactionMockEventReceiver clientTransactionEventReceiver =
          _mockRepository.CreateMock<ClientTransactionMockEventReceiver> (ClientTransactionScope.CurrentTransaction);

      using (_mockRepository.Ordered())
      {
        _extension.ObjectLoading (DomainObjectIDs.ClassWithAllDataTypes1);
        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1));

        clientTransactionEventReceiver.Loaded (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionScope.CurrentTransaction),
            Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));
      }

      _mockRepository.ReplayAll();

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectDelete ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      DomainObjectMockEventReceiver computerEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (computer);
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.ObjectDeleting (computer);
        computerEventReceiver.Deleting (computer, EventArgs.Empty);
        _extension.ObjectDeleted (computer);
        computerEventReceiver.Deleted (computer, EventArgs.Empty);
      }

      _mockRepository.ReplayAll();

      computer.Delete();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectDeleteWithOldRelatedObjects ()
    {
      OrderItem orderItem1 = (OrderItem) _order1.OrderItems[0];
      OrderItem orderItem2 = (OrderItem) _order1.OrderItems[1];
      OrderTicket orderTicket = _order1.OrderTicket;
      Official official = _order1.Official;
      Customer customer = _order1.Customer;
      OrderCollection customerOrders = customer.Orders;
      DomainObjectCollection officialOrders = official.Orders;
      Order preloadedOrder1 = orderTicket.Order;

      DomainObjectMockEventReceiver order1MockEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (_order1);
      DomainObjectMockEventReceiver orderItem1MockEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (orderItem1);
      DomainObjectMockEventReceiver orderItem2MockEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (orderItem2);
      DomainObjectMockEventReceiver orderTicketMockEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (orderTicket);
      DomainObjectMockEventReceiver officialMockEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (official);
      DomainObjectMockEventReceiver customerMockEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (customer);

      DomainObjectCollectionMockEventReceiver customerOrdersMockEventReceiver =
          _mockRepository.CreateMock<DomainObjectCollectionMockEventReceiver> (customerOrders);

      DomainObjectCollectionMockEventReceiver officialOrdersMockEventReceiver =
          _mockRepository.CreateMock<DomainObjectCollectionMockEventReceiver> (officialOrders);

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.ObjectDeleting (_order1);

        using (_mockRepository.Unordered())
        {
          _extension.RelationChanging (customer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _order1, null);
          _extension.RelationChanging (orderTicket, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", _order1, null);
          _extension.RelationChanging (orderItem1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", _order1, null);
          _extension.RelationChanging (orderItem2, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", _order1, null);
          _extension.RelationChanging (official, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Official.Orders", _order1, null);
        }

        order1MockEventReceiver.Deleting (_order1, EventArgs.Empty);

        using (_mockRepository.Unordered())
        {
          customerMockEventReceiver.RelationChanging (customer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _order1, null);
          customerOrdersMockEventReceiver.Removing (customerOrders, _order1);
          orderTicketMockEventReceiver.RelationChanging (
              orderTicket, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", _order1, null);
          orderItem1MockEventReceiver.RelationChanging (orderItem1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", _order1, null);
          orderItem2MockEventReceiver.RelationChanging (orderItem2, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", _order1, null);
          officialMockEventReceiver.RelationChanging (official, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Official.Orders", _order1, null);
          officialOrdersMockEventReceiver.Removing (officialOrders, _order1);
          LastCall.Constraints (Mocks_Is.Same (officialOrders), Mocks_Property.Value ("DomainObject", _order1));
        }

        using (_mockRepository.Unordered())
        {
          _extension.RelationChanged (customer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
          _extension.RelationChanged (orderTicket, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
          _extension.RelationChanged (orderItem1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
          _extension.RelationChanged (orderItem2, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
          _extension.RelationChanged (official, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Official.Orders");
        }

        _extension.ObjectDeleted (_order1);

        using (_mockRepository.Unordered())
        {
          customerMockEventReceiver.RelationChanged (customer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
          customerOrdersMockEventReceiver.Removed (customerOrders, _order1);
          orderTicketMockEventReceiver.RelationChanged (orderTicket, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
          orderItem1MockEventReceiver.RelationChanged (orderItem1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
          orderItem2MockEventReceiver.RelationChanged (orderItem2, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
          officialMockEventReceiver.RelationChanged (official, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Official.Orders");
          officialOrdersMockEventReceiver.Removed (officialOrders, _order1);
        }

        order1MockEventReceiver.Deleted (_order1, EventArgs.Empty);
      }

      _mockRepository.ReplayAll();
      _order1.Delete();
      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectDeleteTwice ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.ObjectDeleting (computer);
        _extension.ObjectDeleted (computer);
      }

      _mockRepository.ReplayAll();

      computer.Delete();
      computer.Delete();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyRead ()
    {
      int orderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.PropertyValueReading (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            ValueAccess.Current);
        _extension.PropertyValueRead (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            orderNumber,
            ValueAccess.Current);
        _extension.PropertyValueReading (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            ValueAccess.Original);
        _extension.PropertyValueRead (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            orderNumber,
            ValueAccess.Original);
      }

      _mockRepository.ReplayAll();
      orderNumber = _order1.OrderNumber;
      orderNumber =
          (int) _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].OriginalValue;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyReadWithoutDataContainer ()
    {
      ClassDefinition orderClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      PropertyDefinition orderNumberDefinition = orderClass.MyPropertyDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];

      PropertyValue propertyValue = new PropertyValue (orderNumberDefinition);
      PropertyValueCollection propertyValueCollection = new PropertyValueCollection();
      propertyValueCollection.Add (propertyValue);

      int orderNumber = (int) propertyValue.Value;

      // Expectation: no exception
    }

    [Test]
    public void ReadObjectIDProperty ()
    {
      PropertyValue customerPropertyValue =
          _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"];
      ObjectID customerID =
          (ObjectID) _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].Value;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.PropertyValueReading (_order1.InternalDataContainer, customerPropertyValue, ValueAccess.Current);
        _extension.PropertyValueRead (_order1.InternalDataContainer, customerPropertyValue, customerID, ValueAccess.Current);
      }

      _mockRepository.ReplayAll();

      customerID = (ObjectID) _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].Value;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertySetToSameValue ()
    {
      int orderNumber = _order1.OrderNumber;

      _mockRepository.BackToRecord (_extension);
      // Note: No method call on the extension is expected.
      _mockRepository.ReplayAll();

      _order1.OrderNumber = orderNumber;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ChangeAndReadProperty ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      int newOrderNumber = oldOrderNumber + 1;

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.PropertyValueChanging (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            newOrderNumber);
        _extension.PropertyValueChanged (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            newOrderNumber);

        _extension.PropertyValueReading (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            ValueAccess.Current);
        _extension.PropertyValueRead (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            newOrderNumber,
            ValueAccess.Current);
        _extension.PropertyValueReading (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            ValueAccess.Original);
        _extension.PropertyValueRead (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            ValueAccess.Original);
      }

      _mockRepository.ReplayAll();
      _order1.OrderNumber = newOrderNumber;
      newOrderNumber = _order1.OrderNumber;
      oldOrderNumber =
          (int) _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].OriginalValue;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyChange ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.PropertyValueChanging (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);
        _extension.PropertyValueChanged (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll();
      _order1.OrderNumber = oldOrderNumber + 1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyChangeWithEvents ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      DomainObjectMockEventReceiver domainObjectMockEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (_order1);
      DataContainerMockEventReceiver dataContainerMockEventReceiver =
          _mockRepository.CreateMock<DataContainerMockEventReceiver> (_order1.InternalDataContainer);
      PropertyValueCollectionMockEventReceiver propertyValueCollectionMockEventReceiver =
          _mockRepository.CreateMock<PropertyValueCollectionMockEventReceiver> (_order1.InternalDataContainer.PropertyValues);
      PropertyValueMockEventReceiver propertyValueMockEventReceiver =
          _mockRepository.CreateMock<PropertyValueMockEventReceiver> (
              _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]);

      using (_mockRepository.Ordered())
      {
        // "Changing" notifications

        _extension.PropertyValueChanging (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);

        domainObjectMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments();

        dataContainerMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments();

        propertyValueCollectionMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments();

        propertyValueMockEventReceiver.Changing (null, null);
        LastCall.IgnoreArguments();


        // "Changed" notifications

        propertyValueMockEventReceiver.Changed (null, null);
        LastCall.IgnoreArguments();

        propertyValueCollectionMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments();

        dataContainerMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments();

        domainObjectMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments();

        _extension.PropertyValueChanged (
            _order1.InternalDataContainer,
            _order1.InternalDataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll();
      _order1.OrderNumber = oldOrderNumber + 1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void LoadRelatedDataContainerForEndPoint ()
    {
      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      _mockRepository.BackToRecord (_extension);

      //Note: no reading notification must be performed

      _mockRepository.ReplayAll();

      using (PersistenceManager persistanceManager = new PersistenceManager())
      {
        ClassDefinition orderTicketDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (OrderTicket)];
        IRelationEndPointDefinition orderEndPointDefinition =
            orderTicketDefinition.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
        persistanceManager.LoadRelatedDataContainer (
            orderTicket.InternalDataContainer, new RelationEndPointID (orderTicket.ID, orderEndPointDefinition));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void LoadRelatedDataContainerForVirtualEndPoint ()
    {
      //Note: no reading notification must be performed
      _mockRepository.ReplayAll();

      using (PersistenceManager persistenceManager = new PersistenceManager())
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Order)];
        IRelationEndPointDefinition orderTicketEndPointDefinition =
            orderDefinition.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
        persistenceManager.LoadRelatedDataContainer (
            _order1.InternalDataContainer, new RelationEndPointID (_order1.ID, orderTicketEndPointDefinition));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObject ()
    {
      OrderTicket orderTicket = _order1.OrderTicket;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", ValueAccess.Current);
        _extension.RelationRead (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", orderTicket, ValueAccess.Current);
      }

      _mockRepository.ReplayAll();

      orderTicket = _order1.OrderTicket;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      OrderTicket originalOrderTicket =
          (OrderTicket) _order1.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", ValueAccess.Original);
        _extension.RelationRead (
            _order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", originalOrderTicket, ValueAccess.Original);
      }

      _mockRepository.ReplayAll();

      originalOrderTicket = (OrderTicket) _order1.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjects ()
    {
      DomainObjectCollection orderItems = _order1.OrderItems;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);

        LastCall.Constraints (
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("IsReadOnly", true) & Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (orderItems[0])
            & Mocks_List.IsIn (orderItems[1]),
            Mocks_Is.Equal (ValueAccess.Current));
      }

      _mockRepository.ReplayAll();

      orderItems = _order1.OrderItems;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      DomainObjectCollection originalOrderItems =
          _order1.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", ValueAccess.Original);
        _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Original);

        LastCall.Constraints (
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("IsReadOnly", true) & Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (originalOrderItems[0])
            & Mocks_List.IsIn (originalOrderItems[1]),
            Mocks_Is.Equal (ValueAccess.Original));
      }

      _mockRepository.ReplayAll();

      originalOrderItems = _order1.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", ValueAccess.Current);

        _extension.ObjectLoading (null);
        LastCall.IgnoreArguments ();

        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1));
        _extension.RelationRead (null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Current));
      }
      _mockRepository.ReplayAll();

      OrderTicket orderTicket = _order1.OrderTicket;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", ValueAccess.Current);
        
        _extension.ObjectLoading (null);
        LastCall.IgnoreArguments ();
        _extension.ObjectLoading (null);
        LastCall.IgnoreArguments ();

        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2));
        _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Current));
      }
      _mockRepository.ReplayAll();

      DomainObjectCollection orderItems = _order1.OrderItems;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", ValueAccess.Original);

        _extension.ObjectLoading (null);
        LastCall.IgnoreArguments ();
        
        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1));
        _extension.RelationRead (null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll();

      OrderTicket orderTicket = (OrderTicket) _order1.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", ValueAccess.Original);

        _extension.ObjectLoading (null);
        LastCall.IgnoreArguments ();
        _extension.ObjectLoading (null);
        LastCall.IgnoreArguments ();

        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2));
        _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll();

      DomainObjectCollection orderItems = _order1.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResult ()
    {
      Query query = new Query ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1);

      ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      _mockRepository.BackToRecord (_extension);

      _extension.FilterQueryResult (null, null);
      LastCall.Constraints (Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query));

      _mockRepository.ReplayAll();

      ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResultWithLoad ()
    {
      Query query = new Query ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);

      using (_mockRepository.Ordered())
      {
        _extension.ObjectLoading (null);
        LastCall.IgnoreArguments ();
        _extension.ObjectLoading (null);
        LastCall.IgnoreArguments ();

        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2));
        _extension.FilterQueryResult (null, null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query));
      }

      _mockRepository.ReplayAll();

      ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResultWithFiltering ()
    {
      Query query = new Query ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);
      IClientTransactionExtension filteringExtension = _mockRepository.CreateMock<ClientTransactionExtensionWithQueryFiltering>();
      ClientTransactionScope.CurrentTransaction.Extensions.Add ("FilteringExtension", filteringExtension);
      IClientTransactionExtension lastExtension = _mockRepository.CreateMock<IClientTransactionExtension>();
      ClientTransactionScope.CurrentTransaction.Extensions.Add ("LastExtension", lastExtension);


      using (_mockRepository.Ordered())
      {

        for (int i = 0; i < 2; ++i)
        {
          _extension.ObjectLoading (null);
          LastCall.IgnoreArguments();
          filteringExtension.ObjectLoading (null);
          LastCall.IgnoreArguments();
          lastExtension.ObjectLoading (null);
          LastCall.IgnoreArguments();
        }

        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2));
        filteringExtension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2));
        lastExtension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2));

        _extension.FilterQueryResult (null, null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query));
        filteringExtension.FilterQueryResult (null, null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query)).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        lastExtension.FilterQueryResult (null, null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1), Mocks_Is.Same (query));
      }

      _mockRepository.ReplayAll();

      DomainObjectCollection queryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      Assert.AreEqual (1, queryResult.Count);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedPropertyValue ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.Committing (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
        _extension.Committed (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll();

      ClientTransactionScope.CurrentTransaction.Commit();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedRelationValue ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee1);
      computer.Employee = employee;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.Committing (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (computer) & Mocks_List.IsIn (employee));
        _extension.Committed (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (computer) & Mocks_List.IsIn (employee));
      }

      _mockRepository.ReplayAll();

      ClientTransactionScope.CurrentTransaction.Commit();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedRelationValueWithClassIDColumn ()
    {
      Customer oldCustomer = _order1.Customer;
      Customer newCustomer = Customer.GetObject (DomainObjectIDs.Customer2);
      _order1.Customer = newCustomer;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.Committing (null);
        LastCall.Constraints (
            Mocks_Property.Value ("Count", 3) & Mocks_List.IsIn (_order1) & Mocks_List.IsIn (newCustomer) & Mocks_List.IsIn (oldCustomer));
        _extension.Committed (null);
        LastCall.Constraints (
            Mocks_Property.Value ("Count", 3) & Mocks_List.IsIn (_order1) & Mocks_List.IsIn (newCustomer) & Mocks_List.IsIn (oldCustomer));
      }

      _mockRepository.ReplayAll();

      ClientTransactionScope.CurrentTransaction.Commit();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithEvents ()
    {
      SetDatabaseModifyable();

      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";
      _mockRepository.BackToRecord (_extension);

      ClientTransactionMockEventReceiver clientTransactionMockEventReceiver =
          _mockRepository.CreateMock<ClientTransactionMockEventReceiver> (ClientTransactionScope.CurrentTransaction);

      DomainObjectMockEventReceiver computerEventReveiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (computer);

      using (_mockRepository.Ordered())
      {
        computerEventReveiver.Committing (computer, EventArgs.Empty);

        _extension.Committing (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));

        clientTransactionMockEventReceiver.Committing (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionScope.CurrentTransaction),
            Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));

        computerEventReveiver.Committed (computer, EventArgs.Empty);

        clientTransactionMockEventReceiver.Committed (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionScope.CurrentTransaction),
            Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));

        _extension.Committed (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll();

      ClientTransactionScope.CurrentTransaction.Commit();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void StorageProviderGetFieldValue ()
    {
      int originalOrderNumber = _order1.OrderNumber;
      _order1.OrderNumber = originalOrderNumber + 1;

      _mockRepository.BackToRecord (_extension);

      using (StorageProviderManager storageProviderManager = new StorageProviderManager())
      {
        using (UnitTestStorageProviderStub storageProvider =
            (UnitTestStorageProviderStub) storageProviderManager.GetMandatory (c_unitTestStorageProviderStubID))
        {
          _mockRepository.ReplayAll();

          Assert.AreEqual (
              originalOrderNumber + 1,
              storageProvider.GetFieldValue (
                  _order1.InternalDataContainer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber", ValueAccess.Current));
          Assert.AreEqual (
              originalOrderNumber,
              storageProvider.GetFieldValue (
                  _order1.InternalDataContainer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber", ValueAccess.Original));

          _mockRepository.VerifyAll();
        }
      }
    }

    [Test]
    public void Rollback ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RollingBack (null);
        LastCall.Constraints (Mocks_List.IsIn (computer));

        _extension.RolledBack (null);
        LastCall.Constraints (Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll();

      ClientTransactionScope.CurrentTransaction.Rollback();

      _mockRepository.VerifyAll();
    }
  }
}
