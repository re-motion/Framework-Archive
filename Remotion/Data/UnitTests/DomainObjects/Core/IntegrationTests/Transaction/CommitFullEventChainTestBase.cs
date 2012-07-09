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
using System.Linq;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.FunctionalProgramming;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  public class CommitFullEventChainTestBase : StandardMappingTest
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

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public DomainObject UnchangedObject
    {
      get { return _unchangedObject; }
    }

    public DomainObject ChangedObject
    {
      get { return _changedObject; }
    }

    public DomainObject NewObject
    {
      get { return _newObject; }
    }

    public DomainObject DeletedObject
    {
      get { return _deletedObject; }
    }

    public MockRepository MockRepository
    {
      get { return _mockRepository; }
    }

    public IClientTransactionListener ListenerMock
    {
      get { return _listenerMock; }
    }

    public IClientTransactionExtension ExtensionMock
    {
      get { return _extensionMock; }
    }

    public ClientTransactionMockEventReceiver TransactionMockEventReceiver
    {
      get { return _transactionMockEventReceiver; }
    }

    public DomainObjectMockEventReceiver ChangedObjectEventReceiverMock
    {
      get { return _changedObjectEventReceiverMock; }
    }

    public DomainObjectMockEventReceiver NewObjectEventReceiverMock
    {
      get { return _newObjectEventReceiverMock; }
    }

    public DomainObjectMockEventReceiver DeletedObjectEventReceiverMock
    {
      get { return _deletedObjectEventReceiverMock; }
    }

    public DomainObjectMockEventReceiver UnchangedObjectEventReceiverMock
    {
      get { return _unchangedObjectEventReceiverMock; }
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

    protected IMethodOptions<RhinoMocksExtensions.VoidType> ExpectCommittingEvents (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
    {
      using (_mockRepository.Ordered ())
      {
        var domainObjects = domainObjectsAndMocks.Select (t => t.Item1).ToArray();
        var methodOptions = ExtensionMock
            .Expect (mock => mock.Committing (Arg.Is (_transaction), ArgIsCommitSet (domainObjects), Arg<CommittingEventRegistrar>.Is.TypeOf));
        ListenerMock
            .Expect (mock => mock.TransactionCommitting (Arg.Is (_transaction), ArgIsCommitSet (domainObjects), Arg<CommittingEventRegistrar>.Is.TypeOf));
        TransactionMockEventReceiver
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

    protected void ExpectCommittedEvents (params Tuple<DomainObject, DomainObjectMockEventReceiver>[] domainObjectsAndMocks)
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

        TransactionMockEventReceiver
            .Expect (mock => mock.Committed (_transaction, domainObjects))
            .WithCurrentTransaction (_transaction);
        ExtensionMock
            .Expect (mock => mock.Committed (Arg.Is (_transaction), ArgIsCommitSet (domainObjects)));
        ListenerMock
            .Expect (mock => mock.TransactionCommitted (Arg.Is (_transaction), ArgIsCommitSet (domainObjects)));
      }
    }

    protected void ExpectCommitValidateEvents (params DomainObject[] domainObjects)
    {
      using (_mockRepository.Ordered ())
      {
        ExtensionMock
            .Expect (mock => mock.CommitValidate (Arg.Is (_transaction), ArgIsPersistableDataSet (domainObjects)));
        ListenerMock
            .Expect (mock => mock.TransactionCommitValidate (Arg.Is (_transaction), ArgIsPersistableDataSet (domainObjects)))
            .WhenCalled (mi => Assert.That (_transaction.HasChanged(), Is.True, "CommitValidate: last event before actual commit."));
      }
    }
  }
}