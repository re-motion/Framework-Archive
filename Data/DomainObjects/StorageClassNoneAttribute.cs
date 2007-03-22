using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>Defines the property as not managed by the persistence framework.</summary>
  public class StorageClassNoneAttribute: StorageClassAttribute
  {
    public StorageClassNoneAttribute()
        : base (StorageClass.None)
    {
    }
  }
}