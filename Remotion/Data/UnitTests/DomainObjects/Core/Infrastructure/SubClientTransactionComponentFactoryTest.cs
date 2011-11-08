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
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Rhino.Mocks;
using Remotion.Data.UnitTests.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class SubClientTransactionComponentFactoryTest : StandardMappingTest
  {
    private ClientTransactionMock _parentTransaction;
    private IInvalidDomainObjectManager _parentInvalidDomainObjectManagerStub;
    private SubClientTransactionComponentFactory _factory;
    private ClientTransactionMock _fakeConstructedTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _parentTransaction = new ClientTransactionMock ();
      _parentInvalidDomainObjectManagerStub = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      _factory = SubClientTransactionComponentFactory.Create (_parentTransaction, _parentInvalidDomainObjectManagerStub);
      _fakeConstructedTransaction = new ClientTransactionMock ();
    }

    [Test]
    public void GetParentTransaction ()
    {
      Assert.That (_factory.GetParentTransaction (_fakeConstructedTransaction), Is.SameAs (_parentTransaction));
    }

    [Test]
    public void CreateApplicationData ()
    {
      Assert.That (_factory.CreateApplicationData (_fakeConstructedTransaction), Is.SameAs (_parentTransaction.ApplicationData));
    }

    [Test]
    public void CreateListeners ()
    {
      IEnumerable<IClientTransactionListener> listeners = _factory.CreateListeners (_fakeConstructedTransaction).ToArray();
      Assert.That (
          listeners,
          Has
              .Length.EqualTo (2)
              .And.Some.TypeOf<LoggingClientTransactionListener>()
              .And.Some.TypeOf<SubClientTransactionListener>());

      var listener = listeners.OfType<SubClientTransactionListener>().Single();
      Assert.That (listener.ParentInvalidDomainObjectManager, Is.SameAs (_parentInvalidDomainObjectManagerStub));
    }

    [Test]
    public void CreateEnlistedObjectManager ()
    {
      var manager = _factory.CreateEnlistedObjectManager (_fakeConstructedTransaction);
      Assert.That (manager, Is.TypeOf (typeof (DelegatingEnlistedDomainObjectManager)));
      Assert.That (((DelegatingEnlistedDomainObjectManager) manager).TargetTransaction, Is.SameAs (_parentTransaction));
    }

    [Test]
    public void CreateInvalidDomainObjectManager ()
    {
      _parentInvalidDomainObjectManagerStub.Stub (stub => stub.InvalidObjectIDs).Return (new ObjectID[0]);

      var manager = _factory.CreateInvalidDomainObjectManager (_fakeConstructedTransaction);
      Assert.That (manager, Is.TypeOf (typeof (SubInvalidDomainObjectManager)));
      Assert.That (((SubInvalidDomainObjectManager) manager).ParentTransactionManager, Is.SameAs (_parentInvalidDomainObjectManagerStub));
    }

    [Test]
    public void CreateInvalidDomainObjectManager_AutomaticallyMarksInvalid_ObjectsInvalidOrDeletedInParentTransaction ()
    {
      var objectInvalidInParent = _parentTransaction.Execute (() => Order.NewObject ());
      var objectDeletedInParent = _parentTransaction.GetObject (DomainObjectIDs.Order2, false);
      var objectLoadedInParent = _parentTransaction.GetObject (DomainObjectIDs.Order3, false);

      _parentInvalidDomainObjectManagerStub.Stub (stub => stub.InvalidObjectIDs).Return (new[] { objectInvalidInParent.ID });
      _parentInvalidDomainObjectManagerStub.Stub (stub => stub.GetInvalidObjectReference (objectInvalidInParent.ID)).Return (objectInvalidInParent);
      
      _parentTransaction.Delete (objectDeletedInParent);

      var invalidOjectManager = _factory.CreateInvalidDomainObjectManager (_fakeConstructedTransaction);

      Assert.That (invalidOjectManager.IsInvalid (objectInvalidInParent.ID), Is.True);
      Assert.That (invalidOjectManager.IsInvalid (objectDeletedInParent.ID), Is.True);
      Assert.That (invalidOjectManager.IsInvalid (objectLoadedInParent.ID), Is.False);
    }

    [Test]
    public void CreatePersistenceStrategy ()
    {
      _parentTransaction.IsReadOnly = true;

      var result = _factory.CreatePersistenceStrategy (_fakeConstructedTransaction);

      Assert.That (result, Is.TypeOf<SubPersistenceStrategy> ());
      var parentTransactionContext = ((SubPersistenceStrategy) result).ParentTransactionContext;
      Assert.That (parentTransactionContext, Is.TypeOf<ParentTransactionContext>());
      Assert.That (((ParentTransactionContext) parentTransactionContext).ParentTransaction, Is.SameAs (_parentTransaction));
      Assert.That (
          ((ParentTransactionContext) parentTransactionContext).ParentInvalidDomainObjectManager, 
          Is.SameAs (_parentInvalidDomainObjectManagerStub));
    }

    [Test]
    public void CreatePersistenceStrategy_CanBeMixed ()
    {
      _parentTransaction.IsReadOnly = true;

      using (MixinConfiguration.BuildNew ().ForClass<SubPersistenceStrategy> ().AddMixin<NullMixin> ().EnterScope ())
      {
        var result = _factory.CreatePersistenceStrategy (_fakeConstructedTransaction);
        Assert.That (Mixin.Get<NullMixin> (result), Is.Not.Null);
      }
    }

    [Test]
    public void CreateDataManager ()
    {
      var eventSink = MockRepository.GenerateStub<IClientTransactionListener> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();

      var dataManager = (DataManager) _factory.CreateDataManager (_fakeConstructedTransaction, eventSink, invalidDomainObjectManager, persistenceStrategy);
      Assert.That (dataManager.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (DataManagerTestHelper.GetInvalidDomainObjectManager (dataManager), Is.SameAs (invalidDomainObjectManager));

      Assert.That (DataManagerTestHelper.GetObjectLoader (dataManager), Is.TypeOf (typeof (ObjectLoader)));
      var objectLoader = (ObjectLoader) DataManagerTestHelper.GetObjectLoader (dataManager);
      Assert.That (objectLoader.PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (objectLoader.EagerFetcher, Is.TypeOf<EagerFetcher> ());
      Assert.That (
          objectLoader.LoadedObjectDataRegistrationAgent,
          Is.TypeOf<LoadedObjectDataRegistrationAgent> ()
              .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.ClientTransaction).SameAs (_fakeConstructedTransaction)
              .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.TransactionEventSink).SameAs (eventSink));

      CheckDelegatingDataManager (objectLoader.DataContainerLifetimeManager, dataManager);

      Assert.That (objectLoader.LoadedObjectDataProvider, Is.TypeOf<LoadedObjectDataProvider> ());
      var loadedObjectDataProvider = (LoadedObjectDataProvider) objectLoader.LoadedObjectDataProvider;
      CheckDelegatingDataManager (loadedObjectDataProvider.LoadedDataContainerProvider, dataManager);
      Assert.That (loadedObjectDataProvider.InvalidDomainObjectManager, Is.SameAs (invalidDomainObjectManager));
      
      var eagerFetcher = ((EagerFetcher) objectLoader.EagerFetcher);
      CheckDelegatingDataManager (eagerFetcher.LoadedDataContainerProvider, dataManager);
      CheckDelegatingDataManager (eagerFetcher.VirtualEndPointProvider, dataManager);
      Assert.That (eagerFetcher.RegistrationAgent, Is.TypeOf<DelegatingFetchedRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).RealObjectDataRegistrationAgent,
          Is.TypeOf<FetchedRealObjectRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).VirtualObjectDataRegistrationAgent,
          Is.TypeOf<FetchedVirtualObjectRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).CollectionDataRegistrationAgent,
          Is.TypeOf<FetchedCollectionRelationDataRegistrationAgent> ());

      var relationEndPointManager = (RelationEndPointManager) DataManagerTestHelper.GetRelationEndPointManager (dataManager);
      Assert.That (relationEndPointManager.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (relationEndPointManager.EndPointFactory, Is.TypeOf<RelationEndPointFactory> ());
      Assert.That (relationEndPointManager.RegistrationAgent, Is.TypeOf<RelationEndPointRegistrationAgent> ());

      var endPointFactory = ((RelationEndPointFactory) relationEndPointManager.EndPointFactory);
      Assert.That (endPointFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (endPointFactory.LazyLoader, Is.SameAs (dataManager));
      Assert.That (endPointFactory.EndPointProvider, Is.SameAs (dataManager));
      Assert.That (endPointFactory.CollectionEndPointDataKeeperFactory, Is.TypeOf (typeof (CollectionEndPointDataKeeperFactory)));

      var collectionEndPointDataKeeperFactory = ((CollectionEndPointDataKeeperFactory) endPointFactory.CollectionEndPointDataKeeperFactory);
      Assert.That (collectionEndPointDataKeeperFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (collectionEndPointDataKeeperFactory.ChangeDetectionStrategy, Is.TypeOf<SubCollectionEndPointChangeDetectionStrategy> ());
      Assert.That (endPointFactory.VirtualObjectEndPointDataKeeperFactory, Is.TypeOf<VirtualObjectEndPointDataKeeperFactory> ());

      var virtualObjectEndPointDataKeeperFactory = ((VirtualObjectEndPointDataKeeperFactory) endPointFactory.VirtualObjectEndPointDataKeeperFactory);
      Assert.That (virtualObjectEndPointDataKeeperFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
    }

    [Test]
    public void CreateQueryManager ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionListener> ();

      var result = _factory.CreateQueryManager (_fakeConstructedTransaction, eventSink, invalidDomainObjectManager, persistenceStrategy, dataManager);

      Assert.That (result, Is.TypeOf (typeof (QueryManager)));
      Assert.That (((QueryManager) result).PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (((QueryManager) result).ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (((QueryManager) result).TransactionEventSink, Is.SameAs (eventSink));

      Assert.That (((QueryManager) result).ObjectLoader, Is.TypeOf (typeof (ObjectLoader)));
      var objectLoader = (ObjectLoader) ((QueryManager) result).ObjectLoader;
      Assert.That (objectLoader.PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (objectLoader.EagerFetcher, Is.TypeOf<EagerFetcher> ());
      Assert.That (
          objectLoader.LoadedObjectDataRegistrationAgent,
          Is.TypeOf<LoadedObjectDataRegistrationAgent> ()
              .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.ClientTransaction).SameAs (_fakeConstructedTransaction)
              .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.TransactionEventSink).SameAs (eventSink));
      Assert.That (objectLoader.DataContainerLifetimeManager, Is.SameAs (dataManager));

      Assert.That (objectLoader.LoadedObjectDataProvider, Is.TypeOf<LoadedObjectDataProvider> ());
      var loadedObjectDataProvider = (LoadedObjectDataProvider) objectLoader.LoadedObjectDataProvider;
      Assert.That (loadedObjectDataProvider.LoadedDataContainerProvider, Is.SameAs (dataManager));
      Assert.That (loadedObjectDataProvider.InvalidDomainObjectManager, Is.SameAs (invalidDomainObjectManager));

      var eagerFetcher = ((EagerFetcher) objectLoader.EagerFetcher);
      Assert.That (eagerFetcher.LoadedDataContainerProvider, Is.SameAs (dataManager));
      Assert.That (eagerFetcher.VirtualEndPointProvider, Is.SameAs (dataManager));
      Assert.That (eagerFetcher.RegistrationAgent, Is.TypeOf<DelegatingFetchedRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).RealObjectDataRegistrationAgent,
          Is.TypeOf<FetchedRealObjectRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).VirtualObjectDataRegistrationAgent,
          Is.TypeOf<FetchedVirtualObjectRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).CollectionDataRegistrationAgent,
          Is.TypeOf<FetchedCollectionRelationDataRegistrationAgent> ());
    }

    [Test]
    public void CreateExtensionCollection ()
    {
      var extensionFactoryMock = MockRepository.GenerateStrictMock<IClientTransactionExtensionFactory> ();
      var extensionStub = MockRepository.GenerateStub<IClientTransactionExtension> ();
      extensionStub.Stub (stub => stub.Key).Return ("stub1");

      extensionFactoryMock.Expect (mock => mock.CreateClientTransactionExtensions (_fakeConstructedTransaction)).Return (new[] { extensionStub });
      extensionFactoryMock.Replay ();

      var serviceLocatorMock = MockRepository.GenerateStrictMock<IServiceLocator> ();
      serviceLocatorMock
          .Expect (mock => mock.GetAllInstances<IClientTransactionExtensionFactory> ())
          .Return (new[] { extensionFactoryMock });
      serviceLocatorMock.Replay ();

      ClientTransactionExtensionCollection extensions;
      using (new ServiceLocatorScope (serviceLocatorMock))
      {
        extensions = _factory.CreateExtensionCollection (_fakeConstructedTransaction);
      }

      serviceLocatorMock.VerifyAllExpectations ();
      extensionFactoryMock.VerifyAllExpectations ();

      Assert.That (extensions.Count, Is.EqualTo (1));
      Assert.That (extensions[0], Is.SameAs (extensionStub));
    }

    private void CheckDelegatingDataManager (object actual, DataManager expectedDataManager)
    {
      Assert.That (
          actual,
          Is.TypeOf<DelegatingDataManager> ().With.Property<DelegatingDataManager> (dm => dm.InnerDataManager).SameAs (expectedDataManager));
    }
  }
}