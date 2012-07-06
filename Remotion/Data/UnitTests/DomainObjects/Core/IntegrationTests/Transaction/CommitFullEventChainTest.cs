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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Rhino.Mocks;
using System.Linq;
using Remotion.FunctionalProgramming;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class CommitFullEventChainTest : StandardMappingTest
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

      _transaction = ClientTransaction.CreateRootTransaction().CreateSubTransaction();

      _unchangedObject = GetUnchangedObject ();
      _changedObject = GetChangedObject ();
      _newObject = GetNewObject ();
      _deletedObject = GetDeletedObject ();

      _mockRepository = new MockRepository ();

      // Listener is a dynamic mock so that we don't have to expect all the internal events of committing
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
        ExpectCommittingEvents (
            Tuple.Create (_changedObject, _changedObjectEventReceiverMock),
            Tuple.Create (_newObject, _newObjectEventReceiverMock),
            Tuple.Create (_deletedObject, _deletedObjectEventReceiverMock));

        ExpectCommitValidateEvents (_changedObject, _newObject, _deletedObject);

        ExpectCommittedEvents (
            Tuple.Create (_changedObject, _changedObjectEventReceiverMock),
            Tuple.Create (_newObject, _newObjectEventReceiverMock));
      }
      _mockRepository.ReplayAll ();

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));

      _transaction.Commit ();
      
      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObject ()
    {
      using (_mockRepository.Ordered ())
      {
        ExpectCommittingEvents (
            Tuple.Create (_changedObject, _changedObjectEventReceiverMock),
            Tuple.Create (_newObject, _newObjectEventReceiverMock),
            Tuple.Create (_deletedObject, _deletedObjectEventReceiverMock))
            // This triggers one additional run
            .WhenCalled (mi => _transaction.Execute (() => _unchangedObject.MarkAsChanged()));

        ExpectCommittingEvents (
            Tuple.Create (_unchangedObject, _unchangedObjectEventReceiverMock))
            // This does not trigger an additional run because the object is no longer new to the commit set
            .WhenCalled (mi => _transaction.Execute (() => _unchangedObject.MarkAsChanged ()));

        ExpectCommitValidateEvents (_changedObject, _newObject, _deletedObject, _unchangedObject);

        ExpectCommittedEvents (
            Tuple.Create (_changedObject, _changedObjectEventReceiverMock),
            Tuple.Create (_newObject, _newObjectEventReceiverMock),
            Tuple.Create (_unchangedObject, _unchangedObjectEventReceiverMock));
      }
      _mockRepository.ReplayAll ();

      _transaction.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToRegisterForAdditionalCommittingEvents ()
    {
      using (_mockRepository.Ordered ())
      {
        ExpectCommittingEvents (
            Tuple.Create (_changedObject, _changedObjectEventReceiverMock),
            Tuple.Create (_newObject, _newObjectEventReceiverMock),
            Tuple.Create (_deletedObject, _deletedObjectEventReceiverMock))
            // This triggers _one_ (not two) additional run for _changedObject
            .WhenCalled (mi => _transaction.Execute (() =>
            {
              ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (_changedObject);
              ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (_changedObject);
            }));

        ExpectCommittingEvents (Tuple.Create (_changedObject, _changedObjectEventReceiverMock))
            // This triggers one additional run for _newObject
            .WhenCalled (
                mi => _transaction.Execute (() => ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (_newObject)));

        ExpectCommittingEvents (Tuple.Create (_newObject, _newObjectEventReceiverMock))
            // This triggers one additional run for _newObject
            .WhenCalled (
                mi => _transaction.Execute (() => ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (_newObject)));

        ExpectCommittingEvents (Tuple.Create (_newObject, _newObjectEventReceiverMock));

        ExpectCommitValidateEvents (_changedObject, _newObject, _deletedObject);

        // Committed events
        ExpectCommittedEvents (
            Tuple.Create (_changedObject, _changedObjectEventReceiverMock),
            Tuple.Create (_newObject, _newObjectEventReceiverMock));
      }
      _mockRepository.ReplayAll ();

      _transaction.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObjectAndRegisterForAdditionalCommittingEvents ()
    {
      using (_mockRepository.Ordered ())
      {
        ExpectCommittingEvents (
            Tuple.Create (_changedObject, _changedObjectEventReceiverMock),
            Tuple.Create (_newObject, _newObjectEventReceiverMock),
            Tuple.Create (_deletedObject, _deletedObjectEventReceiverMock))
            // This triggers _one_ (not two) additional run for _unchangedObject
            .WhenCalled (mi => _transaction.Execute (() =>
            {
              Assert.That (
                  () => ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (_unchangedObject),
                  Throws.ArgumentException.With.Message.EqualTo (
                      string.Format (
                          "The given DomainObject '{0}' cannot be registered due to its state (Unchanged). Only objects that are part of the commit set "
                          + "can be registered. Use MarkAsChanged to add an unchanged object to the commit set.\r\nParameter name: domainObjects",
                          _unchangedObject.ID)));
               _unchangedObject.MarkAsChanged();
               ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (_unchangedObject);
            }));

        ExpectCommittingEvents (Tuple.Create (_unchangedObject, _unchangedObjectEventReceiverMock));
        
        ExpectCommitValidateEvents (_changedObject, _newObject, _deletedObject, _unchangedObject);

        // Committed events
        ExpectCommittedEvents (
            Tuple.Create (_changedObject, _changedObjectEventReceiverMock),
            Tuple.Create (_newObject, _newObjectEventReceiverMock),
            Tuple.Create (_unchangedObject, _unchangedObjectEventReceiverMock));
      }
      _mockRepository.ReplayAll ();

      _transaction.Commit ();

      _mockRepository.VerifyAll ();
    }

    private DomainObject GetDeletedObject ()
    {
      return _transaction.Execute (
          () =>
          {
            var instance = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
            instance.Delete();
            return instance;
          });
    }

    private DomainObject GetNewObject ()
    {
      return _transaction.Execute (() => ClassWithAllDataTypes.NewObject());
    }

    private DomainObject GetChangedObject ()
    {
      return _transaction.Execute (
          () =>
          {
            var instance = Order.GetObject (DomainObjectIDs.Order1);
            instance.MarkAsChanged();
            return instance;
          });
    }

    private DomainObject GetUnchangedObject ()
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

    private DomainObjectMockEventReceiver CreateDomainObjectMockEventReceiver (DomainObject changedObject)
    {
      return _mockRepository.StrictMock<DomainObjectMockEventReceiver> (changedObject);
    }

    private IMethodOptions<RhinoMocksExtensions.VoidType> ExpectCommittingEvents (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
    {
      using (_mockRepository.Ordered ())
      {
        var domainObjects = domainObjectsAndMocks.Select (t => t.Item1).ToArray();
        var methodOptions = _extensionMock
            .Expect (mock => mock.Committing (Arg.Is (_transaction), ArgIsCommitSet (domainObjects), Arg<CommittingEventRegistrar>.Is.TypeOf));
        _listenerMock
            .Expect (mock => mock.TransactionCommitting (Arg.Is (_transaction), ArgIsCommitSet (domainObjects), Arg<CommittingEventRegistrar>.Is.TypeOf));
        _transactionMockEventReceiver
            .Expect (mock => mock.Committing (_transaction, domainObjects))
            .WithCurrentTransaction (_transaction);

        using (_mockRepository.Unordered())
        {
          foreach (var domainObjectAndMock in domainObjectsAndMocks)
          {
            var copy = domainObjectAndMock;
            domainObjectAndMock.Item2.Expect (mock => mock.Committing (copy.Item1)).WithCurrentTransaction (_transaction);
          }
        }
        return methodOptions;
      }
    }

    private void ExpectCommittedEvents (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
    {
      using (_mockRepository.Ordered ())
      {
        using (_mockRepository.Unordered())
        {
          foreach (var domainObjectAndMock in domainObjectsAndMocks)
          {
            var copy = domainObjectAndMock;
            domainObjectAndMock.Item2.Expect (mock => mock.Committed (copy.Item1)).WithCurrentTransaction (_transaction);
          }
        }

        var domainObjects = domainObjectsAndMocks.Select (t => t.Item1).ToArray();

        _transactionMockEventReceiver
            .Expect (mock => mock.Committed (_transaction, domainObjects))
            .WithCurrentTransaction (_transaction);
        _extensionMock
            .Expect (mock => mock.Committed (Arg.Is (_transaction), ArgIsCommitSet (domainObjects)));
        _listenerMock
            .Expect (mock => mock.TransactionCommitted (Arg.Is (_transaction), ArgIsCommitSet (domainObjects)));
      }
    }

    private void ExpectCommitValidateEvents (params DomainObject[] domainObjects)
    {
      using (_mockRepository.Ordered ())
      {
        _extensionMock
            .Expect (mock => mock.CommitValidate (Arg.Is (_transaction), ArgIsPersistableDataSet (domainObjects)));
        _listenerMock
            .Expect (mock => mock.TransactionCommitValidate (Arg.Is (_transaction), ArgIsPersistableDataSet (domainObjects)))
            .WhenCalled (mi => Assert.That (_transaction.HasChanged(), Is.True, "CommitValidate: last event before actual commit."));
      }
    }
  }
}