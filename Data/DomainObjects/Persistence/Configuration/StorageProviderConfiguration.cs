using System;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Configuration
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

  public static void SetCurrent (StorageProviderConfiguration storageProviderConfiguration)
  {
    lock (typeof (StorageProviderConfiguration))
    {
      s_storageProviderConfiguration = storageProviderConfiguration;
    }
  }

  // member fields

  private StorageProviderDefinitionCollection _storageProviderDefinitions;
  private string _configurationFile;
  private string _schemaFile;

  // construction and disposing

  public StorageProviderConfiguration (string configurationFile, string schemaFile) 
      : this (new StorageProviderConfigurationLoader (configurationFile, schemaFile))
  {
  }

  public StorageProviderConfiguration (StorageProviderConfigurationLoader loader)
  {
    ArgumentUtility.CheckNotNull ("loader", loader);

    _storageProviderDefinitions = loader.GetStorageProviderDefinitions ();
    _configurationFile = loader.ConfigurationFile;
    _schemaFile = loader.SchemaFile;
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

  public string ConfigurationFile
  {
    get { return _configurationFile; }
  }

  public string SchemaFile
  {
    get { return _schemaFile; }
  }
}
}
