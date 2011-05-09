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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class DataContainerEndPointsRegistrationAgentBaseTest : StandardMappingTest
  {
    private IRelationEndPointFactory _endPointFactoryMock;
    private IRelationEndPointRegistrationAgent _registrationAgentMock;

    private TestableDataContainerEndPointsRegistrationAgentBase _agentPartialMock;

    private RelationEndPointMap _map;

    private RelationEndPointID _orderTicketEndPointID;
    private RelationEndPointID _customerEndPointID;

    private IVirtualObjectEndPoint _orderTicketEndPointStub;
    private IRealObjectEndPoint _customerEndPointStub;
    private DataContainer _orderDataContainer;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointFactoryMock = MockRepository.GenerateStrictMock<IRelationEndPointFactory>();
      _registrationAgentMock = MockRepository.GenerateStrictMock<IRelationEndPointRegistrationAgent>();

      _agentPartialMock = MockRepository.GeneratePartialMock<TestableDataContainerEndPointsRegistrationAgentBase> (
          _endPointFactoryMock, _registrationAgentMock);

      _map = new RelationEndPointMap (ClientTransaction.CreateRootTransaction());

      _orderTicketEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");
      _customerEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer");

      _orderTicketEndPointStub = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint>();
      _orderTicketEndPointStub.Stub (stub => stub.ID).Return (_orderTicketEndPointID);

      _customerEndPointStub = MockRepository.GenerateStrictMock<IRealObjectEndPoint>();
      _customerEndPointStub.Stub (stub => stub.ID).Return (_customerEndPointID);

      _orderDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
    }

    [Test]
    public void RegisterEndPoints ()
    {
      _agentPartialMock
          .Stub (stub => stub.MockableGetOwnedEndPointIDs (_orderDataContainer))
          .Return (new[] { _orderTicketEndPointID, _customerEndPointID });
      _agentPartialMock.Replay();

      _endPointFactoryMock.Expect (mock => mock.CreateVirtualEndPoint (_orderTicketEndPointID, true)).Return (_orderTicketEndPointStub);
      _endPointFactoryMock.Expect (mock => mock.CreateRealObjectEndPoint (_customerEndPointID, _orderDataContainer)).Return (_customerEndPointStub);
      _endPointFactoryMock.Replay();

      _registrationAgentMock.Expect (mock => mock.RegisterEndPoint (_orderTicketEndPointStub, _map));
      _registrationAgentMock.Expect (mock => mock.RegisterEndPoint (_customerEndPointStub, _map));
      _registrationAgentMock.Replay();

      _agentPartialMock.RegisterEndPoints (_orderDataContainer, _map);

      _endPointFactoryMock.VerifyAllExpectations();
      _registrationAgentMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateUnregisterEndPointsCommand_EndPointsNotLoaded ()
    {
      _agentPartialMock.Stub (stub => stub.MockableGetOwnedEndPointIDs (_orderDataContainer)).Return (
          new[] { _orderTicketEndPointID, _customerEndPointID });
      _agentPartialMock.Replay();

      var result = _agentPartialMock.CreateUnregisterEndPointsCommand (_orderDataContainer, _map);

      Assert.That (result, Is.TypeOf<UnregisterEndPointsCommand>());
      Assert.That (((UnregisterEndPointsCommand) result).EndPoints, Is.Empty);
    }

    [Test]
    public void CreateUnregisterEndPointsCommand_Loaded_NoProblems ()
    {
      _map.AddEndPoint (_orderTicketEndPointStub);
      _map.AddEndPoint (_customerEndPointStub);

      _agentPartialMock.Stub (stub => stub.MockableGetOwnedEndPointIDs (_orderDataContainer)).Return (
          new[] { _orderTicketEndPointID, _customerEndPointID });
      _agentPartialMock.Expect (mock => mock.MockableGetUnregisterProblem (_orderTicketEndPointStub, _map)).Return (null);
      _agentPartialMock.Expect (mock => mock.MockableGetUnregisterProblem (_customerEndPointStub, _map)).Return (null);
      _agentPartialMock.Replay();

      var result = _agentPartialMock.CreateUnregisterEndPointsCommand (_orderDataContainer, _map);

      _agentPartialMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf<UnregisterEndPointsCommand>());
      Assert.That (
          ((UnregisterEndPointsCommand) result).EndPoints, Is.EqualTo (new IRelationEndPoint[] { _orderTicketEndPointStub, _customerEndPointStub }));
    }

    [Test]
    public void CreateUnregisterEndPointsCommand_Loaded_Problems ()
    {
      _map.AddEndPoint (_orderTicketEndPointStub);
      _map.AddEndPoint (_customerEndPointStub);

      _agentPartialMock.Stub (stub => stub.MockableGetOwnedEndPointIDs (_orderDataContainer)).Return (
          new[] { _orderTicketEndPointID, _customerEndPointID });
      _agentPartialMock.Expect (mock => mock.MockableGetUnregisterProblem (_orderTicketEndPointStub, _map)).Return ("Oh no!");
      _agentPartialMock.Expect (mock => mock.MockableGetUnregisterProblem (_customerEndPointStub, _map)).Return ("Oh no 2!");
      _agentPartialMock.Replay();

      var result = _agentPartialMock.CreateUnregisterEndPointsCommand (_orderDataContainer, _map);

      _agentPartialMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf<ExceptionCommand>());
      Assert.That (
          ((ExceptionCommand) result).Exception.Message,
          Is.EqualTo (
              "The relations of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded.\r\nOh no!\r\nOh no 2!"));
    }

    [Test]
    public void CreateUnregisterEndPointsCommand_Loaded_SomeProblems ()
    {
      _map.AddEndPoint (_orderTicketEndPointStub);
      _map.AddEndPoint (_customerEndPointStub);

      _agentPartialMock.Stub (stub => stub.MockableGetOwnedEndPointIDs (_orderDataContainer)).Return (
          new[] { _orderTicketEndPointID, _customerEndPointID });
      _agentPartialMock.Expect (mock => mock.MockableGetUnregisterProblem (_orderTicketEndPointStub, _map)).Return ("Oh no!");
      _agentPartialMock.Expect (mock => mock.MockableGetUnregisterProblem (_customerEndPointStub, _map)).Return (null);
      _agentPartialMock.Replay();

      var result = _agentPartialMock.CreateUnregisterEndPointsCommand (_orderDataContainer, _map);

      _agentPartialMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf<ExceptionCommand>());
      Assert.That (
          ((ExceptionCommand) result).Exception.Message, Is.EqualTo (
            "The relations of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded.\r\nOh no!"));
    }

    [Test]
    public void Serialization ()
    {
      var agent = new TestableDataContainerEndPointsRegistrationAgentBase (
          new SerializableRelationEndPointFactoryFake(), 
          new SerializableRelationEndPointRegistrationAgentFake());

      var deserializedAgent = Serializer.SerializeAndDeserialize (agent);

      Assert.That (deserializedAgent.EndPointFactory, Is.Not.Null);
      Assert.That (deserializedAgent.RegistrationAgent, Is.Not.Null);
    }
  }
}