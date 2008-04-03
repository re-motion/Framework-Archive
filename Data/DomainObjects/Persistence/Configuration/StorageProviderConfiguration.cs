using System;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  [Obsolete ("Use PersistenceConfiguration to access the StorageProviderDefinitions. (Version 1.7.42)")]
  public class StorageProviderConfiguration
  {
    // types

    // static members and constants

    public static StorageProviderConfiguration Current
    {
      get { return new StorageProviderConfiguration (DomainObjectsConfiguration.Current.Storage);}
    }

    [Obsolete ("Use DomainObjectsConfiguration to set the current StorageProviderDefinitions. (Version 1.7.42)", true)]
    public static void SetCurrent (StorageProviderConfiguration storageProviderConfiguration)
    {
      throw new InvalidOperationException ("Use DomainObjectsConfiguration to set the current StorageProviderDefinitions.");
    }

    // member fields

    private PersistenceConfiguration _persistenceConfiguration;

    // construction and disposing

    [Obsolete ("Use StorageProviderConfiguration (PersistenceConfiguration) (Version 1.7.42)", true)]
    public StorageProviderConfiguration (string configurationFile)
    {
      throw new InvalidOperationException ("Use StorageProviderConfiguration (PersistenceConfiguration).");
    }

    [Obsolete ("Use Use StorageProviderConfiguration (PersistenceConfiguration). (Version 1.7.42)", true)]
    public StorageProviderConfiguration (object loader)
    {
      throw new InvalidOperationException ("Use StorageProviderConfiguration (PersistenceConfiguration).");
    }

    public StorageProviderConfiguration (PersistenceConfiguration persistenceConfiguration)
    {
      ArgumentUtility.CheckNotNull ("persistenceConfiguration", persistenceConfiguration);
      _persistenceConfiguration = persistenceConfiguration;
    }

    // methods and properties

    public StorageProviderDefinition this [string storageProviderID]
    {
      get
      {
        ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
        return _persistenceConfiguration.StorageProviderDefinitions[storageProviderID];
      }
    }

    public ProviderCollection<StorageProviderDefinition> StorageProviderDefinitions
    {
      get { return _persistenceConfiguration.StorageProviderDefinitions; }
    }

    [Obsolete ("Contains (string) is no longer supported. (Version 1.7.42)", true)]
    public bool Contains (StorageProviderDefinition storageProviderDefinition)
    {
      throw new InvalidOperationException ("Contains is no longer supported (string).");
    }
  }
}