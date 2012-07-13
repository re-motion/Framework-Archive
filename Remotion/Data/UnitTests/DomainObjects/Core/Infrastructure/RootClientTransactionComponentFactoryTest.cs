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
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Rhino.Mocks;
using Remotion.Data.UnitTests.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class RootClientTransactionComponentFactoryTest
  {
    private RootClientTransactionComponentFactory _factory;
    private TestableClientTransaction _fakeConstructedTransaction;

    [SetUp]
    public void SetUp ()
    {
      _factory = RootClientTransactionComponentFactory.Create ();
      _fakeConstructedTransaction = new TestableClientTransaction ();
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
    public void CreateEnlistedDomainObjectManager ()
    {
      var manager = _factory.CreateEnlistedObjectManager (_fakeConstructedTransaction);
      Assert.That (manager, Is.TypeOf<DictionaryBasedEnlistedDomainObjectManager>());
    }

    [Test]
    public void CreateInvalidDomainObjectManager ()
    {
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink>();
      var manager = _factory.CreateInvalidDomainObjectManager (_fakeConstructedTransaction, eventSink);
      Assert.That (manager, Is.TypeOf (typeof (InvalidDomainObjectManager)));
      Assert.That (((InvalidDomainObjectManager) manager).InvalidObjectCount, Is.EqualTo (0));
      Assert.That (((InvalidDomainObjectManager) manager).TransactionEventSink, Is.SameAs (eventSink));
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
      Assert.That (extensions[0], Is.TypeOf<CommitValidationClientTransactionExtension> ());
      Assert.That (extensions[1], Is.SameAs (extensionStub));
      
      var validationExtension = (CommitValidationClientTransactionExtension) extensions[0];
      var validator = validationExtension.ValidatorFactory (_fakeConstructedTransaction);
      Assert.That (validator, Is.TypeOf<MandatoryRelationValidator>());
    }

    [Test]
    public void CreateRelationEndPointManager ()
    {
      var lazyLoader = MockRepository.GenerateStub<ILazyLoader> ();
      var endPointProvider = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink> ();

      var relationEndPointManager =
          (RelationEndPointManager) PrivateInvoke.InvokeNonPublicMethod (
              _factory,
              "CreateRelationEndPointManager",
              _fakeConstructedTransaction,
              endPointProvider,
              lazyLoader,
              eventSink);

      Assert.That (relationEndPointManager.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (relationEndPointManager.RegistrationAgent, Is.TypeOf<RootRelationEndPointRegistrationAgent> ());
      Assert.That (relationEndPointManager.EndPointFactory, Is.TypeOf<StateUpdateRaisingRelationEndPointFactoryDecorator> ());

      Assert.That (RelationEndPointManagerTestHelper.GetMap (relationEndPointManager).TransactionEventSink, Is.SameAs (eventSink));
      
      var stateUpdateRaisingFactory = (StateUpdateRaisingRelationEndPointFactoryDecorator) relationEndPointManager.EndPointFactory;
      Assert.That (
          stateUpdateRaisingFactory.Listener,
          Is.TypeOf<VirtualEndPointStateUpdateListener>()
              .With.Property<VirtualEndPointStateUpdateListener> (l => l.TransactionEventSink).SameAs (eventSink));
      Assert.That (stateUpdateRaisingFactory.InnerFactory, Is.TypeOf<RelationEndPointFactory> ());
      var endPointFactory = ((RelationEndPointFactory) stateUpdateRaisingFactory.InnerFactory);
      Assert.That (endPointFactory.ClientTransaction, Is.SameAs (_fakeConstructedTransaction));
      Assert.That (endPointFactory.LazyLoader, Is.SameAs (lazyLoader));
      Assert.That (endPointFactory.EndPointProvider, Is.SameAs (endPointProvider));
      Assert.That (endPointFactory.TransactionEventSink, Is.SameAs (eventSink));

      Assert.That (endPointFactory.CollectionEndPointDataManagerFactory, Is.TypeOf (typeof (CollectionEndPointDataManagerFactory)));
      var collectionEndPointDataManagerFactory = ((CollectionEndPointDataManagerFactory) endPointFactory.CollectionEndPointDataManagerFactory);
      Assert.That (collectionEndPointDataManagerFactory.ChangeDetectionStrategy, Is.TypeOf<RootCollectionEndPointChangeDetectionStrategy> ());
      Assert.That (endPointFactory.VirtualObjectEndPointDataManagerFactory, Is.TypeOf<VirtualObjectEndPointDataManagerFactory> ());

      Assert.That (endPointFactory.CollectionEndPointCollectionProvider, Is.TypeOf<CollectionEndPointCollectionProvider> ());
      var collectionEndPointCollectionProvider = (CollectionEndPointCollectionProvider) endPointFactory.CollectionEndPointCollectionProvider;
      Assert.That (
          collectionEndPointCollectionProvider.DataStrategyFactory,
          Is.TypeOf<AssociatedCollectionDataStrategyFactory> ()
              .With.Property ((AssociatedCollectionDataStrategyFactory f) => f.VirtualEndPointProvider).SameAs (endPointProvider));
    }

    [Test]
    public void CreateObjectLoader ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IFetchEnabledPersistenceStrategy> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink> ();

      var fakeBasicObjectLoader = MockRepository.GenerateStub<IFetchEnabledObjectLoader> ();

      var factoryPartialMock = MockRepository.GeneratePartialMock<RootClientTransactionComponentFactory> ();
      factoryPartialMock
          .Expect (mock => CallCreateBasicObjectLoader (
              mock, 
              _fakeConstructedTransaction,
              eventSink,
              persistenceStrategy,
              invalidDomainObjectManager,
              dataManager))
          .Return (fakeBasicObjectLoader);
      factoryPartialMock.Replay ();

      var result = PrivateInvoke.InvokeNonPublicMethod (
          factoryPartialMock, 
          "CreateObjectLoader",
          _fakeConstructedTransaction,
          eventSink,
          persistenceStrategy,
          invalidDomainObjectManager,
          dataManager);

      Assert.That (result, Is.TypeOf (typeof (EagerFetchingObjectLoaderDecorator)));
      var objectLoader = (EagerFetchingObjectLoaderDecorator) result;
      Assert.That (objectLoader.DecoratedObjectLoader, Is.SameAs (fakeBasicObjectLoader));
      Assert.That (objectLoader.EagerFetcher, Is.TypeOf<EagerFetcher> ());

      var eagerFetcher = (EagerFetcher) objectLoader.EagerFetcher;
      Assert.That (eagerFetcher.RegistrationAgent, Is.TypeOf<DelegatingFetchedRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).RealObjectDataRegistrationAgent,
          Is.TypeOf<FetchedRealObjectRelationDataRegistrationAgent> ());
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).VirtualObjectDataRegistrationAgent,
          Is.TypeOf<FetchedVirtualObjectRelationDataRegistrationAgent> ()
              .With.Property<FetchedCollectionRelationDataRegistrationAgent> (a => a.VirtualEndPointProvider).SameAs (dataManager));
      Assert.That (
          ((DelegatingFetchedRelationDataRegistrationAgent) eagerFetcher.RegistrationAgent).CollectionDataRegistrationAgent,
          Is.TypeOf<FetchedCollectionRelationDataRegistrationAgent> ()
              .With.Property<FetchedCollectionRelationDataRegistrationAgent> (a => a.VirtualEndPointProvider).SameAs (dataManager));
    }

    [Test]
    public void CreateBasicObjectLoader ()
    {
      var persistenceStrategy = MockRepository.GenerateStub<IFetchEnabledPersistenceStrategy> ();
      var dataManager = MockRepository.GenerateStub<IDataManager> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var eventSink = MockRepository.GenerateStub<IClientTransactionEventSink> ();

      var result = CallCreateBasicObjectLoader (
          _factory,
          _fakeConstructedTransaction,
          eventSink,
          persistenceStrategy,
          invalidDomainObjectManager,
          dataManager);

      Assert.That (result, Is.TypeOf (typeof (FetchEnabledObjectLoader)));
      var objectLoader = (ObjectLoader) result;
      Assert.That (objectLoader.PersistenceStrategy, Is.SameAs (persistenceStrategy));
      Assert.That (
          objectLoader.LoadedObjectDataRegistrationAgent,
          Is.TypeOf<LoadedObjectDataRegistrationAgent> ()
              .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.ClientTransaction).SameAs (_fakeConstructedTransaction)
              .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.DataManager).SameAs (dataManager)
              .With.Property ((LoadedObjectDataRegistrationAgent agent) => agent.TransactionEventSink).SameAs (eventSink));

      Assert.That (objectLoader.LoadedObjectDataProvider, Is.TypeOf<LoadedObjectDataProvider> ());
      var loadedObjectDataProvider = (LoadedObjectDataProvider) objectLoader.LoadedObjectDataProvider;
      Assert.That (loadedObjectDataProvider.LoadedDataContainerProvider, Is.SameAs (dataManager));
      Assert.That (loadedObjectDataProvider.InvalidDomainObjectManager, Is.SameAs (invalidDomainObjectManager));
    }

    private object CallCreateBasicObjectLoader (
        RootClientTransactionComponentFactory factory,
        TestableClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IFetchEnabledPersistenceStrategy persistenceStrategy,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IDataManager dataManager)
    {
      return PrivateInvoke.InvokeNonPublicMethod (
          factory,
          "CreateBasicObjectLoader",
          constructedTransaction,
          eventSink,
          persistenceStrategy,
          invalidDomainObjectManager,
          dataManager);
    }
  }
}