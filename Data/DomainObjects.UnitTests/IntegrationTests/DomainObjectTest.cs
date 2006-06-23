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
    public void RelationEventTest ()
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

      DomainObject[] domainObjects = new DomainObject[] 
    { 
      newCustomer1, 
      newCustomer2, 
      official2, 
      newCeo1, 
      newCeo2, 
      newOrder1,  
      newOrder2, 
      newOrderItem1, 
      newOrderItem2 
    };

      DomainObjectCollection[] collections = new DomainObjectCollection[] 
    { 
      newCustomer1.Orders, 
      newCustomer2.Orders, 
      official2.Orders, 
      newOrder1.OrderItems,
      newOrder2.OrderItems 
    };

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjects, collections);

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

      ChangeState[] expectedChangeStates = new ChangeState[]
    { 
      new RelationChangeState (newCeo1, "Company", null, newCustomer1, "1: 1. Changing event of newCeo from null to newCustomer1"),
      new RelationChangeState (newCustomer1, "Ceo", null, newCeo1, "1: 2. Changing event of newCustomer1 from null to newCeo1"),
      new RelationChangeState (newCeo1, "Company", null, null, "1: 3. Changed event of newCeo from null to newCustomer1"),
      new RelationChangeState (newCustomer1, "Ceo", null, null, "1: 4. Changed event of newCustomer1 from null to newCeo1"),

      new RelationChangeState (newCeo2, "Company", null, newCustomer1, "2: 1. Changing event of newCeo2 from null to newCustomer1"),
      new RelationChangeState (newCustomer1, "Ceo", newCeo1, newCeo2, "2: 2. Changing event of newCustomer1 from newCeo1 to newCeo2"),
      new RelationChangeState (newCeo1, "Company", newCustomer1, null, "2: 3. Changing event of newCeo1 from newCustomer1 to null"),
      new RelationChangeState (newCeo2, "Company", null, null, "2: 4. Changed event of newCeo2 from null to newCustomer1"),
      new RelationChangeState (newCustomer1, "Ceo", null, null, "2: 5. Changed event of newCustomer1 from newCeo1 to newCeo2"),
      new RelationChangeState (newCeo1, "Company", null, null, "2: 6. Changed event of newCeo1 from newCustomer1 to null"),

      new RelationChangeState (newCeo1, "Company", null, newCustomer2, "3: 1. Changing event of newCeo from null to newCustomer1"),
      new RelationChangeState (newCustomer2, "Ceo", null, newCeo1, "3: 2. Changing event of newCustomer2 from null to newCeo1"),
      new RelationChangeState (newCeo1, "Company", null, null, "3: 3. Changed event of newCeo from null to newCustomer1"),
      new RelationChangeState (newCustomer2, "Ceo", null, null, "3: 4. Changed event of newCustomer2 from null to newCeo1"),

      new RelationChangeState (newCeo1, "Company", newCustomer2, null, "4: 1. Changing event of newCeo from newCustomer1 to null"),
      new RelationChangeState (newCustomer2, "Ceo", newCeo1, null, "4: 2. Changing event of newCustomer2 from newCeo1 to null"),
      new RelationChangeState (newCeo1, "Company", null, null, "4: 3. Changed event of newCeo from newCustomer1 to null"),
      new RelationChangeState (newCustomer2, "Ceo", null, null, "4: 4. Changed event of newCustomer2 from newCeo1 to null"),

      new RelationChangeState (newOrder1, "Customer", null, newCustomer1, "5: 1. Changing event of newOrder1 from null to newCustomer1"),
      new CollectionChangeState (newCustomer1.Orders, newOrder1, "5: 2. Adding of newOrder1 to newCustomer1"),
      new RelationChangeState (newCustomer1, "Orders", null, newOrder1, "5: 3. Changing event of newCustomer1 from null to newOrder1"),
      new RelationChangeState (newOrder1, "Customer", null, null, "5: 4. Changed event of newOrder1 from null to newCustomer1"),
      new CollectionChangeState (newCustomer1.Orders, newOrder1, "5: 5. Added of newOrder1 to newCustomer1"),
      new RelationChangeState (newCustomer1, "Orders", null, null, "5: 6. Changed event of newCustomer1 from null to newOrder1"),

      new RelationChangeState (newOrder2, "Customer", null, newCustomer1, "6: 1. Changing event of newOrder2 from null to newCustomer1"),
      new CollectionChangeState (newCustomer1.Orders, newOrder2, "6: 2. Adding of newOrder2 to newCustomer1"),
      new RelationChangeState (newCustomer1, "Orders", null, newOrder2, "6: 3. Changing event of newCustomer1 from null to newOrder2"),
      new RelationChangeState (newOrder2, "Customer", null, null, "6: 4. Changed event of newOrder2 from null to newCustomer1"),
      new CollectionChangeState (newCustomer1.Orders, newOrder2, "6: 5. Added of newOrder2 to newCustomer1"),
      new RelationChangeState (newCustomer1, "Orders", null, null, "6: 6. Changed event of newCustomer1 from null to newOrder2"),

      new RelationChangeState (newOrder2, "Customer", newCustomer1, null, "7: 1. Changing event of newOrder2 from newCustomer1 to null"),
      new CollectionChangeState (newCustomer1.Orders, newOrder2, "7: 2. Removing of newOrder2 from newCustomer1"),
      new RelationChangeState (newCustomer1, "Orders", newOrder2, null, "7: 3. Changing event of newCustomer1 from newOrder2 to null"),
      new RelationChangeState (newOrder2, "Customer", null, null, "7: 4. Changed event of newOrder2 from newCustomer1 to null"),
      new CollectionChangeState (newCustomer1.Orders, newOrder2, "7: 5. Removed of newOrder2 from newCustomer1"),
      new RelationChangeState (newCustomer1, "Orders", null, null, "7: 6. Changed event of newCustomer1 from newOrder2 to null"),

      new RelationChangeState (newOrderItem1, "Order", null, newOrder1, "8: 1. Changing event of newOrderItem1 from null to newOrder1"),
      new CollectionChangeState (newOrder1.OrderItems, newOrderItem1, "8: 2. Adding of newOrderItem1 to newOrder1"),
      new RelationChangeState (newOrder1, "OrderItems", null, newOrderItem1, "8: 3. Changing event of newOrder1 from null to newOrderItem1"),
      new RelationChangeState (newOrderItem1, "Order", null, null, "8: 4. Changed event of newOrderItem1 from null to newOrder1"),
      new CollectionChangeState (newOrder1.OrderItems, newOrderItem1, "8: 5. Added of newOrderItem1 to newOrder1"),
      new RelationChangeState (newOrder1, "OrderItems", null, null, "8: 6. Changed event of newOrder1 from null to newOrderItem1"),

      new RelationChangeState (newOrderItem2, "Order", null, newOrder1, "9: 1. Changing event of newOrderItem2 from null to newOrder1"),
      new CollectionChangeState (newOrder1.OrderItems, newOrderItem2, "9: 2. Adding of newOrderItem2 to newOrder1"),
      new RelationChangeState (newOrder1, "OrderItems", null, newOrderItem2, "9: 3. Changing event of newOrder1 from null to newOrderItem2"),
      new RelationChangeState (newOrderItem2, "Order", null, null, "9: 4. Changed event of newOrderItem2 from null to newOrder1"),
      new CollectionChangeState (newOrder1.OrderItems, newOrderItem2, "9: 5. Added of newOrderItem2 to newOrder1"),
      new RelationChangeState (newOrder1, "OrderItems", null, null, "9: 6. Changed event of newOrder1 from null to newOrderItem2"),

      new RelationChangeState (newOrderItem1, "Order", newOrder1, null, "10: 1. Changing event of newOrderItem1 from newOrder1 to null"),
      new CollectionChangeState (newOrder1.OrderItems, newOrderItem1, "10: 2. Removing of newOrderItem1 from newOrder1"),
      new RelationChangeState (newOrder1, "OrderItems", newOrderItem1, null, "10: 3. Changing event of newOrder1 from newOrderItem1 to null"),
      new RelationChangeState (newOrderItem1, "Order", null, null, "10: 4. Changed event of newOrderItem2 from newOrder1 to null"),
      new CollectionChangeState (newOrder1.OrderItems, newOrderItem1, "10: 5. Removed of newOrderItem1 from newOrder1"),
      new RelationChangeState (newOrder1, "OrderItems", null, null, "10: 6. Changed event of newOrder1 from newOrderItem1 to null"),

      new RelationChangeState (newOrderItem1, "Order", null, newOrder2, "11: 1. Changing event of newOrderItem1 from null to newOrder2"),
      new CollectionChangeState (newOrder2.OrderItems, newOrderItem1, "11: 2. Adding of newOrderItem1 to newOrder2"),
      new RelationChangeState (newOrder2, "OrderItems", null, newOrderItem1, "11: 3. Changing event of newOrder2 from null to newOrderItem1"),
      new RelationChangeState (newOrderItem1, "Order", null, null, "11: 4. Changed event of newOrderItem2 from null to newOrder2"),
      new CollectionChangeState (newOrder2.OrderItems, newOrderItem1, "11: 5. Adding of newOrderItem1 to newOrder2"),
      new RelationChangeState (newOrder2, "OrderItems", null, null, "11: 6. Changed event of newOrder2 from null to newOrderItem1"),

      new RelationChangeState (newOrder1, "Official", null, official2, "12: 1. Changing event of newOrder1 from null to official2"),
      new CollectionChangeState (official2.Orders, newOrder1, "12: 2. Adding of newOrder1 to official2"),
      new RelationChangeState (official2, "Orders", null, newOrder1, "12: 3. Changing event of official2 from null to newOrder1"),
      new RelationChangeState (newOrder1, "Official", null, null, "12: 4. Changed event of newOrder1 from null to official2"),
      new CollectionChangeState (official2.Orders, newOrder1, "12: 5. Adding of newOrder1 to official2"),
      new RelationChangeState (official2, "Orders", null, null, "12: 6. Changed event of official2 from null to newOrder1"),

      new RelationChangeState (newOrder1, "OrderTicket", null, newOrderTicket1, "13: 1. Changing event of newOrder1 from null to newOrderTicket1"),
      new RelationChangeState (newOrder1, "OrderTicket", null, null, "13: 2. Changed event of newOrder1 from null to newOrderTicket1")
    };

      eventReceiver.Check (expectedChangeStates);
      eventReceiver.Unregister ();

      eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { newCustomer1, newOrderTicket1, newOrder2, newOrder1, newOrderItem1 },
          new DomainObjectCollection[] { newOrder2.OrderItems, newCustomer1.Orders });

      //14
      newOrderTicket1.Order = newOrder2;


      expectedChangeStates = new ChangeState[]
    { 
      new RelationChangeState (newOrderTicket1, "Order", newOrder1, newOrder2, "14: 1. Changing event of newOrderTicket1 from newOrder1 to newOrder2"),
      new RelationChangeState (newOrder1, "OrderTicket", newOrderTicket1, null, "14: 2. Changing event of newOrder1 from newOrderTicket1 to null"),
      new RelationChangeState (newOrder2, "OrderTicket", null, newOrderTicket1, "14: 3. Changing event of newOrder1 from null to newOrderTicket1"),
      new RelationChangeState (newOrderTicket1, "Order", null, null, "14: 4. Changed event of newOrderTicket1 from newOrder1 to newOrder2"),
      new RelationChangeState (newOrder1, "OrderTicket", null, null, "14: 5. Changed event of newOrder1 from newOrderTicket1 to null"),
      new RelationChangeState (newOrder2, "OrderTicket", null, null, "14: 6. Changed event of newOrder1 from null to newOrderTicket1"),
    };

      eventReceiver.Check (expectedChangeStates);
      eventReceiver.Unregister ();

      //15a
      eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { newCustomer1, newOrderTicket1, newOrder2, newOrder1, newOrderItem1 },
          new DomainObjectCollection[] { newOrder2.OrderItems, newCustomer1.Orders });

      newOrder2.Customer = newCustomer1;

      expectedChangeStates = new ChangeState[]
    { 
      new RelationChangeState (newOrder2, "Customer", null, newCustomer1, "15a: 1. Changing event of newOrder2 from null to newCustomer1.Orders"),
      new CollectionChangeState (newCustomer1.Orders, newOrder2, "15a: 2. Adding of newOrder2 to newCustomer1"),
      new RelationChangeState (newCustomer1, "Orders", null, newOrder2, "15a: 3. Changing event of newCustomer1 from null to newOrder2"),
      new RelationChangeState (newOrder2, "Customer", null, null, "15a: 4. Changed event of newOrder2 from null to newCustomer1.Orders"),
      new CollectionChangeState (newCustomer1.Orders, newOrder2, "15a: 5. Added of newOrder2 to newCustomer1"),
      new RelationChangeState (newCustomer1, "Orders", null, null, "15a: 6. Changed event of newCustomer2 from null to newOrder2"),
    };

      eventReceiver.Check (expectedChangeStates);
      eventReceiver.Unregister ();

      //15b
      eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { newCustomer1, newCustomer2, newOrderTicket1, newOrder2, newOrder1, newOrderItem1 },
          new DomainObjectCollection[] { newOrder2.OrderItems, newCustomer1.Orders, newCustomer2.Orders });

      newOrder2.Customer = newCustomer2;

      expectedChangeStates = new ChangeState[]
    { 
      new RelationChangeState (newOrder2, "Customer", newCustomer1, newCustomer2, "15b: 1. Changing event of newOrder2 from null to newCustomer2.Orders"),
      new CollectionChangeState (newCustomer1.Orders, newOrder2, "15b: 2. Removing of newOrder2 from newCustomer1"),
      new RelationChangeState (newCustomer1, "Orders", newOrder2, null, "15b: 3. Changing event of newCustomer1 from newOrder2 to null"),
      new CollectionChangeState (newCustomer2.Orders, newOrder2, "15b: 4. Adding of newOrder2 to newCustomer2"),
      new RelationChangeState (newCustomer2, "Orders", null, newOrder2, "15b: 5. Changing event of newCustomer2 from null to newOrder2"),
      new RelationChangeState (newOrder2, "Customer", null, null, "15b: 6. Changed event of newOrder2 from null to newCustomer2.Orders"),
      new CollectionChangeState (newCustomer1.Orders, newOrder2, "15b: 7. Removed of newOrder2 from newCustomer1"),
      new RelationChangeState (newCustomer1, "Orders", null, null, "15b: 8. Changed event of newCustomer1 from newOrder2 to null"),
      new CollectionChangeState (newCustomer2.Orders, newOrder2, "15b: 9. Added of newOrder2 to newCustomer2"),
      new RelationChangeState (newCustomer2, "Orders", null, null, "15b: 10. Changed event of newCustomer2 from null to newOrder2"),
    };

      eventReceiver.Check (expectedChangeStates);
      eventReceiver.Unregister ();

      //16
      eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { newCustomer1, newCustomer2, newOrderTicket1, newOrder2, newOrder1, newOrderItem1 },
          new DomainObjectCollection[] { newOrder2.OrderItems, newCustomer1.Orders, newCustomer2.Orders });

      newOrder2.Delete ();

      expectedChangeStates = new ChangeState[]
    { 
      new ObjectDeletionState (newOrder2, "16: 1. Deleting event of newOrder2"),
      new CollectionChangeState (newCustomer2.Orders, newOrder2, "16: 2. Removing of newOrder2 from newCustomer2"),
      new RelationChangeState (newCustomer2, "Orders", newOrder2, null, "16: 3. Changing event of newCustomer2 from newOrder2 to null"),
      new RelationChangeState (newOrderTicket1, "Order", newOrder2, null, "16: 4. Changing event of newOrderTicket1 from newOrder2 to null"),
      new RelationChangeState (newOrderItem1, "Order", newOrder2, null, "16: 5. Changing event of newOrderItem1 from newOrder2 to null"),

      new ObjectDeletionState (newOrder2, "16: 6. Deleted event of newOrder2"),
      new CollectionChangeState (newCustomer2.Orders, newOrder2, "16: 7. Removed of newOrder2 from newCustomer2"),
      new RelationChangeState (newCustomer2, "Orders", null, null, "16: 8. Changed event of newCustomer2 from newOrder2 to null"),
      new RelationChangeState (newOrderTicket1, "Order", null, null, "16: 9. Changed event of newOrderTicket1 from newOrder2 to null"),
      new RelationChangeState (newOrderItem1, "Order", null, null, "16: 10. Changed event of newOrderItem1 from newOrder2 to null"),
    };

      eventReceiver.Check (expectedChangeStates);
      eventReceiver.Unregister ();

      //17
      eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { newCustomer1, newCustomer2, newOrderTicket1, newOrder1, newOrderItem1 },
          new DomainObjectCollection[] { newCustomer1.Orders, newCustomer2.Orders });

      newOrderTicket1.Order = newOrder1;

      expectedChangeStates = new ChangeState[]
    { 
      new RelationChangeState (newOrderTicket1, "Order", null, newOrder1, "17: 1. Changing event of newOrderTicket1 from null to newOrder1"),
      new RelationChangeState (newOrder1, "OrderTicket", null, newOrderTicket1, "17: 2. Changing event of newOrder1 from null to newOrderTicket1"),
      new RelationChangeState (newOrderTicket1, "Order", null, null, "17: 3. Changed event of newOrderTicket1 from null to newOrder1"),
      new RelationChangeState (newOrder1, "OrderTicket", null, null, "17: 4. Changed event of newOrder1 from null to newOrderTicket1"),
    };

      eventReceiver.Check (expectedChangeStates);
      eventReceiver.Unregister ();

      //cleanup for commit
      newCustomer2.Delete ();
      newCeo1.Delete ();
      newOrderItem1.Delete ();

      ClientTransaction.Current.Commit ();
    }

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

      //TODO: add ICtxExt mock and the events

      using (mockRepository.Ordered ())
      {
        // 1
        newCeo1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCustomer1));
        //new RelationChangeState (newCeo1, "Company", null, newCustomer1, "1: 1. Changing event of newCeo from null to newCustomer1"),

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Ceo") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCeo1));
        //new RelationChangeState (newCustomer1, "Ceo", null, newCeo1, "1: 2. Changing event of newCustomer1 from null to newCeo1"),

        newCeo1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company"));
        //new RelationChangeState (newCeo1, "Company", null, null, "1: 3. Changed event of newCeo from null to newCustomer1"),

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Ceo"));
        //new RelationChangeState (newCustomer1, "Ceo", null, null, "1: 4. Changed event of newCustomer1 from null to newCeo1"),

        // 2
        newCeo2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCeo2), Property.Value ("PropertyName", "Company") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCustomer1));
        //new RelationChangeState (newCeo2, "Company", null, newCustomer1, "2: 1. Changing event of newCeo2 from null to newCustomer1"),

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Ceo") & Property.Value ("OldRelatedObject", newCeo1) & Property.Value ("NewRelatedObject", newCeo2));
        //new RelationChangeState (newCustomer1, "Ceo", newCeo1, newCeo2, "2: 2. Changing event of newCustomer1 from newCeo1 to newCeo2"),

        newCeo1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company") & Property.Value ("OldRelatedObject", newCustomer1) & Property.Value ("NewRelatedObject", null));
        //new RelationChangeState (newCeo1, "Company", newCustomer1, null, "2: 3. Changing event of newCeo1 from newCustomer1 to null"),

        newCeo2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCeo2), Property.Value ("PropertyName", "Company"));
        //new RelationChangeState (newCeo2, "Company", null, null, "2: 4. Changed event of newCeo2 from null to newCustomer1"),

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Ceo"));
        //new RelationChangeState (newCustomer1, "Ceo", null, null, "2: 5. Changed event of newCustomer1 from newCeo1 to newCeo2"),

        newCeo1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company"));
        //new RelationChangeState (newCeo1, "Company", null, null, "2: 6. Changed event of newCeo1 from newCustomer1 to null"),


        newCeo1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCustomer2));
        //new RelationChangeState (newCeo1, "Company", null, newCustomer2, "3: 1. Changing event of newCeo from null to newCustomer1"),

        newCustomer2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Ceo") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCeo1));
        //new RelationChangeState (newCustomer2, "Ceo", null, newCeo1, "3: 2. Changing event of newCustomer2 from null to newCeo1"),

        newCeo1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company"));
        //new RelationChangeState (newCeo1, "Company", null, null, "3: 3. Changed event of newCeo from null to newCustomer1"),

        newCustomer2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Ceo"));
        //new RelationChangeState (newCustomer2, "Ceo", null, null, "3: 4. Changed event of newCustomer2 from null to newCeo1"),


        newCeo1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company") & Property.Value ("OldRelatedObject", newCustomer2) & Property.Value ("NewRelatedObject", null));
        //new RelationChangeState (newCeo1, "Company", newCustomer2, null, "4: 1. Changing event of newCeo from newCustomer1 to null"),

        newCustomer2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Ceo") & Property.Value ("OldRelatedObject", newCeo1) & Property.Value ("NewRelatedObject", null));
        //new RelationChangeState (newCustomer2, "Ceo", newCeo1, null, "4: 2. Changing event of newCustomer2 from newCeo1 to null"),

        newCeo1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Property.Value ("PropertyName", "Company"));
        //new RelationChangeState (newCeo1, "Company", null, null, "4: 3. Changed event of newCeo from newCustomer1 to null"),

        newCustomer2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Ceo"));
        //new RelationChangeState (newCustomer2, "Ceo", null, null, "4: 4. Changed event of newCustomer2 from newCeo1 to null"),


        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "Customer") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCustomer1));
        //new RelationChangeState (newOrder1, "Customer", null, newCustomer1, "5: 1. Changing event of newOrder1 from null to newCustomer1"),

        newCustomer1OrdersEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder1));
        //new CollectionChangeState (newCustomer1.Orders, newOrder1, "5: 2. Adding of newOrder1 to newCustomer1"),

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder1));
        //new RelationChangeState (newCustomer1, "Orders", null, newOrder1, "5: 3. Changing event of newCustomer1 from null to newOrder1"),

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "Customer"));
        //new RelationChangeState (newOrder1, "Customer", null, null, "5: 4. Changed event of newOrder1 from null to newCustomer1"),

        newCustomer1OrdersEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder1));
        //new CollectionChangeState (newCustomer1.Orders, newOrder1, "5: 5. Added of newOrder1 to newCustomer1"),

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders"));
        //new RelationChangeState (newCustomer1, "Orders", null, null, "5: 6. Changed event of newCustomer1 from null to newOrder1"),


        newOrder2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCustomer1));
        //new RelationChangeState (newOrder2, "Customer", null, newCustomer1, "6: 1. Changing event of newOrder2 from null to newCustomer1"),

        newCustomer1OrdersEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));
        //new CollectionChangeState (newCustomer1.Orders, newOrder2, "6: 2. Adding of newOrder2 to newCustomer1"),

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder2));
        //new RelationChangeState (newCustomer1, "Orders", null, newOrder2, "6: 3. Changing event of newCustomer1 from null to newOrder2"),

        newOrder2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer"));
        //new RelationChangeState (newOrder2, "Customer", null, null, "6: 4. Changed event of newOrder2 from null to newCustomer1"),

        newCustomer1OrdersEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));
        //new CollectionChangeState (newCustomer1.Orders, newOrder2, "6: 5. Added of newOrder2 to newCustomer1"),

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders"));
        //new RelationChangeState (newCustomer1, "Orders", null, null, "6: 6. Changed event of newCustomer1 from null to newOrder2"),


        newOrder2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer") & Property.Value ("OldRelatedObject", newCustomer1) & Property.Value ("NewRelatedObject", null));
        //new RelationChangeState (newOrder2, "Customer", newCustomer1, null, "7: 1. Changing event of newOrder2 from newCustomer1 to null"),

        newCustomer1OrdersEventReceiver.Removing (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));
        //new CollectionChangeState (newCustomer1.Orders, newOrder2, "7: 2. Removing of newOrder2 from newCustomer1"),

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", newOrder2) & Property.Value ("NewRelatedObject", null));
        //new RelationChangeState (newCustomer1, "Orders", newOrder2, null, "7: 3. Changing event of newCustomer1 from newOrder2 to null"),

        newOrder2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer"));
        //new RelationChangeState (newOrder2, "Customer", null, null, "7: 4. Changed event of newOrder2 from newCustomer1 to null"),

        newCustomer1OrdersEventReceiver.Removed (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));
        //new CollectionChangeState (newCustomer1.Orders, newOrder2, "7: 5. Removed of newOrder2 from newCustomer1"),

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders"));
        //new RelationChangeState (newCustomer1, "Orders", null, null, "7: 6. Changed event of newCustomer1 from newOrder2 to null"),


        newOrderItem1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder1));
        //new RelationChangeState (newOrderItem1, "Order", null, newOrder1, "8: 1. Changing event of newOrderItem1 from null to newOrder1"),

        newOrder1OrderItemsEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newOrder1OrderItems), Property.Value ("DomainObject", newOrderItem1));
        //new CollectionChangeState (newOrder1.OrderItems, newOrderItem1, "8: 2. Adding of newOrderItem1 to newOrder1"),

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderItems") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrderItem1));
        //new RelationChangeState (newOrder1, "OrderItems", null, newOrderItem1, "8: 3. Changing event of newOrder1 from null to newOrderItem1"),

        newOrderItem1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order"));
        //new RelationChangeState (newOrderItem1, "Order", null, null, "8: 4. Changed event of newOrderItem1 from null to newOrder1"),

        newOrder1OrderItemsEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newOrder1OrderItems), Property.Value ("DomainObject", newOrderItem1));
        //new CollectionChangeState (newOrder1.OrderItems, newOrderItem1, "8: 5. Added of newOrderItem1 to newOrder1"),

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderItems"));
        //new RelationChangeState (newOrder1, "OrderItems", null, null, "8: 6. Changed event of newOrder1 from null to newOrderItem1"),


        newOrderItem2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderItem2), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder1));
        //new RelationChangeState (newOrderItem2, "Order", null, newOrder1, "9: 1. Changing event of newOrderItem2 from null to newOrder1"),

        newOrder1OrderItemsEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newOrder1OrderItems), Property.Value ("DomainObject", newOrderItem2));
        //new CollectionChangeState (newOrder1.OrderItems, newOrderItem2, "9: 2. Adding of newOrderItem2 to newOrder1"),

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderItems") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrderItem2));
        //new RelationChangeState (newOrder1, "OrderItems", null, newOrderItem2, "9: 3. Changing event of newOrder1 from null to newOrderItem2"),

        newOrderItem2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderItem2), Property.Value ("PropertyName", "Order"));
        //new RelationChangeState (newOrderItem2, "Order", null, null, "9: 4. Changed event of newOrderItem2 from null to newOrder1"),

        newOrder1OrderItemsEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newOrder1OrderItems), Property.Value ("DomainObject", newOrderItem2));
        //new CollectionChangeState (newOrder1.OrderItems, newOrderItem2, "9: 5. Added of newOrderItem2 to newOrder1"),

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderItems"));
        //new RelationChangeState (newOrder1, "OrderItems", null, null, "9: 6. Changed event of newOrder1 from null to newOrderItem2"),


        newOrderItem1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", newOrder1) & Property.Value ("NewRelatedObject", null));
        //new RelationChangeState (newOrderItem1, "Order", newOrder1, null, "10: 1. Changing event of newOrderItem1 from newOrder1 to null"),

        newOrder1OrderItemsEventReceiver.Removing (null, null);
        LastCall.Constraints (Is.Same (newOrder1OrderItems), Property.Value ("DomainObject", newOrderItem1));
        //new CollectionChangeState (newOrder1.OrderItems, newOrderItem1, "10: 2. Removing of newOrderItem1 from newOrder1"),

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderItems") & Property.Value ("OldRelatedObject", newOrderItem1) & Property.Value ("NewRelatedObject", null));
        //new RelationChangeState (newOrder1, "OrderItems", newOrderItem1, null, "10: 3. Changing event of newOrder1 from newOrderItem1 to null"),

        newOrderItem1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order"));
        //new RelationChangeState (newOrderItem1, "Order", null, null, "10: 4. Changed event of newOrderItem2 from newOrder1 to null"),

        newOrder1OrderItemsEventReceiver.Removed (null, null);
        LastCall.Constraints (Is.Same (newOrder1OrderItems), Property.Value ("DomainObject", newOrderItem1));
        //new CollectionChangeState (newOrder1.OrderItems, newOrderItem1, "10: 5. Removed of newOrderItem1 from newOrder1"),

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderItems"));
        //new RelationChangeState (newOrder1, "OrderItems", null, null, "10: 6. Changed event of newOrder1 from newOrderItem1 to null"),


        newOrderItem1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder2));
        //new RelationChangeState (newOrderItem1, "Order", null, newOrder2, "11: 1. Changing event of newOrderItem1 from null to newOrder2"),

        newOrder2OrderItemsEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newOrder2OrderItems), Property.Value ("DomainObject", newOrderItem1));
        //new CollectionChangeState (newOrder2.OrderItems, newOrderItem1, "11: 2. Adding of newOrderItem1 to newOrder2"),

        newOrder2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "OrderItems") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrderItem1));
        //new RelationChangeState (newOrder2, "OrderItems", null, newOrderItem1, "11: 3. Changing event of newOrder2 from null to newOrderItem1"),

        newOrderItem1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order"));
        //new RelationChangeState (newOrderItem1, "Order", null, null, "11: 4. Changed event of newOrderItem2 from null to newOrder2"),

        newOrder2OrderItemsEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newOrder2OrderItems), Property.Value ("DomainObject", newOrderItem1));
        //new CollectionChangeState (newOrder2.OrderItems, newOrderItem1, "11: 5. Adding of newOrderItem1 to newOrder2"),

        newOrder2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "OrderItems"));
        //new RelationChangeState (newOrder2, "OrderItems", null, null, "11: 6. Changed event of newOrder2 from null to newOrderItem1"),

        
        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "Official") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", official2));
        //new RelationChangeState (newOrder1, "Official", null, official2, "12: 1. Changing event of newOrder1 from null to official2"),

        official2OrdersEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (official2Orders), Property.Value ("DomainObject", newOrder1));
        //new CollectionChangeState (official2.Orders, newOrder1, "12: 2. Adding of newOrder1 to official2"),

        official2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (official2), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder1));
        //new RelationChangeState (official2, "Orders", null, newOrder1, "12: 3. Changing event of official2 from null to newOrder1"),

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "Official"));
        //new RelationChangeState (newOrder1, "Official", null, null, "12: 4. Changed event of newOrder1 from null to official2"),

        official2OrdersEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (official2Orders), Property.Value ("DomainObject", newOrder1));
        //new CollectionChangeState (official2.Orders, newOrder1, "12: 5. Adding of newOrder1 to official2"),

        official2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (official2), Property.Value ("PropertyName", "Orders"));
        //new RelationChangeState (official2, "Orders", null, null, "12: 6. Changed event of official2 from null to newOrder1"),


        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderTicket") & Property.Value ("OldRelatedObject", null));
        // Note: NewRelatedObject cannot be checked easily here.
        //new RelationChangeState (newOrder1, "OrderTicket", null, newOrderTicket1, "13: 1. Changing event of newOrder1 from null to newOrderTicket1"),

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderTicket"));
        //new RelationChangeState (newOrder1, "OrderTicket", null, null, "13: 2. Changed event of newOrder1 from null to newOrderTicket1")
      }

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

      mockRepository.BackToRecord (newCustomer1EventReceiver);
      mockRepository.BackToRecord (newCustomer2EventReceiver);
      mockRepository.BackToRecord (official2EventReceiver);
      mockRepository.BackToRecord (newCeo1EventReceiver);
      mockRepository.BackToRecord (newCeo2EventReceiver);
      mockRepository.BackToRecord (newOrder1EventReceiver);
      mockRepository.BackToRecord (newOrder2EventReceiver);
      mockRepository.BackToRecord (newOrderItem1EventReceiver);
      mockRepository.BackToRecord (newOrderItem2EventReceiver);

      mockRepository.BackToRecord (newCustomer1OrdersEventReceiver);
      mockRepository.BackToRecord (newCustomer2OrdersEventReceiver);
      mockRepository.BackToRecord (official2OrdersEventReceiver);
      mockRepository.BackToRecord (newOrder1OrderItemsEventReceiver);
      mockRepository.BackToRecord (newOrder2OrderItemsEventReceiver);
      DomainObjectMockEventReceiver newOrderTicket1EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrderTicket1);

      using (mockRepository.Ordered ())
      {
        newOrderTicket1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderTicket1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", newOrder1) & Property.Value ("NewRelatedObject", newOrder2));
        //new RelationChangeState (newOrderTicket1, "Order", newOrder1, newOrder2, "14: 1. Changing event of newOrderTicket1 from newOrder1 to newOrder2"),

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderTicket") & Property.Value ("OldRelatedObject", newOrderTicket1) & Property.Value ("NewRelatedObject", null));
        //new RelationChangeState (newOrder1, "OrderTicket", newOrderTicket1, null, "14: 2. Changing event of newOrder1 from newOrderTicket1 to null"),

        newOrder2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "OrderTicket") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrderTicket1));
        //new RelationChangeState (newOrder2, "OrderTicket", null, newOrderTicket1, "14: 3. Changing event of newOrder1 from null to newOrderTicket1"),

        newOrderTicket1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderTicket1), Property.Value ("PropertyName", "Order"));
        //new RelationChangeState (newOrderTicket1, "Order", null, null, "14: 4. Changed event of newOrderTicket1 from newOrder1 to newOrder2"),

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderTicket"));
        //new RelationChangeState (newOrder1, "OrderTicket", null, null, "14: 5. Changed event of newOrder1 from newOrderTicket1 to null"),

        newOrder2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "OrderTicket"));
        //new RelationChangeState (newOrder2, "OrderTicket", null, null, "14: 6. Changed event of newOrder1 from null to newOrderTicket1"),


        newOrder2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newCustomer1));
        //new RelationChangeState (newOrder2, "Customer", null, newCustomer1, "15a: 1. Changing event of newOrder2 from null to newCustomer1.Orders"),

        newCustomer1OrdersEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));
        //new CollectionChangeState (newCustomer1.Orders, newOrder2, "15a: 2. Adding of newOrder2 to newCustomer1"),

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder2));
        //new RelationChangeState (newCustomer1, "Orders", null, newOrder2, "15a: 3. Changing event of newCustomer1 from null to newOrder2"),

        newOrder2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer"));
        //new RelationChangeState (newOrder2, "Customer", null, null, "15a: 4. Changed event of newOrder2 from null to newCustomer1.Orders"),

        newCustomer1OrdersEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));
        //new CollectionChangeState (newCustomer1.Orders, newOrder2, "15a: 5. Added of newOrder2 to newCustomer1"),

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders"));
        //new RelationChangeState (newCustomer1, "Orders", null, null, "15a: 6. Changed event of newCustomer2 from null to newOrder2"),

        newOrder2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer") & Property.Value ("OldRelatedObject", newCustomer1) & Property.Value ("NewRelatedObject", newCustomer2));
        //new RelationChangeState (newOrder2, "Customer", newCustomer1, newCustomer2, "15b: 1. Changing event of newOrder2 from null to newCustomer2.Orders"),

        newCustomer1OrdersEventReceiver.Removing (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));
        //new CollectionChangeState (newCustomer1.Orders, newOrder2, "15b: 2. Removing of newOrder2 from newCustomer1"),

        newCustomer1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", newOrder2) & Property.Value ("NewRelatedObject", null));
        //new RelationChangeState (newCustomer1, "Orders", newOrder2, null, "15b: 3. Changing event of newCustomer1 from newOrder2 to null"),

        newCustomer2OrdersEventReceiver.Adding (null, null);
        LastCall.Constraints (Is.Same (newCustomer2Orders), Property.Value ("DomainObject", newOrder2));
        //new CollectionChangeState (newCustomer2.Orders, newOrder2, "15b: 4. Adding of newOrder2 to newCustomer2"),

        newCustomer2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder2));
        //new RelationChangeState (newCustomer2, "Orders", null, newOrder2, "15b: 5. Changing event of newCustomer2 from null to newOrder2"),

        newOrder2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Property.Value ("PropertyName", "Customer"));
        //new RelationChangeState (newOrder2, "Customer", null, null, "15b: 6. Changed event of newOrder2 from null to newCustomer2.Orders"),

        newCustomer1OrdersEventReceiver.Removed (null, null);
        LastCall.Constraints (Is.Same (newCustomer1Orders), Property.Value ("DomainObject", newOrder2));
        //new CollectionChangeState (newCustomer1.Orders, newOrder2, "15b: 7. Removed of newOrder2 from newCustomer1"),

        newCustomer1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer1), Property.Value ("PropertyName", "Orders"));
        //new RelationChangeState (newCustomer1, "Orders", null, null, "15b: 8. Changed event of newCustomer1 from newOrder2 to null"),

        newCustomer2OrdersEventReceiver.Added (null, null);
        LastCall.Constraints (Is.Same (newCustomer2Orders), Property.Value ("DomainObject", newOrder2));
        //new CollectionChangeState (newCustomer2.Orders, newOrder2, "15b: 9. Added of newOrder2 to newCustomer2"),

        newCustomer2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Orders"));
        //new RelationChangeState (newCustomer2, "Orders", null, null, "15b: 10. Changed event of newCustomer2 from null to newOrder2"),

        newOrder2EventReceiver.Deleting (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Is.NotNull ());
        //new ObjectDeletionState (newOrder2, "16: 1. Deleting event of newOrder2"),

        newCustomer2OrdersEventReceiver.Removing (null, null);
        LastCall.Constraints (Is.Same (newCustomer2Orders), Property.Value ("DomainObject", newOrder2));
        //new CollectionChangeState (newCustomer2.Orders, newOrder2, "16: 2. Removing of newOrder2 from newCustomer2"),

        newCustomer2EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Orders") & Property.Value ("OldRelatedObject", newOrder2) & Property.Value ("NewRelatedObject", null));
        //new RelationChangeState (newCustomer2, "Orders", newOrder2, null, "16: 3. Changing event of newCustomer2 from newOrder2 to null"),

        newOrderTicket1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderTicket1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", newOrder2) & Property.Value ("NewRelatedObject", null));
        //new RelationChangeState (newOrderTicket1, "Order", newOrder2, null, "16: 4. Changing event of newOrderTicket1 from newOrder2 to null"),

        newOrderItem1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", newOrder2) & Property.Value ("NewRelatedObject", null));
        //new RelationChangeState (newOrderItem1, "Order", newOrder2, null, "16: 5. Changing event of newOrderItem1 from newOrder2 to null"),

        newOrder2EventReceiver.Deleted (null, null);
        LastCall.Constraints (Is.Same (newOrder2), Is.NotNull ());
        //new ObjectDeletionState (newOrder2, "16: 6. Deleted event of newOrder2"),

        newCustomer2OrdersEventReceiver.Removed (null, null);
        LastCall.Constraints (Is.Same (newCustomer2Orders), Property.Value ("DomainObject", newOrder2));
        //new CollectionChangeState (newCustomer2.Orders, newOrder2, "16: 7. Removed of newOrder2 from newCustomer2"),

        newCustomer2EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Property.Value ("PropertyName", "Orders"));
        //new RelationChangeState (newCustomer2, "Orders", null, null, "16: 8. Changed event of newCustomer2 from newOrder2 to null"),

        newOrderTicket1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderTicket1), Property.Value ("PropertyName", "Order"));
        //new RelationChangeState (newOrderTicket1, "Order", null, null, "16: 9. Changed event of newOrderTicket1 from newOrder2 to null"),

        newOrderItem1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Property.Value ("PropertyName", "Order"));
        //new RelationChangeState (newOrderItem1, "Order", null, null, "16: 10. Changed event of newOrderItem1 from newOrder2 to null"),


        newOrderTicket1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrderTicket1), Property.Value ("PropertyName", "Order") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrder1));
        //new RelationChangeState (newOrderTicket1, "Order", null, newOrder1, "17: 1. Changing event of newOrderTicket1 from null to newOrder1"),

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderTicket") & Property.Value ("OldRelatedObject", null) & Property.Value ("NewRelatedObject", newOrderTicket1));
        //new RelationChangeState (newOrder1, "OrderTicket", null, newOrderTicket1, "17: 2. Changing event of newOrder1 from null to newOrderTicket1"),

        newOrderTicket1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrderTicket1), Property.Value ("PropertyName", "Order"));
        //new RelationChangeState (newOrderTicket1, "Order", null, null, "17: 3. Changed event of newOrderTicket1 from null to newOrder1"),

        newOrder1EventReceiver.RelationChanged (null, null);
        LastCall.Constraints (Is.Same (newOrder1), Property.Value ("PropertyName", "OrderTicket"));
        //new RelationChangeState (newOrder1, "OrderTicket", null, null, "17: 4. Changed event of newOrder1 from null to newOrderTicket1"),


        newCustomer2EventReceiver.Deleting (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Is.NotNull ());
        newCustomer2EventReceiver.Deleted (null, null);
        LastCall.Constraints (Is.Same (newCustomer2), Is.NotNull ());
        //newCustomer2.Delete (); // 18

        newCeo1EventReceiver.Deleting (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Is.NotNull ());
        newCeo1EventReceiver.Deleted (null, null);
        LastCall.Constraints (Is.Same (newCeo1), Is.NotNull ());
        //newCeo1.Delete (); // 19

        newOrderItem1EventReceiver.Deleting (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Is.NotNull ());
        newOrderItem1EventReceiver.Deleted (null, null);
        LastCall.Constraints (Is.Same (newOrderItem1), Is.NotNull ());
        //newOrderItem1.Delete (); // 20

        using (mockRepository.Unordered ())
        {
          newCustomer1EventReceiver.Committing (null, null);
          LastCall.Constraints (Is.Same(newCustomer1), Is.NotNull ());

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
  }
}
