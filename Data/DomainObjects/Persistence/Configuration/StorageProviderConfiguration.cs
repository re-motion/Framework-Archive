using System;

using Rubicon.Data.DomainObjects.Configuration.Loader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Configuration.StorageProviders
{
public class StorageProviderConfiguration
{
  // types

  // static members and constants

  private static StorageProviderConfiguration s_storageProviderConfiguration;

  public static StorageProviderConfiguration Current
  {
    get 
    {
      lock (typeof (StorageProviderConfiguration))
      {
        if (s_storageProviderConfiguration == null)
        {
          s_storageProviderConfiguration = new StorageProviderConfiguration (StorageProviderConfigurationLoader.Create ());
        }
        
        return s_storageProviderConfiguration;
      }
    }
  }

  // member fields

  private StorageProviderDefinitionCollection _storageProviderDefinitions;

  // construction and disposing

  private StorageProviderConfiguration (StorageProviderConfigurationLoader loader)
  {
    _storageProviderDefinitions = loader.GetStorageProviderDefinitions ();
  }

  // methods and properties

  public StorageProviderDefinition this [string storageProviderID]
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
      return _storageProviderDefinitions[storageProviderID]; 
    }
  }

  public StorageProviderDefinitionCollection StorageProviderDefinitions
  {
    get { return _storageProviderDefinitions; }
  }
}
}
