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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class CommitRollbackAgentTest : StandardMappingTest
  {
    private MockRepository _mockRepository;

    private ClientTransactionEventSinkWithMock _eventSinkWithMock;
    private IPersistenceStrategy _persistenceStrategyMock;
    private IDataManager _dataManagerMock;

    private CommitRollbackAgent _agent;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository();
      
      _eventSinkWithMock = ClientTransactionEventSinkWithMock.CreateWithStrictMock (mockRepository: _mockRepository);
      _persistenceStrategyMock = _mockRepository.StrictMock<IPersistenceStrategy> ();
      _dataManagerMock = _mockRepository.StrictMock<IDataManager> ();

      _agent = new CommitRollbackAgent (_eventSinkWithMock, _persistenceStrategyMock, _dataManagerMock);
    }

    [Test]
    public void HasDataChanged_True ()
    {
      var fakeDomainObject = DomainObjectMother.CreateFakeObject<Order>();
      var fakeDataContainer = DataContainer.CreateNew (fakeDomainObject.ID);

      var item = new PersistableData (fakeDomainObject, StateType.Changed, fakeDataContainer, new IRelationEndPoint[0]);

      _dataManagerMock.Stub (stub => stub.GetNewChangedDeletedData()).Return (new[] { item });
      _mockRepository.ReplayAll ();

      var result = _agent.HasDataChanged();
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasDataChanged_False ()
    {
      var fakeDomainObject = DomainObjectMother.CreateFakeObject<Order> ();
      var fakeDataContainer = DataContainer.CreateNew (fakeDomainObject.ID);

      var item = new PersistableData (fakeDomainObject, StateType.Changed, fakeDataContainer, new IRelationEndPoint[0]);

      _dataManagerMock.Stub (stub => stub.GetNewChangedDeletedData ()).Return (new PersistableData[0]);
      _mockRepository.ReplayAll ();

      var result = _agent.HasDataChanged ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void Commit ()
    {
      var fakeDomainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      var fakeDomainObject2 = DomainObjectMother.CreateFakeObject<Order>();
      var fakeDomainObject3 = DomainObjectMother.CreateFakeObject<Order>();

      var fakeDataContainer1 = DataContainer.CreateNew (fakeDomainObject1.ID);
      var fakeDataContainer2 = DataContainer.CreateNew (fakeDomainObject2.ID);
      var fakeDataContainer3 = DataContainer.CreateNew (fakeDomainObject3.ID);

      var item1 = new PersistableData (fakeDomainObject1, StateType.Changed, fakeDataContainer1, new IRelationEndPoint[0]);
      var item2 = new PersistableData (fakeDomainObject2, StateType.New, fakeDataContainer2, new IRelationEndPoint[0]);
      var item3 = new PersistableData (fakeDomainObject3, StateType.Deleted, fakeDataContainer3, new IRelationEndPoint[0]);

      using (_mockRepository.Ordered())
      {
        // First run of BeginCommit: item1, item2 in commit set - event raised for both
        _dataManagerMock.Expect (mock => mock.GetNewChangedDeletedData ()).Return (new[] { item1, item2 });
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionCommitting (
                Arg.Is (_eventSinkWithMock.ClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { fakeDomainObject1, fakeDomainObject2 })));

        // Second run of BeginCommit: item1, item2, item3 in commit set - event raised just for item3
        _dataManagerMock.Expect (mock => mock.GetNewChangedDeletedData ()).Return (new[] { item1, item2, item3 });
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionCommitting (
                Arg.Is (_eventSinkWithMock.ClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { fakeDomainObject3 })));

        // End of BeginCommit: item2, item3 in commit set - events already raised for all of those
        _dataManagerMock.Expect (mock => mock.GetNewChangedDeletedData ()).Return (new[] { item2, item3 });

        // CommitValidate: item2, item3 in commit set - this is what actually gets committed
        _dataManagerMock.Expect (mock => mock.GetNewChangedDeletedData ()).Return (new[] { item2, item3 });
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionCommitValidate (
                Arg.Is (_eventSinkWithMock.ClientTransaction),
                Arg<ReadOnlyCollection<PersistableData>>.List.Equivalent (new[] { item2, item3 })));
        
        // Commit item2, item3 found earlier
        _persistenceStrategyMock.Expect (
            mock => mock.PersistData (Arg<ReadOnlyCollection<PersistableData>>.List.Equivalent (new[] { item2, item3 })));
        _dataManagerMock.Expect (mock => mock.Commit());

        // Raise event for item2 only, item3 was deleted
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionCommitted (
                Arg.Is (_eventSinkWithMock.ClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { fakeDomainObject2 })));
      }
      _mockRepository.ReplayAll();

      _agent.CommitData();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Rollback ()
    {
      var fakeDomainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var fakeDomainObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      var fakeDomainObject3 = DomainObjectMother.CreateFakeObject<Order> ();

      var fakeDataContainer1 = DataContainer.CreateNew (fakeDomainObject1.ID);
      var fakeDataContainer2 = DataContainer.CreateNew (fakeDomainObject2.ID);
      var fakeDataContainer3 = DataContainer.CreateNew (fakeDomainObject3.ID);

      var item1 = new PersistableData (fakeDomainObject1, StateType.Changed, fakeDataContainer1, new IRelationEndPoint[0]);
      var item2 = new PersistableData (fakeDomainObject2, StateType.New, fakeDataContainer2, new IRelationEndPoint[0]);
      var item3 = new PersistableData (fakeDomainObject3, StateType.Deleted, fakeDataContainer3, new IRelationEndPoint[0]);

      using (_mockRepository.Ordered ())
      {
        // First run of BeginRollback: item1, item2 in rollback set - event raised for both
        _dataManagerMock.Expect (mock => mock.GetNewChangedDeletedData ()).Return (new[] { item1, item2 });
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionRollingBack (
                Arg.Is (_eventSinkWithMock.ClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { fakeDomainObject1, fakeDomainObject2 })));

        // Second run of BeginRollback: item1, item2, item3 in rollback set - event raised just for item3
        _dataManagerMock.Expect (mock => mock.GetNewChangedDeletedData ()).Return (new[] { item1, item2, item3 });
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionRollingBack (
                Arg.Is (_eventSinkWithMock.ClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { fakeDomainObject3 })));

        // End of BeginRollback: item2, item3 in rollback set - events already raised for all of those
        _dataManagerMock.Expect (mock => mock.GetNewChangedDeletedData ()).Return (new[] { item2, item3 });

        // Get non-new objects that will be rolled back (for the RolledBack event below)
        _dataManagerMock.Expect (mock => mock.GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted)).Return (new[] { item3 });

        // Rollback
        _dataManagerMock.Expect (mock => mock.Rollback ());

        // Raise event for item3 gathered above
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionRolledBack (
                Arg.Is (_eventSinkWithMock.ClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { fakeDomainObject3 })));
      }
      _mockRepository.ReplayAll ();

      _agent.RollbackData ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Serializable ()
    {
      var instance = new CommitRollbackAgent (
          new SerializableClientTransactionEventSinkFake(), new SerializablePersistenceStrategyFake(), new SerializableDataManagerFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.EventSink, Is.Not.Null);
      Assert.That (deserializedInstance.PersistenceStrategy, Is.Not.Null);
      Assert.That (deserializedInstance.DataManager, Is.Not.Null);
    }
  }
}