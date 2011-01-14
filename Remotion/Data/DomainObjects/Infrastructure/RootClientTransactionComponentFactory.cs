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
using System.Reflection;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Creates all parts necessary to construct a <see cref="ClientTransaction"/> with root-transaction semantics.
  /// </summary>
  [Serializable]
  public class RootClientTransactionComponentFactory : IClientTransactionComponentFactory
  {
    public virtual Dictionary<Enum, object> CreateApplicationData ()
    {
      return new Dictionary<Enum, object> ();
    }

    public virtual ClientTransactionExtensionCollection CreateExtensions ()
    {
      return new ClientTransactionExtensionCollection ();
    }

    public virtual IEnumerable<IClientTransactionListener> CreateListeners (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      var factories = SafeServiceLocator.Current.GetAllInstances<IClientTransactionListenerFactory> ();
      return factories.Select (factory => factory.CreateClientTransactionListener (clientTransaction));
    }

    public virtual IPersistenceStrategy CreatePersistenceStrategy (Guid id)
    {
      return ObjectFactory.Create<RootPersistenceStrategy> (true, ParamList.Create (id));
    }

    public virtual IObjectLoader CreateObjectLoader (ClientTransaction clientTransaction, IDataManager dataManager, IPersistenceStrategy persistenceStrategy, IClientTransactionListener eventSink)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);
      
      var eagerFetcher = new EagerFetcher (dataManager);
      return new ObjectLoader (clientTransaction, persistenceStrategy, eventSink, eagerFetcher);
    }

    public virtual IEnlistedDomainObjectManager CreateEnlistedObjectManager ()
    {
      return new DictionaryBasedEnlistedDomainObjectManager ();
    }

    public IInvalidDomainObjectManager CreateInvalidDomainObjectManager ()
    {
      return new InvalidDomainObjectManager ();
    }

    public virtual IDataManager CreateDataManager (ClientTransaction clientTransaction, IInvalidDomainObjectManager invalidDomainObjectManager)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("invalidDomainObjectManager", invalidDomainObjectManager);

      return new DataManager (clientTransaction, new RootCollectionEndPointChangeDetectionStrategy (), invalidDomainObjectManager);
    }

    public virtual Func<ClientTransaction, ClientTransaction> CreateCloneFactory ()
    {
      return templateTransaction => (ClientTransaction) TypesafeActivator
        .CreateInstance (templateTransaction.GetType (), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        .With (this);
    }
  }
}