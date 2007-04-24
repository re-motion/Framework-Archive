using System;
using Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.Legacy.Mapping
{
  public static class XmlBasedMappingConfiguration
  {
    public static MappingConfiguration Create()
    {
      return new MappingConfiguration (MappingLoader.Create());
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