using System.Configuration;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.Configuration
{
  /// <summary>
  /// The <see cref="IDomainObjectsConfiguration"/> interface is an abstraction for the <see cref="ConfigurationSectionGroup"/> and the fake 
  /// implementation of the domain objects configuration.
  /// </summary>
  public interface IDomainObjectsConfiguration
  {
    MappingLoaderConfiguration MappingLoader { get; }

    PersistenceConfiguration Storage { get; }
  }
}