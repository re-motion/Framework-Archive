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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class NonExistingDataContainerEndPointsRegistrationAgentTest : StandardMappingTest
  {
    private IRelationEndPointFactory _endPointFactoryStub;
    private IRelationEndPointRegistrationAgent _registrationAgentStub;
    private RelationEndPointMap _map;

    private NonExistingDataContainerEndPointsRegistrationAgent _agent;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointFactoryStub = MockRepository.GenerateStub<IRelationEndPointFactory> ();
      _registrationAgentStub = MockRepository.GenerateStub<IRelationEndPointRegistrationAgent> ();
      _map = new RelationEndPointMap(ClientTransaction.CreateRootTransaction());

      _agent = new NonExistingDataContainerEndPointsRegistrationAgent (_endPointFactoryStub, _registrationAgentStub);
    }

    [Test]
    public void GetOwnedEndPointIDs ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);

      var result = (IEnumerable<RelationEndPointID>) PrivateInvoke.InvokeNonPublicMethod (_agent, "GetOwnedEndPointIDs", dataContainer);

      Assert.That (
          result,
          Is.EquivalentTo (
              new[]
              {
                  RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket"),
                  RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems"),
                  RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Official"),
                  RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer")
              }));
    }

    [Test]
    public void GetUnregisterProblem_ObjectEndPoint_CurrentReferencesNonNull ()
    {
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint>();
      endPointStub.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.Customer1);
      endPointStub.Stub (stub => stub.OriginalOppositeObjectID).Return (null);
      endPointStub.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer"));

      var result = (string) PrivateInvoke.InvokeNonPublicMethod (_agent, "GetUnregisterProblem", endPointStub, _map);

      Assert.That (result, Is.EqualTo (
          "Relation end-point 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' "
          + "would leave a dangling reference."));
    }

    [Test]
    public void GetUnregisterProblem_ObjectEndPoint_OriginalReferencesNonNull ()
    {
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();
      endPointStub.Stub (stub => stub.OppositeObjectID).Return (null);
      endPointStub.Stub (stub => stub.OriginalOppositeObjectID).Return (DomainObjectIDs.Customer1);
      endPointStub.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer"));

      var result = (string) PrivateInvoke.InvokeNonPublicMethod (_agent, "GetUnregisterProblem", endPointStub, _map);

      Assert.That (result, Is.EqualTo (
          "Relation end-point 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' "
          + "would leave a dangling reference."));
    }

    [Test]
    public void GetUnregisterProblem_ObjectEndPoint_NonDangling ()
    {
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();
      endPointStub.Stub (stub => stub.OppositeObjectID).Return (null);
      endPointStub.Stub (stub => stub.OriginalOppositeObjectID).Return (null);
      endPointStub.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer"));

      var result = (string) PrivateInvoke.InvokeNonPublicMethod (_agent, "GetUnregisterProblem", endPointStub, _map);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetUnregisterProblem_CollectionEndPoint_CurrentReferencesNonNull ()
    {
      var item = DomainObjectMother.CreateFakeObject<Customer>();
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      endPointStub.Stub (stub => stub.GetData ()).Return (new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData (new[] { item }), false));
      endPointStub.Stub (stub => stub.GetOriginalData ()).Return (new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData (), false));
      endPointStub.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer"));

      var result = (string) PrivateInvoke.InvokeNonPublicMethod (_agent, "GetUnregisterProblem", endPointStub, _map);

      Assert.That (result, Is.EqualTo (
          "Relation end-point 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' "
          + "would leave a dangling reference."));
    }

    [Test]
    public void GetUnregisterProblem_CollectionEndPoint_OriginalReferencesNonNull ()
    {
      var item = DomainObjectMother.CreateFakeObject<Customer> ();
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      endPointStub.Stub (stub => stub.GetData ()).Return (new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData (), false));
      endPointStub.Stub (stub => stub.GetOriginalData ()).Return (new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData (new[] { item }), false));
      endPointStub.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer"));

      var result = (string) PrivateInvoke.InvokeNonPublicMethod (_agent, "GetUnregisterProblem", endPointStub, _map);

      Assert.That (result, Is.EqualTo (
          "Relation end-point 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' "
          + "would leave a dangling reference."));
    }

    [Test]
    public void GetUnregisterProblem_CollectionEndPoint_NonDangling ()
    {
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      endPointStub.Stub (stub => stub.GetData ()).Return (new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData (), false));
      endPointStub.Stub (stub => stub.GetOriginalData ()).Return (new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData (), false));
      endPointStub.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer"));

      var result = (string) PrivateInvoke.InvokeNonPublicMethod (_agent, "GetUnregisterProblem", endPointStub, _map);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void Serialization ()
    {
      var agent = new NonExistingDataContainerEndPointsRegistrationAgent (
          new SerializableRelationEndPointFactoryFake (),
          new SerializableRelationEndPointRegistrationAgentFake ());

      var deserializedAgent = Serializer.SerializeAndDeserialize (agent);

      Assert.That (deserializedAgent.EndPointFactory, Is.Not.Null);
      Assert.That (deserializedAgent.RegistrationAgent, Is.Not.Null);
    }
  }
}