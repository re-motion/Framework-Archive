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
using System.Reflection;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Creates all parts necessary to construct a <see cref="ClientTransaction"/> with root-transaction semantics.
  /// </summary>
  [Serializable]
  public class RootClientTransactionComponentFactory : IClientTransactionComponentFactory
  {
    public static RootClientTransactionComponentFactory Create()
    {
      return ObjectFactory.Create<RootClientTransactionComponentFactory> (true, ParamList.Empty);
    }

    protected RootClientTransactionComponentFactory ()
    {
    }

    public ClientTransaction GetParentTransaction (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return null;
    }

    public virtual Dictionary<Enum, object> CreateApplicationData (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return ClientTransactionComponentFactoryUtility.CreateApplicationData ();
    }

    public virtual IEnumerable<IClientTransactionListener> CreateListeners (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return ClientTransactionComponentFactoryUtility.GetListenersFromServiceLocator (constructedTransaction);
    }

    public virtual IEnlistedDomainObjectManager CreateEnlistedObjectManager (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return new DictionaryBasedEnlistedDomainObjectManager ();
    }

    public IInvalidDomainObjectManager CreateInvalidDomainObjectManager (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return new RootInvalidDomainObjectManager ();
    }

    public virtual IPersistenceStrategy CreatePersistenceStrategy (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return ObjectFactory.Create<RootPersistenceStrategy> (true, ParamList.Create (constructedTransaction.ID));
    }

    public virtual IObjectLoader CreateObjectLoader (
        ClientTransaction constructedTransaction,
        IPersistenceStrategy persistenceStrategy,
        IClientTransactionListener eventSink)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);

      return ClientTransactionComponentFactoryUtility.CreateObjectLoader (constructedTransaction, persistenceStrategy, eventSink);
    }

    public virtual IDataManager CreateDataManager (
        ClientTransaction constructedTransaction, 
        IInvalidDomainObjectManager invalidDomainObjectManager, 
        IObjectLoader objectLoader)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull ("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull ("objectLoader", objectLoader);

      Func<DataManager, IRelationEndPointManager> endPointManagerFactory = dataManager => CreateRelationEndPointManager (
          constructedTransaction,
          GetEndPointProvider (dataManager),
          GetLazyLoader (dataManager));
      return new DataManager (constructedTransaction, invalidDomainObjectManager, objectLoader, endPointManagerFactory);
    }

    public IQueryManager CreateQueryManager (
        ClientTransaction constructedTransaction,
        IPersistenceStrategy persistenceStrategy,
        IObjectLoader objectLoader,
        IDataManager dataManager,
        IClientTransactionListener eventSink)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("objectLoader", objectLoader);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);

      return ClientTransactionComponentFactoryUtility.CreateQueryManager (constructedTransaction, persistenceStrategy, objectLoader, dataManager, eventSink);
    }

    public virtual ClientTransactionExtensionCollection CreateExtensionCollection (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return ClientTransactionComponentFactoryUtility.CreateExtensionCollectionFromServiceLocator (constructedTransaction);
    }

    public virtual Func<ClientTransaction, ClientTransaction> CreateCloneFactory ()
    {
      return templateTransaction => (ClientTransaction) TypesafeActivator
        .CreateInstance (templateTransaction.GetType (), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        .With (this);
    }

    protected virtual ILazyLoader GetLazyLoader (DataManager dataManager)
    {
      return dataManager;
    }

    protected virtual IRelationEndPointProvider GetEndPointProvider (DataManager dataManager)
    {
      return dataManager;
    }

    protected virtual IRelationEndPointManager CreateRelationEndPointManager (
        ClientTransaction constructedTransaction,
        IRelationEndPointProvider endPointProvider, 
        ILazyLoader lazyLoader)
    {
      var endPointChangeDetectionStrategy = new RootCollectionEndPointChangeDetectionStrategy();
      var collectionEndPointDataKeeperFactory = new CollectionEndPointDataKeeperFactory (constructedTransaction, endPointChangeDetectionStrategy);
      var virtualObjectEndPointDataKeeperFactory = new VirtualObjectEndPointDataKeeperFactory (constructedTransaction);

      var relationEndPointFactory = new RelationEndPointFactory (
          constructedTransaction,
          endPointProvider,
          lazyLoader,
          virtualObjectEndPointDataKeeperFactory,
          collectionEndPointDataKeeperFactory);
      var relationEndPointRegistrationAgent = new RootRelationEndPointRegistrationAgent (endPointProvider);
      return new RelationEndPointManager (constructedTransaction, lazyLoader, relationEndPointFactory, relationEndPointRegistrationAgent);
    }
  }
}