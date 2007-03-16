using System;
using System.Configuration;
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

    private DomainObjectsConfiguration()
    {
    }

    [ConfigurationProperty (MappingLoaderPropertyName)]
    public MappingLoaderConfiguration MappingLoader
    {
      get { return (MappingLoaderConfiguration) ConfigurationManager.GetSection (ConfigKey + "/" + MappingLoaderPropertyName); }
    }

    [ConfigurationProperty (StoragePropertyName)]
    public PersistenceConfiguration Storage
    {
      get { return (PersistenceConfiguration) ConfigurationManager.GetSection (ConfigKey + "/" + StoragePropertyName); }
    }

    private string ConfigKey
    {
      get { return string.IsNullOrEmpty (SectionGroupName) ? "rubicon.data.domainObjects" : SectionGroupName; }
    }
  }
}