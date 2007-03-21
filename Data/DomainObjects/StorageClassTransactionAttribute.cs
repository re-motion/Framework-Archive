using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects
{
  internal class StorageClassTransactionAttribute: StorageClassAttribute
  {
    public StorageClassTransactionAttribute()
        : base (StorageClass.Transaction)
    {
    }
  }
}