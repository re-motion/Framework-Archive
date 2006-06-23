using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  // TODO ES: Move relation change tests to fixture ClientTransactionExtensionRelationChangesTest
  [TestFixture]
  public class ClientTransactionExtensionTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    private MockRepository _mockRepository;
    private IClientTransactionExtension _extension;

    private Order _order1;

    // construction and disposing

    public ClientTransactionExtensionTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
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
    public void PropertyChange ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyValueChanging (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, oldOrderNumber + 1);
        _extension.PropertyValueChanged (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, oldOrderNumber + 1);
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
      PropertyValueMockEventReceiver propertyValueMockEventReceiver = _mockRepository.CreateMock<PropertyValueMockEventReceiver> (_order1.DataContainer.PropertyValues["OrderNumber"]);

      using (_mockRepository.Ordered ())
      {
        // "Changing" notifications

        _extension.PropertyValueChanging (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, oldOrderNumber + 1);

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

        _extension.PropertyValueChanged (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll ();
      _order1.OrderNumber = oldOrderNumber + 1;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyRead ()
    {
      int orderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyValueReading (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], ValueAccess.Current);
        _extension.PropertyValueRead (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], orderNumber, ValueAccess.Current);
        _extension.PropertyValueReading (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], ValueAccess.Original);
        _extension.PropertyValueRead (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], orderNumber, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();
      orderNumber = _order1.OrderNumber;
      orderNumber = (int) _order1.DataContainer.PropertyValues["OrderNumber"].OriginalValue;

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
        _extension.PropertyValueChanging (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, newOrderNumber);
        _extension.PropertyValueChanged (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, newOrderNumber);

        _extension.PropertyValueReading (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], ValueAccess.Current);
        _extension.PropertyValueRead (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], newOrderNumber, ValueAccess.Current);
        _extension.PropertyValueReading (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], ValueAccess.Original);
        _extension.PropertyValueRead (_order1.DataContainer, _order1.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();
      _order1.OrderNumber = newOrderNumber;
      newOrderNumber = _order1.OrderNumber;
      oldOrderNumber = (int) _order1.DataContainer.PropertyValues["OrderNumber"].OriginalValue;

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
    public void PropertyReadWithoutDataContainer ()
    {
      ClassDefinition orderClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof(Order));
      PropertyDefinition orderNumberDefinition = orderClass.MyPropertyDefinitions["OrderNumber"];

      PropertyValue propertyValue = new PropertyValue (orderNumberDefinition);
      PropertyValueCollection propertyValueCollection = new PropertyValueCollection ();
      propertyValueCollection.Add (propertyValue);

      int orderNumber = (int) propertyValue.Value;

      // Expectation: no exception
    }

    [Test]
    public void ChangeOneToOneRelationWithoutOldRelatedObject ()
    {
      Order order = new Order ();
      OrderTicket orderTicket = new OrderTicket ();
      _mockRepository.BackToRecord (_extension);
      PropertyValueMockEventReceiver propertyValueMockEventReceiver = _mockRepository.CreateMock<PropertyValueMockEventReceiver> (orderTicket.DataContainer.PropertyValues["Order"]);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (order, "OrderTicket", null, orderTicket);
        _extension.RelationChanging (orderTicket, "Order", null, order);
        _extension.RelationChanged (order, "OrderTicket");
        _extension.RelationChanged (orderTicket, "Order");

        //Note: no events are expected on propertyValueMockEventReceiver
      }

      _mockRepository.ReplayAll ();
      
      order.OrderTicket = orderTicket;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ChangeOneToOneRelationWithOldRelatedObject ()
    {
      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket3);

      Order oldRelatedOrder = orderTicket.Order;
      OrderTicket oldRelatedOrderTicket = _order1.OrderTicket;

      _mockRepository.BackToRecord (_extension);
      PropertyValueMockEventReceiver propertyValueMockEventReceiver = _mockRepository.CreateMock<PropertyValueMockEventReceiver> (orderTicket.DataContainer.PropertyValues["Order"]);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (_order1, "OrderTicket", oldRelatedOrderTicket, orderTicket);
        _extension.RelationChanging (oldRelatedOrderTicket, "Order", _order1, null);
        _extension.RelationChanging (orderTicket, "Order", oldRelatedOrder, _order1);
        _extension.RelationChanging (oldRelatedOrder, "OrderTicket", orderTicket, null);

        _extension.RelationChanged (_order1, "OrderTicket");
        _extension.RelationChanged (oldRelatedOrderTicket, "Order");
        _extension.RelationChanged (orderTicket, "Order");
        _extension.RelationChanged (oldRelatedOrder, "OrderTicket");

        //Note: no events are expected on propertyValueMockEventReceiver
      }

      _mockRepository.ReplayAll ();

      _order1.OrderTicket = orderTicket;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ChangeUnidirectionalRelation ()
    {
      Location location = new Location ();
      Client client = new Client ();

      _mockRepository.BackToRecord (_extension);
      PropertyValueMockEventReceiver propertyValueMockEventReceiver = _mockRepository.CreateMock<PropertyValueMockEventReceiver> (location.DataContainer.PropertyValues["Client"]);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (location, "Client", null, client);
        _extension.RelationChanged (location, "Client");

        //Note: no events are expected on propertyValueMockEventReceiver
      }

      _mockRepository.ReplayAll ();

      location.Client = client;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ReplaceInOneToManyRelation ()
    {
      Customer customer = _order1.Customer;
      OrderCollection ordersOfCustomer = customer.Orders;

      int replaceIndex = ordersOfCustomer.IndexOf (_order1);

      Order newOrder = Order.GetObject (DomainObjectIDs.Order2);
      Customer oldRelatedCustomerOfNewOrder = newOrder.Customer;

      //Note: this relation must be preloaded in order to prevent ObjectLoaded calls on the extension
      OrderCollection ordersOfOldRelatedCustomerOfNewOrder = oldRelatedCustomerOfNewOrder.Orders;

      _mockRepository.BackToRecord (_extension);
      PropertyValueMockEventReceiver orderPropertyValueMockEventReceiver = _mockRepository.CreateMock<PropertyValueMockEventReceiver> (_order1.DataContainer.PropertyValues["Customer"]);
      PropertyValueMockEventReceiver newOrderPropertyValueMockEventReceiver = _mockRepository.CreateMock<PropertyValueMockEventReceiver> (newOrder.DataContainer.PropertyValues["Customer"]);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (customer, "Orders", ValueAccess.Current);
        _extension.RelationRead (customer, "Orders", ordersOfCustomer, ValueAccess.Current);

        _extension.RelationChanging (_order1, "Customer", customer, null);
        _extension.RelationChanging (newOrder, "Customer", oldRelatedCustomerOfNewOrder, customer);
        _extension.RelationChanging (customer, "Orders", _order1, newOrder);
        _extension.RelationChanging (oldRelatedCustomerOfNewOrder, "Orders", newOrder, null);

        _extension.RelationChanged (_order1, "Customer");
        _extension.RelationChanged (newOrder, "Customer");
        _extension.RelationChanged (customer, "Orders");
        _extension.RelationChanged (oldRelatedCustomerOfNewOrder, "Orders");
      }

      _mockRepository.ReplayAll ();

      customer.Orders[replaceIndex] = newOrder;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ReadObjectIDProperty ()
    {
      PropertyValue customerPropertyValue = _order1.DataContainer.PropertyValues["Customer"];
      ObjectID customerID = (ObjectID) _order1.DataContainer.PropertyValues["Customer"].Value;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyValueReading (_order1.DataContainer, customerPropertyValue, ValueAccess.Current);
        _extension.PropertyValueRead (_order1.DataContainer, customerPropertyValue, customerID, ValueAccess.Current);
      }

      _mockRepository.ReplayAll ();

      customerID = (ObjectID) _order1.DataContainer.PropertyValues["Customer"].Value;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NewObjectCreation ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.NewObjectCreating (typeof (Order));
        _extension.NewObjectCreated (null);
        LastCall.Constraints (Is.TypeOf<Order> ());
      }

      _mockRepository.ReplayAll ();

      new Order ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectDelete ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.ObjectDeleting (computer);
        _extension.ObjectDeleted (computer);
      }

      _mockRepository.ReplayAll ();

      computer.Delete ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CommitWithChangedPropertyValue ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.Committing (null);
        LastCall.Constraints (Property.Value ("Count", 1) & List.IsIn (computer));
        _extension.Committed (null);
        LastCall.Constraints (Property.Value ("Count", 1) & List.IsIn (computer));
      }

      _mockRepository.ReplayAll ();

      ClientTransaction.Current.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CommitWithChangedRelationValue ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee1);
      computer.Employee = employee;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.Committing (null);
        LastCall.Constraints (Property.Value ("Count", 2) & List.IsIn (computer) & List.IsIn (employee));
        _extension.Committed (null);
        LastCall.Constraints (Property.Value ("Count", 2) & List.IsIn (computer) & List.IsIn (employee));
      }

      _mockRepository.ReplayAll ();

      ClientTransaction.Current.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CommitWithChangedRelationValueWithClassIDColumn ()
    {
      Customer oldCustomer = _order1.Customer;
      Customer newCustomer = Customer.GetObject (DomainObjectIDs.Customer2);
      _order1.Customer = newCustomer;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.Committing (null);
        LastCall.Constraints (Property.Value ("Count", 3) & List.IsIn (_order1) & List.IsIn (newCustomer) & List.IsIn (oldCustomer));
        _extension.Committed (null);
        LastCall.Constraints (Property.Value ("Count", 3) & List.IsIn (_order1) & List.IsIn (newCustomer) & List.IsIn (oldCustomer));
      }

      _mockRepository.ReplayAll ();

      ClientTransaction.Current.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Rollback ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RollingBack ();
        _extension.RolledBack ();
      }

      _mockRepository.ReplayAll ();

      ClientTransaction.Current.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoaded ()
    {
      _extension.ObjectsLoaded (null);
      LastCall.Constraints (Property.Value ("Count", 1));

      _mockRepository.ReplayAll ();

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoadedWithRelations ()
    {
      _extension.ObjectsLoaded (null);
      LastCall.Constraints (Property.Value ("Count", 1));

      _mockRepository.ReplayAll ();

      Order order = Order.GetObject (DomainObjectIDs.Order2);

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
      LastCall.Constraints (Property.Value ("Count", 2), Is.Same (query));

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
        LastCall.Constraints (Property.Value ("Count", 2));
        _extension.FilterQueryResult (null, null);
        LastCall.Constraints (Property.Value ("Count", 2), Is.Same (query));
      }

      _mockRepository.ReplayAll ();

      ClientTransaction.Current.QueryManager.GetCollection (query);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void StorageProviderGetFieldValue()
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

          Assert.AreEqual (originalOrderNumber + 1, storageProvider.GetFieldValue (_order1.DataContainer, "OrderNumber", ValueAccess.Current));
          Assert.AreEqual (originalOrderNumber, storageProvider.GetFieldValue (_order1.DataContainer, "OrderNumber", ValueAccess.Original));

          _mockRepository.VerifyAll ();
        }
      }
    }

    [Test]
    public void LoadRelatedDataContainerForEndPoint ()
    {
      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      _mockRepository.BackToRecord (_extension);

      //Note: no reading notification must be performed

      _mockRepository.ReplayAll ();

      using (PersistenceManager persistanceManager = new PersistenceManager ())
      {
        ClassDefinition orderTicketDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (OrderTicket)];
        IRelationEndPointDefinition orderEndPointDefinition = orderTicketDefinition.GetRelationEndPointDefinition ("Order");
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
        IRelationEndPointDefinition orderTicketEndPointDefinition = orderDefinition.GetRelationEndPointDefinition ("OrderTicket");
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
        _extension.RelationReading (_order1, "OrderTicket", ValueAccess.Current);
        _extension.RelationRead (_order1, "OrderTicket", orderTicket, ValueAccess.Current);
      }

      _mockRepository.ReplayAll ();

      orderTicket = _order1.OrderTicket;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      OrderTicket originalOrderTicket = (OrderTicket) _order1.GetOriginalRelatedObject ("OrderTicket");
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "OrderTicket", ValueAccess.Original);
        _extension.RelationRead (_order1, "OrderTicket", originalOrderTicket, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();

      originalOrderTicket = (OrderTicket) _order1.GetOriginalRelatedObject ("OrderTicket");

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetRelatedObjects ()
    {
      DomainObjectCollection orderItems = _order1.OrderItems;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "OrderItems", ValueAccess.Current);
        _extension.RelationRead (_order1, "OrderItems", orderItems, ValueAccess.Current);
      }

      _mockRepository.ReplayAll ();

      orderItems = _order1.OrderItems;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      DomainObjectCollection originalOrderItems = _order1.GetOriginalRelatedObjects ("OrderItems");
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "OrderItems", ValueAccess.Original);
        _extension.RelationRead (_order1, "OrderItems", originalOrderItems, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();

      originalOrderItems = _order1.GetOriginalRelatedObjects ("OrderItems");

      _mockRepository.VerifyAll ();
    }
  }
}
