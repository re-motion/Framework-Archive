using System;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Development
{
  /// <summary>
  /// Fake implementation of the <see cref="IDomainObjectsConfiguration"/> interface. Use this class for programmatically setting up the configuration 
  /// in unit test scenarios.
  /// </summary>
  public class FakeDomainObjectsConfiguration: IDomainObjectsConfiguration
  {
    private readonly PersistenceConfiguration _storage;
    private readonly MappingLoaderConfiguration _mappingLoader;
    private readonly QueryConfiguration _query;

    public FakeDomainObjectsConfiguration (MappingLoaderConfiguration mappingLoader, PersistenceConfiguration storage, QueryConfiguration query)
    {
      ArgumentUtility.CheckNotNull ("mappingLoader", mappingLoader);
      ArgumentUtility.CheckNotNull ("storage", storage);
      ArgumentUtility.CheckNotNull ("query", query);

      _mappingLoader = mappingLoader;
      _storage = storage;
      _query = query;
    }

    public MappingLoaderConfiguration MappingLoader
    {
      get { return _mappingLoader; }
    }

    public PersistenceConfiguration Storage
    {
      get { return _storage; }
    }

    public QueryConfiguration Query
    {
      get { return _query; }
    }
  }
}