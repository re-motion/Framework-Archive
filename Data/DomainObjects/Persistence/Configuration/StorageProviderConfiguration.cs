using System;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Configuration
{
public class StorageProviderConfiguration : ConfigurationBase
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

  public static void SetCurrent (StorageProviderConfiguration storageProviderConfiguration)
  {
    lock (typeof (StorageProviderConfiguration))
    {
      s_storageProviderConfiguration = storageProviderConfiguration;
    }
  }

  // member fields

  private StorageProviderDefinitionCollection _storageProviderDefinitions;

  // construction and disposing

  public StorageProviderConfiguration (string configurationFile, string schemaFile) 
      : this (new StorageProviderConfigurationLoader (configurationFile, schemaFile))
  {
  }

  public StorageProviderConfiguration (StorageProviderConfigurationLoader loader) : base (loader)
  {
    ArgumentUtility.CheckNotNull ("loader", loader);

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
