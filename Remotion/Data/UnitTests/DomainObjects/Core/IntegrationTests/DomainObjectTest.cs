// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_Property = Rhino.Mocks.Constraints.Property;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class DomainObjectTest : ClientTransactionBaseTest
  {
    private DataContainer _orderDataContainer;
    private PropertyValueCollection _orderPropertyValues;
    private PropertyValue _orderDeliveryDateProperty;

    private DomainObjectEventReceiver _orderDomainObjectEventReceiver;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
      SetDatabaseModifyable();
    }

    [Test]
    public void RelationEventTestWithMockObject ()
    {
      Customer newCustomer1 = Customer.NewObject();
      newCustomer1.Name = "NewCustomer1";

      Customer newCustomer2 = Customer.NewObject();
      newCustomer2.Name = "NewCustomer2";

      Official official2 = Official.GetObject (DomainObjectIDs.Official2);
      Ceo newCeo1 = Ceo.NewObject();
      Ceo newCeo2 = Ceo.NewObject();
      Order newOrder1 = Order.NewObject();
      newOrder1.DeliveryDate = new DateTime (2006, 1, 1);

      Order newOrder2 = Order.NewObject();
      newOrder2.DeliveryDate = new DateTime (2006, 2, 2);

      OrderItem newOrderItem1 = OrderItem.NewObject();
      OrderItem newOrderItem2 = OrderItem.NewObject();

      var mockRepository = new MockRepository();

      DomainObjectCollection newCustomer1Orders = newCustomer1.Orders;
      DomainObjectCollection newCustomer2Orders = newCustomer2.Orders;
      DomainObjectCollection official2Orders = official2.Orders;
      DomainObjectCollection newOrder1OrderItems = newOrder1.OrderItems;
      DomainObjectCollection newOrder2OrderItems = newOrder2.OrderItems;

      var newCustomer1EventReceiver = mockRepository.StrictMock<DomainObjectMockEventReceiver> (newCustomer1);
      var newCustomer2EventReceiver = mockRepository.StrictMock<DomainObjectMockEventReceiver> (newCustomer2);
      var official2EventReceiver = mockRepository.StrictMock<DomainObjectMockEventReceiver> (official2);
      var newCeo1EventReceiver = mockRepository.StrictMock<DomainObjectMockEventReceiver> (newCeo1);
      var newCeo2EventReceiver = mockRepository.StrictMock<DomainObjectMockEventReceiver> (newCeo2);
      var newOrder1EventReceiver = mockRepository.StrictMock<DomainObjectMockEventReceiver> (newOrder1);
      var newOrder2EventReceiver = mockRepository.StrictMock<DomainObjectMockEventReceiver> (newOrder2);
      var newOrderItem1EventReceiver = mockRepository.StrictMock<DomainObjectMockEventReceiver> (newOrderItem1);
      var newOrderItem2EventReceiver = mockRepository.StrictMock<DomainObjectMockEventReceiver> (newOrderItem2);

      var newCustomer1OrdersEventReceiver =
          mockRepository.StrictMock<DomainObjectCollectionMockEventReceiver> (newCustomer1.Orders);
      var newCustomer2OrdersEventReceiver =
          mockRepository.StrictMock<DomainObjectCollectionMockEventReceiver> (newCustomer2.Orders);
      var official2OrdersEventReceiver =
          mockRepository.StrictMock<DomainObjectCollectionMockEventReceiver> (official2.Orders);
      var newOrder1OrderItemsEventReceiver =
          mockRepository.StrictMock<DomainObjectCollectionMockEventReceiver> (newOrder1.OrderItems);
      var newOrder2OrderItemsEventReceiver =
          mockRepository.StrictMock<DomainObjectCollectionMockEventReceiver> (newOrder2.OrderItems);

      var extension = mockRepository.StrictMock<IClientTransactionExtension>();

      using (mockRepository.Ordered())
      {
        //1
        //newCeo1.Company = newCustomer1;
        extension.RelationChanging (
            TestableClientTransaction, newCeo1, GetEndPointDefinition (typeof (Ceo), "Company"), null, newCustomer1);
        newCeo1EventReceiver.RelationChanging (newCeo1, GetEndPointDefinition (typeof (Ceo), "Company"), null, newCustomer1);

        extension.RelationChanging (
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition (typeof (Company), "Ceo"),
            null,
            newCeo1);
        newCustomer1EventReceiver.RelationChanging (newCustomer1, GetEndPointDefinition (typeof (Company), "Ceo"), null, newCeo1);

        newCustomer1EventReceiver.RelationChanged (newCustomer1, GetEndPointDefinition (typeof (Company), "Ceo"), null, newCeo1);
        extension.RelationChanged (TestableClientTransaction, newCustomer1, GetEndPointDefinition (typeof (Company), "Ceo"), null, newCeo1);

        newCeo1EventReceiver.RelationChanged (newCeo1, GetEndPointDefinition (typeof (Ceo), "Company"), null, newCustomer1);
        extension.RelationChanged (TestableClientTransaction, newCeo1, GetEndPointDefinition (typeof (Ceo), "Company"), null, newCustomer1);


        //2
        //newCeo2.Company = newCustomer1;
        extension.RelationChanging (
            TestableClientTransaction, newCeo2, GetEndPointDefinition (typeof (Ceo), "Company"), null, newCustomer1);
        newCeo2EventReceiver.RelationChanging (newCeo2, GetEndPointDefinition (typeof (Ceo), "Company"), null, newCustomer1);

        extension.RelationChanging (
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition (typeof (Company), "Ceo"),
            newCeo1,
            newCeo2);
        newCustomer1EventReceiver.RelationChanging (newCustomer1, GetEndPointDefinition (typeof (Company), "Ceo"), newCeo1, newCeo2);

        extension.RelationChanging (
            TestableClientTransaction, newCeo1, GetEndPointDefinition (typeof (Ceo), "Company"), newCustomer1, null);
        newCeo1EventReceiver.RelationChanging (newCeo1, GetEndPointDefinition (typeof (Ceo), "Company"), newCustomer1, null);

        newCeo1EventReceiver.RelationChanged (newCeo1, GetEndPointDefinition (typeof (Ceo), "Company"), newCustomer1, null);
        extension.RelationChanged (
            TestableClientTransaction,
            newCeo1,
            GetEndPointDefinition (typeof (Ceo), "Company"),
            newCustomer1,
            null);

        newCustomer1EventReceiver.RelationChanged (newCustomer1, GetEndPointDefinition (typeof (Company), "Ceo"), newCeo1, newCeo2);
        extension.RelationChanged (
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition (typeof (Company), "Ceo"),
            newCeo1,
            newCeo2);

        newCeo2EventReceiver.RelationChanged (newCeo2, GetEndPointDefinition (typeof (Ceo), "Company"), null, newCustomer1);
        extension.RelationChanged (
            TestableClientTransaction,
            newCeo2,
            GetEndPointDefinition (typeof (Ceo), "Company"),
             null,
            newCustomer1);

        //3
        //newCeo1.Company = newCustomer2;
        extension.RelationChanging (
            TestableClientTransaction, newCeo1, GetEndPointDefinition (typeof (Ceo), "Company"), null, newCustomer2);
        newCeo1EventReceiver.RelationChanging (newCeo1, GetEndPointDefinition (typeof (Ceo), "Company"), null, newCustomer2);

        extension.RelationChanging (
            TestableClientTransaction,
            newCustomer2,
            GetEndPointDefinition (typeof (Company), "Ceo"),
            null,
            newCeo1);
        newCustomer2EventReceiver.RelationChanging (newCustomer2, GetEndPointDefinition (typeof (Company), "Ceo"), null, newCeo1);

        newCustomer2EventReceiver.RelationChanged (newCustomer2, GetEndPointDefinition (typeof (Company), "Ceo"), null, newCeo1);
        extension.RelationChanged (
            TestableClientTransaction,
            newCustomer2,
            GetEndPointDefinition (typeof (Company), "Ceo"),
            null,
            newCeo1);

        newCeo1EventReceiver.RelationChanged (newCeo1, GetEndPointDefinition (typeof (Ceo), "Company"), null, newCustomer2);
        extension.RelationChanged (
            TestableClientTransaction,
            newCeo1,
            GetEndPointDefinition (typeof (Ceo), "Company"),
            null,
            newCustomer2);


        //4
        //newCeo1.Company = null;
        extension.RelationChanging (
            TestableClientTransaction, newCeo1, GetEndPointDefinition (typeof (Ceo), "Company"), newCustomer2, null);
        newCeo1EventReceiver.RelationChanging (newCeo1, GetEndPointDefinition (typeof (Ceo), "Company"), newCustomer2, null);

        extension.RelationChanging (
            TestableClientTransaction,
            newCustomer2,
            GetEndPointDefinition (typeof (Company), "Ceo"),
            newCeo1,
            null);
        newCustomer2EventReceiver.RelationChanging (newCustomer2, GetEndPointDefinition (typeof (Company), "Ceo"), newCeo1, null);

        newCustomer2EventReceiver.RelationChanged (newCustomer2, GetEndPointDefinition (typeof (Company), "Ceo"), newCeo1, null);
        extension.RelationChanged (
            TestableClientTransaction,
            newCustomer2,
            GetEndPointDefinition (typeof (Company), "Ceo"),
            newCeo1,
            null);

        newCeo1EventReceiver.RelationChanged (newCeo1, GetEndPointDefinition (typeof (Ceo), "Company"), newCustomer2, null);
        extension.RelationChanged (
            TestableClientTransaction,
            newCeo1,
            GetEndPointDefinition (typeof (Ceo), "Company"),
            newCustomer2,
            null);


        //5
        //newCustomer1.Orders.Add (newOrder1);
        extension.RelationReading (
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition (typeof (Customer), "Orders"),
            ValueAccess.Current);
        extension.Expect (
            mock => mock.RelationRead (
                Arg.Is (ClientTransactionScope.CurrentTransaction),
                Arg.Is (newCustomer1),
                Arg.Is (GetEndPointDefinition (typeof (Customer), "Orders")),
                Arg<ReadOnlyDomainObjectCollectionAdapter<DomainObject>>.Matches (data => data.Count == 0),
                Arg.Is (ValueAccess.Current)));

        extension.RelationChanging (
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition (typeof (Order), "Customer"),
            null,
            newCustomer1);
        newOrder1EventReceiver.RelationChanging (newOrder1, GetEndPointDefinition (typeof (Order), "Customer"), null, newCustomer1);

        newCustomer1OrdersEventReceiver.Adding (newCustomer1Orders, newOrder1);
        extension.RelationChanging (
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition (typeof (Customer), "Orders"),
            null,
            newOrder1);
        newCustomer1EventReceiver.RelationChanging (newCustomer1, GetEndPointDefinition (typeof (Customer), "Orders"), null, newOrder1);

        newCustomer1EventReceiver.RelationChanged (newCustomer1, GetEndPointDefinition (typeof (Customer), "Orders"), null, newOrder1);
        extension.RelationChanged (
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition (typeof (Customer), "Orders"),
            null,
            newOrder1);
        newCustomer1OrdersEventReceiver.Added (newCustomer1Orders, newOrder1);

        newOrder1EventReceiver.RelationChanged (newOrder1, GetEndPointDefinition (typeof (Order), "Customer"), null, newCustomer1);
        extension.RelationChanged (
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition (typeof (Order), "Customer"),
            null,
            newCustomer1);


        //6
        //newCustomer1.Orders.Add (newOrder2);
        extension.RelationReading (
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition (typeof (Customer), "Orders"),
            ValueAccess.Current);
        extension.Expect (
            mock => mock.RelationRead (
                Arg.Is (ClientTransactionScope.CurrentTransaction),
                Arg.Is (newCustomer1),
                Arg.Is (GetEndPointDefinition (typeof (Customer), "Orders")),
                Arg<ReadOnlyDomainObjectCollectionAdapter<DomainObject>>.Matches (data => data.Count == 1 && data.ContainsObject (newOrder1)),
                Arg.Is (ValueAccess.Current)));

        extension.RelationChanging (
            TestableClientTransaction,
            newOrder2,
            GetEndPointDefinition (typeof (Order), "Customer"),
            null,
            newCustomer1);
        newOrder2EventReceiver.RelationChanging (newOrder2, GetEndPointDefinition (typeof (Order), "Customer"), null, newCustomer1);

        newCustomer1OrdersEventReceiver.Adding (newCustomer1Orders, newOrder2);
        extension.RelationChanging (
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition (typeof (Customer), "Orders"),
            null,
            newOrder2);
        newCustomer1EventReceiver.RelationChanging (newCustomer1, GetEndPointDefinition (typeof (Customer), "Orders"), null, newOrder2);

        newCustomer1EventReceiver.RelationChanged (newCustomer1, GetEndPointDefinition (typeof (Customer), "Orders"), null, newOrder2);
        extension.RelationChanged (
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition (typeof (Customer), "Orders"),
            null,
            newOrder2);
        newCustomer1OrdersEventReceiver.Added (newCustomer1Orders, newOrder2);

        newOrder2EventReceiver.RelationChanged (newOrder2, GetEndPointDefinition (typeof (Order), "Customer"), null, newCustomer1);
        extension.RelationChanged (
            TestableClientTransaction,
            newOrder2,
            GetEndPointDefinition (typeof (Order), "Customer"),
            null,
            newCustomer1);


        //7
        //newCustomer1.Orders.Remove (newOrder2);
        extension.RelationReading (
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition (typeof (Customer), "Orders"),
            ValueAccess.Current);
        extension.Expect (
            mock => mock.RelationRead (
                Arg.Is (ClientTransactionScope.CurrentTransaction),
                Arg.Is (newCustomer1),
                Arg.Is (GetEndPointDefinition (typeof (Customer), "Orders")),
                Arg<ReadOnlyDomainObjectCollectionAdapter<DomainObject>>.Matches (
                    data => data.Count == 2 && data.ContainsObject (newOrder1) && data.ContainsObject (newOrder2)),
                Arg.Is (ValueAccess.Current)));

        extension.RelationChanging (
            TestableClientTransaction,
            newOrder2,
            GetEndPointDefinition (typeof (Order), "Customer"),
            newCustomer1,
            null);
        newOrder2EventReceiver.RelationChanging (newOrder2, GetEndPointDefinition (typeof (Order), "Customer"), newCustomer1, null);

        newCustomer1OrdersEventReceiver.Removing (newCustomer1Orders, newOrder2);
        extension.RelationChanging (
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition (typeof (Customer), "Orders"),
            newOrder2,
            null);
        newCustomer1EventReceiver.RelationChanging (newCustomer1, GetEndPointDefinition (typeof (Customer), "Orders"), newOrder2, null);

        newCustomer1EventReceiver.RelationChanged (newCustomer1, GetEndPointDefinition (typeof (Customer), "Orders"), newOrder2, null);
        extension.RelationChanged (
            TestableClientTransaction,
            newCustomer1,
            GetEndPointDefinition (typeof (Customer), "Orders"),
            newOrder2,
            null);
        newCustomer1OrdersEventReceiver.Removed (newCustomer1Orders, newOrder2);
        
        newOrder2EventReceiver.RelationChanged (newOrder2, GetEndPointDefinition (typeof (Order), "Customer"), newCustomer1, null);
        extension.RelationChanged (
            TestableClientTransaction,
            newOrder2,
            GetEndPointDefinition (typeof (Order), "Customer"),
            newCustomer1,
            null);


        //8
        //newOrderItem1.Order = newOrder1;
        extension.RelationChanging (
            TestableClientTransaction,
            newOrderItem1,
            GetEndPointDefinition (typeof (OrderItem), "Order"),
            null,
            newOrder1);
        newOrderItem1EventReceiver.RelationChanging (newOrderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), null, newOrder1);

        newOrder1OrderItemsEventReceiver.Adding (newOrder1OrderItems, newOrderItem1);
        extension.RelationChanging (
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition (typeof (Order), "OrderItems"),
            null,
            newOrderItem1);
        newOrder1EventReceiver.RelationChanging (newOrder1, GetEndPointDefinition (typeof (Order), "OrderItems"), null, newOrderItem1);

        newOrder1EventReceiver.RelationChanged (newOrder1, GetEndPointDefinition (typeof (Order), "OrderItems"), null, newOrderItem1);
        extension.RelationChanged (
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition (typeof (Order), "OrderItems"),
            null,
            newOrderItem1);
        newOrder1OrderItemsEventReceiver.Added (newOrder1OrderItems, newOrderItem1);

        newOrderItem1EventReceiver.RelationChanged (newOrderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), null, newOrder1);
        extension.RelationChanged (
            TestableClientTransaction,
            newOrderItem1,
            GetEndPointDefinition (typeof (OrderItem), "Order"),
            null,
            newOrder1);


        //9
        //newOrderItem2.Order = newOrder1;
        extension.RelationChanging (
            TestableClientTransaction,
            newOrderItem2,
            GetEndPointDefinition (typeof (OrderItem), "Order"),
            null,
            newOrder1);
        newOrderItem2EventReceiver.RelationChanging (newOrderItem2, GetEndPointDefinition (typeof (OrderItem), "Order"), null, newOrder1);

        newOrder1OrderItemsEventReceiver.Adding (newOrder1OrderItems, newOrderItem2);
        extension.RelationChanging (
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition (typeof (Order), "OrderItems"),
            null,
            newOrderItem2);
        newOrder1EventReceiver.RelationChanging (newOrder1, GetEndPointDefinition (typeof (Order), "OrderItems"), null, newOrderItem2);

        newOrder1EventReceiver.RelationChanged (newOrder1, GetEndPointDefinition (typeof (Order), "OrderItems"), null, newOrderItem2);
        extension.RelationChanged (
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition (typeof (Order), "OrderItems"),
            null,
            newOrderItem2);
        newOrder1OrderItemsEventReceiver.Added (newOrder1OrderItems, newOrderItem2);

        newOrderItem2EventReceiver.RelationChanged (newOrderItem2, GetEndPointDefinition (typeof (OrderItem), "Order"), null, newOrder1);
        extension.RelationChanged (
            TestableClientTransaction,
            newOrderItem2,
            GetEndPointDefinition (typeof (OrderItem), "Order"),
            null,
            newOrder1);


        //10
        //newOrderItem1.Order = null;
        extension.RelationChanging (
            TestableClientTransaction,
            newOrderItem1,
            GetEndPointDefinition (typeof (OrderItem), "Order"),
            newOrder1,
            null);
        newOrderItem1EventReceiver.RelationChanging (newOrderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), newOrder1, null);

        newOrder1OrderItemsEventReceiver.Removing (newOrder1OrderItems, newOrderItem1);
        extension.RelationChanging (
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition (typeof (Order), "OrderItems"),
            newOrderItem1,
            null);
        newOrder1EventReceiver.RelationChanging (newOrder1, GetEndPointDefinition (typeof (Order), "OrderItems"), newOrderItem1, null);

        newOrder1EventReceiver.RelationChanged (newOrder1, GetEndPointDefinition (typeof (Order), "OrderItems"), newOrderItem1, null);
        extension.RelationChanged (
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition (typeof (Order), "OrderItems"),
            newOrderItem1,
            null);
        newOrder1OrderItemsEventReceiver.Removed (newOrder1OrderItems, newOrderItem1);

        newOrderItem1EventReceiver.RelationChanged (newOrderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), newOrder1, null);
        extension.RelationChanged (
            TestableClientTransaction,
            newOrderItem1,
            GetEndPointDefinition (typeof (OrderItem), "Order"),
            newOrder1,
            null);


        //11
        //newOrderItem1.Order = newOrder2;
        extension.RelationChanging (
            TestableClientTransaction,
            newOrderItem1,
            GetEndPointDefinition (typeof (OrderItem), "Order"),
            null,
            newOrder2);
        newOrderItem1EventReceiver.RelationChanging (newOrderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), null, newOrder2);

        newOrder2OrderItemsEventReceiver.Adding (newOrder2OrderItems, newOrderItem1);
        extension.RelationChanging (
            TestableClientTransaction,
            newOrder2,
            GetEndPointDefinition (typeof (Order), "OrderItems"),
            null,
            newOrderItem1);
        newOrder2EventReceiver.RelationChanging (newOrder2, GetEndPointDefinition (typeof (Order), "OrderItems"), null, newOrderItem1);

        newOrder2EventReceiver.RelationChanged (newOrder2, GetEndPointDefinition (typeof (Order), "OrderItems"), null, newOrderItem1);
        extension.RelationChanged (
            TestableClientTransaction,
            newOrder2,
            GetEndPointDefinition (typeof (Order), "OrderItems"),
            null,
            newOrderItem1);
        newOrder2OrderItemsEventReceiver.Added (newOrder2OrderItems, newOrderItem1);

        newOrderItem1EventReceiver.RelationChanged (newOrderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), null, newOrder2);
        extension.RelationChanged (
            TestableClientTransaction,
            newOrderItem1,
            GetEndPointDefinition (typeof (OrderItem), "Order"),
            null,
            newOrder2);


        //12
        //newOrder1.Official = official2;
        extension.RelationChanging (
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition (typeof (Order), "Official"),
            null,
            official2);
        newOrder1EventReceiver.RelationChanging (newOrder1, GetEndPointDefinition (typeof (Order), "Official"), null, official2);

        official2OrdersEventReceiver.Adding (official2Orders, newOrder1);
        extension.RelationChanging (
            TestableClientTransaction,
            official2,
            GetEndPointDefinition (typeof (Official), "Orders"),
            null,
            newOrder1);
        official2EventReceiver.RelationChanging (official2, GetEndPointDefinition (typeof (Official), "Orders"), null, newOrder1);


        official2EventReceiver.RelationChanged (official2, GetEndPointDefinition (typeof (Official), "Orders"), null, newOrder1);
        extension.RelationChanged (
            TestableClientTransaction,
            official2,
            GetEndPointDefinition (typeof (Official), "Orders"),
            null,
            newOrder1);
        official2OrdersEventReceiver.Added (official2Orders, newOrder1);

        newOrder1EventReceiver.RelationChanged (newOrder1, GetEndPointDefinition (typeof (Order), "Official"), null, official2);
        extension.RelationChanged (
            TestableClientTransaction,
            newOrder1,
            GetEndPointDefinition (typeof (Order), "Official"),
            null,
            official2);


        //13
        //OrderTicket newOrderTicket1 = OrderTicket.NewObject (newOrder1);

        extension.NewObjectCreating (TestableClientTransaction, typeof (OrderTicket));

        extension.RelationChanging (
            Arg.Is (ClientTransactionScope.CurrentTransaction),
            Arg<OrderTicket>.Is.TypeOf,
            Arg.Is (GetEndPointDefinition (typeof (OrderTicket), "Order")),
            Arg.Is ((DomainObject) null),
            Arg.Is (newOrder1));
        
        extension.RelationChanging (
            Arg.Is (ClientTransactionScope.CurrentTransaction),
            Arg.Is (newOrder1),
            Arg.Is (GetEndPointDefinition (typeof (Order), "OrderTicket")),
            Arg.Is ((DomainObject) null),
            Arg<OrderTicket>.Is.TypeOf);
        newOrder1EventReceiver.RelationChanging (
            Arg.Is (newOrder1),
            Arg<RelationChangingEventArgs>.Matches (
                args =>
                args.RelationEndPointDefinition == GetEndPointDefinition (typeof (Order), "OrderTicket")
                && args.OldRelatedObject == null
                && args.NewRelatedObject is OrderTicket));

        newOrder1EventReceiver.RelationChanged (Arg.Is (newOrder1),
            Arg<RelationChangedEventArgs>.Matches (
                args =>
                args.RelationEndPointDefinition == GetEndPointDefinition (typeof (Order), "OrderTicket")
                && args.OldRelatedObject == null
                && args.NewRelatedObject is OrderTicket));
        extension.RelationChanged (
            Arg.Is (ClientTransactionScope.CurrentTransaction),
            Arg.Is (newOrder1),
            Arg.Is (GetEndPointDefinition (typeof (Order), "OrderTicket")),
            Arg.Is ((DomainObject) null),
            Arg<OrderTicket>.Is.TypeOf);
        
        extension.RelationChanged (
            Arg.Is (ClientTransactionScope.CurrentTransaction),
            Arg<OrderTicket>.Is.TypeOf,
            Arg.Is (GetEndPointDefinition (typeof (OrderTicket), "Order")),
            Arg.Is ((DomainObject) null),
            Arg.Is (newOrder1));
      }

      extension.Stub (stub => stub.Key).Return ("Extension");
      mockRepository.ReplayAll();

      ClientTransactionScope.CurrentTransaction.Extensions.Add (extension);
      try
      {
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
        OrderTicket newOrderTicket1 = OrderTicket.NewObject (newOrder1);

        mockRepository.VerifyAll ();

        BackToRecord (
            mockRepository,
            extension,
            newCustomer1EventReceiver,
            newCustomer2EventReceiver,
            official2EventReceiver,
            newCeo1EventReceiver,
            newCeo2EventReceiver,
            newOrder1EventReceiver,
            newOrder2EventReceiver,
            newOrderItem1EventReceiver,
            newOrderItem2EventReceiver,
            newCustomer1OrdersEventReceiver,
            newCustomer2OrdersEventReceiver,
            official2OrdersEventReceiver,
            newOrder1OrderItemsEventReceiver,
            newOrder2OrderItemsEventReceiver);

        var newOrderTicket1EventReceiver = mockRepository.StrictMock<DomainObjectMockEventReceiver> (newOrderTicket1);

        using (mockRepository.Ordered ())
        {
          //14
          //newOrderTicket1.Order = newOrder2;
          extension.RelationChanging (
              TestableClientTransaction,
              newOrderTicket1,
              GetEndPointDefinition (typeof (OrderTicket), "Order"),
              newOrder1,
              newOrder2);
          newOrderTicket1EventReceiver.RelationChanging (newOrderTicket1, GetEndPointDefinition (typeof (OrderTicket), "Order"), newOrder1, newOrder2);

          extension.RelationChanging (
              TestableClientTransaction,
              newOrder1,
              GetEndPointDefinition (typeof (Order), "OrderTicket"),
              newOrderTicket1,
              null);
          newOrder1EventReceiver.RelationChanging (newOrder1, GetEndPointDefinition (typeof (Order), "OrderTicket"), newOrderTicket1, null);

          extension.RelationChanging (
              TestableClientTransaction,
              newOrder2,
              GetEndPointDefinition (typeof (Order), "OrderTicket"),
              null,
              newOrderTicket1);
          newOrder2EventReceiver.RelationChanging (newOrder2, GetEndPointDefinition (typeof (Order), "OrderTicket"), null, newOrderTicket1);

          newOrder2EventReceiver.RelationChanged (newOrder2, GetEndPointDefinition (typeof (Order), "OrderTicket"), null, newOrderTicket1);
          extension.RelationChanged (
              TestableClientTransaction,
              newOrder2,
              GetEndPointDefinition (typeof (Order), "OrderTicket"),
              null,
              newOrderTicket1);

          newOrder1EventReceiver.RelationChanged (newOrder1, GetEndPointDefinition (typeof (Order), "OrderTicket"), newOrderTicket1, null);
          extension.RelationChanged (
              TestableClientTransaction,
              newOrder1,
              GetEndPointDefinition (typeof (Order), "OrderTicket"),
              newOrderTicket1,
              null);

          newOrderTicket1EventReceiver.RelationChanged (newOrderTicket1, GetEndPointDefinition (typeof (OrderTicket), "Order"), newOrder1, newOrder2);
          extension.RelationChanged (
              TestableClientTransaction,
              newOrderTicket1,
              GetEndPointDefinition (typeof (OrderTicket), "Order"),
              newOrder1,
              newOrder2);


          //15a
          //newOrder2.Customer = newCustomer1;
          extension.RelationChanging (
              TestableClientTransaction,
              newOrder2,
              GetEndPointDefinition (typeof (Order), "Customer"),
              null,
              newCustomer1);
          newOrder2EventReceiver.RelationChanging (newOrder2, GetEndPointDefinition (typeof (Order), "Customer"), null, newCustomer1);

          newCustomer1OrdersEventReceiver.Adding (newCustomer1Orders, newOrder2);
          extension.RelationChanging (
              TestableClientTransaction,
              newCustomer1,
              GetEndPointDefinition (typeof (Customer), "Orders"),
              null,
              newOrder2);
          newCustomer1EventReceiver.RelationChanging (newCustomer1, GetEndPointDefinition (typeof (Customer), "Orders"), null, newOrder2);

          newCustomer1EventReceiver.RelationChanged (newCustomer1, GetEndPointDefinition (typeof (Customer), "Orders"), null, newOrder2);
          extension.RelationChanged (
              TestableClientTransaction,
              newCustomer1,
              GetEndPointDefinition (typeof (Customer), "Orders"),
              null,
              newOrder2);
          newCustomer1OrdersEventReceiver.Added (newCustomer1Orders, newOrder2);

          newOrder2EventReceiver.RelationChanged (newOrder2, GetEndPointDefinition (typeof (Order), "Customer"), null, newCustomer1);
          extension.RelationChanged (
              TestableClientTransaction,
              newOrder2,
              GetEndPointDefinition (typeof (Order), "Customer"),
              null,
              newCustomer1);


          //15b
          //newOrder2.Customer = newCustomer2;
          extension.RelationChanging (
              TestableClientTransaction,
              newOrder2,
              GetEndPointDefinition (typeof (Order), "Customer"),
              newCustomer1,
              newCustomer2);
          newOrder2EventReceiver.RelationChanging (newOrder2, GetEndPointDefinition (typeof (Order), "Customer"), newCustomer1, newCustomer2);

          newCustomer2OrdersEventReceiver.Adding (newCustomer2Orders, newOrder2);
          extension.RelationChanging (
              TestableClientTransaction,
              newCustomer2,
              GetEndPointDefinition (typeof (Customer), "Orders"),
              null,
              newOrder2);
          newCustomer2EventReceiver.RelationChanging (newCustomer2, GetEndPointDefinition (typeof (Customer), "Orders"), null, newOrder2);

          newCustomer1OrdersEventReceiver.Removing (newCustomer1Orders, newOrder2);
          extension.RelationChanging (
              TestableClientTransaction,
              newCustomer1,
              GetEndPointDefinition (typeof (Customer), "Orders"),
              newOrder2,
              null);
          newCustomer1EventReceiver.RelationChanging (newCustomer1, GetEndPointDefinition (typeof (Customer), "Orders"), newOrder2, null);

          newCustomer1EventReceiver.RelationChanged (newCustomer1, GetEndPointDefinition (typeof (Customer), "Orders"), newOrder2, null);
          extension.RelationChanged (
              TestableClientTransaction, newCustomer1, GetEndPointDefinition (typeof (Customer), "Orders"), newOrder2, null);
          newCustomer1OrdersEventReceiver.Removed (newCustomer1Orders, newOrder2);

          newCustomer2EventReceiver.RelationChanged (newCustomer2, GetEndPointDefinition (typeof (Customer), "Orders"), null, newOrder2);
          extension.RelationChanged (
              TestableClientTransaction, newCustomer2, GetEndPointDefinition (typeof (Customer), "Orders"), null, newOrder2);
          newCustomer2OrdersEventReceiver.Added (newCustomer2Orders, newOrder2);

          newOrder2EventReceiver.RelationChanged (newOrder2, GetEndPointDefinition (typeof (Order), "Customer"), newCustomer1, newCustomer2);
          extension.RelationChanged (
              TestableClientTransaction, newOrder2, GetEndPointDefinition (typeof (Order), "Customer"), newCustomer1, newCustomer2);


          //16
          //newOrder2.Delete ();
          extension.ObjectDeleting (TestableClientTransaction, newOrder2);
          newOrder2EventReceiver.Deleting (Arg.Is (newOrder2), Arg<EventArgs>.Is.NotNull);

          using (mockRepository.Unordered ())
          {
            newCustomer2OrdersEventReceiver.Removing (newCustomer2Orders, newOrder2);
            extension.RelationChanging (
                TestableClientTransaction,
                newCustomer2,
                GetEndPointDefinition (typeof (Customer), "Orders"),
                newOrder2,
                null);
            newCustomer2EventReceiver.RelationChanging (newCustomer2, GetEndPointDefinition (typeof (Customer), "Orders"), newOrder2, null);

            extension.RelationChanging (
                TestableClientTransaction,
                newOrderTicket1,
                GetEndPointDefinition (typeof (OrderTicket), "Order"),
                newOrder2,
                null);
            newOrderTicket1EventReceiver.RelationChanging (newOrderTicket1, GetEndPointDefinition (typeof (OrderTicket), "Order"), newOrder2, null);

            extension.RelationChanging (
                TestableClientTransaction,
                newOrderItem1,
                GetEndPointDefinition (typeof (OrderItem), "Order"),
                newOrder2,
                null);
            newOrderItem1EventReceiver.RelationChanging (newOrderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), newOrder2, null);
          }

          using (mockRepository.Unordered ())
          {
            newCustomer2EventReceiver.RelationChanged (newCustomer2, GetEndPointDefinition (typeof (Customer), "Orders"), newOrder2, null);
            extension.RelationChanged (
                TestableClientTransaction, newCustomer2, GetEndPointDefinition (typeof (Customer), "Orders"), newOrder2, null);
            newCustomer2OrdersEventReceiver.Removed (newCustomer2Orders, newOrder2);

            newOrderTicket1EventReceiver.RelationChanged (newOrderTicket1, GetEndPointDefinition (typeof (OrderTicket), "Order"), newOrder2, null);
            extension.RelationChanged (
                TestableClientTransaction,
                newOrderTicket1,
                GetEndPointDefinition (typeof (OrderTicket), "Order"), newOrder2, null);

            newOrderItem1EventReceiver.RelationChanged (newOrderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), newOrder2, null);
            extension.RelationChanged (
                TestableClientTransaction, newOrderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), newOrder2, null);
          }

          newOrder2EventReceiver.Deleted (Arg.Is (newOrder2), Arg<EventArgs>.Is.NotNull);
          extension.ObjectDeleted (TestableClientTransaction, newOrder2);


          //17
          //newOrderTicket1.Order = newOrder1;
          extension.RelationChanging (
              TestableClientTransaction,
              newOrderTicket1,
              GetEndPointDefinition (typeof (OrderTicket), "Order"),
              null,
              newOrder1);
          newOrderTicket1EventReceiver.RelationChanging (newOrderTicket1, GetEndPointDefinition (typeof (OrderTicket), "Order"), null, newOrder1);

          extension.RelationChanging (
              TestableClientTransaction,
              newOrder1,
              GetEndPointDefinition (typeof (Order), "OrderTicket"),
              null,
              newOrderTicket1);
          newOrder1EventReceiver.RelationChanging (newOrder1, GetEndPointDefinition (typeof (Order), "OrderTicket"), null, newOrderTicket1);

          newOrder1EventReceiver.RelationChanged (newOrder1, GetEndPointDefinition (typeof (Order), "OrderTicket"), null, newOrderTicket1);
          extension.RelationChanged (
              TestableClientTransaction, newOrder1, GetEndPointDefinition (typeof (Order), "OrderTicket"), null, newOrderTicket1);

          newOrderTicket1EventReceiver.RelationChanged (newOrderTicket1, GetEndPointDefinition (typeof (OrderTicket), "Order"), null, newOrder1);
          extension.RelationChanged (
              TestableClientTransaction,
              newOrderTicket1,
              GetEndPointDefinition (typeof (OrderTicket), "Order"), null, newOrder1);


          //cleanup for commit
          //18
          //newCustomer2.Delete ();
          extension.ObjectDeleting (TestableClientTransaction, newCustomer2);
          newCustomer2EventReceiver.Deleting (Arg.Is (newCustomer2), Arg<EventArgs>.Is.NotNull);
          
          newCustomer2EventReceiver.Deleted (Arg.Is (newCustomer2), Arg<EventArgs>.Is.NotNull);
          extension.ObjectDeleted (TestableClientTransaction, newCustomer2);


          //19
          //newCeo1.Delete ();
          extension.ObjectDeleting (TestableClientTransaction, newCeo1);
          newCeo1EventReceiver.Deleting (Arg.Is (newCeo1), Arg<EventArgs>.Is.NotNull);

          newCeo1EventReceiver.Deleted (Arg.Is (newCeo1), Arg<EventArgs>.Is.NotNull);
          extension.ObjectDeleted (TestableClientTransaction, newCeo1);


          //20
          //newOrderItem1.Delete ();
          extension.ObjectDeleting (TestableClientTransaction, newOrderItem1);
          newOrderItem1EventReceiver.Deleting (Arg.Is (newOrderItem1), Arg<EventArgs>.Is.NotNull);

          newOrderItem1EventReceiver.Deleted (Arg.Is (newOrderItem1), Arg<EventArgs>.Is.NotNull);
          extension.ObjectDeleted (TestableClientTransaction, newOrderItem1);


          //21
          //ClientTransactionScope.CurrentTransaction.Commit ();
          using (mockRepository.Unordered ())
          {
            newCustomer1EventReceiver.Committing (null, null);
            LastCall.Constraints (Mocks_Is.Same (newCustomer1), Mocks_Is.NotNull ());

            official2EventReceiver.Committing (null, null);
            LastCall.Constraints (Mocks_Is.Same (official2), Mocks_Is.NotNull ());

            newCeo2EventReceiver.Committing (null, null);
            LastCall.Constraints (Mocks_Is.Same (newCeo2), Mocks_Is.NotNull ());

            newOrder1EventReceiver.Committing (null, null);
            LastCall.Constraints (Mocks_Is.Same (newOrder1), Mocks_Is.NotNull ());

            newOrderItem2EventReceiver.Committing (null, null);
            LastCall.Constraints (Mocks_Is.Same (newOrderItem2), Mocks_Is.NotNull ());

            newOrderTicket1EventReceiver.Committing (null, null);
            LastCall.Constraints (Mocks_Is.Same (newOrderTicket1), Mocks_Is.NotNull ());
          }
          extension.Committing (null, null);
          LastCall.Constraints (
              Mocks_Is.Same (ClientTransactionScope.CurrentTransaction),
              new ContainsConstraint (newCustomer1, official2, newCeo2, newOrder1, newOrderItem2, newOrderTicket1));

          extension.CommitValidate (
              Arg.Is (ClientTransactionScope.CurrentTransaction),
              Arg<ReadOnlyCollection<PersistableData>>.Matches (
                  collection => collection.Select (c => c.DomainObject)
                      .SetEquals (new DomainObject[] { newCustomer1, official2, newCeo2, newOrder1, newOrderItem2, newOrderTicket1 })));

          using (mockRepository.Unordered ())
          {
            newCustomer1EventReceiver.Committed (null, null);
            LastCall.Constraints (Mocks_Is.Same (newCustomer1), Mocks_Is.Anything ());

            official2EventReceiver.Committed (null, null);
            LastCall.Constraints (Mocks_Is.Same (official2), Mocks_Is.NotNull ());

            newCeo2EventReceiver.Committed (null, null);
            LastCall.Constraints (Mocks_Is.Same (newCeo2), Mocks_Is.NotNull ());

            newOrder1EventReceiver.Committed (null, null);
            LastCall.Constraints (Mocks_Is.Same (newOrder1), Mocks_Is.NotNull ());

            newOrderItem2EventReceiver.Committed (null, null);
            LastCall.Constraints (Mocks_Is.Same (newOrderItem2), Mocks_Is.NotNull ());

            newOrderTicket1EventReceiver.Committed (null, null);
            LastCall.Constraints (Mocks_Is.Same (newOrderTicket1), Mocks_Is.NotNull ());
          }
          extension.Committed (null, null);
          LastCall.Constraints (
              Mocks_Is.Same (ClientTransactionScope.CurrentTransaction),
              Mocks_Property.Value ("Count", 6) & new ContainsConstraint (newCustomer1, official2, newCeo2, newOrder1, newOrderItem2, newOrderTicket1));
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
        ClientTransactionScope.CurrentTransaction.Commit ();

        mockRepository.VerifyAll ();
      }
      finally
      {
        ClientTransactionScope.CurrentTransaction.Extensions.Remove ("Extension");
      }
    }

    [Test]
    public void SetValuesAndAccessOriginalValuesTest ()
    {
      OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

      DataContainer dataContainer = orderItem.InternalDataContainer;

      var propertyDefinition = GetPropertyDefinition (typeof (OrderItem), "Product");
      dataContainer.SetValue (propertyDefinition, "newProduct");

      Assert.AreNotEqual ("newProduct", dataContainer.GetValue (propertyDefinition, ValueAccess.Original));
      Assert.AreEqual ("newProduct", orderItem.Product);

      TestableClientTransaction.Commit();
      orderItem.Product = "newProduct2";

      Assert.AreEqual ("newProduct", dataContainer.GetValue (propertyDefinition, ValueAccess.Original));
      Assert.AreEqual ("newProduct2", orderItem.Product);
    }

    [Test]
    public void NewCustomerAndCEOTest ()
    {
      IndustrialSector industrialSector = IndustrialSector.NewObject();
      Customer customer = Customer.NewObject();
      customer.Ceo = Ceo.NewObject();

      industrialSector.Companies.Add (customer);

      Order order1 = Order.NewObject();
      OrderTicket.NewObject (order1);

      //getting an SQL Exception without this line
      order1.DeliveryDate = DateTime.Now;

      OrderItem orderItem = OrderItem.NewObject();
      order1.OrderItems.Add (orderItem);
      order1.Official = Official.GetObject (DomainObjectIDs.Official2);
      customer.Orders.Add (order1);

      Assert.That (() => TestableClientTransaction.Commit (), Throws.Nothing);

      customer.Delete();

      Assert.That (() => TestableClientTransaction.Commit (), Throws.TypeOf<MandatoryRelationNotSetException> ());
    }

    [Test]
    public void InsertComputerAndEmployee ()
    {
      Computer computer = Computer.NewObject();
      computer.Employee = Employee.NewObject();
      computer.SerialNumber = "12345";
      computer.Employee.Name = "ABCDE";

      TestableClientTransaction.Commit();

      computer.Employee.Delete();
      computer.Delete();

      TestableClientTransaction.Commit();
    }

    [Test]
    public void PropertyEventsOfNewObjectPropertyChangeTest ()
    {
      Order newOrder = Order.NewObject();

      InitializeEventReceivers (newOrder);
      CheckNoEvents();

      newOrder.DeliveryDate = DateTime.Now;

      CheckEvents (_orderDeliveryDateProperty);
    }

    [Test]
    public void PropertyEventsOfNewObjectRelationChangeTest ()
    {
      Order newOrder = Order.NewObject();

      InitializeEventReceivers (newOrder);
      CheckNoEvents();

      newOrder.Customer = null;

      CheckNoEvents();
    }

    [Test]
    public void PropertyEventsOfExistingObjectPropertyChangeTest ()
    {
      Order order2 = Order.GetObject (DomainObjectIDs.Order2);

      InitializeEventReceivers (order2);
      CheckNoEvents();

      order2.DeliveryDate = DateTime.Now;

      CheckEvents (_orderDeliveryDateProperty);
    }

    [Test]
    public void PropertyEventsOfExistingObjectRelationChangeTest ()
    {
      Order order2 = Order.GetObject (DomainObjectIDs.Order2);

      InitializeEventReceivers (order2);
      CheckNoEvents();

      order2.Customer = null;

      CheckNoEvents();
    }

    [Test]
    public void SaveObjectWithNonMandatoryOneToManyRelation ()
    {
      Customer newCustomer = Customer.NewObject();
      newCustomer.Ceo = Ceo.NewObject();

      Customer existingCustomer = Customer.GetObject (DomainObjectIDs.Customer3);
      Assert.AreEqual (1, existingCustomer.Orders.Count);
      Assert.IsNotNull (existingCustomer.Orders[0].OrderTicket);
      Assert.AreEqual (1, existingCustomer.Orders[0].OrderItems.Count);

      existingCustomer.Orders[0].OrderTicket.Delete();
      existingCustomer.Orders[0].OrderItems[0].Delete();
      existingCustomer.Orders[0].Delete();

      ClientTransactionScope.CurrentTransaction.Commit();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        newCustomer = Customer.GetObject (newCustomer.ID);
        existingCustomer = Customer.GetObject (DomainObjectIDs.Customer3);

        Assert.AreEqual (0, newCustomer.Orders.Count);
        Assert.AreEqual (0, existingCustomer.Orders.Count);
      }
    }

    private void InitializeEventReceivers (Order order)
    {
      _orderDataContainer = order.InternalDataContainer;
      _orderPropertyValues = _orderDataContainer.PropertyValues;
      _orderDeliveryDateProperty = _orderPropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.DeliveryDate"];

      _orderDomainObjectEventReceiver = new DomainObjectEventReceiver (order);
    }

    private void CheckNoEvents ()
    {
      Assert.IsFalse (_orderDomainObjectEventReceiver.HasChangingEventBeenCalled);
      Assert.IsFalse (_orderDomainObjectEventReceiver.HasChangedEventBeenCalled);
      Assert.IsNull (_orderDomainObjectEventReceiver.ChangingPropertyValue);
      Assert.IsNull (_orderDomainObjectEventReceiver.ChangedPropertyValue);
    }

    private void CheckEvents (PropertyValue propertyValue)
    {
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