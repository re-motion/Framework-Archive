using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.Legacy.Mapping
{
  public static class XmlBasedMappingConfiguration
  {
    public const string ConfigurationAppSettingKey = "Rubicon.Data.DomainObjects.Mapping.ConfigurationFile";
    public const string DefaultConfigurationFile = "Mapping.xml";

    public static MappingConfiguration Create()
    {
      return Create (LoaderUtility.GetConfigurationFileName (ConfigurationAppSettingKey, DefaultConfigurationFile), true);
    }

    public static MappingConfiguration Create (string configurationFile)
    {
      return Create (configurationFile, true);
    }

    public static MappingConfiguration Create (string configurationFile, bool resolveTypes)
    {
      return new MappingConfiguration (new MappingLoader (configurationFile, resolveTypes));
    }
  }
}