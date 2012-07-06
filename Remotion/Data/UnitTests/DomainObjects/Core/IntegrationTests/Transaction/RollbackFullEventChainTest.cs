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
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Rhino.Mocks;
using System.Linq;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class RollbackFullEventChainTest : StandardMappingTest
  {
    private ClientTransaction _transaction;

    private DomainObject _unchangedObject;
    private DomainObject _changedObject;
    private DomainObject _newObject;
    private DomainObject _deletedObject;

    private MockRepository _mockRepository;

    private IClientTransactionListener _listenerMock;
    private IClientTransactionExtension _extensionMock;
    private ClientTransactionMockEventReceiver _transactionMockEventReceiver;

    private DomainObjectMockEventReceiver _changedObjectEventReceiverMock;
    private DomainObjectMockEventReceiver _newObjectEventReceiverMock;
    private DomainObjectMockEventReceiver _deletedObjectEventReceiverMock;
    private DomainObjectMockEventReceiver _unchangedObjectEventReceiverMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = ClientTransaction.CreateRootTransaction ().CreateSubTransaction ();

      _unchangedObject = GetUnchangedObject ();
      _changedObject = GetChangedObject ();
      _newObject = GetNewObject ();
      _deletedObject = GetDeletedObject ();

      _mockRepository = new MockRepository ();

      // Listener is a dynamic mock so that we don't have to expect all the internal events of RollingBack
      _listenerMock = _mockRepository.DynamicMock<IClientTransactionListener> ();
      _extensionMock = _mockRepository.StrictMock<ClientTransactionExtensionBase> ("test");
      _transactionMockEventReceiver = _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_transaction);

      _changedObjectEventReceiverMock = CreateDomainObjectMockEventReceiver (_changedObject);
      _newObjectEventReceiverMock = CreateDomainObjectMockEventReceiver (_newObject);
      _deletedObjectEventReceiverMock = CreateDomainObjectMockEventReceiver (_deletedObject);
      _unchangedObjectEventReceiverMock = CreateDomainObjectMockEventReceiver (_unchangedObject);

      ClientTransactionTestHelper.AddListener (_transaction, _listenerMock);
      _transaction.Extensions.Add (_extensionMock);
    }

    [Test]
    public void FullEventChain ()
    {
      using (_mockRepository.Ordered ())
      {
        ExpectRollingBackEvents (
            Tuple.Create (_changedObject, _changedObjectEventReceiverMock),
            Tuple.Create (_newObject, _newObjectEventReceiverMock),
            Tuple.Create (_deletedObject, _deletedObjectEventReceiverMock));

        ExpectRolledBackEvents (
            Tuple.Create (_changedObject, _changedObjectEventReceiverMock),
            Tuple.Create (_deletedObject, _deletedObjectEventReceiverMock));
      }
      _mockRepository.ReplayAll ();

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));

      _transaction.Rollback ();

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObject ()
    {
      using (_mockRepository.Ordered ())
      {
        ExpectRollingBackEvents (
            Tuple.Create (_changedObject, _changedObjectEventReceiverMock),
            Tuple.Create (_newObject, _newObjectEventReceiverMock),
            Tuple.Create (_deletedObject, _deletedObjectEventReceiverMock))
          // This triggers one additional run
            .WhenCalled (mi => _transaction.Execute (() => _unchangedObject.MarkAsChanged ()));

        ExpectRollingBackEvents (
            Tuple.Create (_unchangedObject, _unchangedObjectEventReceiverMock))
          // This does not trigger an additional run because the object is no longer new to the Rollback set
            .WhenCalled (mi => _transaction.Execute (() => _unchangedObject.MarkAsChanged ()));

        ExpectRolledBackEvents (
            Tuple.Create (_changedObject, _changedObjectEventReceiverMock),
            Tuple.Create (_deletedObject, _deletedObjectEventReceiverMock),
            Tuple.Create (_unchangedObject, _unchangedObjectEventReceiverMock));
      }
      _mockRepository.ReplayAll ();

      _transaction.Rollback ();

      _mockRepository.VerifyAll ();
    }
    
    private DomainObject GetDeletedObject ()
    {
      return _transaction.Execute (
          () =>
          {
            var instance = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
            instance.Delete ();
            return instance;
          });
    }

    private DomainObject GetNewObject ()
    {
      return _transaction.Execute (() => ClassWithAllDataTypes.NewObject ());
    }

    private DomainObject GetChangedObject ()
    {
      return _transaction.Execute (
          () =>
          {
            var instance = Order.GetObject (DomainObjectIDs.Order1);
            instance.MarkAsChanged ();
            return instance;
          });
    }

    private DomainObject GetUnchangedObject ()
    {
      return _transaction.Execute (() => Customer.GetObject (DomainObjectIDs.Customer1));
    }

    private ReadOnlyCollection<DomainObject> ArgIsRollbackSet (params DomainObject[] domainObjects)
    {
      return Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (domainObjects);
    }

    private DomainObjectMockEventReceiver CreateDomainObjectMockEventReceiver (DomainObject changedObject)
    {
      return _mockRepository.StrictMock<DomainObjectMockEventReceiver> (changedObject);
    }

    private IMethodOptions<RhinoMocksExtensions.VoidType> ExpectRollingBackEvents (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
    {
      using (_mockRepository.Ordered ())
      {
        var domainObjects = domainObjectsAndMocks.Select (t => t.Item1).ToArray ();
        var methodOptions = _extensionMock
            .Expect (mock => mock.RollingBack (Arg.Is (_transaction), ArgIsRollbackSet (domainObjects)));
        _listenerMock
            .Expect (mock => mock.TransactionRollingBack (Arg.Is (_transaction), ArgIsRollbackSet (domainObjects)));
        _transactionMockEventReceiver
            .Expect (mock => mock.RollingBack (_transaction, domainObjects))
            .WithCurrentTransaction (_transaction);

        using (_mockRepository.Unordered ())
        {
          foreach (var domainObjectAndMock in domainObjectsAndMocks)
          {
            var copy = domainObjectAndMock;
            domainObjectAndMock.Item2.Expect (mock => mock.RollingBack (copy.Item1)).WithCurrentTransaction (_transaction);
          }
        }
        return methodOptions;
      }
    }

    private void ExpectRolledBackEvents (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
    {
      using (_mockRepository.Ordered ())
      {
        using (_mockRepository.Unordered ())
        {
          foreach (var domainObjectAndMock in domainObjectsAndMocks)
          {
            var copy = domainObjectAndMock;
            domainObjectAndMock.Item2.Expect (mock => mock.RolledBack (copy.Item1)).WithCurrentTransaction (_transaction);
          }
        }

        var domainObjects = domainObjectsAndMocks.Select (t => t.Item1).ToArray ();

        _transactionMockEventReceiver
            .Expect (mock => mock.RolledBack (_transaction, domainObjects))
            .WithCurrentTransaction (_transaction);
        _extensionMock
            .Expect (mock => mock.RolledBack (Arg.Is (_transaction), ArgIsRollbackSet (domainObjects)));
        _listenerMock
            .Expect (mock => mock.TransactionRolledBack (Arg.Is (_transaction), ArgIsRollbackSet (domainObjects)));
      }
    }
  }
}