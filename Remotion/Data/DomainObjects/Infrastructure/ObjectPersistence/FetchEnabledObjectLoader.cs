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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Extends an <see cref="IObjectLoader"/> with the ability to execute an eager fetch query.
  /// </summary>
  [Serializable]
  public class FetchEnabledObjectLoader : ObjectLoader, IFetchEnabledObjectLoader
  {
    private readonly IFetchEnabledPersistenceStrategy _persistenceStrategy;
    private readonly IEagerFetcher _eagerFetcher;

    public FetchEnabledObjectLoader (
        IFetchEnabledPersistenceStrategy persistenceStrategy,
        ILoadedObjectDataRegistrationAgent loadedObjectDataRegistrationAgent,
        ILoadedObjectDataProvider loadedObjectDataProvider,
        IEagerFetcher eagerFetcher)
        : base (persistenceStrategy, loadedObjectDataRegistrationAgent, loadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("eagerFetcher", eagerFetcher);
      
      _persistenceStrategy = persistenceStrategy;
      _eagerFetcher = eagerFetcher;
    }

    public new IFetchEnabledPersistenceStrategy PersistenceStrategy
    {
      get { return _persistenceStrategy; }
    }

    public IEagerFetcher EagerFetcher
    {
      get { return _eagerFetcher; }
    }

    public override ICollection<ILoadedObjectData> GetOrLoadCollectionQueryResult (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      var pendingRegistrationCollector = new DataContainersPendingRegistrationCollector ();

      var loadedObjectData = _persistenceStrategy.ExecuteCollectionQuery (query, LoadedObjectDataProvider).ConvertToCollection();
      LoadedObjectDataRegistrationAgent.BeginRegisterIfRequired (loadedObjectData, true, pendingRegistrationCollector);

      try
      {
        _eagerFetcher.PerformEagerFetching (loadedObjectData, query.EagerFetchQueries, this, pendingRegistrationCollector);
      }
      finally
      {
        // Note: It's important to run the call to EndRegister within a finally block, otherwise we'd leak "currently loading objects" in the case
        // of exceptions.
        LoadedObjectDataRegistrationAgent.EndRegisterIfRequired (pendingRegistrationCollector);
      }

      return loadedObjectData;
    }

    public ICollection<LoadedObjectDataWithDataSourceData> GetOrLoadFetchQueryResult (
        IQuery query, 
        DataContainersPendingRegistrationCollector pendingRegistrationCollector)
    {
      ArgumentUtility.CheckNotNull ("query", query);
      ArgumentUtility.CheckNotNull ("pendingRegistrationCollector", pendingRegistrationCollector);
      
      var loadedObjectDataWithSource = _persistenceStrategy.ExecuteFetchQuery (query, LoadedObjectDataProvider).ConvertToCollection();
      
      var loadedObjectData = loadedObjectDataWithSource.Select (data => data.LoadedObjectData).ConvertToCollection ();
      LoadedObjectDataRegistrationAgent.BeginRegisterIfRequired (loadedObjectData, true, pendingRegistrationCollector);

      _eagerFetcher.PerformEagerFetching (loadedObjectData, query.EagerFetchQueries, this, pendingRegistrationCollector);
      
      return loadedObjectDataWithSource;
    }
  }
}