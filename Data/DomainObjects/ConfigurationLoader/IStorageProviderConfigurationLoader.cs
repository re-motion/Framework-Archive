using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader
{
  public interface IStorageProviderConfigurationLoader
  {
    StorageProviderDefinitionCollection GetStorageProviderDefinitions();
  }
}