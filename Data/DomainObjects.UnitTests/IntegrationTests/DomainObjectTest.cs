using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.DomainObjects;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.UnitTests.MockConstraints;

namespace Rubicon.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class DomainObjectTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    DataContainer _orderDataContainer;
    PropertyValueCollection _orderPropertyValues;
    PropertyValue _orderDeliveryDateProperty;
    PropertyValue _orderCustomerProperty;

    DomainObjectEventReceiver _orderDomainObjectEventReceiver;
    PropertyValueContainerEventReceiver _orderDataContainerEventReceiver;
    PropertyValueContainerEventReceiver _orderPropertyValuesEventReceiver;
    PropertyValueEventReceiver _orderDeliveryDatePropertyEventReceiver;
    PropertyValueEventReceiver _orderCustomerPropertyEventReceiver;

    // construction and disposing

    public DomainObjectTest ()
    {
    }

    // methods and properties

    [Test]
    public void RelationEventTestWithMockObject ()
    {
      Customer newCustomer1 = new Customer ();
      newCustomer1.Name = "NewCustomer1";

      Customer newCustomer2 = new Customer ();
      newCustomer2.Name = "NewCustomer2";

      Official official2 = Official.GetObject (DomainObjectIDs.Official2);
      Ceo newCeo1 = new Ceo ();
      Ceo newCeo2 = new Ceo ();
      Order newOrder1 = new Order ();
      newOrder1.DeliveryDate = new DateTime (2006, 1, 1);

      Order newOrder2 = new Order ();
      newOrder2.DeliveryDate = new DateTime (2006, 2, 2);

      OrderItem newOrderItem1 = new OrderItem ();
      OrderItem newOrderItem2 = new OrderItem ();

      MockRepository mockRepository = new MockRepository ();

      DomainObjectCollection newCustomer1Orders = newCustomer1.Orders;
      DomainObjectCollection newCustomer2Orders = newCustomer2.Orders;
      DomainObjectCollection official2Orders = official2.Orders;
      DomainObjectCollection newOrder1OrderItems = newOrder1.OrderItems;
      DomainObjectCollection newOrder2OrderItems = newOrder2.OrderItems;

      DomainObjectMockEventReceiver newCustomer1EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newCustomer1);
      DomainObjectMockEventReceiver newCustomer2EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newCustomer2);
      DomainObjectMockEventReceiver official2EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (official2);
      DomainObjectMockEventReceiver newCeo1EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newCeo1);
      DomainObjectMockEventReceiver newCeo2EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newCeo2);
      DomainObjectMockEventReceiver newOrder1EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrder1);
      DomainObjectMockEventReceiver newOrder2EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrder2);
      DomainObjectMockEventReceiver newOrderItem1EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrderItem1);
      DomainObjectMockEventReceiver newOrderItem2EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrderItem2);

      DomainObjectCollectionMockEventReceiver newCustomer1OrdersEventReceiver = mockRepository.CreateMock<DomainObjectCollectionMockEventReceiver> (newCustomer1.Orders);
      DomainObjectCollectionMockEventReceiver newCustomer2OrdersEventReceiver = mockRepository.CreateMock<DomainObjectCollectionMockEventReceiver> (newCustomer2.Orders);
      DomainObjectCollectionMockEventReceiver official2OrdersEventReceiver = mockRepository.CreateMock<DomainObjectCollectionMockEventReceiver> (official2.Orders);
      DomainObjectCollectionMockEventReceiver newOrder1OrderItemsEventReceiver = mockRepository.CreateMock<DomainObjectCollectionMockEventReceiver> (newOrder1.OrderItems);
      DomainObjectCollectionMockEventReceiver newOrder2OrderItemsEventReceiver = mockRepository.CreateMock<DomainObjectCollectionMockEventReceiver> (newOrder2.OrderItems);

      IClientTransactionExtension extension = mockRepository.CreateMock<IClientTransactionExtension> ();

      using (mockRepository.Ordered ())
      {
        //1
        //newCeo1.Company = newCustomer1;
        extension.RelationChanging (newCeo1, "Company", null, newCustomer1);
        extension.RelationChanging (newCustomer1, "Ceo", null, newCeo1);

        newCeo1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCustomer1));

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Ceo") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCeo1));

        newCeo1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company"));

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Ceo"));

        extension.RelationChanged (newCeo1, "Company");
        extension.RelationChanged (newCustomer1, "Ceo");


        //2
        //newCeo2.Company = newCustomer1;
        extension.RelationChanging (newCeo2, "Company", null, newCustomer1);
        extension.RelationChanging (newCustomer1, "Ceo", newCeo1, newCeo2);
        extension.RelationChanging (newCeo1, "Company", newCustomer1, null);

        newCeo2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCeo2), Property.Value ("PropertyName", "Company") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCustomer1));

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Ceo") & Property.Value ("OldRelatedObject", newCeo1) & Property.Value ("NewRelatedObject", newCeo2));

        newCeo1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company") & Property.Value ("OldRelatedObject", newCustomer1) & Property.Value ("NewRelatedObject", null));

        newCeo2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCeo2), Property.Value ("PropertyName", "Company"));

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Ceo"));

        newCeo1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company"));

        extension.RelationChanged (newCeo2, "Company");
        extension.RelationChanged (newCustomer1, "Ceo");
        extension.RelationChanged (newCeo1, "Company");


        //3
        //newCeo1.Company = newCustomer2;
        extension.RelationChanging (newCeo1, "Company", null, newCustomer2);
        extension.RelationChanging (newCustomer2, "Ceo", null, newCeo1);

        newCeo1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCustomer2));

        newCustomer2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Ceo") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCeo1));

        newCeo1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company"));

        newCustomer2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Ceo"));

        extension.RelationChanged (newCeo1, "Company");
        extension.RelationChanged (newCustomer2, "Ceo");


        //4
        //newCeo1.Company = null;
        extension.RelationChanging (newCeo1, "Company", newCustomer2, null);
        extension.RelationChanging (newCustomer2, "Ceo", newCeo1, null);

        newCeo1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company") & Property.Value ("OldRelatedObject", newCustomer2) & Property.Value ("NewRelatedObject", null));

        newCustomer2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Ceo") & Property.Value ("OldRelatedObject", newCeo1) & Property.Value ("NewRelatedObject", null));

        newCeo1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company"));

        newCustomer2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Ceo"));

        extension.RelationChanged (newCeo1, "Company");
        extension.RelationChanged (newCustomer2, "Ceo");


        //5
        //newCustomer1.Orders.Add (newOrder1);
        extension.RelationReading (newCustomer1, "Orders", ValueAccess.Current);
        extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (Is.Same (newCustomer1), Is.Equal ("Orders"), Property.Value ("Count", 0), Is.Equal (ValueAccess.Current));

        extension.RelationChanging (newOrder1, "Customer", null, newCustomer1);
        extension.RelationChanging (newCustomer1, "Orders", null, newOrder1);

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "Customer") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCustomer1));

        newCustomer1OrdersEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder1));

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder1));

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "Customer"));

        newCustomer1OrdersEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder1));

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders"));

        extension.RelationChanged (newOrder1, "Customer");
        extension.RelationChanged (newCustomer1, "Orders");


        //6
        //newCustomer1.Orders.Add (newOrder2);
        extension.RelationReading (newCustomer1, "Orders", ValueAccess.Current);
        extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (Is.Same (newCustomer1), Is.Equal ("Orders"), Property.Value ("Count", 1) & List.IsIn (newOrder1), Is.Equal (ValueAccess.Current));

        extension.RelationChanging (newOrder2, "Customer", null, newCustomer1);
        extension.RelationChanging (newCustomer1, "Orders", null, newOrder2);

        newOrder2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCustomer1));

        newCustomer1OrdersEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder2));

        newOrder2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer"));

        newCustomer1OrdersEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders"));

        extension.RelationChanged (newOrder2, "Customer");
        extension.RelationChanged (newCustomer1, "Orders");


        //7
        //newCustomer1.Orders.Remove (newOrder2);
        extension.RelationReading (newCustomer1, "Orders", ValueAccess.Current);
        extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (Is.Same (newCustomer1), Is.Equal ("Orders"), Property.Value ("Count", 2) & List.IsIn (newOrder1) & List.IsIn (newOrder2), Is.Equal (ValueAccess.Current));

        extension.RelationChanging (newOrder2, "Customer", newCustomer1, null);
        extension.RelationChanging (newCustomer1, "Orders", newOrder2, null);

        newOrder2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer") & Property.Value ("OldRelatedObject", newCustomer1) & Property.Value ("NewRelatedObject", null));

        newCustomer1OrdersEventReceiver.Removing (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", newOrder2) & Property.Value ("NewRelatedObject", null));

        newOrder2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer"));

        newCustomer1OrdersEventReceiver.Removed (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders"));

        extension.RelationChanged (newOrder2, "Customer");
        extension.RelationChanged (newCustomer1, "Orders");


        //8
        //newOrderItem1.Order = newOrder1;
        extension.RelationChanging (newOrderItem1, "Order", null, newOrder1);
        extension.RelationChanging (newOrder1, "OrderItems", null, newOrderItem1);

        newOrderItem1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder1));

        newOrder1OrderItemsEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newOrder1OrderItems), Property.Value ("DomainObject", newOrderItem1));

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderItems") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrderItem1));

        newOrderItem1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order"));

        newOrder1OrderItemsEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newOrder1OrderItems), Property.Value ("DomainObject", newOrderItem1));

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderItems"));

        extension.RelationChanged (newOrderItem1, "Order");
        extension.RelationChanged (newOrder1, "OrderItems");


        //9
        //newOrderItem2.Order = newOrder1;
        extension.RelationChanging (newOrderItem2, "Order", null, newOrder1);
        extension.RelationChanging (newOrder1, "OrderItems", null, newOrderItem2);

        newOrderItem2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderItem2), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder1));

        newOrder1OrderItemsEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newOrder1OrderItems), Property.Value ("DomainObject", newOrderItem2));

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderItems") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrderItem2));

        newOrderItem2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderItem2), Property.Value ("PropertyName", "Order"));

        newOrder1OrderItemsEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newOrder1OrderItems), Property.Value ("DomainObject", newOrderItem2));

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderItems"));

        extension.RelationChanged (newOrderItem2, "Order");
        extension.RelationChanged (newOrder1, "OrderItems");


        //10
        //newOrderItem1.Order = null;
        extension.RelationChanging (newOrderItem1, "Order", newOrder1, null);
        extension.RelationChanging (newOrder1, "OrderItems", newOrderItem1, null);

        newOrderItem1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", newOrder1) & Property.Value ("NewRelatedObject", null));

        newOrder1OrderItemsEventReceiver.Removing (null, null);
        LastCall.Constraints (Is.Same (newOrder1OrderItems), Property.Value ("DomainObject", newOrderItem1));

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderItems") & Property.Value ("OldRelatedObject", newOrderItem1) & Property.Value ("NewRelatedObject", null));

        newOrderItem1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order"));

        newOrder1OrderItemsEventReceiver.Removed (null, null);
        LastCall.Constraints (Is.Same (newOrder1OrderItems), Property.Value ("DomainObject", newOrderItem1));

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderItems"));

        extension.RelationChanged (newOrderItem1, "Order");
        extension.RelationChanged (newOrder1, "OrderItems");


        //11
        //newOrderItem1.Order = newOrder2;
        extension.RelationChanging (newOrderItem1, "Order", null, newOrder2);
        extension.RelationChanging (newOrder2, "OrderItems", null, newOrderItem1);

        newOrderItem1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder2));

        newOrder2OrderItemsEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newOrder2OrderItems), Property.Value ("DomainObject", newOrderItem1));

        newOrder2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "OrderItems") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrderItem1));

        newOrderItem1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order"));

        newOrder2OrderItemsEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newOrder2OrderItems), Property.Value ("DomainObject", newOrderItem1));

        newOrder2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "OrderItems"));

        extension.RelationChanged (newOrderItem1, "Order");
        extension.RelationChanged (newOrder2, "OrderItems");


        //12
        //newOrder1.Official = official2;
        extension.RelationChanging (newOrder1, "Official", null, official2);
        extension.RelationChanging (official2, "Orders", null, newOrder1);

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "Official") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", official2));

        official2OrdersEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (official2Orders), Property.Value ("DomainObject", newOrder1));

        official2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (official2), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder1));

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "Official"));

        official2OrdersEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (official2Orders), Property.Value ("DomainObject", newOrder1));

        official2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (official2), Property.Value ("PropertyName", "Orders"));

        extension.RelationChanged (newOrder1, "Official");
        extension.RelationChanged (official2, "Orders");


        //13
        //OrderTicket newOrderTicket1 = new OrderTicket (newOrder1);

        extension.NewObjectCreating (typeof (OrderTicket));

        extension.RelationChanging (null, null, null, null);
        LastCall.Constraints (Is.TypeOf<OrderTicket> (), Is.Equal ("Order"), Is.Null (), Is.Same (newOrder1));
        extension.RelationChanging (null, null, null, null);
        LastCall.Constraints (Is.Same (newOrder1), Is.Equal ("OrderTicket"), Is.Null (), Is.TypeOf<OrderTicket> ());

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderTicket") & Property.Value ("OldRelatedObject", null) & Property.ValueConstraint ("NewRelatedObject", Is.TypeOf<OrderTicket> ()));

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderTicket"));

        extension.RelationChanged (null, null);
        LastCall.Constraints (Is.TypeOf<OrderTicket> (), Is.Equal ("Order"));
        extension.RelationChanged (newOrder1, "OrderTicket");
      }

      ClientTransaction.Current.Extensions.Add ("Extension", extension);
      mockRepository.ReplayAll ();

      //1
      newCeo1.Company = newCustomer1;
      //2
      newCeo2.Company = newCustomer1;
      //3
      newCeo1.Company = newCustomer2;
      //4
      newCeo1.Company = null;
      //5
      newCustomer1.Orders.Add (newOrder1);
      //6
      newCustomer1.Orders.Add (newOrder2);
      //7
      newCustomer1.Orders.Remove (newOrder2);
      //8
      newOrderItem1.Order = newOrder1;
      //9
      newOrderItem2.Order = newOrder1;
      //10
      newOrderItem1.Order = null;
      //11
      newOrderItem1.Order = newOrder2;
      //12
      newOrder1.Official = official2;
      //13
      OrderTicket newOrderTicket1 = new OrderTicket (newOrder1);

      mockRepository.VerifyAll ();

      BackToRecord (mockRepository, extension, newCustomer1EventReceiver, newCustomer2EventReceiver, official2EventReceiver, newCeo1EventReceiver,
          newCeo2EventReceiver, newOrder1EventReceiver, newOrder2EventReceiver, newOrderItem1EventReceiver, newOrderItem2EventReceiver,
          newCustomer1OrdersEventReceiver, newCustomer2OrdersEventReceiver, official2OrdersEventReceiver,
          newOrder1OrderItemsEventReceiver, newOrder2OrderItemsEventReceiver);

      DomainObjectMockEventReceiver newOrderTicket1EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrderTicket1);

      using (mockRepository.Ordered ())
      {
        //14
        //newOrderTicket1.Order = newOrder2;
        extension.RelationChanging (newOrderTicket1, "Order", newOrder1, newOrder2);
        extension.RelationChanging (newOrder1, "OrderTicket", newOrderTicket1, null);
        extension.RelationChanging (newOrder2, "OrderTicket", null, newOrderTicket1);

        newOrderTicket1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderTicket1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", newOrder1) & Property.Value ("NewRelatedObject", newOrder2));

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderTicket") & Property.Value ("OldRelatedObject", newOrderTicket1) & Property.Value ("NewRelatedObject", null));

        newOrder2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "OrderTicket") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrderTicket1));

        newOrderTicket1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderTicket1), Property.Value ("PropertyName", "Order"));

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderTicket"));

        newOrder2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "OrderTicket"));

        extension.RelationChanged (newOrderTicket1, "Order");
        extension.RelationChanged (newOrder1, "OrderTicket");
        extension.RelationChanged (newOrder2, "OrderTicket");


        //15a
        //newOrder2.Customer = newCustomer1;
        extension.RelationChanging (newOrder2, "Customer", null, newCustomer1);
        extension.RelationChanging (newCustomer1, "Orders", null, newOrder2);

        newOrder2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCustomer1));

        newCustomer1OrdersEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder2));

        newOrder2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer"));

        newCustomer1OrdersEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders"));

        extension.RelationChanged (newOrder2, "Customer");
        extension.RelationChanged (newCustomer1, "Orders");


        //15b
        //newOrder2.Customer = newCustomer2;
        extension.RelationChanging (newOrder2, "Customer", newCustomer1, newCustomer2);
        extension.RelationChanging (newCustomer2, "Orders", null, newOrder2);
        extension.RelationChanging (newCustomer1, "Orders", newOrder2, null);

        newOrder2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer") & Property.Value ("OldRelatedObject", newCustomer1) & Property.Value ("NewRelatedObject", newCustomer2));

        newCustomer2OrdersEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newCustomer2Orders), Property.Value ("DomainObject", newOrder2));

        newCustomer2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder2));

        newCustomer1OrdersEventReceiver.Removing (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", newOrder2) & Property.Value ("NewRelatedObject", null));

        newOrder2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer"));

        newCustomer2OrdersEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newCustomer2Orders), Property.Value ("DomainObject", newOrder2));

        newCustomer2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Orders"));

        newCustomer1OrdersEventReceiver.Removed (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders"));

        extension.RelationChanged (newOrder2, "Customer");
        extension.RelationChanged (newCustomer2, "Orders");
        extension.RelationChanged (newCustomer1, "Orders");


        //16
        //newOrder2.Delete ();
        extension.ObjectDeleting (newOrder2);
        extension.RelationChanging (newCustomer2, "Orders", newOrder2, null);
        extension.RelationChanging (newOrderTicket1, "Order", newOrder2, null);
        extension.RelationChanging (newOrderItem1, "Order", newOrder2, null);

        newOrder2EventReceiver.Deleting (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Is.NotNull ());

        newCustomer2OrdersEventReceiver.Removing (null, null);
        LastCall.Constraints (Is.Same (newCustomer2Orders), Property.Value ("DomainObject", newOrder2));

        newCustomer2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", newOrder2) & Property.Value ("NewRelatedObject", null));

        newOrderTicket1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderTicket1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", newOrder2) & Property.Value ("NewRelatedObject", null));

        newOrderItem1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", newOrder2) & Property.Value ("NewRelatedObject", null));

        newCustomer2OrdersEventReceiver.Removed (null, null);
        LastCall.Constraints (Is.Same (newCustomer2Orders), Property.Value ("DomainObject", newOrder2));

        newCustomer2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Orders"));

        newOrderTicket1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderTicket1), Property.Value ("PropertyName", "Order"));

        newOrderItem1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order"));

        newOrder2EventReceiver.Deleted (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Is.NotNull ());

        extension.RelationChanged (newCustomer2, "Orders");
        extension.RelationChanged (newOrderTicket1, "Order");
        extension.RelationChanged (newOrderItem1, "Order");
        extension.ObjectDeleted (newOrder2);

        //17
        //newOrderTicket1.Order = newOrder1;
        extension.RelationChanging (newOrderTicket1, "Order", null, newOrder1);
        extension.RelationChanging (newOrder1, "OrderTicket", null, newOrderTicket1);

        newOrderTicket1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderTicket1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder1));

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderTicket") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrderTicket1));

        newOrderTicket1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderTicket1), Property.Value ("PropertyName", "Order"));

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderTicket"));

        extension.RelationChanged (newOrderTicket1, "Order");
        extension.RelationChanged (newOrder1, "OrderTicket");


        //cleanup for commit
        //18
        //newCustomer2.Delete ();
        extension.ObjectDeleting (newCustomer2);

        newCustomer2EventReceiver.Deleting (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Is.NotNull ());
        newCustomer2EventReceiver.Deleted (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Is.NotNull ());

        extension.ObjectDeleted (newCustomer2);


        //19
        //newCeo1.Delete ();
        extension.ObjectDeleting (newCeo1);

        newCeo1EventReceiver.Deleting (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Is.NotNull ());
        newCeo1EventReceiver.Deleted (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Is.NotNull ());

        extension.ObjectDeleted (newCeo1);

        //20
        //newOrderItem1.Delete ();
        extension.ObjectDeleting (newOrderItem1);

        newOrderItem1EventReceiver.Deleting (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Is.NotNull ());
        newOrderItem1EventReceiver.Deleted (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Is.NotNull ());

        extension.ObjectDeleted (newOrderItem1);


        //21
        //ClientTransaction.Current.Commit ();
        using (mockRepository.Unordered ())
        {
          newCustomer1EventReceiver.Committing (null, null);
          LastCall.Constraints (Is.Same (newCustomer1), Is.NotNull ());

          official2EventReceiver.Committing (null, null);
          LastCall.Constraints (Is.Same (official2), Is.NotNull ());

          newCeo2EventReceiver.Committing (null, null);
          LastCall.Constraints (Is.Same (newCeo2), Is.NotNull ());

          newOrder1EventReceiver.Committing (null, null);
          LastCall.Constraints (Is.Same (newOrder1), Is.NotNull ());

          newOrderItem2EventReceiver.Committing (null, null);
          LastCall.Constraints (Is.Same (newOrderItem2), Is.NotNull ());

          newOrderTicket1EventReceiver.Committing (null, null);
          LastCall.Constraints (Is.Same (newOrderTicket1), Is.NotNull ());
        }
        extension.Committing (null);
        LastCall.Constraints (new ContainsConstraint (new DomainObject[] { newCustomer1, official2, newCeo2, newOrder1, newOrderItem2, newOrderTicket1 }));

        using (mockRepository.Unordered ())
        {
          newCustomer1EventReceiver.Committed (null, null);
          LastCall.Constraints (Is.Same (newCustomer1), Is.Anything ());

          official2EventReceiver.Committed (null, null);
          LastCall.Constraints (Is.Same (official2), Is.NotNull ());

          newCeo2EventReceiver.Committed (null, null);
          LastCall.Constraints (Is.Same (newCeo2), Is.NotNull ());

          newOrder1EventReceiver.Committed (null, null);
          LastCall.Constraints (Is.Same (newOrder1), Is.NotNull ());

          newOrderItem2EventReceiver.Committed (null, null);
          LastCall.Constraints (Is.Same (newOrderItem2), Is.NotNull ());

          newOrderTicket1EventReceiver.Committed (null, null);
          LastCall.Constraints (Is.Same (newOrderTicket1), Is.NotNull ());
        }
        extension.Committed (null);
        LastCall.Constraints (Property.Value ("Count", 6) & new ContainsConstraint (new DomainObject[] { newCustomer1, official2, newCeo2, newOrder1, newOrderItem2, newOrderTicket1 }));
      }

      mockRepository.ReplayAll ();

      //14
      newOrderTicket1.Order = newOrder2;
      //15a
      newOrder2.Customer = newCustomer1;
      //15b
      newOrder2.Customer = newCustomer2;
      //16
      newOrder2.Delete ();
      //17
      newOrderTicket1.Order = newOrder1;
      //cleanup for commit
      //18
      newCustomer2.Delete ();
      //19
      newCeo1.Delete ();
      //20
      newOrderItem1.Delete ();

      //21
      ClientTransaction.Current.Commit ();

      mockRepository.VerifyAll ();
    }

    [Test]
    public void SetValuesAndAccessOriginalValuesTest ()
    {
      OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

      DataContainer dataContainer = orderItem.DataContainer;

      dataContainer.SetValue ("Product", "newProduct");

      Assert.IsFalse (dataContainer.PropertyValues["Product"].OriginalValue.ToString () == "newProduct");
      Assert.AreEqual ("newProduct", orderItem.Product);

      ClientTransactionMock.Commit ();
      orderItem.Product = "newProduct2";

      Assert.IsTrue (dataContainer.PropertyValues["Product"].OriginalValue.ToString () == "newProduct");
      Assert.AreEqual ("newProduct2", orderItem.Product);
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException))]
    public void NewCustomerAndCEOTest ()
    {
      IndustrialSector industrialSector = new IndustrialSector ();
      Customer customer = new Customer ();
      customer.Ceo = new Ceo ();

      industrialSector.Companies.Add (customer);

      Order order1 = new Order ();
      new OrderTicket (order1);

      //getting an SQL Exception without this line
      order1.DeliveryDate = DateTime.Now;

      OrderItem orderItem = new OrderItem ();
      order1.OrderItems.Add (orderItem);
      order1.Official = Official.GetObject (DomainObjectIDs.Official2);
      customer.Orders.Add (order1);

      try
      {
        ClientTransactionMock.Commit ();
      }
      catch (MandatoryRelationNotSetException)
      {
        Assert.Fail ("MandatoryRelationNotSetException was thrown when none was expected.");
      }

      customer.Delete ();
      ClientTransaction.Current.Commit ();
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void AddInvalidPropertyValueTest ()
    {
      Employee employee = new Employee ();

      PropertyDefinition propertyDefinition = new PropertyDefinition ("testproperty", "testproperty", "string", true, true, 10);
      PropertyValueCollection propertyValues = employee.DataContainer.PropertyValues;

      Assert.IsFalse (propertyValues.Contains ("testproperty"));

      propertyValues.Add (new PropertyValue (propertyDefinition));

      Assert.IsTrue (propertyValues.Contains ("testproperty"));
      Assert.IsNotNull (propertyValues["testproperty"]);

      ClientTransactionMock.Commit ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void AddPropertyValueWithExistingNameTest ()
    {
      Employee employee = new Employee ();

      PropertyDefinition propertyDefinition = new PropertyDefinition ("Name", "Name", "string", true, true, 10);
      PropertyValueCollection propertyValues = employee.DataContainer.PropertyValues;

      Assert.IsTrue (propertyValues.Contains ("Name"));

      propertyValues.Add (new PropertyValue (propertyDefinition));
    }

    [Test]
    public void PropertyEventsOfNewObjectPropertyChangeTest ()
    {
      Order newOrder = new Order ();

      InitializeEventReceivers (newOrder);
      CheckNoEvents (_orderDeliveryDatePropertyEventReceiver);

      newOrder.DeliveryDate = DateTime.Now;

      CheckEvents (_orderDeliveryDatePropertyEventReceiver, _orderDeliveryDateProperty);
    }

    [Test]
    public void PropertyEventsOfNewObjectRelationChangeTest ()
    {
      Order newOrder = new Order ();

      InitializeEventReceivers (newOrder);
      CheckNoEvents (_orderCustomerPropertyEventReceiver);

      newOrder.Customer = null;

      CheckNoEvents (_orderCustomerPropertyEventReceiver);
    }

    [Test]
    public void PropertyEventsOfExistingObjectPropertyChangeTest ()
    {
      Order order2 = Order.GetObject (DomainObjectIDs.Order2);

      InitializeEventReceivers (order2);
      CheckNoEvents (_orderDeliveryDatePropertyEventReceiver);

      order2.DeliveryDate = DateTime.Now;

      CheckEvents (_orderDeliveryDatePropertyEventReceiver, _orderDeliveryDateProperty);
    }

    [Test]
    public void PropertyEventsOfExistingObjectRelationChangeTest ()
    {
      Order order2 = Order.GetObject (DomainObjectIDs.Order2);

      InitializeEventReceivers (order2);
      CheckNoEvents (_orderDeliveryDatePropertyEventReceiver);

      order2.Customer = null;

      CheckNoEvents (_orderDeliveryDatePropertyEventReceiver);
    }

    [Test]
    public void SaveObjectWithNonMandatoryOneToManyRelation ()
    {
      Customer newCustomer = new Customer ();
      newCustomer.Ceo = new Ceo ();

      Customer existingCustomer = Customer.GetObject (DomainObjectIDs.Customer3);
      Assert.AreEqual (1, existingCustomer.Orders.Count);
      Assert.IsNotNull (existingCustomer.Orders[0].OrderTicket);
      Assert.AreEqual (1, existingCustomer.Orders[0].OrderItems.Count);

      existingCustomer.Orders[0].OrderTicket.Delete ();
      existingCustomer.Orders[0].OrderItems[0].Delete ();
      existingCustomer.Orders[0].Delete ();

      ClientTransaction.Current.Commit ();
      ClientTransaction.SetCurrent (null);

      newCustomer = Customer.GetObject (newCustomer.ID);
      existingCustomer = Customer.GetObject (DomainObjectIDs.Customer3);

      Assert.AreEqual (0, newCustomer.Orders.Count);
      Assert.AreEqual (0, existingCustomer.Orders.Count);
    }

    private void InitializeEventReceivers (Order order)
    {
      _orderDataContainer = order.DataContainer;
      _orderPropertyValues = _orderDataContainer.PropertyValues;
      _orderDeliveryDateProperty = _orderPropertyValues["DeliveryDate"];
      _orderCustomerProperty = _orderPropertyValues["Customer"];

      _orderDomainObjectEventReceiver = new DomainObjectEventReceiver (order);
      _orderDataContainerEventReceiver = new PropertyValueContainerEventReceiver (_orderDataContainer, false);
      _orderPropertyValuesEventReceiver = new PropertyValueContainerEventReceiver (_orderPropertyValues, false);

      _orderDeliveryDatePropertyEventReceiver = new PropertyValueEventReceiver (_orderDeliveryDateProperty);
      _orderCustomerPropertyEventReceiver = new PropertyValueEventReceiver (_orderCustomerProperty);
    }

    private void CheckNoEvents (PropertyValueEventReceiver propertyValueEventReceiver)
    {
      Assert.IsFalse (propertyValueEventReceiver.HasChangingEventBeenCalled);
      Assert.IsFalse (propertyValueEventReceiver.HasChangedEventBeenCalled);
      Assert.IsNull (_orderPropertyValuesEventReceiver.ChangingPropertyValue);
      Assert.IsNull (_orderPropertyValuesEventReceiver.ChangedPropertyValue);
      Assert.IsNull (_orderDataContainerEventReceiver.ChangingPropertyValue);
      Assert.IsNull (_orderDataContainerEventReceiver.ChangedPropertyValue);
      Assert.IsFalse (_orderDomainObjectEventReceiver.HasChangingEventBeenCalled);
      Assert.IsFalse (_orderDomainObjectEventReceiver.HasChangedEventBeenCalled);
      Assert.IsNull (_orderDomainObjectEventReceiver.ChangingPropertyValue);
      Assert.IsNull (_orderDomainObjectEventReceiver.ChangedPropertyValue);
    }

    private void CheckEvents (PropertyValueEventReceiver propertyValueEventReceiver, PropertyValue propertyValue)
    {
      Assert.IsTrue (propertyValueEventReceiver.HasChangingEventBeenCalled);
      Assert.IsTrue (propertyValueEventReceiver.HasChangedEventBeenCalled);
      Assert.AreSame (propertyValue, _orderPropertyValuesEventReceiver.ChangingPropertyValue);
      Assert.AreSame (propertyValue, _orderPropertyValuesEventReceiver.ChangedPropertyValue);
      Assert.AreSame (propertyValue, _orderDataContainerEventReceiver.ChangingPropertyValue);
      Assert.AreSame (propertyValue, _orderDataContainerEventReceiver.ChangedPropertyValue);
      Assert.IsTrue (_orderDomainObjectEventReceiver.HasChangingEventBeenCalled);
      Assert.IsTrue (_orderDomainObjectEventReceiver.HasChangedEventBeenCalled);
      Assert.AreSame (propertyValue, _orderDomainObjectEventReceiver.ChangingPropertyValue);
      Assert.AreSame (propertyValue, _orderDomainObjectEventReceiver.ChangedPropertyValue);
    }

    private void BackToRecord (MockRepository mockRepository, params object[] objects)
    {
      foreach (object obj in objects)
        mockRepository.BackToRecord (obj);
    }
  }
}
