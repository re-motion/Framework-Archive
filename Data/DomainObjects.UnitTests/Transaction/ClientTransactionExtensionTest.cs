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

      _order1 = DomainObject.GetObject<Order> (DomainObjectIDs.Order1);
      _mockRepository = new MockRepository ();
      _extension = _mockRepository.CreateMock<IClientTransactionExtension> ();

      ClientTransaction.Current.Extensions.Add ("Name", _extension);
    }

    [Test]
    public void Extensions ()
    {
      Assert.IsNotNull (ClientTransaction.Current.Extensions);
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
    public void ObjectsLoaded ()
    {
      _extension.ObjectsLoaded (null);
      LastCall.Constraints (Mocks_Property.Value ("Count", 1));

      _mockRepository.ReplayAll ();

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoadedWithRelations ()
    {
      _extension.ObjectsLoaded (null);
      LastCall.Constraints (Mocks_Property.Value ("Count", 1));

      _mockRepository.ReplayAll ();

      Order order = DomainObject.GetObject<Order> (DomainObjectIDs.Order2);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoadedWithEvents ()
    {
      ClientTransactionMockEventReceiver clientTransactionEventReceiver =
          _mockRepository.CreateMock<ClientTransactionMockEventReceiver> (ClientTransaction.Current);

      using (_mockRepository.Ordered ())
      {
        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1));

        clientTransactionEventReceiver.Loaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (ClientTransaction.Current), Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));
      }

      _mockRepository.ReplayAll ();

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectDelete ()
    {
      Computer computer = DomainObject.GetObject<Computer> (DomainObjectIDs.Computer4);
      DomainObjectMockEventReceiver computerEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (computer);
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.ObjectDeleting (computer);
        computerEventReceiver.Deleting (computer, EventArgs.Empty);
        _extension.ObjectDeleted (computer);
        computerEventReceiver.Deleted (computer, EventArgs.Empty);
      }

      _mockRepository.ReplayAll ();

      computer.Delete ();

      _mockRepository.VerifyAll ();
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

      using (_mockRepository.Ordered ())
      {
        _extension.ObjectDeleting (_order1);

        using (_mockRepository.Unordered ())
        {
          _extension.RelationChanging (customer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _order1, null);
          _extension.RelationChanging (orderTicket, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", _order1, null);
          _extension.RelationChanging (orderItem1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", _order1, null);
          _extension.RelationChanging (orderItem2, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", _order1, null);
          _extension.RelationChanging (official, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Official.Orders", _order1, null);
        }

        order1MockEventReceiver.Deleting (_order1, EventArgs.Empty);

        using (_mockRepository.Unordered ())
        {
          customerMockEventReceiver.RelationChanging (customer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", _order1, null);
          customerOrdersMockEventReceiver.Removing (customerOrders, _order1);
          orderTicketMockEventReceiver.RelationChanging (orderTicket, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", _order1, null);
          orderItem1MockEventReceiver.RelationChanging (orderItem1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", _order1, null);
          orderItem2MockEventReceiver.RelationChanging (orderItem2, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", _order1, null);
          officialMockEventReceiver.RelationChanging (official, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Official.Orders", _order1, null);
          officialOrdersMockEventReceiver.Removing (officialOrders, _order1);
          LastCall.Constraints (Mocks_Is.Same (officialOrders), Mocks_Property.Value ("DomainObject", _order1));
        }

        using (_mockRepository.Unordered ())
        {
          _extension.RelationChanged (customer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
          _extension.RelationChanged (orderTicket, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
          _extension.RelationChanged (orderItem1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
          _extension.RelationChanged (orderItem2, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
          _extension.RelationChanged (official, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Official.Orders");
        }

        _extension.ObjectDeleted (_order1);

        using (_mockRepository.Unordered ())
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

      _mockRepository.ReplayAll ();
      _order1.Delete ();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectDeleteTwice ()
    {
      Computer computer = DomainObject.GetObject<Computer> (DomainObjectIDs.Computer4);
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.ObjectDeleting (computer);
        _extension.ObjectDeleted (computer);
      }

      _mockRepository.ReplayAll ();

      computer.Delete ();
      computer.Delete ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyRead ()
    {
      int orderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyValueReading (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], ValueAccess.Current);
        _extension.PropertyValueRead (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], orderNumber, ValueAccess.Current);
        _extension.PropertyValueReading (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], ValueAccess.Original);
        _extension.PropertyValueRead (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], orderNumber, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();
      orderNumber = _order1.OrderNumber;
      orderNumber = (int) _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].OriginalValue;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyReadWithoutDataContainer ()
    {
      ClassDefinition orderClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      PropertyDefinition orderNumberDefinition = orderClass.MyPropertyDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];

      PropertyValue propertyValue = new PropertyValue (orderNumberDefinition);
      PropertyValueCollection propertyValueCollection = new PropertyValueCollection ();
      propertyValueCollection.Add (propertyValue);

      int orderNumber = (int) propertyValue.Value;

      // Expectation: no exception
    }

    [Test]
    public void ReadObjectIDProperty ()
    {
      PropertyValue customerPropertyValue = _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"];
      ObjectID customerID = (ObjectID) _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].Value;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyValueReading (_order1.DataContainer, customerPropertyValue, ValueAccess.Current);
        _extension.PropertyValueRead (_order1.DataContainer, customerPropertyValue, customerID, ValueAccess.Current);
      }

      _mockRepository.ReplayAll ();

      customerID = (ObjectID) _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].Value;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertySetToSameValue ()
    {
      int orderNumber = _order1.OrderNumber;

      _mockRepository.BackToRecord (_extension);
      // Note: No method call on the extension is expected.
      _mockRepository.ReplayAll ();

      _order1.OrderNumber = orderNumber;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ChangeAndReadProperty ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      int newOrderNumber = oldOrderNumber + 1;

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyValueChanging (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], oldOrderNumber, newOrderNumber);
        _extension.PropertyValueChanged (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], oldOrderNumber, newOrderNumber);

        _extension.PropertyValueReading (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], ValueAccess.Current);
        _extension.PropertyValueRead (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], newOrderNumber, ValueAccess.Current);
        _extension.PropertyValueReading (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], ValueAccess.Original);
        _extension.PropertyValueRead (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], oldOrderNumber, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();
      _order1.OrderNumber = newOrderNumber;
      newOrderNumber = _order1.OrderNumber;
      oldOrderNumber = (int) _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].OriginalValue;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyChange ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyValueChanging (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], oldOrderNumber, oldOrderNumber + 1);
        _extension.PropertyValueChanged (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], oldOrderNumber, oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll ();
      _order1.OrderNumber = oldOrderNumber + 1;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyChangeWithEvents ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      DomainObjectMockEventReceiver domainObjectMockEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (_order1);
      DataContainerMockEventReceiver dataContainerMockEventReceiver = _mockRepository.CreateMock<DataContainerMockEventReceiver> (_order1.DataContainer);
      PropertyValueCollectionMockEventReceiver propertyValueCollectionMockEventReceiver = _mockRepository.CreateMock<PropertyValueCollectionMockEventReceiver> (_order1.DataContainer.PropertyValues);
      PropertyValueMockEventReceiver propertyValueMockEventReceiver = _mockRepository.CreateMock<PropertyValueMockEventReceiver> (_order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]);

      using (_mockRepository.Ordered ())
      {
        // "Changing" notifications

        _extension.PropertyValueChanging (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], oldOrderNumber, oldOrderNumber + 1);

        domainObjectMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();

        dataContainerMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();

        propertyValueCollectionMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();

        propertyValueMockEventReceiver.Changing (null, null);
        LastCall.IgnoreArguments ();


        // "Changed" notifications

        propertyValueMockEventReceiver.Changed (null, null);
        LastCall.IgnoreArguments ();

        propertyValueCollectionMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();

        dataContainerMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();

        domainObjectMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();

        _extension.PropertyValueChanged (_order1.DataContainer, _order1.DataContainer.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], oldOrderNumber, oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll ();
      _order1.OrderNumber = oldOrderNumber + 1;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void LoadRelatedDataContainerForEndPoint ()
    {
      OrderTicket orderTicket = DomainObject.GetObject<OrderTicket> (DomainObjectIDs.OrderTicket2);

      _mockRepository.BackToRecord (_extension);

      //Note: no reading notification must be performed

      _mockRepository.ReplayAll ();

      using (PersistenceManager persistanceManager = new PersistenceManager ())
      {
        ClassDefinition orderTicketDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (OrderTicket)];
        IRelationEndPointDefinition orderEndPointDefinition = orderTicketDefinition.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
        persistanceManager.LoadRelatedDataContainer (orderTicket.DataContainer, new RelationEndPointID (orderTicket.ID, orderEndPointDefinition));
      }

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void LoadRelatedDataContainerForVirtualEndPoint ()
    {
      //Note: no reading notification must be performed
      _mockRepository.ReplayAll ();

      using (PersistenceManager persistenceManager = new PersistenceManager ())
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Order)];
        IRelationEndPointDefinition orderTicketEndPointDefinition = orderDefinition.GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
        persistenceManager.LoadRelatedDataContainer (_order1.DataContainer, new RelationEndPointID (_order1.ID, orderTicketEndPointDefinition));
      }

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetRelatedObject ()
    {
      OrderTicket orderTicket = _order1.OrderTicket;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", ValueAccess.Current);
        _extension.RelationRead (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", orderTicket, ValueAccess.Current);
      }

      _mockRepository.ReplayAll ();

      orderTicket = _order1.OrderTicket;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      OrderTicket originalOrderTicket = (OrderTicket) _order1.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", ValueAccess.Original);
        _extension.RelationRead (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", originalOrderTicket, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();

      originalOrderTicket = (OrderTicket) _order1.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetRelatedObjects ()
    {
      DomainObjectCollection orderItems = _order1.OrderItems;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);

        LastCall.Constraints (
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("IsReadOnly", true) & Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (orderItems[0]) & Mocks_List.IsIn (orderItems[1]),
            Mocks_Is.Equal (ValueAccess.Current));
      }

      _mockRepository.ReplayAll ();

      orderItems = _order1.OrderItems;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      DomainObjectCollection originalOrderItems = _order1.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", ValueAccess.Original);
        _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Original);

        LastCall.Constraints (
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("IsReadOnly", true) & Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (originalOrderItems[0]) & Mocks_List.IsIn (originalOrderItems[1]),
            Mocks_Is.Equal (ValueAccess.Original));
      }

      _mockRepository.ReplayAll ();

      originalOrderItems = _order1.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", ValueAccess.Current);
        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1));
        _extension.RelationRead (null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (_order1), Mocks_Is.Equal ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"), Mocks_Is.NotNull (), Mocks_Is.Equal (ValueAccess.Current));
      }
      _mockRepository.ReplayAll ();

      OrderTicket orderTicket = _order1.OrderTicket;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", ValueAccess.Current);
        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2));
        _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (_order1), Mocks_Is.Equal ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"), Mocks_Is.NotNull (), Mocks_Is.Equal (ValueAccess.Current));
      }
      _mockRepository.ReplayAll ();

      DomainObjectCollection orderItems = _order1.OrderItems;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", ValueAccess.Original);
        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1));
        _extension.RelationRead (null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (_order1), Mocks_Is.Equal ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"), Mocks_Is.NotNull (), Mocks_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll ();

      OrderTicket orderTicket = (OrderTicket) _order1.GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", ValueAccess.Original);
        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2));
        _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (_order1), Mocks_Is.Equal ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"), Mocks_Is.NotNull (), Mocks_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll ();

      DomainObjectCollection orderItems = _order1.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FilterQueryResult ()
    {
      Query query = new Query ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1);

      ClientTransaction.Current.QueryManager.GetCollection (query);
      _mockRepository.BackToRecord (_extension);

      _extension.FilterQueryResult (null, null);
      LastCall.Constraints (Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query));

      _mockRepository.ReplayAll ();

      ClientTransaction.Current.QueryManager.GetCollection (query);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FilterQueryResultWithLoad ()
    {
      Query query = new Query ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);

      using (_mockRepository.Ordered ())
      {
        _extension.ObjectsLoaded (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2));
        _extension.FilterQueryResult (null, null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query));
      }

      _mockRepository.ReplayAll ();

      ClientTransaction.Current.QueryManager.GetCollection (query);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FilterQueryResultWithFiltering ()
    {
      Query query = new Query ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);
      IClientTransactionExtension filteringExtension = _mockRepository.CreateMock<ClientTransactionExtensionWithQueryFiltering> ();
      ClientTransaction.Current.Extensions.Add ("FilteringExtension", filteringExtension);
      IClientTransactionExtension lastExtension = _mockRepository.CreateMock<IClientTransactionExtension> ();
      ClientTransaction.Current.Extensions.Add ("LastExtension", lastExtension);


      using (_mockRepository.Ordered ())
      {
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

      _mockRepository.ReplayAll ();

      DomainObjectCollection queryResult = ClientTransaction.Current.QueryManager.GetCollection (query);
      Assert.AreEqual (1, queryResult.Count);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CommitWithChangedPropertyValue ()
    {
      Computer computer = DomainObject.GetObject<Computer> (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.Committing (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
        _extension.Committed (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll ();

      ClientTransaction.Current.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CommitWithChangedRelationValue ()
    {
      Computer computer = DomainObject.GetObject<Computer> (DomainObjectIDs.Computer4);
      Employee employee = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee1);
      computer.Employee = employee;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.Committing (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (computer) & Mocks_List.IsIn (employee));
        _extension.Committed (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (computer) & Mocks_List.IsIn (employee));
      }

      _mockRepository.ReplayAll ();

      ClientTransaction.Current.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CommitWithChangedRelationValueWithClassIDColumn ()
    {
      Customer oldCustomer = _order1.Customer;
      Customer newCustomer = DomainObject.GetObject<Customer> (DomainObjectIDs.Customer2);
      _order1.Customer = newCustomer;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.Committing (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 3) & Mocks_List.IsIn (_order1) & Mocks_List.IsIn (newCustomer) & Mocks_List.IsIn (oldCustomer));
        _extension.Committed (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 3) & Mocks_List.IsIn (_order1) & Mocks_List.IsIn (newCustomer) & Mocks_List.IsIn (oldCustomer));
      }

      _mockRepository.ReplayAll ();

      ClientTransaction.Current.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CommitWithEvents ()
    {
      SetDatabaseModifyable ();

      Computer computer = DomainObject.GetObject<Computer> (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";
      _mockRepository.BackToRecord (_extension);

      ClientTransactionMockEventReceiver clientTransactionMockEventReceiver =
          _mockRepository.CreateMock<ClientTransactionMockEventReceiver> (ClientTransaction.Current);

      DomainObjectMockEventReceiver computerEventReveiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (computer);

      using (_mockRepository.Ordered ())
      {
        computerEventReveiver.Committing (computer, EventArgs.Empty);

        _extension.Committing (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));

        clientTransactionMockEventReceiver.Committing (null, null);
        LastCall.Constraints (Mocks_Is.Same (ClientTransaction.Current), Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));

        computerEventReveiver.Committed (computer, EventArgs.Empty);

        clientTransactionMockEventReceiver.Committed (null, null);
        LastCall.Constraints (Mocks_Is.Same (ClientTransaction.Current), Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));

        _extension.Committed (null);
        LastCall.Constraints (Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll ();

      ClientTransaction.Current.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void StorageProviderGetFieldValue ()
    {
      int originalOrderNumber = _order1.OrderNumber;
      _order1.OrderNumber = originalOrderNumber + 1;

      _mockRepository.BackToRecord (_extension);

      using (StorageProviderManager storageProviderManager = new StorageProviderManager ())
      {
        using (UnitTestStorageProviderStub storageProvider =
            (UnitTestStorageProviderStub) storageProviderManager.GetMandatory (c_unitTestStorageProviderStubID))
        {
          _mockRepository.ReplayAll ();

          Assert.AreEqual (originalOrderNumber + 1, storageProvider.GetFieldValue (_order1.DataContainer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber", ValueAccess.Current));
          Assert.AreEqual (originalOrderNumber, storageProvider.GetFieldValue (_order1.DataContainer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber", ValueAccess.Original));

          _mockRepository.VerifyAll ();
        }
      }
    }

    [Test]
    public void Rollback ()
    {
      Computer computer = DomainObject.GetObject<Computer> (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RollingBack (null);
        LastCall.Constraints (Mocks_List.IsIn (computer));

        _extension.RolledBack (null);
        LastCall.Constraints (Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll ();

      ClientTransaction.Current.Rollback ();

      _mockRepository.VerifyAll ();
    }
  }
}
