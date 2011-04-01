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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RealObjectEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RealObjectEndPoints
{
  [TestFixture]
  public class SynchronizedRealObjectEndPointSyncStateTest : StandardMappingTest
  {
    private IRealObjectEndPoint _endPointMock;
    private SynchronizedRealObjectEndPointSyncState _state;

    private Order _order;
    private Location _location;

    private IRelationEndPointDefinition _orderOrderTicketEndPointDefinition;
    private IRelationEndPointDefinition _locationClientEndPointDefinition;
    private IRelationEndPointDefinition _orderCustomerEndPointDefinition;
    private IRelationEndPointProvider _endPointProviderStub;

    private Action<ObjectID> _fakeSetter;
    private bool _fakeSetterCalled;
    private ObjectID _fakeSetterCallID;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint>();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();

      _state = new SynchronizedRealObjectEndPointSyncState (_endPointProviderStub);

      _order = DomainObjectMother.CreateFakeObject<Order> ();
      _location = DomainObjectMother.CreateFakeObject<Location> ();

      _orderOrderTicketEndPointDefinition = GetRelationEndPointDefinition(typeof (Order), "OrderTicket");
      _locationClientEndPointDefinition = GetRelationEndPointDefinition (typeof (Location), "Client");
      _orderCustomerEndPointDefinition = GetRelationEndPointDefinition (typeof (Order), "Customer");

      _fakeSetter = id => { _fakeSetterCalled = true; _fakeSetterCallID = id; };
      _fakeSetterCalled = false;
      _fakeSetterCallID = null;
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.That (_state.IsSynchronized(_endPointMock), Is.True);
    }

    [Test]
    public void Synchronize ()
    {
      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint> ();
      oppositeEndPointMock.Replay();
      
      _state.Synchronize (_endPointMock, oppositeEndPointMock);

      oppositeEndPointMock.AssertWasNotCalled(mock=>mock.SynchronizeOppositeEndPoint (_endPointMock));
    }

    [Test]
    public void CreateDeleteCommand_NonVirtualOpposite ()
    {
      var virtualDefinition = RelationEndPointObjectMother.GetEndPointDefinition (typeof (Order), "OrderTicket");

      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);
      _endPointMock.Stub (stub => stub.Definition).Return (virtualDefinition);
      _endPointMock.Replay();

      var command = (RelationEndPointModificationCommand) _state.CreateDeleteCommand (_endPointMock, _fakeSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointDeleteCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_order));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void CreateDeleteCommand_VirtualOpposite ()
    {
      var realDefinition = RelationEndPointObjectMother.GetEndPointDefinition (typeof (OrderTicket), "Order");

      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);
      _endPointMock.Stub (stub => stub.Definition).Return (realDefinition);
      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.Order1);
      _endPointMock.Replay ();

      var oldOppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint>();
      oldOppositeEndPointMock.Replay();
      _endPointProviderStub.Stub (stub => stub.GetOppositeVirtualEndPointWithLazyLoad (_endPointMock, DomainObjectIDs.Order1)).Return (oldOppositeEndPointMock);

      var command = (RelationEndPointModificationCommand) _state.CreateDeleteCommand (_endPointMock, _fakeSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointDeleteCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_order));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      
      var objectIDSetter = GetOppositeObjectIDSetter (command);
      Assert.That (objectIDSetter, Is.Not.SameAs (_fakeSetter));

      oldOppositeEndPointMock.BackToRecord();
      oldOppositeEndPointMock.Expect (mock => mock.UnregisterCurrentOppositeEndPoint (_endPointMock));
      oldOppositeEndPointMock.Replay();

      Assert.That (_fakeSetterCalled, Is.False);
      
      objectIDSetter (DomainObjectIDs.OrderTicket1);

      oldOppositeEndPointMock.VerifyAllExpectations();
      Assert.That (_fakeSetterCalled, Is.True);
      Assert.That (_fakeSetterCallID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void CreateSetCommand_Same ()
    {
      var relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      _endPointMock.Stub (stub => stub.Definition).Return (_orderOrderTicketEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (relatedObject.ID);
      _endPointMock.Stub (stub => stub.GetOppositeObject (Arg<bool>.Is.Anything)).Return (relatedObject);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (_endPointMock, relatedObject, _fakeSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_order));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.OldRelatedObject, Is.SameAs (relatedObject));
      Assert.That (command.NewRelatedObject, Is.SameAs (relatedObject));
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void CreateSetCommand_Same_Null ()
    {
      _endPointMock.Stub (stub => stub.Definition).Return (_orderOrderTicketEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (null);
      _endPointMock.Stub (stub => stub.GetOppositeObject (Arg<bool>.Is.Anything)).Return (null);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (_endPointMock, null, _fakeSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_order));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.OldRelatedObject, Is.Null);
      Assert.That (command.NewRelatedObject, Is.Null);
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void CreateSetCommand_Unidirectional ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<Client> ();
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Client> ();

      _endPointMock.Stub (stub => stub.Definition).Return (_locationClientEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_location);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (oldRelatedObject.ID);
      _endPointMock.Stub (stub => stub.GetOppositeObject (Arg<bool>.Is.Anything)).Return (oldRelatedObject);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (_endPointMock, newRelatedObject, _fakeSetter);

      Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetUnidirectionalCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.NewRelatedObject, Is.SameAs (newRelatedObject));
      Assert.That (command.OldRelatedObject, Is.SameAs (oldRelatedObject));
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void CreateSetCommand_OneOne ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket>();
      var newRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      _endPointMock.Stub (stub => stub.Definition).Return (_orderOrderTicketEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (oldRelatedObject.ID);
      _endPointMock.Stub (stub => stub.GetOppositeObject (Arg<bool>.Is.Anything)).Return (oldRelatedObject);

      var oldOppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint> ();
      oldOppositeEndPointMock.Replay ();

      var newOppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint> ();
      newOppositeEndPointMock.Replay ();

      _endPointProviderStub.Stub (stub => stub.GetOppositeVirtualEndPointWithLazyLoad (_endPointMock, oldRelatedObject.ID)).Return (oldOppositeEndPointMock);
      _endPointProviderStub.Stub (stub => stub.GetOppositeVirtualEndPointWithLazyLoad (_endPointMock, newRelatedObject.ID)).Return (newOppositeEndPointMock);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (_endPointMock, newRelatedObject, _fakeSetter);

      Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetOneOneCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.NewRelatedObject, Is.SameAs (newRelatedObject));
      Assert.That (command.OldRelatedObject, Is.SameAs (oldRelatedObject));

      var oppositeObjectIDSetter = GetOppositeObjectIDSetter (command);
      Assert.That (oppositeObjectIDSetter, Is.Not.SameAs (_fakeSetter));

      oldOppositeEndPointMock.BackToRecord();
      oldOppositeEndPointMock.Expect (mock => mock.UnregisterCurrentOppositeEndPoint (_endPointMock));
      oldOppositeEndPointMock.Replay();

      newOppositeEndPointMock.BackToRecord();
      newOppositeEndPointMock.Expect (mock => mock.RegisterCurrentOppositeEndPoint (_endPointMock));
      newOppositeEndPointMock.Replay();

      Assert.That (_fakeSetterCalled, Is.False);
      
      oppositeObjectIDSetter (DomainObjectIDs.OrderTicket1);

      oldOppositeEndPointMock.VerifyAllExpectations ();
      newOppositeEndPointMock.VerifyAllExpectations ();

      Assert.That (_fakeSetterCalled, Is.True);
      Assert.That (_fakeSetterCallID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void CreateSetCommand_OneMany ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<Customer> ();
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Customer> ();

      _endPointMock.Stub (stub => stub.Definition).Return (_orderCustomerEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (oldRelatedObject.ID);
      _endPointMock.Stub (stub => stub.GetOppositeObject (Arg<bool>.Is.Anything)).Return (oldRelatedObject);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (_endPointMock, newRelatedObject, _fakeSetter);

      Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetOneManyCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.NewRelatedObject, Is.SameAs (newRelatedObject));
      Assert.That (command.OldRelatedObject, Is.SameAs (oldRelatedObject));
      Assert.That (((ObjectEndPointSetOneManyCommand) command).EndPointProvider, Is.SameAs (_endPointProviderStub));
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var state = new SynchronizedRealObjectEndPointSyncState (new SerializableRelationEndPointProviderFake());

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.EndPointProvider, Is.Not.Null);
    }

    private IRelationEndPointDefinition GetRelationEndPointDefinition (Type classType, string shortPropertyName)
    {
      return Configuration.ClassDefinitions.GetMandatory (classType).GetRelationEndPointDefinition (classType.FullName + "." + shortPropertyName);
    }

    private Action<ObjectID> GetOppositeObjectIDSetter (RelationEndPointModificationCommand command)
    {
      return (Action<ObjectID>) PrivateInvoke.GetNonPublicField (command, "_oppositeObjectIDSetter");
    }

  }
}