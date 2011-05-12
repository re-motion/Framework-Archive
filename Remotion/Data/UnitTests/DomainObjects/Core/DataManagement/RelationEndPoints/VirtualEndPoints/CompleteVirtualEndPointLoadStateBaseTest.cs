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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints.VirtualEndPoints
{
  [TestFixture]
  public class CompleteVirtualEndPointLoadStateBaseTest : StandardMappingTest
  {
    private IVirtualEndPoint<object> _virtualEndPointMock;
    private IVirtualEndPointDataKeeper _dataKeeperMock;
    private IRelationEndPointProvider _endPointProviderStub;
    private ClientTransaction _clientTransaction;

    private TestableCompleteVirtualEndPointLoadState _loadState;

    private IRelationEndPointDefinition _definition;
    private Order _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _definition = Configuration.GetTypeDefinition (typeof (Customer)).GetRelationEndPointDefinition (typeof (Customer).FullName + ".Orders");

      _virtualEndPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint<object>> ();
      _dataKeeperMock = MockRepository.GenerateStrictMock<IVirtualEndPointDataKeeper>();
      _dataKeeperMock.Stub (stub => stub.EndPointID).Return (RelationEndPointID.Create (DomainObjectIDs.Customer1, _definition));
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();
      _clientTransaction = ClientTransaction.CreateRootTransaction();

      _loadState = new TestableCompleteVirtualEndPointLoadState (_dataKeeperMock, _endPointProviderStub, _clientTransaction);

      _relatedObject = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
      _relatedEndPointStub.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer"));
      _relatedEndPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_relatedObject);
      _relatedEndPointStub.Stub (stub => stub.ObjectID).Return (_relatedObject.ID);
    }

    [Test]
    public void IsDataComplete ()
    {
      Assert.That (_loadState.IsDataComplete(), Is.True);
    }

    [Test]
    public void EnsureDataComplete_DoesNothing ()
    {
      _virtualEndPointMock.Replay();
      _dataKeeperMock.Replay();

      _loadState.EnsureDataComplete (_virtualEndPointMock);

      _virtualEndPointMock.VerifyAllExpectations();
      _dataKeeperMock.VerifyAllExpectations();
    }

    [Test]
    public void CanDataBeMarkedIncomplete_True ()
    {
      _virtualEndPointMock.Replay();
      _dataKeeperMock.Stub (stub => stub.HasDataChanged()).Return (false);
      _dataKeeperMock.Replay();

      Assert.That (_loadState.CanDataBeMarkedIncomplete (_virtualEndPointMock), Is.True);
    }

    [Test]
    public void CanDataBeMarkedIncomplete_False ()
    {
      _virtualEndPointMock.Replay ();
      _dataKeeperMock.Stub (stub => stub.HasDataChanged ()).Return (true);
      _dataKeeperMock.Replay ();

      Assert.That (_loadState.CanDataBeMarkedIncomplete (_virtualEndPointMock), Is.False);
    }

    [Test]
    public void MarkDataIncomplete_RaisesEvent ()
    {
      _virtualEndPointMock
          .Stub (stub => stub.ID)
          .Return (RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems"));
      _virtualEndPointMock.Replay();

      _dataKeeperMock.Stub (stub => stub.HasDataChanged ()).Return (false);
      _dataKeeperMock.Replay ();

      _loadState.StubOriginalOppositeEndPoints (new IRealObjectEndPoint[0]);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_clientTransaction);

      _loadState.MarkDataIncomplete (_virtualEndPointMock, () => { });

      _virtualEndPointMock.VerifyAllExpectations();
      _dataKeeperMock.VerifyAllExpectations();

      listenerMock.AssertWasCalled (mock => mock.RelationEndPointUnloading (_clientTransaction, _virtualEndPointMock));
    }

    [Test]
    public void MarkDataIncomplete_ExecutesStateSetter_AndSynchronizesOppositeEndPoints ()
    {
      // ReSharper disable AccessToModifiedClosure
      bool stateSetterCalled = false;

      var synchronizedOppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _loadState.StubOriginalOppositeEndPoints (new[] { synchronizedOppositeEndPointStub });

      var unsynchronizedOppositeEndPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      unsynchronizedOppositeEndPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      AddUnsynchronizedOppositeEndPoint (_loadState, unsynchronizedOppositeEndPointMock);
      unsynchronizedOppositeEndPointMock.Replay ();

      _virtualEndPointMock
          .Stub (stub => stub.ID)
          .Return (RelationEndPointID.Create (DomainObjectIDs.Customer1, _definition));
      _virtualEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (synchronizedOppositeEndPointStub))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      _virtualEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (unsynchronizedOppositeEndPointMock))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      _virtualEndPointMock.Replay();

      _dataKeeperMock.Stub (stub => stub.HasDataChanged ()).Return (false);
      _dataKeeperMock.Replay ();

      _loadState.MarkDataIncomplete (_virtualEndPointMock, () => stateSetterCalled = true);

      _virtualEndPointMock.VerifyAllExpectations();
      unsynchronizedOppositeEndPointMock.VerifyAllExpectations ();
      _dataKeeperMock.VerifyAllExpectations();

      Assert.That (stateSetterCalled, Is.True);
      // ReSharper restore AccessToModifiedClosure
    }

    [Test]
    public void MarkDataIncomplete_Throws_WithChangedData ()
    {
      _virtualEndPointMock
          .Stub (stub => stub.ID)
          .Return (RelationEndPointID.Create (DomainObjectIDs.Customer1, _definition));
      _virtualEndPointMock.Replay ();

      _dataKeeperMock.Stub (stub => stub.HasDataChanged ()).Return (true);
      _dataKeeperMock.Replay ();

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (_clientTransaction);

      Assert.That (
          () =>_loadState.MarkDataIncomplete (_virtualEndPointMock, () => Assert.Fail ("Must not be called.")),
          Throws.InvalidOperationException.With.Message.EqualTo (
          "Cannot mark virtual end-point "
          + "'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders' incomplete "
          + "because it has been changed."));
    }

    [Test]
    public void CanEndPointBeCollected ()
    {
      var result = _loadState.CanEndPointBeCollected (_virtualEndPointMock);
      Assert.That (result, Is.False);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_WithoutExistingItem ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint>();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Expect (mock => mock.MarkUnsynchronized());
      endPointMock.Replay();

      _dataKeeperMock.Stub (stub => stub.ContainsOriginalObjectID (DomainObjectIDs.Order1)).Return (false);
      _dataKeeperMock.Replay();

      _loadState.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);

      _dataKeeperMock.AssertWasNotCalled (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      endPointMock.VerifyAllExpectations();
      _dataKeeperMock.VerifyAllExpectations();

      Assert.That (_loadState.UnsynchronizedOppositeEndPoints, Has.Member(endPointMock));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_WithExistingItem ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint>();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Expect (mock => mock.MarkSynchronized());
      endPointMock.Replay();

      _dataKeeperMock.Stub (stub => stub.ContainsOriginalObjectID (DomainObjectIDs.Order1)).Return (true);
      _dataKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      _dataKeeperMock.Replay();

      _loadState.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);

      endPointMock.VerifyAllExpectations();
      _dataKeeperMock.VerifyAllExpectations();
      Assert.That (_loadState.UnsynchronizedOppositeEndPoints, Has.No.Member(endPointMock));
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      using (_virtualEndPointMock.GetMockRepository().Ordered())
      {
        _virtualEndPointMock.Expect (mock => mock.MarkDataIncomplete());
        _virtualEndPointMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (_relatedEndPointStub));
      }
      _virtualEndPointMock.Replay();

      _loadState.UnregisterOriginalOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub);

      _dataKeeperMock.VerifyAllExpectations();
      _virtualEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_InUnsyncedList ()
    {
      AddUnsynchronizedOppositeEndPoint (_loadState, _relatedEndPointStub);

      _virtualEndPointMock.Replay();
      _dataKeeperMock.Replay();

      _loadState.UnregisterOriginalOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub);

      _dataKeeperMock.VerifyAllExpectations();
      _virtualEndPointMock.AssertWasNotCalled (mock => mock.MarkDataIncomplete());
      _virtualEndPointMock.AssertWasNotCalled (mock => mock.UnregisterOriginalOppositeEndPoint (_relatedEndPointStub));
      Assert.That (_loadState.UnsynchronizedOppositeEndPoints, Has.No.Member(_relatedEndPointStub));
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _relatedEndPointStub.Stub (stub => stub.IsSynchronized).Return (true);

      _dataKeeperMock.Expect (mock => mock.RegisterCurrentOppositeEndPoint (_relatedEndPointStub));
      _dataKeeperMock.Replay();

      _loadState.RegisterCurrentOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub);

      _dataKeeperMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot register end-points that are out-of-sync.")]
    public void RegisterCurrentOppositeEndPoint_WithOutOfSyncEndPoint ()
    {
      _relatedEndPointStub.Stub (stub => stub.IsSynchronized).Return (false);

      _loadState.RegisterCurrentOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub);
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _dataKeeperMock.Expect (mock => mock.UnregisterCurrentOppositeEndPoint (_relatedEndPointStub));
      _dataKeeperMock.Replay();

      _loadState.UnregisterCurrentOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub);

      _dataKeeperMock.VerifyAllExpectations();
    }

    [Test]
    public void IsSynchronized_True ()
    {
      _loadState.StubOriginalItemsWithoutEndPoints (new DomainObject[0]);

      var result = _loadState.IsSynchronized (_virtualEndPointMock);

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsSynchronized_False ()
    {
      _loadState.StubOriginalItemsWithoutEndPoints (new DomainObject[] { _relatedObject });

      var result = _loadState.IsSynchronized (_virtualEndPointMock);

      Assert.That (result, Is.False);
    }

    [Test]
    public void Synchronize ()
    {
      _loadState.StubOriginalItemsWithoutEndPoints (new[] { _relatedObject });

      _dataKeeperMock.Expect (mock => mock.UnregisterOriginalItemWithoutEndPoint (_relatedObject));
      _dataKeeperMock.Replay ();

      _loadState.Synchronize (_virtualEndPointMock);

      _dataKeeperMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetUnsynchronizedOppositeEndPoints_Empty ()
    {
      var result = _loadState.UnsynchronizedOppositeEndPoints;

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void SynchronizeOppositeEndPoint_InList ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint>();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Stub (mock => mock.MarkUnsynchronized());
      endPointMock.Expect (mock => mock.MarkSynchronized());
      endPointMock.Replay();

      _dataKeeperMock.Stub (stub => stub.ContainsOriginalObjectID (DomainObjectIDs.Order1)).Return (false);
      _dataKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      _dataKeeperMock.Replay();

      _loadState.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);
      Assert.That (_loadState.UnsynchronizedOppositeEndPoints, Has.Member(endPointMock));

      _loadState.SynchronizeOppositeEndPoint (_virtualEndPointMock, endPointMock);

      _dataKeeperMock.VerifyAllExpectations();
      endPointMock.VerifyAllExpectations();
      Assert.That (_loadState.UnsynchronizedOppositeEndPoints, Has.No.Member(endPointMock));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot synchronize opposite end-point "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' - the "
        + "end-point is not in the list of unsynchronized end-points.")]
    public void SynchronizeOppositeEndPoint_NotInList ()
    {
      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
      endPointStub
          .Stub (stub => stub.ID)
          .Return (RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order"));
      endPointStub
          .Stub (stub => stub.ObjectID)
          .Return (DomainObjectIDs.OrderItem1);

      _loadState.SynchronizeOppositeEndPoint (_virtualEndPointMock, endPointStub);
    }
    
    [Test]
    public void HasChanged ()
    {
      _dataKeeperMock.Expect (mock => mock.HasDataChanged()).Return (true);
      _dataKeeperMock.Replay();

      var result = _loadState.HasChanged();

      _dataKeeperMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Commit ()
    {
      _dataKeeperMock.Expect (mock => mock.Commit());
      _dataKeeperMock.Replay();

      _loadState.Commit (_virtualEndPointMock);

      _dataKeeperMock.VerifyAllExpectations();
    }

    [Test]
    public void Rollback ()
    {
      _dataKeeperMock.Expect (mock => mock.Rollback());
      _dataKeeperMock.Replay();

      _loadState.Rollback (_virtualEndPointMock);

      _dataKeeperMock.VerifyAllExpectations();
    }

    [Test]
    public void IsSynchronizedWithItem_False_EndPointNotFound ()
    {
      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (_relatedEndPointStub.ID))
          .Return (null);

      var result = _loadState.IsSynchronizedWithItem (_relatedObject.ID);

      Assert.That (result, Is.False);
    }

    [Test]
    public void IsSynchronizedWithItem_False_EndPointPointsSomewhereElse ()
    {
      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (_relatedEndPointStub.ID))
          .Return (_relatedEndPointStub);
      _relatedEndPointStub.Stub (stub => stub.OppositeObjectID).Return (null);

      var result = _loadState.IsSynchronizedWithItem (_relatedObject.ID);

      Assert.That (result, Is.False);
    }

    [Test]
    public void IsSynchronizedWithItem_True ()
    {
      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (_relatedEndPointStub.ID))
          .Return (_relatedEndPointStub);
      _relatedEndPointStub.Stub (stub => stub.OriginalOppositeObjectID).Return (_dataKeeperMock.EndPointID.ObjectID);

      var result = _loadState.IsSynchronizedWithItem (_relatedObject.ID);

      Assert.That (result, Is.True);
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var dataKeeper = new SerializableVirtualEndPointDataKeeperFake();
      var endPointProvider = new SerializableRelationEndPointProviderFake();
      var state = new TestableCompleteVirtualEndPointLoadState (dataKeeper, endPointProvider, _clientTransaction);

      var oppositeEndPoint = new SerializableRealObjectEndPointFake (null, _relatedObject);
      AddUnsynchronizedOppositeEndPoint (state, oppositeEndPoint);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.DataKeeper, Is.Not.Null);
      Assert.That (result.ClientTransaction, Is.Not.Null);
      Assert.That (result.EndPointProvider, Is.Not.Null);
      Assert.That (result.UnsynchronizedOppositeEndPoints.Count, Is.EqualTo (1));
    }

    private void AddUnsynchronizedOppositeEndPoint (
        CompleteVirtualEndPointLoadStateBase<IVirtualEndPoint<object>, object, IVirtualEndPointDataKeeper> loadState, IRealObjectEndPoint oppositeEndPoint)
    {
      var dictionary = (Dictionary<ObjectID, IRealObjectEndPoint>) PrivateInvoke.GetNonPublicField (loadState, "_unsynchronizedOppositeEndPoints");
      dictionary.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
    }
  }
}