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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Rhino.Mocks;
using System.Linq;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class CommitFullEventChainTest : StandardMappingTest
  {
    private ClientTransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = ClientTransaction.CreateRootTransaction().CreateSubTransaction();
    }

    [Test]
    public void FullEventChain ()
    {
      var unchangedObject = GetUnchangedObject();
      var changedObject = GetChangedObject();
      var newObject = GetNewObject();
      var deletedObject = GetDeletedObject();

      var mockRepository = new MockRepository ();

      // Listener is a dynamic mock so that we don't have to expect all internal events
      var clientTransactionListenerMock = mockRepository.DynamicMock<IClientTransactionListener> ();
      var clientTransactionExtensionMock = mockRepository.StrictMock<IClientTransactionExtension> ();
      clientTransactionExtensionMock.Stub (stub => stub.Key).Return ("test");
      var clientTransactionEventReceiverMock = mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_transaction);
      var changedObjectEventReceiverMock = mockRepository.StrictMock<DomainObjectMockEventReceiver> (changedObject);
      var newObjectEventReceiverMock = mockRepository.StrictMock<DomainObjectMockEventReceiver> (newObject);
      var deletedObjectEventReceiverMock = mockRepository.StrictMock<DomainObjectMockEventReceiver> (deletedObject);

      // This mock is to ensure the unchanged object doesn't get any events
      mockRepository.StrictMock<DomainObjectMockEventReceiver> (unchangedObject);

      using (mockRepository.Ordered ())
      {
        // Committing events
        clientTransactionExtensionMock
            .Expect (mock => mock.Committing (Arg.Is (_transaction), ArgIsCommitSet (changedObject, newObject, deletedObject)));
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionCommitting (Arg.Is (_transaction), ArgIsCommitSet (changedObject, newObject, deletedObject)));
        clientTransactionEventReceiverMock
            .Expect (mock => mock.Committing (_transaction, changedObject, newObject, deletedObject))
            .WithCurrentTransaction (_transaction);
        using (mockRepository.Unordered ())
        {
          changedObjectEventReceiverMock.Expect (mock => mock.Committing (changedObject)).WithCurrentTransaction (_transaction);
          newObjectEventReceiverMock.Expect (mock => mock.Committing (newObject)).WithCurrentTransaction (_transaction);
          deletedObjectEventReceiverMock.Expect (mock => mock.Committing (deletedObject)).WithCurrentTransaction (_transaction);
        }

        // CommitValidate events
        clientTransactionExtensionMock
            .Expect (mock => mock.CommitValidate (Arg.Is (_transaction), ArgIsPersistableDataSet (changedObject, newObject, deletedObject)));
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionCommitValidate (Arg.Is (_transaction), ArgIsPersistableDataSet (changedObject, newObject, deletedObject)))
            .WhenCalled (mi => Assert.That (_transaction.HasChanged (), Is.True, "CommitValidate: last event before actual commit."));

        // Committed events
        using (mockRepository.Unordered ())
        {
          changedObjectEventReceiverMock
              .Expect (mock => mock.Committed (changedObject))
              .WhenCalledWithCurrentTransaction (
                  _transaction,
                  mi => Assert.That (_transaction.HasChanged(), Is.False, "Committed: first event after actual commit."));
          newObjectEventReceiverMock.Expect (mock => mock.Committed (newObject)).WithCurrentTransaction (_transaction);
        }
        clientTransactionEventReceiverMock
            .Expect (mock => mock.Committed (_transaction, changedObject, newObject))
            .WithCurrentTransaction (_transaction);
        clientTransactionExtensionMock.Expect (mock => mock.Committed (Arg.Is (_transaction), ArgIsCommitSet (changedObject, newObject)));
        clientTransactionListenerMock.Expect (mock => mock.TransactionCommitted (Arg.Is (_transaction), ArgIsCommitSet (changedObject, newObject)));
      }
      mockRepository.ReplayAll ();

      ClientTransactionTestHelper.AddListener (_transaction, clientTransactionListenerMock);
      _transaction.Extensions.Add (clientTransactionExtensionMock);

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      try
      {
        _transaction.Commit();
      }
      finally
      {
        _transaction.Extensions.Remove (clientTransactionExtensionMock.Key);
        ClientTransactionTestHelper.RemoveListener (_transaction, clientTransactionListenerMock);
      }
      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));

      mockRepository.VerifyAll ();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObject ()
    {
      var changedObject = GetChangedObject ();
      var additionalObject = GetUnchangedObject ();

      var mockRepository = new MockRepository ();

      // Listener is a dynamic mock so that we don't have to expect all internal events
      var clientTransactionListenerMock = mockRepository.DynamicMock<IClientTransactionListener> ();
      var clientTransactionExtensionMock = mockRepository.StrictMock<IClientTransactionExtension> ();
      clientTransactionExtensionMock.Stub (stub => stub.Key).Return ("test");
      var clientTransactionEventReceiverMock = mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_transaction);
      var changedObjectEventReceiverMock = mockRepository.StrictMock<DomainObjectMockEventReceiver> (changedObject);
      var additionalObjectEventReceiverMock = mockRepository.StrictMock<DomainObjectMockEventReceiver> (additionalObject);

      using (mockRepository.Ordered ())
      {
        // Committing events
        clientTransactionExtensionMock
            .Expect (mock => mock.Committing (Arg.Is (_transaction), ArgIsCommitSet (changedObject)))
            .WhenCalled (mi => _transaction.Execute (additionalObject.MarkAsChanged));
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionCommitting (Arg.Is (_transaction), ArgIsCommitSet (changedObject)));
        clientTransactionEventReceiverMock
            .Expect (mock => mock.Committing (_transaction, changedObject))
            .WithCurrentTransaction (_transaction);
        changedObjectEventReceiverMock.Expect (mock => mock.Committing (changedObject)).WithCurrentTransaction (_transaction);
    
        // Committing events - second iteration for additionalObject
        clientTransactionExtensionMock
            .Expect (mock => mock.Committing (Arg.Is (_transaction), ArgIsCommitSet (additionalObject)))
            // this will _not_ trigger an additional run
            .WhenCalled (mi => _transaction.Execute (changedObject.MarkAsChanged));
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionCommitting (Arg.Is (_transaction), ArgIsCommitSet (additionalObject)));
        clientTransactionEventReceiverMock
            .Expect (mock => mock.Committing (_transaction, additionalObject))
            .WithCurrentTransaction (_transaction);
        additionalObjectEventReceiverMock.Expect (mock => mock.Committing (additionalObject)).WithCurrentTransaction (_transaction);

        // CommitValidate events
        clientTransactionExtensionMock
            .Expect (mock => mock.CommitValidate (Arg.Is (_transaction), ArgIsPersistableDataSet (changedObject, additionalObject)));
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionCommitValidate (Arg.Is (_transaction), ArgIsPersistableDataSet (changedObject, additionalObject)))
            .WhenCalled (mi => Assert.That (_transaction.HasChanged (), Is.True, "CommitValidate: last event before actual commit."));

        // Committed events
        using (mockRepository.Unordered ())
        {
          changedObjectEventReceiverMock
              .Expect (mock => mock.Committed (changedObject))
              .WhenCalledWithCurrentTransaction (
                  _transaction,
                  mi => Assert.That (_transaction.HasChanged (), Is.False, "Committed: first event after actual commit."));
          additionalObjectEventReceiverMock.Expect (mock => mock.Committed (additionalObject)).WithCurrentTransaction (_transaction);
        }
        clientTransactionEventReceiverMock
            .Expect (mock => mock.Committed (_transaction, changedObject, additionalObject))
            .WithCurrentTransaction (_transaction);
        clientTransactionExtensionMock.Expect (mock => mock.Committed (Arg.Is (_transaction), ArgIsCommitSet (changedObject, additionalObject)));
        clientTransactionListenerMock.Expect (mock => mock.TransactionCommitted (Arg.Is (_transaction), ArgIsCommitSet (changedObject, additionalObject)));
      }
      mockRepository.ReplayAll ();

      ClientTransactionTestHelper.AddListener (_transaction, clientTransactionListenerMock);
      _transaction.Extensions.Add (clientTransactionExtensionMock);

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      try
      {
        _transaction.Commit ();
      }
      finally
      {
        _transaction.Extensions.Remove (clientTransactionExtensionMock.Key);
        ClientTransactionTestHelper.RemoveListener (_transaction, clientTransactionListenerMock);
      }
      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));

      mockRepository.VerifyAll ();
    }

    [Test]
    [Ignore ("TODO 1807")]
    public void FullEventChain_WithReiterationDueToRegisterForAdditionalCommittingEvents ()
    {
      var changedObject = GetChangedObject ();

      var mockRepository = new MockRepository ();

      // Listener is a dynamic mock so that we don't have to expect all internal events
      var clientTransactionListenerMock = mockRepository.DynamicMock<IClientTransactionListener> ();
      var clientTransactionExtensionMock = mockRepository.StrictMock<IClientTransactionExtension> ();
      clientTransactionExtensionMock.Stub (stub => stub.Key).Return ("test");
      var clientTransactionEventReceiverMock = mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_transaction);
      var changedObjectEventReceiverMock = mockRepository.StrictMock<DomainObjectMockEventReceiver> (changedObject);

      using (mockRepository.Ordered ())
      {
        // Committing events
        clientTransactionExtensionMock
            .Expect (mock => mock.Committing (Arg.Is (_transaction), ArgIsCommitSet (changedObject)))
            .WhenCalled (mi => /*TODO 1807: mi.Arguments[2].RegisterForAdditionalCommittingEvents (changedObject) */ { });
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionCommitting (Arg.Is (_transaction), ArgIsCommitSet (changedObject)));
        clientTransactionEventReceiverMock
            .Expect (mock => mock.Committing (_transaction, changedObject))
            .WithCurrentTransaction (_transaction);
        changedObjectEventReceiverMock.Expect (mock => mock.Committing (changedObject)).WithCurrentTransaction (_transaction);

        // Committing events - second iteration for changedObject
        clientTransactionExtensionMock
            .Expect (mock => mock.Committing (Arg.Is (_transaction), ArgIsCommitSet (changedObject)))
          // this will trigger just one additional run
            .WhenCalled (
                mi =>
                {
                  /*TODO 1807: mi.Arguments[2].RegisterForAdditionalCommittingEvents (changedObject); */
                  /*TODO 1807: mi.Arguments[2].RegisterForAdditionalCommittingEvents (changedObject); */
                });
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionCommitting (Arg.Is (_transaction), ArgIsCommitSet (changedObject)));
        clientTransactionEventReceiverMock
            .Expect (mock => mock.Committing (_transaction, changedObject))
            .WithCurrentTransaction (_transaction);
        changedObjectEventReceiverMock.Expect (mock => mock.Committing (changedObject)).WithCurrentTransaction (_transaction);

        // Committing events - third iteration for changedObject
        clientTransactionExtensionMock.Expect (mock => mock.Committing (Arg.Is (_transaction), ArgIsCommitSet (changedObject)));
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionCommitting (Arg.Is (_transaction), ArgIsCommitSet (changedObject)));
        clientTransactionEventReceiverMock
            .Expect (mock => mock.Committing (_transaction, changedObject))
            .WithCurrentTransaction (_transaction);
        changedObjectEventReceiverMock.Expect (mock => mock.Committing (changedObject)).WithCurrentTransaction (_transaction);

        // CommitValidate events
        clientTransactionExtensionMock
            .Expect (mock => mock.CommitValidate (Arg.Is (_transaction), ArgIsPersistableDataSet (changedObject)));
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionCommitValidate (Arg.Is (_transaction), ArgIsPersistableDataSet (changedObject)))
            .WhenCalled (mi => Assert.That (_transaction.HasChanged (), Is.True, "CommitValidate: last event before actual commit."));

        // Committed events
        using (mockRepository.Unordered ())
        {
          changedObjectEventReceiverMock
              .Expect (mock => mock.Committed (changedObject))
              .WhenCalledWithCurrentTransaction (
                  _transaction,
                  mi => Assert.That (_transaction.HasChanged (), Is.False, "Committed: first event after actual commit."));
        }
        clientTransactionEventReceiverMock
            .Expect (mock => mock.Committed (_transaction, changedObject))
            .WithCurrentTransaction (_transaction);
        clientTransactionExtensionMock.Expect (mock => mock.Committed (Arg.Is (_transaction), ArgIsCommitSet (changedObject)));
        clientTransactionListenerMock.Expect (mock => mock.TransactionCommitted (Arg.Is (_transaction), ArgIsCommitSet (changedObject)));
      }
      mockRepository.ReplayAll ();

      ClientTransactionTestHelper.AddListener (_transaction, clientTransactionListenerMock);
      _transaction.Extensions.Add (clientTransactionExtensionMock);

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      try
      {
        _transaction.Commit ();
      }
      finally
      {
        _transaction.Extensions.Remove (clientTransactionExtensionMock.Key);
        ClientTransactionTestHelper.RemoveListener (_transaction, clientTransactionListenerMock);
      }
      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));

      mockRepository.VerifyAll ();
    }

    [Test]
    [Ignore ("TODO 1807")]
    public void FullEventChain_WithReiterationDueToAddedObjectAndRegisterForAdditionalCommittingEvents ()
    {
      var changedObject = GetChangedObject ();
      var additionalObject = GetUnchangedObject ();

      var mockRepository = new MockRepository ();

      // Listener is a dynamic mock so that we don't have to expect all internal events
      var clientTransactionListenerMock = mockRepository.DynamicMock<IClientTransactionListener> ();
      var clientTransactionExtensionMock = mockRepository.DynamicMock<IClientTransactionExtension> ();
      clientTransactionExtensionMock.Stub (stub => stub.Key).Return ("test");
      var clientTransactionEventReceiverMock = mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_transaction);
      var changedObjectEventReceiverMock = mockRepository.StrictMock<DomainObjectMockEventReceiver> (changedObject);
      var additionalObjectEventReceiverMock = mockRepository.StrictMock<DomainObjectMockEventReceiver> (additionalObject);

      using (mockRepository.Ordered ())
      {
        // Committing events
        clientTransactionExtensionMock
            .Expect (mock => mock.Committing (Arg.Is (_transaction), ArgIsCommitSet (changedObject)))
          // This will trigger just one additional run, not two
            .WhenCalled (
                mi =>
                {
                  /* TODO 1807: Assert.That (
                   *    () => mi.Arguments[2].RegisterForAdditionalCommittingEvents (additionalObject), 
                   *    Throws.TypeOf<ArgumentException>.With.Message.EqualTo (...)); */
                  _transaction.Execute (additionalObject.MarkAsChanged);
                  /* TODO 1807: mi.Arguments[2].RegisterForAdditionalCommittingEvents (additionalObject); */
                });
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionCommitting (Arg.Is (_transaction), ArgIsCommitSet (changedObject)));
        clientTransactionEventReceiverMock
            .Expect (mock => mock.Committing (_transaction, changedObject))
            .WithCurrentTransaction (_transaction);
        changedObjectEventReceiverMock.Expect (mock => mock.Committing (changedObject)).WithCurrentTransaction (_transaction);

        // Committing events - second iteration for additionalObject
        clientTransactionExtensionMock
            .Expect (mock => mock.Committing (Arg.Is (_transaction), ArgIsCommitSet (additionalObject)))
          // this will _not_ trigger an additional run
            .WhenCalled (mi => _transaction.Execute (changedObject.MarkAsChanged));
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionCommitting (Arg.Is (_transaction), ArgIsCommitSet (additionalObject)));
        clientTransactionEventReceiverMock
            .Expect (mock => mock.Committing (_transaction, additionalObject))
            .WithCurrentTransaction (_transaction);
        additionalObjectEventReceiverMock.Expect (mock => mock.Committing (additionalObject)).WithCurrentTransaction (_transaction);

        // CommitValidate events
        clientTransactionExtensionMock
            .Expect (mock => mock.CommitValidate (Arg.Is (_transaction), ArgIsPersistableDataSet (changedObject, additionalObject)));
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionCommitValidate (Arg.Is (_transaction), ArgIsPersistableDataSet (changedObject, additionalObject)))
            .WhenCalled (mi => Assert.That (_transaction.HasChanged (), Is.True, "CommitValidate: last event before actual commit."));

        // Committed events
        using (mockRepository.Unordered ())
        {
          changedObjectEventReceiverMock
              .Expect (mock => mock.Committed (changedObject))
              .WhenCalledWithCurrentTransaction (
                  _transaction,
                  mi => Assert.That (_transaction.HasChanged (), Is.False, "Committed: first event after actual commit."));
          additionalObjectEventReceiverMock.Expect (mock => mock.Committed (additionalObject)).WithCurrentTransaction (_transaction);
        }
        clientTransactionEventReceiverMock
            .Expect (mock => mock.Committed (_transaction, changedObject, additionalObject))
            .WithCurrentTransaction (_transaction);
        clientTransactionExtensionMock.Expect (mock => mock.Committed (Arg.Is (_transaction), ArgIsCommitSet (changedObject, additionalObject)));
        clientTransactionListenerMock.Expect (mock => mock.TransactionCommitted (Arg.Is (_transaction), ArgIsCommitSet (changedObject, additionalObject)));
      }
      mockRepository.ReplayAll ();

      ClientTransactionTestHelper.AddListener (_transaction, clientTransactionListenerMock);
      _transaction.Extensions.Add (clientTransactionExtensionMock);

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      try
      {
        _transaction.Commit ();
      }
      finally
      {
        _transaction.Extensions.Remove (clientTransactionExtensionMock.Key);
        ClientTransactionTestHelper.RemoveListener (_transaction, clientTransactionListenerMock);
      }
      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));

      mockRepository.VerifyAll ();
    }

    private ClassWithAllDataTypes GetDeletedObject ()
    {
      return _transaction.Execute (
          () =>
          {
            var instance = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
            instance.Delete();
            return instance;
          });
    }

    private ClassWithAllDataTypes GetNewObject ()
    {
      return _transaction.Execute (() => ClassWithAllDataTypes.NewObject());
    }

    private Order GetChangedObject ()
    {
      return _transaction.Execute (
          () =>
          {
            var instance = Order.GetObject (DomainObjectIDs.Order1);
            instance.MarkAsChanged();
            return instance;
          });
    }

    private Customer GetUnchangedObject ()
    {
      return _transaction.Execute (() => Customer.GetObject (DomainObjectIDs.Customer1));
    }

    private ReadOnlyCollection<DomainObject> ArgIsCommitSet (params DomainObject[] domainObjects)
    {
      return Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (domainObjects);
    }

    private ReadOnlyCollection<PersistableData> ArgIsPersistableDataSet (params DomainObject[] domainObjecs)
    {
      return Arg<ReadOnlyCollection<PersistableData>>.Matches (c => c.Select (d => d.DomainObject).SetEquals (domainObjecs));
    }
  }
}