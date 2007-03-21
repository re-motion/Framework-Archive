using System;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public class StorageClassAttribute: Attribute
  {
    public StorageClassAttribute (StorageClass storageClass)
    {
    }
  }
}