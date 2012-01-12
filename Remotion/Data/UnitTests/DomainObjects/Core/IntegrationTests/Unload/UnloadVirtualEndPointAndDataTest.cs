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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Unload
{
  [TestFixture]
  public class UnloadVirtualEndPointAndDataTest : UnloadTestBase
  {
    [Test]
    public void UnloadCollectionEndPointAndData ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      Assert.That (orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadCollectionEndPointAndData (TestableClientTransaction, orderItems.AssociatedEndPointID);

      CheckDataContainerExists (order, true);
      CheckDataContainerExists (orderItem1, false);
      CheckDataContainerExists (orderItem2, false);

      CheckEndPointExists (order, "OrderItems", false);
      CheckEndPointExists (orderItem1, "Order", false);
      CheckEndPointExists (orderItem2, "Order", false);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (orderItems.IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadCollectionEndPointAndData_EnsureDataAvailable_AndComplete ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      Assert.That (orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadCollectionEndPointAndData (TestableClientTransaction, orderItems.AssociatedEndPointID);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItems.IsDataComplete, Is.False);

      orderItem1.EnsureDataAvailable ();

      Assert.That (orderItem1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItems.IsDataComplete, Is.False);

      orderItems.EnsureDataComplete ();

      Assert.That (orderItem1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItems.IsDataComplete, Is.True);
      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem1, orderItem2 }));
    }

    [Test]
    public void UnloadCollectionEndPointAndData_Reload ()
    {
      SetDatabaseModifyable ();

      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      ObjectID newOrderItemID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var orderInOtherTx = Order.GetObject (DomainObjectIDs.Order1);
        var orderItem1InOtherTx = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
        var newOrderItem = OrderItem.NewObject ();
        newOrderItemID = newOrderItem.ID;
        orderInOtherTx.OrderItems.Add (newOrderItem);
        orderInOtherTx.OrderItems.Remove (orderItem1InOtherTx);

        orderItem1InOtherTx.Order = Order.GetObject (DomainObjectIDs.Order2);

        ClientTransaction.Current.Commit ();
      }

      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem1, orderItem2 }));

      UnloadService.UnloadCollectionEndPointAndData (TestableClientTransaction, orderItems.AssociatedEndPointID);

      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem2, OrderItem.GetObject (newOrderItemID) }));
      Assert.That (orderItem1.Order, Is.SameAs (Order.GetObject (DomainObjectIDs.Order2)));
    }

    [Test]
    public void Events ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);
      using (listenerMock.GetMockRepository ().Ordered ())
      {
        listenerMock
            .Expect (mock => mock.ObjectsUnloading (
                Arg.Is (TestableClientTransaction), 
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderItemA, orderItemB })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderItemA.OnUnloadingCalled, Is.False, "items unloaded after this method is called");
              Assert.That (orderItemB.OnUnloadingCalled, Is.False, "items unloaded after this method is called");
              Assert.That (orderItemA.OnUnloadedCalled, Is.False, "items unloaded after this method is called");
              Assert.That (orderItemB.OnUnloadedCalled, Is.False, "items unloaded after this method is called");

              Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
              Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));
            });
        listenerMock
            .Expect (mock => mock.ObjectsUnloaded (
                Arg.Is (TestableClientTransaction), 
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderItemA, orderItemB })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderItemA.OnUnloadingCalled, Is.True, "items unloaded before this method is called");
              Assert.That (orderItemB.OnUnloadingCalled, Is.True, "items unloaded before this method is called");
              Assert.That (orderItemA.OnUnloadedCalled, Is.True, "items unloaded before this method is called");
              Assert.That (orderItemB.OnUnloadedCalled, Is.True, "items unloaded before this method is called");

              Assert.That (orderItemA.State, Is.EqualTo (StateType.NotLoadedYet));
              Assert.That (orderItemB.State, Is.EqualTo (StateType.NotLoadedYet));
            });
      }

      listenerMock.Replay ();

      UnloadService.UnloadCollectionEndPointAndData (TestableClientTransaction, order1.OrderItems.AssociatedEndPointID);

      listenerMock.VerifyAllExpectations ();
      listenerMock.BackToRecord(); // For Discarding

      Assert.That (orderItemA.UnloadingState, Is.EqualTo (StateType.Unchanged), "OnUnloading before state change");
      Assert.That (orderItemB.UnloadingState, Is.EqualTo (StateType.Unchanged), "OnUnloading before state change");
      Assert.That (orderItemA.OnUnloadingDateTime, Is.LessThan (orderItemB.OnUnloadingDateTime), "orderItemA.OnUnloading before orderItemB.OnUnloading");

      Assert.That (orderItemA.UnloadedState, Is.EqualTo (StateType.NotLoadedYet), "OnUnloaded after state change");
      Assert.That (orderItemB.UnloadedState, Is.EqualTo (StateType.NotLoadedYet), "OnUnloaded after state change");
      Assert.That (orderItemA.OnUnloadedDateTime, Is.GreaterThan (orderItemB.OnUnloadedDateTime), "orderItemA.OnUnloaded after orderItemB.OnUnloaded");
    }
  }
}