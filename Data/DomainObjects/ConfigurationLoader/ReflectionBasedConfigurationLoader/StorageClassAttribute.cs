using System;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public class StorageClassAttribute: Attribute
  {
    private StorageClass _storageClass;

    public StorageClassAttribute (StorageClass storageClass)
    {
      _storageClass = storageClass;
    }

    public StorageClass StorageClass
    {
      get { return _storageClass; }
    }
  }
}