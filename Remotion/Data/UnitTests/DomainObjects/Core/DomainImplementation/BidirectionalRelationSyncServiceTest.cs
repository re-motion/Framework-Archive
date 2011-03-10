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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects.Mapping;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainImplementation
{
  [TestFixture]
  public class BidirectionalRelationSyncServiceTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private Order _order1;
    private RelationEndPointMap _map;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransaction.CreateRootTransaction();
      _order1 = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      
      var dataManager = ClientTransactionTestHelper.GetDataManager (_transaction);
      _map = DataManagerTestHelper.GetRelationEndPointMap (dataManager);
    }
    
    [Test]
    public void IsSynchronized_True_OneMany ()
    {
      _transaction.Execute (() => _order1.OrderItems.EnsureDataComplete());

      var orderItem = _transaction.Execute (() => _order1.OrderItems[0]);

      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (_order1, o => o.OrderItems)), Is.True);
      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (orderItem, oi => oi.Order)), Is.True);
    }

    [Test]
    public void IsSynchronized_True_OneOne ()
    {
      var orderTicket = _transaction.Execute (() => _order1.OrderTicket);

      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (_order1, o => o.OrderTicket)), Is.True);
      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (orderTicket, ot => ot.Order)), Is.True);
    }

    [Test]
    public void IsSynchronized_False_OneMany_ObjectEndPoint ()
    {
      _transaction.Execute (() => _order1.OrderItems.EnsureDataComplete ());

      var orderItem = _transaction.Execute (() => _order1.OrderItems[0]);

      SetDatabaseModifyable ();

      var newOrderItemID = CreateOrderItemAndSetOrderInOtherTransaction (_order1.ID);
      var newOrderItem = _transaction.Execute (() => OrderItem.GetObject (newOrderItemID));

      Assert.That (_transaction.Execute (() => newOrderItem.Order), Is.SameAs (_order1));
      Assert.That (_transaction.Execute (() => _order1.OrderItems), List.Not.Contains (newOrderItem));

      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (newOrderItem, oi => oi.Order)), Is.False);
      
      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (_order1, o => o.OrderItems)), Is.True);
      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (orderItem, oi => oi.Order)), Is.True);
    }

    [Test]
    public void IsSynchronized_False_OneMany_CollectionEndPoint ()
    {
      var orderItem1 = _transaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem1));
      _transaction.Execute (() => orderItem1.Order.OrderItems.EnsureDataComplete());

      var order2 = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order2));
      Assert.That (_transaction.Execute (() => orderItem1.Order), Is.Not.SameAs (order2));

      SetDatabaseModifyable ();

      SetOrderInOtherTransaction (orderItem1.ID, order2.ID);
      
      _transaction.Execute (() => order2.OrderItems.EnsureDataComplete ());

      Assert.That (_transaction.Execute (() => orderItem1.Order), Is.Not.SameAs (order2));
      Assert.That (_transaction.Execute (() => order2.OrderItems), List.Contains (orderItem1));

      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (order2, o => o.OrderItems)), Is.False);
      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (orderItem1, oi => oi.Order)), Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' of object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has not yet been loaded into the given ClientTransaction.")]
    public void IsSynchronized_RelationNotLoaded ()
    {
      BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (_order1, o => o.OrderTicket));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "BidirectionalSyncService cannot be used for unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void IsSynchronized_UnidirectionalRelationEndPoint ()
    {
      BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "BidirectionalSyncService cannot be used for unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void IsSynchronized_AnonymousRelationEndPoint ()
    {
      var locationClientEndPoint = RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client");
      var oppositeEndPoint = RelationEndPointID.Create (DomainObjectIDs.Client1, locationClientEndPoint.Definition.GetOppositeEndPointDefinition());
      BidirectionalRelationSyncService.IsSynchronized (_transaction, oppositeEndPoint);
    }

    [Test]
    public void Synchronize_WithObjectEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.OrderItem1, typeof (OrderItem), "Order");

      var oppositeEndPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      oppositeEndPointStub.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems"));

      var objectEndPointMock = MockRepository.GenerateStrictMock<IObjectEndPoint> ();
      objectEndPointMock.Stub (stub => stub.ID).Return (endPointID);
      objectEndPointMock.Stub (stub => stub.Definition).Return (endPointID.Definition);
      objectEndPointMock.Stub (stub => stub.GetOppositeRelationEndPointID ()).Return (oppositeEndPointStub.ID);
      objectEndPointMock.Expect (mock => mock.Synchronize (oppositeEndPointStub));
      objectEndPointMock.Replay ();

      RelationEndPointMapTestHelper.AddEndPoint (_map, objectEndPointMock);
      RelationEndPointMapTestHelper.AddEndPoint (_map, oppositeEndPointStub);

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);

      objectEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void Synchronize_WithCollectionEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      var oppositeEndPointMock1 = MockRepository.GenerateStrictMock<IObjectEndPoint> ();
      var oppositeEndPointMock2 = MockRepository.GenerateStrictMock<IObjectEndPoint> ();

      var collectionEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      collectionEndPointStub.Stub (stub => stub.ID).Return (endPointID);
      collectionEndPointStub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      collectionEndPointStub
          .Stub (stub => stub.GetUnsynchronizedOppositeEndPoints ())
          .Return (Array.AsReadOnly (new[] { oppositeEndPointMock1, oppositeEndPointMock2 }));

      oppositeEndPointMock1.Expect (mock => mock.Synchronize (collectionEndPointStub));
      oppositeEndPointMock1.Replay ();

      oppositeEndPointMock2.Expect (mock => mock.Synchronize (collectionEndPointStub));
      oppositeEndPointMock2.Replay ();

      RelationEndPointMapTestHelper.AddEndPoint (_map, collectionEndPointStub);

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);

      oppositeEndPointMock1.VerifyAllExpectations ();
      oppositeEndPointMock2.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
      "BidirectionalSyncService cannot be used for unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void Synchronize_UnidirectionalEndpoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client");

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "BidirectionalSyncService cannot be used for unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void Synchronize_AnonymousEndPoint ()
    {
      var locationClientEndPoint = RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client");
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Client1, locationClientEndPoint.Definition.GetOppositeEndPointDefinition ());

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' of object "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' has not yet been loaded into the given ClientTransaction.")]
    public void Synchronize_NonExistingEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.OrderItem1, typeof (OrderItem), "Order");
      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);
    }

    private ObjectID CreateOrderItemAndSetOrderInOtherTransaction (ObjectID orderID)
    {
      return DomainObjectMother.CreateObjectAndSetRelationInOtherTransaction<OrderItem, Order> (orderID, (oi, o) => oi.Order = o);
    }

    private void SetOrderInOtherTransaction (ObjectID orderItemID, ObjectID orderID)
    {
      DomainObjectMother.SetRelationInOtherTransaction<OrderItem, Order> (orderItemID, orderID, (oi, o) => oi.Order = o);
    }
  }
}