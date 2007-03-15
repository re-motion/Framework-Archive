using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Rubicon.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Configuration
{
  public class PersistenceConfiguration: ExtendedConfigurationSection
  {
    // constants

    // types

    // static members

    // member fields

    private ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private StorageProviderDefinitionHelper _storageProviderDefinitionHelper;

    private List<ProviderHelperBase> _providerHelpers = new List<ProviderHelperBase>();

    // construction and disposing

    public PersistenceConfiguration()
    {
      _storageProviderDefinitionHelper = new StorageProviderDefinitionHelper (this);
      _providerHelpers.Add (_storageProviderDefinitionHelper);

      _providerHelpers.ForEach (delegate (ProviderHelperBase current) { current.InitializeProperties (_properties); });
    }

    public PersistenceConfiguration (ProviderCollection<StorageProviderDefinition> providers, StorageProviderDefinition provider)
      : this()
    {
      ArgumentUtility.CheckNotNull ("providers", providers);
      ArgumentUtility.CheckNotNull ("provider", provider);

      _storageProviderDefinitionHelper.Provider = provider;

      ProviderCollection<StorageProviderDefinition> providersCopy = CopyProvidersAsReadOnly (providers);
      _storageProviderDefinitionHelper.Providers = providersCopy;
    }

    // methods and properties

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinitionHelper.Provider; }
    }

    public ProviderCollection<StorageProviderDefinition> StorageProviderDefinitions
    {
      get { return _storageProviderDefinitionHelper.Providers; }
    }

    protected override void PostDeserialize ()
    {
      base.PostDeserialize ();

      _providerHelpers.ForEach (delegate (ProviderHelperBase current) { current.PostDeserialze (); });
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    private ProviderCollection<StorageProviderDefinition> CopyProvidersAsReadOnly (ProviderCollection<StorageProviderDefinition> providers)
    {
      ProviderCollection<StorageProviderDefinition> providersCopy = new ProviderCollection<StorageProviderDefinition> ();
      foreach (StorageProviderDefinition provider in providers)
        providersCopy.Add (provider);

      providersCopy.SetReadOnly ();
      return providersCopy;
    }
  }
}