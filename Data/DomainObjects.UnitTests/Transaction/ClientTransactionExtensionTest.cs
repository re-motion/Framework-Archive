using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class ClientTransactionExtensionTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    private MockRepository _mockRepository;
    private IClientTransactionExtension _extension;

    private Order _order;

    // construction and disposing

    public ClientTransactionExtensionTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _order = Order.GetObject (DomainObjectIDs.Order1);
      _mockRepository = new MockRepository ();
      _extension = _mockRepository.CreateMock<IClientTransactionExtension> ();

      ClientTransaction.Current.RegisterExtension ("Name", _extension);
    }

    [Test]
    public void ManageExtensions ()
    {
      Assert.AreSame (_extension, ClientTransaction.Current.GetExtension ("Name"));

      ClientTransaction.Current.UnregisterExtension ("Name");
      Assert.IsNull (ClientTransaction.Current.GetExtension ("Name"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "An extension with name 'Name' is already registered.\r\nParameter name: extensionName")]
    public void RegisterExtensionTwice ()
    {
      ClientTransaction.Current.RegisterExtension ("Name", _extension);
    }

    [Test]
    public void PropertyChange ()
    {
      _order = Order.GetObject (DomainObjectIDs.Order1);
      int oldOrderNumber = _order.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyChanging (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, oldOrderNumber + 1);
        _extension.PropertyChanged (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll ();
      _order.OrderNumber = oldOrderNumber + 1;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyChangeWithEvents ()
    {
      _order = Order.GetObject (DomainObjectIDs.Order1);
      int oldOrderNumber = _order.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      DomainObjectMockEventReceiver domainObjectMockEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (_order);
      DataContainerMockEventReceiver dataContainerMockEventReceiver = _mockRepository.CreateMock<DataContainerMockEventReceiver> (_order.DataContainer);
      PropertyValueCollectionMockEventReceiver propertyValueCollectionMockEventReceiver = _mockRepository.CreateMock<PropertyValueCollectionMockEventReceiver> (_order.DataContainer.PropertyValues);
      PropertyValueMockEventReceiver propertyValueMockEventReceiver = _mockRepository.CreateMock<PropertyValueMockEventReceiver> (_order.DataContainer.PropertyValues["OrderNumber"]);

      using (_mockRepository.Ordered ())
      {
        // "Changing" notifications

        _extension.PropertyChanging (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, oldOrderNumber + 1);
        
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

        _extension.PropertyChanged (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll ();
      _order.OrderNumber = oldOrderNumber + 1;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyChangeWith2Extensions ()
    {
      int oldOrderNumber = _order.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      IClientTransactionExtension extension2 = _mockRepository.CreateMock<IClientTransactionExtension> ();
      ClientTransaction.Current.RegisterExtension ("Name2", extension2);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyChanging (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, oldOrderNumber + 1);
        extension2.PropertyChanging (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, oldOrderNumber + 1);

        _extension.PropertyChanged (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, oldOrderNumber + 1);
        extension2.PropertyChanged (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll ();
      _order.OrderNumber = oldOrderNumber + 1;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyRead ()
    {
      int orderNumber = _order.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyReading (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], RetrievalType.CurrentValue);
        _extension.PropertyRead (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], orderNumber, RetrievalType.CurrentValue);
        _extension.PropertyReading (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], RetrievalType.OriginalValue);
        _extension.PropertyRead (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], orderNumber, RetrievalType.OriginalValue);
      }

      _mockRepository.ReplayAll ();
      orderNumber = _order.OrderNumber;
      orderNumber = (int) _order.DataContainer.PropertyValues["OrderNumber"].OriginalValue;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ChangeAndReadProperty ()
    {
      int oldOrderNumber = _order.OrderNumber;
      int newOrderNumber = oldOrderNumber + 1;

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyChanging (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, newOrderNumber);
        _extension.PropertyChanged (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, newOrderNumber);

        _extension.PropertyReading (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], RetrievalType.CurrentValue);
        _extension.PropertyRead (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], newOrderNumber, RetrievalType.CurrentValue);
        _extension.PropertyReading (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], RetrievalType.OriginalValue);
        _extension.PropertyRead (_order.DataContainer, _order.DataContainer.PropertyValues["OrderNumber"], oldOrderNumber, RetrievalType.OriginalValue);
      }

      _mockRepository.ReplayAll ();
      _order.OrderNumber = newOrderNumber;
      newOrderNumber = _order.OrderNumber;
      oldOrderNumber = (int) _order.DataContainer.PropertyValues["OrderNumber"].OriginalValue;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertySetToSameValue ()
    {
      int orderNumber = _order.OrderNumber;

      _mockRepository.BackToRecord (_extension);
      // Note: No method call on the extension is expected.
      _mockRepository.ReplayAll ();

      _order.OrderNumber = orderNumber;

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
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket3);

      Order oldRelatedOrder = orderTicket.Order;
      OrderTicket oldRelatedOrderTicket = order.OrderTicket;

      _mockRepository.BackToRecord (_extension);
      PropertyValueMockEventReceiver propertyValueMockEventReceiver = _mockRepository.CreateMock<PropertyValueMockEventReceiver> (orderTicket.DataContainer.PropertyValues["Order"]);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (order, "OrderTicket", oldRelatedOrderTicket, orderTicket);
        _extension.RelationChanging (oldRelatedOrderTicket, "Order", order, null);
        _extension.RelationChanging (orderTicket, "Order", oldRelatedOrder, order);
        _extension.RelationChanging (oldRelatedOrder, "OrderTicket", orderTicket, null);

        _extension.RelationChanged (order, "OrderTicket");
        _extension.RelationChanged (oldRelatedOrderTicket, "Order");
        _extension.RelationChanged (orderTicket, "Order");
        _extension.RelationChanged (oldRelatedOrder, "OrderTicket");

        //Note: no events are expected on propertyValueMockEventReceiver
      }

      _mockRepository.ReplayAll ();

      order.OrderTicket = orderTicket;

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
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Customer customer = order.Customer;

      int replaceIndex = customer.Orders.IndexOf (order);

      Order newOrder = Order.GetObject (DomainObjectIDs.Order2);
      Customer oldRelatedCustomerOfNewOrder = newOrder.Customer;

      _mockRepository.BackToRecord (_extension);
      PropertyValueMockEventReceiver orderPropertyValueMockEventReceiver = _mockRepository.CreateMock<PropertyValueMockEventReceiver> (order.DataContainer.PropertyValues["Customer"]);
      PropertyValueMockEventReceiver newOrderPropertyValueMockEventReceiver = _mockRepository.CreateMock<PropertyValueMockEventReceiver> (newOrder.DataContainer.PropertyValues["Customer"]);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (order, "Customer", customer, null);
        _extension.RelationChanging (newOrder, "Customer", oldRelatedCustomerOfNewOrder, customer);
        _extension.RelationChanging (customer, "Orders", order, newOrder);
        _extension.RelationChanging (oldRelatedCustomerOfNewOrder, "Orders", newOrder, null);

        _extension.RelationChanged (order, "Customer");
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
      PropertyValue customerPropertyValue = _order.DataContainer.PropertyValues["Customer"];
      ObjectID customerID = (ObjectID) _order.DataContainer.PropertyValues["Customer"].Value;
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyReading (_order.DataContainer, customerPropertyValue, RetrievalType.CurrentValue);
        _extension.PropertyRead (_order.DataContainer, customerPropertyValue, customerID, RetrievalType.CurrentValue);
      }

      _mockRepository.ReplayAll ();

      customerID = (ObjectID) _order.DataContainer.PropertyValues["Customer"].Value;

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
  }
}
