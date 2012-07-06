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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
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

    private DomainObject _fakeChangedDomainObject;
    private DomainObject _fakeNewDomainObject;
    private DomainObject _fakeDeletedDomainObject;

    private PersistableData _fakeChangedPersistableItem;
    private PersistableData _fakeNewPersistableItem;
    private PersistableData _fakeDeletedPersistableItem;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository();
      
      _eventSinkWithMock = ClientTransactionEventSinkWithMock.CreateWithStrictMock (mockRepository: _mockRepository);
      _persistenceStrategyMock = _mockRepository.StrictMock<IPersistenceStrategy> ();
      _dataManagerMock = _mockRepository.StrictMock<IDataManager> ();

      _agent = new CommitRollbackAgent (_eventSinkWithMock, _persistenceStrategyMock, _dataManagerMock);

      _fakeChangedDomainObject = DomainObjectMother.CreateFakeObject ();
      _fakeNewDomainObject = DomainObjectMother.CreateFakeObject ();
      _fakeDeletedDomainObject = DomainObjectMother.CreateFakeObject ();

      var fakeDataContainer1 = DataContainerObjectMother.CreateDataContainer();
      var fakeDataContainer2 = DataContainerObjectMother.CreateDataContainer();
      var fakeDataContainer3 = DataContainerObjectMother.CreateDataContainer();

      _fakeChangedPersistableItem = new PersistableData (_fakeChangedDomainObject, StateType.Changed, fakeDataContainer1, new IRelationEndPoint[0]);
      _fakeNewPersistableItem = new PersistableData (_fakeNewDomainObject, StateType.New, fakeDataContainer2, new IRelationEndPoint[0]);
      _fakeDeletedPersistableItem = new PersistableData (_fakeDeletedDomainObject, StateType.Deleted, fakeDataContainer3, new IRelationEndPoint[0]);
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
      _dataManagerMock.Stub (stub => stub.GetNewChangedDeletedData ()).Return (new PersistableData[0]);
      _mockRepository.ReplayAll ();

      var result = _agent.HasDataChanged ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void Commit ()
    {
      using (_mockRepository.Ordered())
      {
        // First run of BeginCommit: fakeChangedPersistableItem, _fakeNewPersistableItem in commit set - event raised for both
        _dataManagerMock
            .Expect (mock => mock.GetNewChangedDeletedData ())
            .Return (new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem });
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionCommitting (
                Arg.Is (_eventSinkWithMock.ClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { _fakeChangedDomainObject, _fakeNewDomainObject }),
                Arg<CommittingEventRegistrar>.Is.TypeOf));

        // Second run of BeginCommit: _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set 
        // Event is raised just for _fakeDeletedPersistableItem - the others have already got their event
        _dataManagerMock
            .Expect (mock => mock.GetNewChangedDeletedData ())
            .Return (new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem });
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionCommitting (
                Arg.Is (_eventSinkWithMock.ClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { _fakeDeletedDomainObject }),
                Arg<CommittingEventRegistrar>.Is.TypeOf));

        // End of BeginCommit: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - events already raised for all of those
        _dataManagerMock.Expect (mock => mock.GetNewChangedDeletedData ()).Return (new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });

        // CommitValidate: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - this is what actually gets committed
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionCommitValidate (
                Arg.Is (_eventSinkWithMock.ClientTransaction),
                Arg<ReadOnlyCollection<PersistableData>>.List.Equivalent (new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem })));
        
        // Commit _fakeNewPersistableItem, _fakeDeletedPersistableItem found earlier
        _persistenceStrategyMock.Expect (
            mock => mock.PersistData (
                Arg<IEnumerable<PersistableData>>.List.Equivalent (new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem })));
        _dataManagerMock.Expect (mock => mock.Commit());

        // Raise event for _fakeNewPersistableItem only, _fakeDeletedPersistableItem was deleted
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionCommitted (
                Arg.Is (_eventSinkWithMock.ClientTransaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { _fakeNewDomainObject })));
      }
      _mockRepository.ReplayAll();

      _agent.CommitData();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Rollback ()
    {
      using (_mockRepository.Ordered ())
      {
        // First run of BeginRollback: fakeChangedPersistableItem, _fakeNewPersistableItem in rollback set - event raised for both
        _dataManagerMock.Expect (mock => mock.GetNewChangedDeletedData()).Return (new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem });
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionRollingBack (
                Arg.Is (_eventSinkWithMock.ClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { _fakeChangedDomainObject, _fakeNewDomainObject })));

        // Second run of BeginRollback: fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem in rollback set 
        // Event is raised just for _fakeDeletedPersistableItem -  the others have alreay got their event
        _dataManagerMock
            .Expect (mock => mock.GetNewChangedDeletedData ())
            .Return (new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem });
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionRollingBack (
                Arg.Is (_eventSinkWithMock.ClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { _fakeDeletedDomainObject })));

        // End of BeginRollback: _fakeNewPersistableItem, _fakeDeletedPersistableItem in rollback set - events already raised for all of those
        _dataManagerMock.Expect (mock => mock.GetNewChangedDeletedData ()).Return (new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });

        // Rollback
        _dataManagerMock.Expect (mock => mock.Rollback ());

        // Raise event only for _fakeDeletedPersistableItem, _fakeNewPersistableItem was New
        _eventSinkWithMock.ExpectMock (
            mock => mock.TransactionRolledBack (
                Arg.Is (_eventSinkWithMock.ClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { _fakeDeletedDomainObject })));
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