using System.Configuration;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.Configuration
{
  public interface IDomainObjectsConfiguration
  {
    MappingLoaderConfiguration MappingLoader { get; }

    PersistenceConfiguration Storage { get; }
  }
}