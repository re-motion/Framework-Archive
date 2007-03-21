using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects
{
  public class StorageClassNoneAttribute: StorageClassAttribute
  {
    public StorageClassNoneAttribute()
        : base (StorageClass.None)
    {
    }
  }
}