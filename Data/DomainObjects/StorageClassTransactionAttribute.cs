using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects
{
  public class StorageClassTransactionAttribute: StorageClassAttribute
  {
    public StorageClassTransactionAttribute()
        : base (StorageClass.Transaction)
    {
    }
  }
}