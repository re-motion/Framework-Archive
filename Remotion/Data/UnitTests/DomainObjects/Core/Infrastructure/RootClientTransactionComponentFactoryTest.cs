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
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class RootClientTransactionComponentFactoryTest
  {
    private RootClientTransactionComponentFactory _factory;
    private ClientTransactionMock _fakeConstructedTransaction;

    [SetUp]
    public void SetUp ()
    {
      _factory = RootClientTransactionComponentFactory.Create ();
      _fakeConstructedTransaction = new ClientTransactionMock ();
    }

    [Test]
    public void GetParentTransaction ()
    {
      Assert.That (_factory.GetParentTransaction (_fakeConstructedTransaction), Is.Null);
    }

    [Test]
    public void CreateApplicationData ()
    {
      var applicationData = _factory.CreateApplicationData (_fakeConstructedTransaction);

      Assert.That (applicationData, Is.Not.Null);
      Assert.That (applicationData.Count, Is.EqualTo (0));
    }

    [Test]
    public void CreateListeners ()
    {
     IEnumerable<IClientTransactionListener> listeners = _factory.CreateListeners (_fakeConstructedTransaction).ToArray();
     Assert.That (listeners, Has.Length.EqualTo (1).And.Some.TypeOf<LoggingClientTransactionListener>());
    }
    
    [Test]
    public void CreateInvalidDomainObjectManager ()
    {
      var manager = _factory.CreateInvalidDomainObjectManager (_fakeConstructedTransaction);
      Assert.That (manager, Is.TypeOf (typeof (RootInvalidDomainObjectManager)));
      Assert.That (((RootInvalidDomainObjectManager) manager).InvalidObjectCount, Is.EqualTo (0));
    }

    [Test]
    public void CreatePersistenceStrategy ()
    {
      var result = _factory.CreatePersistenceStrategy (_fakeConstructedTransaction);

      Assert.That (result, Is.TypeOf<RootPersistenceStrategy> ());
      Assert.That (((RootPersistenceStrategy) result).TransactionID, Is.EqualTo (_fakeConstructedTransaction.ID));
    }

    [Test]
    public void CreatePersistenceStrategy_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<RootPersistenceStrategy> ().AddMixin<NullMixin> ().EnterScope ())
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
      var persistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy>();

      var dataManager = (DataManager) _factory.CreateDataManager (_fakeConstructedTransaction, eventSink, invalidDomainObjectManager, persistenceStrategy);
      Assert.That (dataManager.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (DataManagerTestHelper.GetInvalidDomainObjectManager (dataManager), Is.SameAs (invalidDomainObjectManager));

      Assert.That (DataManagerTestHelper.GetObjectLoader (dataManager), Is.TypeOf (typeof (ObjectLoader)));
      var objectLoader = (ObjectLoader) DataManagerTestHelper.GetObjectLoader (dataManager);
      Assert.That (objectLoader.PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (objectLoader.EagerFetcher, Is.TypeOf<EagerFetcher>());
      Assert.That (
          objectLoader.LoadedObjectDataRegistrationAgent,
          Is.TypeOf<LoadedObjectDataRegistrationAgent>()
              .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.ClientTransaction).SameAs (_fakeConstructedTransaction)
              .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.TransactionEventSink).SameAs (eventSink));
      CheckDelegatingDataManager (objectLoader.DataContainerLifetimeManager, dataManager);
      
      Assert.That (objectLoader.LoadedObjectDataProvider, Is.TypeOf<LoadedObjectDataProvider>());
      var loadedObjectDataProvider = (LoadedObjectDataProvider) objectLoader.LoadedObjectDataProvider;
      CheckDelegatingDataManager (loadedObjectDataProvider.LoadedDataContainerProvider, dataManager);
      Assert.That (loadedObjectDataProvider.InvalidDomainObjectManager, Is.SameAs (invalidDomainObjectManager));

      var eagerFetcher = ((EagerFetcher) objectLoader.EagerFetcher);
      CheckDelegatingDataManager (eagerFetcher.LoadedDataContainerProvider, dataManager);
      CheckDelegatingDataManager (eagerFetcher.VirtualEndPointProvider, dataManager);
      Assert.That (eagerFetcher.RegistrationAgent, Is.TypeOf<DelegatingFetchedRelationDataRegistrationAgent>());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).RealObjectDataRegistrationAgent,
          Is.TypeOf<FetchedRealObjectRelationDataRegistrationAgent>());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).VirtualObjectDataRegistrationAgent,
          Is.TypeOf<FetchedVirtualObjectRelationDataRegistrationAgent>());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).CollectionDataRegistrationAgent,
          Is.TypeOf<FetchedCollectionRelationDataRegistrationAgent>());

      var relationEndPointManager = (RelationEndPointManager) DataManagerTestHelper.GetRelationEndPointManager (dataManager);
      Assert.That (relationEndPointManager.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (relationEndPointManager.EndPointFactory, Is.TypeOf<RelationEndPointFactory>());
      Assert.That (relationEndPointManager.RegistrationAgent, Is.TypeOf<RootRelationEndPointRegistrationAgent>());

      var endPointFactory = ((RelationEndPointFactory) relationEndPointManager.EndPointFactory);
      Assert.That (endPointFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (endPointFactory.LazyLoader, Is.SameAs (dataManager));
      Assert.That (endPointFactory.EndPointProvider, Is.SameAs (dataManager));
      Assert.That (endPointFactory.CollectionEndPointDataKeeperFactory, Is.TypeOf (typeof (CollectionEndPointDataKeeperFactory)));

      var collectionEndPointDataKeeperFactory = ((CollectionEndPointDataKeeperFactory) endPointFactory.CollectionEndPointDataKeeperFactory);
      Assert.That (collectionEndPointDataKeeperFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (collectionEndPointDataKeeperFactory.ChangeDetectionStrategy, Is.TypeOf<RootCollectionEndPointChangeDetectionStrategy>());
      Assert.That (endPointFactory.VirtualObjectEndPointDataKeeperFactory, Is.TypeOf<VirtualObjectEndPointDataKeeperFactory>());

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

      Assert.That (extensions.Count, Is.EqualTo (2));
      Assert.That (extensions[1], Is.SameAs (extensionStub));
      Assert.That (extensions[0], Is.TypeOf<CommitValidationClientTransactionExtension> ());
      
      var validationExtension = (CommitValidationClientTransactionExtension) extensions[0];
      var validator = validationExtension.ValidatorFactory (_fakeConstructedTransaction);
      Assert.That (validator, Is.TypeOf<MandatoryRelationValidator>());
    }

    private void CheckDelegatingDataManager (object actual, DataManager expectedDataManager)
    {
      Assert.That (
          actual,
          Is.TypeOf<DelegatingDataManager> ().With.Property<DelegatingDataManager> (dm => dm.InnerDataManager).SameAs (expectedDataManager));
    }
  }
}