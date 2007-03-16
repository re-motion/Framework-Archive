using System;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Development
{
  public class FakeDomainObjectsConfiguration: IDomainObjectsConfiguration
  {
    private PersistenceConfiguration _storage;
    private MappingLoaderConfiguration _mappingLoader;

    public FakeDomainObjectsConfiguration (MappingLoaderConfiguration mappingLoader, PersistenceConfiguration storage)
    {
      ArgumentUtility.CheckNotNull ("mappingLoader", mappingLoader);
      ArgumentUtility.CheckNotNull ("storage", storage);

      _mappingLoader = mappingLoader;
      _storage = storage;
    }

    public MappingLoaderConfiguration MappingLoader
    {
      get { return _mappingLoader; }
    }

    public PersistenceConfiguration Storage
    {
      get { return _storage; }
    }
  }
}