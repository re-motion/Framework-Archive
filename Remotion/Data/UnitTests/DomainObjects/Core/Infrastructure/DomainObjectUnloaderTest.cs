// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class DomainObjectUnloaderTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _endPointID;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
    }

    [Test]
    public void UnloadCollectionEndPoint ()
    {
      EnsureEndPointLoaded (_endPointID);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].IsDataAvailable, Is.True);

      DomainObjectUnloader.UnloadCollectionEndPoint (ClientTransactionMock, _endPointID);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].IsDataAvailable, Is.False);
    }

    [Test]
    public void UnloadCollectionEndPoint_AlreadyUnloaded ()
    {
      EnsureEndPointLoaded (_endPointID);
      DomainObjectUnloader.UnloadCollectionEndPoint (ClientTransactionMock, _endPointID);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].IsDataAvailable, Is.False);

      DomainObjectUnloader.UnloadCollectionEndPoint (ClientTransactionMock, _endPointID);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].IsDataAvailable, Is.False);
    }

    [Test]
    public void UnloadCollectionEndPoint_NotLoadedYet ()
    {
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Null);

      DomainObjectUnloader.UnloadCollectionEndPoint (ClientTransactionMock, _endPointID);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The given end point ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' does not denote a CollectionEndPoint.\r\nParameter name: endPointID")]
    public void UnloadCollectionEndPoint_ObjectEndPoint ()
    {
      var objectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      EnsureEndPointLoaded (objectEndPointID);

      DomainObjectUnloader.UnloadCollectionEndPoint (ClientTransactionMock, objectEndPointID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The end point with ID 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid/"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders' has been changed. Changed end points cannot be unloaded.")]
    public void UnloadCollectionEndPoint_Changed ()
    {
      var orders = Customer.GetObject (_endPointID.ObjectID).Orders;
      orders.Clear ();

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].HasChanged, Is.True);

      DomainObjectUnloader.UnloadCollectionEndPoint (ClientTransactionMock, _endPointID);
    }

    [Test]
    public void UnloadData ()
    {
      ClientTransactionMock.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);

      DomainObjectUnloader.UnloadData (ClientTransactionMock, DomainObjectIDs.Order1);

      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' (Changed).")]
    public void UnloadData_Changed ()
    {
      ++Order.GetObject (DomainObjectIDs.Order1).OrderNumber;
      DomainObjectUnloader.UnloadData (ClientTransactionMock, DomainObjectIDs.Order1);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The following objects cannot be unloaded because they take part in a relation that has been changed: "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' "
        + "(Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems).")]
    public void UnloadData_ChangedCollection ()
    {
      OrderItem.GetObject (DomainObjectIDs.OrderItem1).Order.OrderItems.Add (OrderItem.NewObject());
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.OrderItem1].State, Is.EqualTo (StateType.Unchanged));
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[endPointID].HasChanged, Is.True);

      DomainObjectUnloader.UnloadData (ClientTransactionMock, DomainObjectIDs.OrderItem1);
    }

    [Test]
    public void UnloadCollectionEndPointAndData_UnloadsEndPointAndItems ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemsEndPoint = order.OrderItems.AssociatedEndPoint;

      var orderItem1 = orderItemsEndPoint.OppositeDomainObjects[0];
      var orderItem2 = orderItemsEndPoint.OppositeDomainObjects[1];

      DomainObjectUnloader.UnloadCollectionEndPointAndData (ClientTransactionMock, orderItemsEndPoint.ID);

      Assert.That (orderItemsEndPoint.IsDataAvailable, Is.False);
      Assert.That (orderItem1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadCollectionEndPointAndData_UnloadsEndPoint_EmptyCollection ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer2);
      var ordersEndPoint = customer.Orders.AssociatedEndPoint;

      Assert.That (ordersEndPoint.OppositeDomainObjects, Is.Empty);

      DomainObjectUnloader.UnloadCollectionEndPointAndData (ClientTransactionMock, ordersEndPoint.ID);

      Assert.That (ordersEndPoint.IsDataAvailable, Is.False);
      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given end point ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' does not denote a CollectionEndPoint.\r\nParameter name: endPointID")]
    public void UnloadCollectionEndPointAndData_ObjectEndPoint ()
    {
      var objectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      EnsureEndPointLoaded (objectEndPointID);

      DomainObjectUnloader.UnloadCollectionEndPointAndData (ClientTransactionMock, objectEndPointID);
    }

    [Test]
    public void UnloadCollectionEndPointAndData_DoesNothing_IfEndPointNotLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[endPointID], Is.Null);

      var listenerMock = new MockRepository ().StrictMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);
      listenerMock.Replay (); // no calls are expected

      DomainObjectUnloader.UnloadCollectionEndPointAndData (ClientTransactionMock, endPointID);

      listenerMock.VerifyAllExpectations ();
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[endPointID], Is.Null);
    }

    [Test]
    public void UnloadCollectionEndPointAndData_DoesNothing_IfEndPointUnloaded ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var ordersEndPoint = customer.Orders.AssociatedEndPoint;

      DomainObjectUnloader.UnloadCollectionEndPoint (ClientTransactionMock, ordersEndPoint.ID);

      Assert.That (ordersEndPoint.IsDataAvailable, Is.False);

      var listenerMock = new MockRepository ().StrictMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);
      listenerMock.Replay (); // no calls are expected

      DomainObjectUnloader.UnloadCollectionEndPointAndData (ClientTransactionMock, ordersEndPoint.ID);

      listenerMock.VerifyAllExpectations ();

      Assert.That (ordersEndPoint.IsDataAvailable, Is.False);
    }

    [Test]
    public void UnloadCollectionEndPointAndData_ThrowsAndDoesNothing_IfItemCannotBeUnloaded ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var ordersEndPoint = customer.Orders.AssociatedEndPoint;

      var order1 = (Order) ordersEndPoint.OppositeDomainObjects[0];
      var orderWithoutOrderItem = (Order) ordersEndPoint.OppositeDomainObjects[1];
      
      // this will cause the orderWithoutOrderItem to be rejected for unload; order1 won't be unloaded either although it comes before 
      // orderWithoutOrderItem
      ++orderWithoutOrderItem.OrderNumber;
      
      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderWithoutOrderItem.State, Is.EqualTo (StateType.Changed));

      try
      {
        DomainObjectUnloader.UnloadCollectionEndPointAndData (ClientTransactionMock, ordersEndPoint.ID);
        Assert.Fail ("Expected InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
        // ok
      }

      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderWithoutOrderItem.State, Is.EqualTo (StateType.Changed));
      Assert.That (ordersEndPoint.IsDataAvailable, Is.True);
    }

    private void EnsureEndPointLoaded (RelationEndPointID endPointID)
    {
      ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[endPointID], Is.Not.Null);
    }
  }
}