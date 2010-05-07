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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  /// <summary>
  /// 
  /// </summary>
  [TestFixture]
  public class DataManagerTest : ClientTransactionBaseTest
  {
    private DataManager _dataManager;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataManager = ClientTransactionMock.DataManager;
    }

    [Test]
    public void GetLoadedDomainObjects_Empty ()
    {
      var loadedDomainObjects = _dataManager.GetLoadedData ();
      Assert.That (loadedDomainObjects.ToArray (), Is.Empty);
    }

    [Test]
    public void GetLoadedDomainObjects_NonEmpty ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

      var loadedDomainObjects = _dataManager.GetLoadedData ();

      var expected = new[] { 
          new Tuple<DomainObject, DataContainer> (order1, order1.InternalDataContainer), 
          new Tuple<DomainObject, DataContainer> (orderItem1, orderItem1.InternalDataContainer) };
      Assert.That (loadedDomainObjects.ToArray (), Is.EquivalentTo (expected));
    }

    [Test]
    public void GetLoadedDomainObjects_WithStates ()
    {
      var unchangedInstance = DomainObjectMother.GetUnchangedObject(ClientTransactionMock, DomainObjectIDs.Order1);
      var changedInstance = DomainObjectMother.GetChangedObject(ClientTransactionMock, DomainObjectIDs.OrderItem1);
      var newInstance = DomainObjectMother.GetNewObject();
      var deletedInstance = DomainObjectMother.GetDeletedObject(ClientTransactionMock, DomainObjectIDs.ClassWithAllDataTypes1);

      var unchangedObjects = _dataManager.GetLoadedDataByObjectState (StateType.Unchanged);
      var changedOrNewObjects = _dataManager.GetLoadedDataByObjectState (StateType.Changed, StateType.New);
      var deletedOrUnchangedObjects = _dataManager.GetLoadedDataByObjectState (StateType.Deleted, StateType.Unchanged);

      Assert.That (unchangedObjects.ToArray (), Is.EquivalentTo (new[] { CreateDataTuple (unchangedInstance) }));
      Assert.That (changedOrNewObjects.ToArray (), Is.EquivalentTo (new[] { CreateDataTuple (changedInstance), CreateDataTuple (newInstance) }));
      Assert.That (deletedOrUnchangedObjects.ToArray (), Is.EquivalentTo (new[] { CreateDataTuple (deletedInstance), CreateDataTuple (unchangedInstance) }));
    }

    [Test]
    public void GetChangedData_Empty ()
    {
      var changedDomainObjects = _dataManager.GetChangedDataByObjectState ();
      Assert.That (changedDomainObjects.ToArray(), Is.Empty);
    }

    [Test]
    public void GetChangedData ()
    {
      var changedInstance = DomainObjectMother.GetChangedObject (ClientTransactionMock, DomainObjectIDs.OrderItem1);
      var newInstance = DomainObjectMother.GetNewObject ();
      var deletedInstance = DomainObjectMother.GetDeletedObject (ClientTransactionMock, DomainObjectIDs.ClassWithAllDataTypes1);

      DomainObjectMother.GetUnchangedObject (ClientTransactionMock, DomainObjectIDs.Order1);
      DomainObjectMother.GetDiscardedObject (ClientTransactionMock);
      DomainObjectMother.GetNotLoadedObject (ClientTransactionMock, DomainObjectIDs.Order2);

      var changedDomainObjects = _dataManager.GetChangedDataByObjectState ();
      
      var expected = new[] { 
          CreateDataTuple (changedInstance), 
          CreateDataTuple (newInstance),
          CreateDataTuple (deletedInstance) };
      Assert.That (changedDomainObjects.ToArray (), Is.EquivalentTo (expected));
    }

    [Test]
    public void GetChangedData_ReturnsObjectsChangedByRelation ()
    {
      var orderWithChangedRelation = Order.GetObject (DomainObjectIDs.Order1);

      orderWithChangedRelation.OrderTicket = null;
      Assert.That (orderWithChangedRelation.State, Is.EqualTo (StateType.Changed));
      Assert.That (orderWithChangedRelation.InternalDataContainer.State, Is.EqualTo (StateType.Unchanged));

      var changedDomainObjects = _dataManager.GetChangedDataByObjectState ();
      
      var expected = CreateDataTuple (orderWithChangedRelation);
      Assert.That (changedDomainObjects.ToArray (), List.Contains (expected));
    }

    [Test]
    public void GetChangedDataContainersForCommit_ReturnsObjectsToBeCommitted ()
    {
      var changedInstance = (TestDomainBase) DomainObjectMother.GetChangedObject (ClientTransactionMock, DomainObjectIDs.OrderItem1);
      var newInstance = (TestDomainBase) DomainObjectMother.GetNewObject ();
      var deletedInstance = (TestDomainBase) DomainObjectMother.GetDeletedObject (ClientTransactionMock, DomainObjectIDs.ClassWithAllDataTypes1);

      DomainObjectMother.GetUnchangedObject (ClientTransactionMock, DomainObjectIDs.Order1);
      DomainObjectMother.GetDiscardedObject (ClientTransactionMock);
      DomainObjectMother.GetNotLoadedObject (ClientTransactionMock, DomainObjectIDs.Order2);

      var result = _dataManager.GetChangedDataContainersForCommit ();

      var expected = new[] { changedInstance.InternalDataContainer, newInstance.InternalDataContainer, deletedInstance.InternalDataContainer };
      Assert.That (result.ToArray (), Is.EquivalentTo (expected));
    }

    [Test]
    public void GetChangedDataContainersForCommit_DoesNotReturnRelationChangedObjects ()
    {
      var orderWithChangedRelation = Order.GetObject (DomainObjectIDs.Order1);

      orderWithChangedRelation.OrderItems.Add (OrderItem.NewObject ());
      Assert.That (orderWithChangedRelation.State, Is.EqualTo (StateType.Changed));
      Assert.That (orderWithChangedRelation.InternalDataContainer.State, Is.EqualTo (StateType.Unchanged));

      var result = _dataManager.GetChangedDataContainersForCommit ();

      Assert.That (result.ToArray (), List.Not.Contains (orderWithChangedRelation.InternalDataContainer));
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException))]
    public void GetChangedDataContainersForCommit_ChecksMandatoryRelations ()
    {
      var invalidOrder = Order.GetObject (DomainObjectIDs.Order1);
      invalidOrder.OrderTicket = null;

      _dataManager.GetChangedDataContainersForCommit().ToArray();
    }

    [Test]
    public void GetChangedDataContainersForCommit_WithDeletedObject ()
    {
      OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      orderItem1.Delete ();

      _dataManager.GetChangedDataContainersForCommit ().ToArray ();

      // expectation: no exception
    }

    [Test]
    public void GetChangedRelationEndPoints ()
    {
      Order order1 = Order.GetObject (DomainObjectIDs.Order1);
      Dev.Null = Order.GetObject (DomainObjectIDs.Order2);

      OrderItem orderItem1 = order1.OrderItems[0];
      OrderTicket orderTicket = order1.OrderTicket;

      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee employee = computer.Employee;

      Location location = Location.GetObject (DomainObjectIDs.Location1);
      Dev.Null = location.Client;

      Assert.IsEmpty (new List<RelationEndPoint> (_dataManager.GetChangedRelationEndPoints ()));

      orderItem1.Order = null; // 2 endpoints
      orderTicket.Order = null; // 2 endpoints

      computer.Employee = Employee.NewObject (); // 3 endpoints
      employee.Computer = null; // (1 endpoint)

      location.Client = Client.NewObject (); // 1 endpoint

      var changedEndPoints = new List<RelationEndPoint> (_dataManager.GetChangedRelationEndPoints ());

      Assert.That (changedEndPoints.Count, Is.EqualTo (8));

      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (order1.ID, typeof (Order) + ".OrderItems")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (orderItem1.ID, typeof (OrderItem) + ".Order")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (orderTicket.ID, typeof (OrderTicket) + ".Order")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (order1.ID, typeof (Order) + ".OrderTicket")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (computer.ID, typeof (Computer) + ".Employee")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (computer.Employee.ID, typeof (Employee) + ".Computer")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (employee.ID, typeof (Employee) + ".Computer")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (location.ID, typeof (Location) + ".Client")],
          changedEndPoints);
    }

    [Test]
    public void RegisterDataContainer_RegistersDataContainerInMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);

      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.SameAs (dataContainer));
    }

    [Test]
    public void RegisterDataContainer_RegistersEndPointsInMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Null);

      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Not.Null);
    }
    [Test]
    public void IsDiscarded ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (_dataManager.IsDiscarded (dataContainer.ID), Is.False);
      Assert.That (_dataManager.DiscardedObjectCount, Is.EqualTo (0));

      _dataManager.Rollback ();

      Assert.That (_dataManager.IsDiscarded (dataContainer.ID), Is.True);
      Assert.That (_dataManager.DiscardedObjectCount, Is.EqualTo (1));
    }

    [Test]
    public void GetDiscardedObject ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      _dataManager.Discard (dataContainer);

      Assert.That (_dataManager.GetDiscardedObject (dataContainer.ID), Is.SameAs (dataContainer.DomainObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has "
        + "not been discarded.\r\nParameter name: id")]
    public void GetDiscardedDataContainerThrowsWhenNotDiscarded ()
    {
      _dataManager.GetDiscardedObject (DomainObjectIDs.Order1);
    }

    [Test]
    public void Discard_RemovesEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      var endPointID = new RelationEndPointID (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Not.Null);

      _dataManager.Discard (dataContainer);

      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Null);
    }

    [Test]
    public void Discard_RemovesDataContainer ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.SameAs (dataContainer));

      _dataManager.Discard (dataContainer);

      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.Null);
    }

    [Test]
    public void Discard_DiscardsDataContainer ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      _dataManager.Discard (dataContainer);

      Assert.That (dataContainer.IsDiscarded, Is.True);
    }

    [Test]
    public void Discard_MarksObjectDiscarded ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _dataManager.Discard (dataContainer);

      Assert.That (_dataManager.IsDiscarded (dataContainer.ID), Is.True);
      listenerMock.AssertWasCalled (mock => mock.DataManagerMarkingObjectDiscarded (dataContainer.ID));
    }

    [Test]
    public void Discard_WithoutDomainObject_DataContainerNotMarkedDiscarded ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      _dataManager.RegisterDataContainer (dataContainer);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);
      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.Not.Null);

      _dataManager.Discard (dataContainer);
      
      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.Null);
      Assert.That (_dataManager.IsDiscarded (dataContainer.ID), Is.False);
      listenerMock.AssertWasNotCalled (mock => mock.DataManagerMarkingObjectDiscarded (dataContainer.ID));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot discard data container 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid', it might leave dangling references: 'End point "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' still "
        + "references object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.'\r\nParameter name: dataContainer")]
    public void Discard_ThrowsOnDanglingReferences ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      _dataManager.RegisterDataContainer (dataContainer);

      var endPointID = new RelationEndPointID (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      var endPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[endPointID];
      endPoint.OppositeObjectID = DomainObjectIDs.Order1;

      _dataManager.Discard (dataContainer);
    }

    [Test]
    public void MarkObjectDiscarded ()
    {
      var domainObject = LifetimeService.GetObjectReference (ClientTransactionMock, DomainObjectIDs.Order1);
      Assert.That (_dataManager.IsDiscarded (domainObject.ID), Is.False);

      _dataManager.MarkObjectDiscarded (domainObject);

      Assert.That (_dataManager.IsDiscarded (domainObject.ID), Is.True);
      Assert.That (_dataManager.GetDiscardedObject (domainObject.ID), Is.SameAs (domainObject));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot discard object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'; there is a DataContainer registered for that object. "
        + "Discard the DataContainer instead of the object.")]
    public void MarkObjectDiscarded_DataContainerExists ()
    {
      var domainObject = LifetimeService.GetObjectReference (ClientTransactionMock, DomainObjectIDs.Order1);
      var dataContainer = DataContainer.CreateNew (domainObject.ID);
      dataContainer.SetDomainObject (domainObject);
      _dataManager.RegisterDataContainer (dataContainer);

      _dataManager.MarkObjectDiscarded (domainObject);
    }

    [Test]
    public void Commit_CommitsRelationEndPointMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      _dataManager.RegisterDataContainer (dataContainer);

      var endPointID = new RelationEndPointID (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      var endPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[endPointID];
      endPoint.OppositeObjectID = DomainObjectIDs.Order2;

      Assert.That (endPoint.HasChanged, Is.True);

      _dataManager.Commit ();

      Assert.That (endPoint.HasChanged, Is.False);
      Assert.That (endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void Commit_CommitsDataContainerMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (dataContainer.State, Is.EqualTo (StateType.New));
      
      _dataManager.Commit ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void Commit_RemovesDeletedEndPoints ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      dataContainer.Delete ();

      var endPointID = new RelationEndPointID (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Not.Null);

      _dataManager.Commit ();

      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Null);
    }

    [Test]
    public void Commit_RemovesDeletedDataContainers ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      dataContainer.Delete ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));
      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.Not.Null);

      _dataManager.Commit ();

      Assert.That (dataContainer.IsDiscarded, Is.True);
      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.Null);
    }

    [Test]
    public void Commit_DiscardsDeletedDataContainers ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      dataContainer.Delete ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));
      Assert.That (dataContainer.IsDiscarded, Is.False);

      _dataManager.Commit ();

      Assert.That (dataContainer.IsDiscarded, Is.True);
    }

    [Test]
    public void Commit_MarksDeletedDataContainersAsDiscarded ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      dataContainer.Delete ();

      Assert.That (_dataManager.IsDiscarded (DomainObjectIDs.Order1), Is.False);

      _dataManager.Commit ();

      Assert.That (_dataManager.IsDiscarded (DomainObjectIDs.Order1), Is.True);
    }

    [Test]
    public void Rollback_RollsBackRelationEndPointStates ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);
      _dataManager.RegisterDataContainer (dataContainer);

      var endPointID = new RelationEndPointID (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      var endPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[endPointID];
      endPoint.OppositeObjectID = DomainObjectIDs.Order2;

      Assert.That (endPoint.HasChanged, Is.True);

      _dataManager.Rollback ();

      Assert.That (endPoint.HasChanged, Is.False);
      Assert.That (endPoint.OppositeObjectID, Is.Null);
    }

    [Test]
    public void Rollback_RollsBackDataContainerStates ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);
      _dataManager.RegisterDataContainer (dataContainer);

      dataContainer.Delete ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));

      _dataManager.Rollback ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void Rollback_RemovesNewEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      var endPointID = new RelationEndPointID (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Not.Null);

      _dataManager.Rollback ();

      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Null);
    }

    [Test]
    public void Rollback_RemovesNewDataContainers ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (dataContainer.State, Is.EqualTo (StateType.New));

      _dataManager.Rollback ();

      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.Null);
    }

    [Test]
    public void Rollback_DiscardsNewDataContainers ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (dataContainer.IsDiscarded, Is.False);

      _dataManager.Rollback ();

      Assert.That (dataContainer.IsDiscarded, Is.True);
    }

    [Test]
    public void Rollback_MarksNewDataContainersAsDiscarded ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);
      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (_dataManager.IsDiscarded (DomainObjectIDs.Order1), Is.False);

      _dataManager.Rollback ();

      Assert.That (_dataManager.IsDiscarded (DomainObjectIDs.Order1), Is.True);
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var deletedObject = Order.GetObject (DomainObjectIDs.Order1);

      var command = _dataManager.CreateDeleteCommand (deletedObject);

      Assert.That (command, Is.InstanceOfType (typeof (DeleteCommand)));
      Assert.That (((DeleteCommand) command).ClientTransaction, Is.SameAs (_dataManager.ClientTransaction));
      Assert.That (((DeleteCommand) command).DeletedObject, Is.SameAs (deletedObject));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
       ExpectedMessage = "Cannot delete DomainObject '.*', because it belongs to a different ClientTransaction.",
       MatchType = MessageMatch.Regex)]
    public void CreateDeleteCommand_OtherClientTransaction ()
    {
      var order1 = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();
      _dataManager.CreateDeleteCommand (order1);
    }

    [Test]
    public void CreateDeleteCommand_DeletedObject ()
    {
      var deletedObject = Order.GetObject (DomainObjectIDs.Order1);
      deletedObject.Delete ();
      Assert.That (deletedObject.State, Is.EqualTo (StateType.Deleted));

      var command = _dataManager.CreateDeleteCommand (deletedObject);
      Assert.That (command, Is.InstanceOfType (typeof (NopCommand)));
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void CreateDeleteCommand_DiscardedObject ()
    {
      var discardedObject = Order.NewObject ();
      discardedObject.Delete ();
      Assert.That (discardedObject.IsDiscarded, Is.True);

      _dataManager.CreateDeleteCommand (discardedObject);
    }

    [Test]
    public void CheckMandatoryRelations_AllRelationsOk ()
    {
      var dataContainer = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1).InternalDataContainer;

      _dataManager.CheckMandatoryRelations (dataContainer);
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException), ExpectedMessage = 
        "Mandatory relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official' of domain object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be null.")]
    public void CheckMandatoryRelations_RelationsNotOk ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataManager.RegisterDataContainer (dataContainer);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer.ID, "Official");
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Not.Null);

      _dataManager.CheckMandatoryRelations (dataContainer);
    }

    [Test]
    public void CheckMandatoryRelations_UnregisteredRelations_Ignored ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataManager.RegisterDataContainer (dataContainer);

      foreach (var endPointID in dataContainer.AssociatedRelationEndPointIDs)
        _dataManager.RelationEndPointMap.RemoveEndPoint (endPointID);

      _dataManager.CheckMandatoryRelations (dataContainer);
    }

    [Test]
    public void HasRelationChanged_True ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataManager.RegisterDataContainer (dataContainer);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer.ID, "Official");
      var endPoint = (RealObjectEndPoint) _dataManager.RelationEndPointMap[endPointID];
      endPoint.OppositeObjectID = DomainObjectIDs.Official1;
      Assert.That (endPoint.HasChanged, Is.True);

      var result = _dataManager.HasRelationChanged (dataContainer);

      Assert.That (result, Is.True);
    }

    [Test]
    public void HasRelationChanged_False ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataManager.RegisterDataContainer (dataContainer);

      var result = _dataManager.HasRelationChanged (dataContainer);

      Assert.That (result, Is.False);
    }

    [Test]
    public void HasRelationChanged_IgnoresUnregisteredEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataManager.RegisterDataContainer (dataContainer);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer.ID, "Official");
      var endPoint = (RealObjectEndPoint) _dataManager.RelationEndPointMap[endPointID];
      endPoint.OppositeObjectID = DomainObjectIDs.Official1;
      Assert.That (endPoint.HasChanged, Is.True);

      _dataManager.RelationEndPointMap.RemoveEndPoint (endPointID);

      Assert.That (_dataManager.HasRelationChanged (dataContainer), Is.False);
    }

    [Test]
    public void GetOppositeRelationEndPoints ()
    {
      var dataContainer = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;

      var endPoints = _dataManager.GetOppositeRelationEndPoints (dataContainer).ToArray ();

      var expectedIDs = new[] {
        RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order"),
        RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem2, "Order"),
        RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order"),
        RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Official1, "Orders"),
        RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders"),
      };
      var expectedEndPoints = expectedIDs.Select (id => _dataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (id)).ToArray();
      Assert.That (endPoints, Is.EquivalentTo (expectedEndPoints));
    }

    private Tuple<DomainObject, DataContainer, StateType> CreateDataTuple (DomainObject domainObject)
    {
      var dataContainer = ClientTransactionMock.DataManager.DataContainerMap[domainObject.ID];
      return Tuple.Create (domainObject, dataContainer, domainObject.State);
    }

    private void SetDomainObject (DataContainer dataContainer)
    {
      dataContainer.SetDomainObject (LifetimeService.GetObjectReference (ClientTransactionMock, dataContainer.ID));
    }
  }
}
