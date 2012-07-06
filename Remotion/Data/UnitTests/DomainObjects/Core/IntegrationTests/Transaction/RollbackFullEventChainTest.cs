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
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class RollbackFullEventChainTest : StandardMappingTest
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
        // RollingBack events
        clientTransactionExtensionMock
            .Expect (mock => mock.RollingBack (Arg.Is (_transaction), ArgIsRollbackSet (changedObject, newObject, deletedObject)));
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionRollingBack (Arg.Is (_transaction), ArgIsRollbackSet (changedObject, newObject, deletedObject)));
        clientTransactionEventReceiverMock
            .Expect (mock => mock.RollingBack (_transaction, changedObject, newObject, deletedObject))
            .WithCurrentTransaction (_transaction);
        using (mockRepository.Unordered ())
        {
          changedObjectEventReceiverMock.Expect (mock => mock.RollingBack (changedObject)).WithCurrentTransaction (_transaction);
          newObjectEventReceiverMock.Expect (mock => mock.RollingBack (newObject)).WithCurrentTransaction (_transaction);
          deletedObjectEventReceiverMock.Expect (mock => mock.RollingBack (deletedObject)).WithCurrentTransaction (_transaction);
        }

        // RolledBack events
        using (mockRepository.Unordered ())
        {
          changedObjectEventReceiverMock
              .Expect (mock => mock.RolledBack (changedObject))
              .WhenCalledWithCurrentTransaction (
                  _transaction,
                  mi => Assert.That (_transaction.HasChanged(), Is.False, "RolledBack: first event after actual Rollback."));
          deletedObjectEventReceiverMock.Expect (mock => mock.RolledBack (deletedObject)).WithCurrentTransaction (_transaction);
        }
        clientTransactionEventReceiverMock
            .Expect (mock => mock.RolledBack (_transaction, changedObject, deletedObject))
            .WithCurrentTransaction (_transaction);
        clientTransactionExtensionMock.Expect (mock => mock.RolledBack (Arg.Is (_transaction), ArgIsRollbackSet (changedObject, deletedObject)));
        clientTransactionListenerMock.Expect (mock => mock.TransactionRolledBack (Arg.Is (_transaction), ArgIsRollbackSet (changedObject, deletedObject)));
      }
      mockRepository.ReplayAll ();

      ClientTransactionTestHelper.AddListener (_transaction, clientTransactionListenerMock);
      _transaction.Extensions.Add (clientTransactionExtensionMock);

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      try
      {
        _transaction.Rollback();
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
        // RollingBack events
        clientTransactionExtensionMock
            .Expect (mock => mock.RollingBack (Arg.Is (_transaction), ArgIsRollbackSet (changedObject)))
            .WhenCalled (mi => _transaction.Execute (additionalObject.MarkAsChanged));
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionRollingBack (Arg.Is (_transaction), ArgIsRollbackSet (changedObject)));
        clientTransactionEventReceiverMock
            .Expect (mock => mock.RollingBack (_transaction, changedObject))
            .WithCurrentTransaction (_transaction);
        changedObjectEventReceiverMock.Expect (mock => mock.RollingBack (changedObject)).WithCurrentTransaction (_transaction);

        // RollingBack events - second iteration for additionalObject
        clientTransactionExtensionMock
            .Expect (mock => mock.RollingBack (Arg.Is (_transaction), ArgIsRollbackSet (additionalObject)))
            // this will _not_ trigger an additional run
            .WhenCalled (mi => _transaction.Execute (changedObject.MarkAsChanged));
        clientTransactionListenerMock
            .Expect (mock => mock.TransactionRollingBack (Arg.Is (_transaction), ArgIsRollbackSet (additionalObject)));
        clientTransactionEventReceiverMock
            .Expect (mock => mock.RollingBack (_transaction, additionalObject))
            .WithCurrentTransaction (_transaction);
        additionalObjectEventReceiverMock.Expect (mock => mock.RollingBack (additionalObject)).WithCurrentTransaction (_transaction);

        // RolledBack events
        using (mockRepository.Unordered ())
        {
          changedObjectEventReceiverMock
              .Expect (mock => mock.RolledBack (changedObject))
              .WhenCalledWithCurrentTransaction (
                  _transaction,
                  mi => Assert.That (_transaction.HasChanged (), Is.False, "RolledBack: first event after actual Rollback."));
          additionalObjectEventReceiverMock.Expect (mock => mock.RolledBack (additionalObject)).WithCurrentTransaction (_transaction);
        }
        clientTransactionEventReceiverMock
            .Expect (mock => mock.RolledBack (_transaction, changedObject, additionalObject))
            .WithCurrentTransaction (_transaction);
        clientTransactionExtensionMock.Expect (mock => mock.RolledBack (Arg.Is (_transaction), ArgIsRollbackSet (changedObject, additionalObject)));
        clientTransactionListenerMock.Expect (mock => mock.TransactionRolledBack (Arg.Is (_transaction), ArgIsRollbackSet (changedObject, additionalObject)));
      }
      mockRepository.ReplayAll ();

      ClientTransactionTestHelper.AddListener (_transaction, clientTransactionListenerMock);
      _transaction.Extensions.Add (clientTransactionExtensionMock);

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      try
      {
        _transaction.Rollback ();
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

    private ReadOnlyCollection<DomainObject> ArgIsRollbackSet (params DomainObject[] domainObjects)
    {
      return Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (domainObjects);
    }
  }
}