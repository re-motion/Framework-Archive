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
using System.Collections.ObjectModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class ObjectLoaderTest : StandardMappingTest
  {
    private MockRepository _mockRepository;

    private ClientTransactionMock _clientTransaction;
    private IPersistenceStrategy _persistenceStrategyMock;
    private IClientTransactionListener _eventSinkMock;
    private IEagerFetcher _fetcherMock;

    private ObjectLoader _objectLoader;

    private DataContainer _order1DataContainer;
    private DataContainer _order2DataContainer;
    private DataContainer _orderTicket1DataContainer;

    private IQuery _fakeQuery;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      
      _clientTransaction = new ClientTransactionMock();
      _persistenceStrategyMock = _mockRepository.StrictMock<IPersistenceStrategy> ();
      _eventSinkMock = _mockRepository.DynamicMock<IClientTransactionListener> ();
      _fetcherMock = _mockRepository.StrictMock<IEagerFetcher> ();

      _objectLoader = new ObjectLoader (_clientTransaction, _persistenceStrategyMock, _eventSinkMock, _fetcherMock);

      _order1DataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      _order2DataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order2, null, pd => pd.DefaultValue);
      _orderTicket1DataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);

      _fakeQuery = CreateFakeQuery();
    }

    [Test]
    public void LoadObject ()
    {
      _persistenceStrategyMock.Stub (mock => mock.LoadDataContainer (DomainObjectIDs.Order1)).Return (_order1DataContainer);

      _persistenceStrategyMock.Replay ();

      var result = _objectLoader.LoadObject (DomainObjectIDs.Order1);

      CheckLoadedObject (result, _order1DataContainer);
    }

    [Test]
    public void LoadObject_Events ()
    {
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _persistenceStrategyMock.Expect (mock => mock.LoadDataContainer (DomainObjectIDs.Order1)).Return (_order1DataContainer);
        
        ExpectObjectsLoading (DomainObjectIDs.Order1);
        ExpectObjectsLoaded (transactionEventReceiver, _order1DataContainer);
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObject (DomainObjectIDs.Order1);

      _mockRepository.VerifyAll();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result }));
    }

    [Test]
    public void LoadObjects ()
    {
      _persistenceStrategyMock
          .Stub (mock => mock.LoadDataContainers (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true))
          .Return (new DataContainerCollection (new[] { _order1DataContainer, _order2DataContainer }, true));

      _persistenceStrategyMock.Replay ();

      var result = _objectLoader.LoadObjects (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true);

      Assert.That (result.Length, Is.EqualTo (2));

      CheckLoadedObject (result[0], _order1DataContainer);
      CheckLoadedObject (result[1], _order2DataContainer);
    }

    [Test]
    public void LoadObjects_WithUnknownObjects ()
    {
      _persistenceStrategyMock
          .Stub (mock => mock.LoadDataContainers (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order2 }, true))
          .Return (new DataContainerCollection (new[] { _order1DataContainer, _order2DataContainer }, true));

      _persistenceStrategyMock.Replay ();

      var result = _objectLoader.LoadObjects (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order2 }, true);

      Assert.That (result.Length, Is.EqualTo (3));

      CheckLoadedObject (result[0], _order1DataContainer);
      Assert.That (result[1], Is.Null);
      CheckLoadedObject (result[2], _order2DataContainer);
    }

    [Test]
    public void LoadObjects_Ordering ()
    {
      _persistenceStrategyMock
          .Stub (mock => mock.LoadDataContainers (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true))
          .Return (new DataContainerCollection (new[] { _order2DataContainer, _order1DataContainer }, true));

      _persistenceStrategyMock.Replay ();

      var result = _objectLoader.LoadObjects (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true);

      Assert.That (result.Length, Is.EqualTo (2));

      CheckLoadedObject (result[0], _order1DataContainer);
      CheckLoadedObject (result[1], _order2DataContainer);
    }

    [Test]
    public void LoadObjects_Events ()
    {
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _persistenceStrategyMock
            .Expect (mock => mock.LoadDataContainers (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true))
          .Return (new DataContainerCollection (new[] { _order1DataContainer, _order2DataContainer }, true));

        ExpectObjectsLoading (DomainObjectIDs.Order1, DomainObjectIDs.Order2);
        ExpectObjectsLoaded (transactionEventReceiver, _order1DataContainer, _order2DataContainer);
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObjects (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true);

      _mockRepository.VerifyAll ();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result[0], result[1] }));
    }

    [Test]
    public void LoadObjects_EmptyResult_Events ()
    {
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      _persistenceStrategyMock
          .Expect (mock => mock.LoadDataContainers (new ObjectID[0], true))
        .Return (new DataContainerCollection (new DataContainer[0], true));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObjects (new ObjectID[0], true);
      Assert.That (result, Is.Empty);

      _eventSinkMock.AssertWasNotCalled (mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));

      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void LoadRelatedObject ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      _persistenceStrategyMock
          .Expect (mock => mock.LoadRelatedDataContainer (
              Arg<DataContainer>.Matches (dc => dc == _clientTransaction.DataManager.DataContainerMap[dc.ID]), 
              Arg.Is (endPointID)))
          .Return (_orderTicket1DataContainer);

      _persistenceStrategyMock.Replay ();

      var result = _objectLoader.LoadRelatedObject (endPointID);

      _persistenceStrategyMock.VerifyAllExpectations ();
      CheckLoadedObject (result, _orderTicket1DataContainer);
    }

    [Test]
    [ExpectedException (typeof (RelatedObjectNotLoadableException), ExpectedMessage = 
        "Cannot load the related 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' of "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid': The database returned related object "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid', but that object already exists in the current ClientTransaction (and "
        + "points to a different object 'null').")]
    public void LoadRelatedObject_InconsistentDatabaseResult_DataContainerAlreadyExists_PointsToNull ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _orderTicket1DataContainer.SetDomainObject (LifetimeService.GetObjectReference (_clientTransaction, _orderTicket1DataContainer.ID));
      _clientTransaction.DataManager.RegisterDataContainer (_orderTicket1DataContainer);

      _persistenceStrategyMock
          .Expect (mock => mock.LoadRelatedDataContainer (
              Arg<DataContainer>.Matches (dc => dc == _clientTransaction.DataManager.DataContainerMap[dc.ID]),
              Arg.Is (endPointID)))
          .Return (_orderTicket1DataContainer);

      _persistenceStrategyMock.Replay ();

      _objectLoader.LoadRelatedObject (endPointID);
    }

    [Test]
    [ExpectedException (typeof (RelatedObjectNotLoadableException), ExpectedMessage = 
        "Cannot load the related 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' of "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid': The database returned related object "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid', but that object already exists in the current ClientTransaction (and "
        + "points to a different object 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid').")]
    public void LoadRelatedObject_InconsistentDatabaseResult_DataContainerAlreadyExists_PointsToDifferentObject ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _orderTicket1DataContainer.PropertyValues[typeof (OrderTicket).FullName + ".Order"].Value = DomainObjectIDs.Order2;
      _orderTicket1DataContainer.SetDomainObject (LifetimeService.GetObjectReference (_clientTransaction, _orderTicket1DataContainer.ID));
      _clientTransaction.DataManager.RegisterDataContainer (_orderTicket1DataContainer);

      _persistenceStrategyMock
          .Expect (mock => mock.LoadRelatedDataContainer (
              Arg<DataContainer>.Matches (dc => dc == _clientTransaction.DataManager.DataContainerMap[dc.ID]),
              Arg.Is (endPointID)))
          .Return (_orderTicket1DataContainer);

      _persistenceStrategyMock.Replay ();

      _objectLoader.LoadRelatedObject (endPointID);
    }

    [Test]
    public void LoadRelatedObject_Null ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      _persistenceStrategyMock
          .Stub (mock => mock.LoadRelatedDataContainer (
              Arg<DataContainer>.Matches (dc => dc == _clientTransaction.DataManager.DataContainerMap[dc.ID]), 
              Arg.Is (endPointID)))
          .Return (null);

      _persistenceStrategyMock.Replay ();

      var result = _objectLoader.LoadRelatedObject (endPointID);

      _persistenceStrategyMock.VerifyAllExpectations ();
      Assert.That (result, Is.Null);
    }

    [Test]
    public void LoadRelatedObject_Events ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _clientTransaction.EnsureDataAvailable (endPointID.ObjectID); // preload originating DataContainer to get clean events below

      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _persistenceStrategyMock
            .Expect (mock => mock.LoadRelatedDataContainer (Arg<DataContainer>.Is.Anything, Arg<RelationEndPointID>.Is.Anything))
            .Return (_orderTicket1DataContainer);

        ExpectObjectsLoading (DomainObjectIDs.OrderTicket1);
        ExpectObjectsLoaded (transactionEventReceiver, _orderTicket1DataContainer);
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadRelatedObject (endPointID);

      _mockRepository.VerifyAll ();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "LoadRelatedObject can only be used with virtual end points.\r\nParameter name: relationEndPointID")]
    public void LoadRelatedObject_NonVirtualID ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      _objectLoader.LoadRelatedObject (endPointID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "LoadRelatedObject can only be used with one-valued end points.\r\nParameter name: relationEndPointID")]
    public void LoadRelatedObject_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      _objectLoader.LoadRelatedObject (endPointID);
    }

    [Test]
    public void LoadRelatedObjects ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");

      _persistenceStrategyMock
          .Stub (mock => mock.LoadRelatedDataContainers (endPointID))
          .Return (new DataContainerCollection (new[] { _order1DataContainer, _order2DataContainer }, true));

      _persistenceStrategyMock.Replay ();

      var result = _objectLoader.LoadRelatedObjects (endPointID);

      Assert.That (result.Length, Is.EqualTo (2));
      CheckLoadedObject (result[0], _order1DataContainer);
      CheckLoadedObject (result[1], _order2DataContainer);
    }

    [Test]
    public void LoadRelatedObjects_Events ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _persistenceStrategyMock
            .Expect (mock => mock.LoadRelatedDataContainers (endPointID))
          .Return (new DataContainerCollection (new[] { _order1DataContainer, _order2DataContainer }, true));

        ExpectObjectsLoading (DomainObjectIDs.Order1, DomainObjectIDs.Order2);
        ExpectObjectsLoaded (transactionEventReceiver, _order1DataContainer, _order2DataContainer);
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadRelatedObjects (endPointID);

      _mockRepository.VerifyAll ();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result[0], result[1] }));
    }

    [Test]
    public void LoadRelatedObjects_EmptyResult_Events ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      _persistenceStrategyMock
          .Expect (mock => mock.LoadRelatedDataContainers (endPointID))
        .Return (new DataContainerCollection (new DataContainer[0], true));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadRelatedObjects (endPointID);
      Assert.That (result, Is.Empty);

      _eventSinkMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));

      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "LoadRelatedObjects can only be used with many-valued end points.\r\nParameter name: relationEndPointID")]
    public void LoadRelatedObjects_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _objectLoader.LoadRelatedObjects (endPointID);
    }

    [Test]
    public void LoadRelatedObjects_MergedResult ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");

      var domainObject1 = PreregisterDataContainer(_order1DataContainer);

      _persistenceStrategyMock
          .Stub (mock => mock.LoadRelatedDataContainers (endPointID))
          .Return (new DataContainerCollection (new[] { _order1DataContainer, _order2DataContainer }, true));

      _persistenceStrategyMock.Replay ();

      var result = _objectLoader.LoadRelatedObjects (endPointID);

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0], Is.SameAs (domainObject1));
      CheckLoadedObject (result[1], _order2DataContainer);
    }

    [Test]
    public void LoadRelatedObjects_MergedResult_Events ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      PreregisterDataContainer (_order1DataContainer);

      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _persistenceStrategyMock
            .Expect (mock => mock.LoadRelatedDataContainers (endPointID))
          .Return (new DataContainerCollection (new[] { _order1DataContainer, _order2DataContainer }, true));

        ExpectObjectsLoading (DomainObjectIDs.Order2);
        ExpectObjectsLoaded (transactionEventReceiver, _order2DataContainer);
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadRelatedObjects (endPointID);

      _mockRepository.VerifyAll ();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result[1] }));
    }

    [Test]
    public void LoadCollectionQueryResult ()
    {
      _persistenceStrategyMock
          .Stub (mock => mock.LoadDataContainersForQuery (_fakeQuery))
          .Return (new[] { _order1DataContainer, _order2DataContainer });

      _persistenceStrategyMock.Replay ();

      var result = _objectLoader.LoadCollectionQueryResult<Order> (_fakeQuery);

      Assert.That (result.Length, Is.EqualTo (2));
      CheckLoadedObject (result[0], _order1DataContainer);
      CheckLoadedObject (result[1], _order2DataContainer);
    }

    [Test]
    public void LoadCollectionQueryResult_EmptyResult_Events ()
    {
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      _persistenceStrategyMock
          .Stub (mock => mock.LoadDataContainersForQuery (_fakeQuery))
        .Return (new DataContainer[0]);

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadCollectionQueryResult<Order> (_fakeQuery);
      Assert.That (result, Is.Empty);

      _eventSinkMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));

      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void LoadCollectionQueryResult_MergedResult ()
    {
      var domainObject1 = PreregisterDataContainer (_order1DataContainer);

      _persistenceStrategyMock
          .Stub (mock => mock.LoadDataContainersForQuery (_fakeQuery))
          .Return (new []{ _order1DataContainer, _order2DataContainer });

      _persistenceStrategyMock.Replay ();

      var result = _objectLoader.LoadCollectionQueryResult<Order> (_fakeQuery);

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0], Is.SameAs (domainObject1));
      CheckLoadedObject (result[1], _order2DataContainer);
    }

    [Test]
    public void LoadCollectionQueryResult_MergedResult_Events ()
    {
      PreregisterDataContainer (_order1DataContainer);

      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _persistenceStrategyMock
            .Expect (mock => mock.LoadDataContainersForQuery (_fakeQuery))
          .Return (new[] { _order1DataContainer, _order2DataContainer });

        ExpectObjectsLoading (DomainObjectIDs.Order2);
        ExpectObjectsLoaded (transactionEventReceiver, _order2DataContainer);
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadCollectionQueryResult<Order> (_fakeQuery);

      _mockRepository.VerifyAll ();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result[1] }));
    }

    [Test]
    [ExpectedException (typeof (UnexpectedQueryResultException), ExpectedMessage = 
        "The query returned an object of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order', but a query result of type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer' was expected.")]
    public void LoadCollectionQueryResult_CastError ()
    {
      _persistenceStrategyMock
          .Stub (mock => mock.LoadDataContainersForQuery (_fakeQuery))
          .Return (new[] { _order1DataContainer, _order2DataContainer });

      _persistenceStrategyMock.Replay ();

      var result = _objectLoader.LoadCollectionQueryResult<Customer> (_fakeQuery);

      Assert.That (result.Length, Is.EqualTo (2));
      CheckLoadedObject (result[0], _order1DataContainer);
      CheckLoadedObject (result[1], _order2DataContainer);
    }

    [Test]
    public void LoadCollectionQueryResult_WithFetching ()
    {
      var fetchQueryStub = CreateFakeQuery();
      var endPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      _fakeQuery.EagerFetchQueries.Add (endPointDefinition, fetchQueryStub);
      
      _persistenceStrategyMock
          .Expect (mock => mock.LoadDataContainersForQuery (_fakeQuery))
          .Return (new[] { _order1DataContainer });
      _fetcherMock
          .Expect (mock => mock.PerformEagerFetching (
              Arg<DomainObject[]>.Matches (list => list.Single ().ID == _order1DataContainer.ID),
              Arg.Is (endPointDefinition),
              Arg.Is (fetchQueryStub),
              Arg.Is (_objectLoader)));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadCollectionQueryResult<Order> (_fakeQuery);

      Assert.That (result.Length, Is.EqualTo (1));
      CheckLoadedObject (result[0], _order1DataContainer);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void LoadCollectionQueryResult_WithFetching_NoOriginalObjects_DoesNotRegisterAnything ()
    {
      var fetchQueryStub = CreateFakeQuery ();
      var endPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      _fakeQuery.EagerFetchQueries.Add (endPointDefinition, fetchQueryStub);

      _persistenceStrategyMock
          .Expect (mock => mock.LoadDataContainersForQuery (_fakeQuery))
          .Return (new DataContainer[0]);

      _mockRepository.ReplayAll ();

      _objectLoader.LoadCollectionQueryResult<Order> (_fakeQuery);

      _fetcherMock.AssertWasNotCalled (mock => mock.PerformEagerFetching (
          Arg<DomainObject[]>.Is.Anything,
          Arg<IRelationEndPointDefinition>.Is.Anything,
          Arg<IQuery>.Is.Anything,
          Arg<IObjectLoader>.Is.Anything));
    }

    [Test]
    public void ClientTransactionLoadedEvent_Transaction ()
    {
      ClientTransaction loadTransaction = null;
      _clientTransaction.Loaded += delegate { loadTransaction = ClientTransaction.Current; };

      _persistenceStrategyMock.Stub (mock => mock.LoadDataContainer (DomainObjectIDs.Order1)).Return (_order1DataContainer);
      _mockRepository.ReplayAll ();

      _objectLoader.LoadObject (DomainObjectIDs.Order1);

      Assert.That (loadTransaction, Is.SameAs (_clientTransaction));
    }

    private void CheckLoadedObject (DomainObject loadedObject, DataContainer dataContainer)
    {
      Assert.That (loadedObject, Is.InstanceOfType (dataContainer.DomainObjectType));
      Assert.That (loadedObject.ID, Is.EqualTo (dataContainer.ID));
      Assert.That (_clientTransaction.IsEnlisted (loadedObject), Is.True);
      Assert.That (dataContainer.ClientTransaction, Is.SameAs (_clientTransaction));
      Assert.That (dataContainer.DomainObject, Is.SameAs (loadedObject));
    }

    private void ExpectObjectsLoading (params ObjectID[] expectedIDs)
    {
      _eventSinkMock
          .Expect (mock => mock.ObjectsLoading (Arg.Is (_clientTransaction), Arg<ReadOnlyCollection<ObjectID>>.List.Equal (expectedIDs)))
          .WhenCalled (mi => Assert.That (
              expectedIDs.All (id => _clientTransaction.DataManager.DataContainerMap[id] == null), 
              "ObjectsLoading must be raised before IDs are registered"));
    }

    private void ExpectObjectsLoaded (ClientTransactionEventReceiver transactionEventReceiver, params DataContainer[] expectedDataContainers)
    {
      _eventSinkMock
          .Expect (mock => mock.ObjectsLoaded (
              Arg.Is (_clientTransaction), 
              Arg<ReadOnlyCollection<DomainObject>>.Matches (list =>
                  list.Select (item => item.ID).SequenceEqual (expectedDataContainers.Select (dc => dc.ID)))))
          .WhenCalled (mi =>
          {
            Assert.That (
                expectedDataContainers.All (dc => _clientTransaction.DataManager.DataContainerMap[dc.ID] == dc),
                "ObjectsLoaded must be raised after IDs are registered");
            Assert.That (
                ((ReadOnlyCollection<DomainObject>) mi.Arguments[1]).All (item => ((TestDomainBase) item).OnLoadedCalled),
                "ObjectsLoaded must be raised after OnLoaded is called");
            Assert.That (
                ((ReadOnlyCollection<DomainObject>) mi.Arguments[1]).All (item => ((TestDomainBase) item).OnLoadedTx == _clientTransaction),
                "ObjectsLoaded must be raised after OnLoaded is called");
            if (transactionEventReceiver != null)
            {
              Assert.That (
                  transactionEventReceiver.LoadedDomainObjects,
                  Is.Empty,
                  "ObjectsLoaded must be raised before transaction OnLoaded is called");
            }
          });
    }

    private DomainObject PreregisterDataContainer (DataContainer dataContainer)
    {
      ClientTransactionTestHelper.RegisterDataContainer (_clientTransaction, dataContainer);
      return dataContainer.DomainObject;
    }

    private IQuery CreateFakeQuery ()
    {
      return QueryFactory.CreateCollectionQuery (
          "test",
          DomainObjectIDs.Order1.StorageProviderID,
          "TEST",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));
    }
  }
}