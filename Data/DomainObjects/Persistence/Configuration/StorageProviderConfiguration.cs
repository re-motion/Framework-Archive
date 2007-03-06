using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Configuration
{
public class StorageProviderConfiguration
{
  // types

  // static members and constants

  [Obsolete ("Check after Refactoring. (Version 1.7.42)")]
  public const string ConfigurationAppSettingKey = "Rubicon.Data.DomainObjects.Persistence.Configuration.ConfigurationFile";
  [Obsolete ("Check after Refactoring. (Version 1.7.42)")]
  public const string DefaultConfigurationFile = "StorageProviders.xml";

  private static StorageProviderConfiguration s_storageProviderConfiguration;

  public static StorageProviderConfiguration Current
  {
    get 
    {
      if (s_storageProviderConfiguration == null)
      {
        lock (typeof (StorageProviderConfiguration))
        {
          if (s_storageProviderConfiguration == null)
            s_storageProviderConfiguration = CreateConfigurationFromFileBasedLoader ();
        }
      }

      return s_storageProviderConfiguration;
    }
  }

  public static void SetCurrent (StorageProviderConfiguration storageProviderConfiguration)
  {
    lock (typeof (StorageProviderConfiguration))
    {
      s_storageProviderConfiguration = storageProviderConfiguration;
    }
  }

  [Obsolete ("Check after Refactoring. (Version 1.7.42)")]
  public static StorageProviderConfiguration CreateConfigurationFromFileBasedLoader ()
  {
    return CreateConfigurationFromFileBasedLoader (
        LoaderUtility.GetConfigurationFileName (ConfigurationAppSettingKey, DefaultConfigurationFile));
  }

  [Obsolete ("Check after Refactoring. (Version 1.7.42)")]
  public static StorageProviderConfiguration CreateConfigurationFromFileBasedLoader (string configurationFile)
  {
    Type loaderType = TypeUtility.GetType ("Rubicon.Data.DomainObjects.Legacy::ConfigurationLoader.FileBasedConfigurationLoader.StorageProviderConfigurationLoader", true, false);
    IStorageProviderConfigurationLoader loader = (IStorageProviderConfigurationLoader) Activator.CreateInstance (loaderType, configurationFile);
    return new StorageProviderConfiguration (loader);
  }

  // member fields

  private StorageProviderDefinitionCollection _storageProviderDefinitions;
  private IStorageProviderConfigurationLoader _loader;

  // construction and disposing

  [Obsolete ("Use StorageProviderConfiguration.CreateConfigurationFromFileBasedLoader (string). (Version 1.7.42)", true)]
  public StorageProviderConfiguration (string configurationFile) 
  {
    throw new InvalidOperationException ("Use StorageProviderConfiguration.CreateConfigurationFromFileBasedLoader (string).");
  }

  public StorageProviderConfiguration (IStorageProviderConfigurationLoader loader)
  {
    ArgumentUtility.CheckNotNull ("loader", loader);
    _loader = loader;

    _storageProviderDefinitions = _loader.GetStorageProviderDefinitions ();
  }

  // methods and properties

  public IStorageProviderConfigurationLoader Loader
  {
    get { return _loader; }
  }

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

  public bool Contains (StorageProviderDefinition storageProviderDefinition)
  {
    ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

    return _storageProviderDefinitions.Contains (storageProviderDefinition);
  }
}
}
