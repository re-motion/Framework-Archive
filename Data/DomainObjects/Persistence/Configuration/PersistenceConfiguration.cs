using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader;

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

    // methods and properties

    protected override void PostDeserialize()
    {
      base.PostDeserialize();

      _providerHelpers.ForEach (delegate (ProviderHelperBase current) { current.PostDeserialze(); });
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinitionHelper.Provider; }
    }

    public ProviderCollection StorageProviderDefinitions
    {
      get { return _storageProviderDefinitionHelper.Providers; }
    }
  }
}