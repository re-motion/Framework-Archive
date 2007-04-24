using System;
using System.Configuration;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.Configuration
{
  /// <summary>
  /// <see cref="ConfigurationSectionGroup"/> for grouping the <see cref="ConfigurationSection"/> in the <b>Rubicon.Data.DomainObjects</b> namespace.
  /// </summary>
  public sealed class DomainObjectsConfiguration: ConfigurationSectionGroup, IDomainObjectsConfiguration
  {
    private const string MappingLoaderPropertyName = "mapping";
    private const string StoragePropertyName = "storage";

    private static readonly DoubleCheckedLockingContainer<IDomainObjectsConfiguration> s_current =
        new DoubleCheckedLockingContainer<IDomainObjectsConfiguration> (delegate { return new DomainObjectsConfiguration(); });

    public static IDomainObjectsConfiguration Current
    {
      get { return s_current.Value; }
    }

    public static void SetCurrent (IDomainObjectsConfiguration configuration)
    {
      s_current.Value = configuration;
    }

    public DomainObjectsConfiguration()
    {
      _mappingLoaderConfiguration =
          new DoubleCheckedLockingContainer<MappingLoaderConfiguration> (delegate { return GetMappingLoaderConfiguration(); });

      _persistenceConfiguration =
          new DoubleCheckedLockingContainer<PersistenceConfiguration> (delegate { return GetPersistenceConfiguration(); });
    }

    private DoubleCheckedLockingContainer<MappingLoaderConfiguration> _mappingLoaderConfiguration;
    private DoubleCheckedLockingContainer<PersistenceConfiguration> _persistenceConfiguration;

    [ConfigurationProperty (MappingLoaderPropertyName)]
    public MappingLoaderConfiguration MappingLoader
    {
      get { return _mappingLoaderConfiguration.Value; }
    }

    [ConfigurationProperty (StoragePropertyName)]
    public PersistenceConfiguration Storage
    {
      get { return _persistenceConfiguration.Value; }
    }

    private MappingLoaderConfiguration GetMappingLoaderConfiguration()
    {
      return
          (MappingLoaderConfiguration) ConfigurationWrapper.Current.GetSection (ConfigKey + "/" + MappingLoaderPropertyName, false)
          ?? new MappingLoaderConfiguration();
    }

    private PersistenceConfiguration GetPersistenceConfiguration()
    {
      return
          (PersistenceConfiguration) ConfigurationWrapper.Current.GetSection (ConfigKey + "/" + StoragePropertyName, false) 
          ?? new PersistenceConfiguration();
    }

    private string ConfigKey
    {
      get { return string.IsNullOrEmpty (SectionGroupName) ? "rubicon.data.domainObjects" : SectionGroupName; }
    }
  }
}