using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Rubicon.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Design;
using Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.Legacy.Design
{
  /// <summary>
  /// Design mode version of the <see cref="MappingLoader"/> type. Associated with the <see cref="MappingLoader"/> by use of the 
  /// <see cref="DesignModeMappingLoaderAttribute"/>.
  /// </summary>
  public class DesignModeXmlBasedMappingLoader: MappingLoader
  {
    public DesignModeXmlBasedMappingLoader (IDesignerHost designerHost)
        : base (LoaderUtility.GetConfigurationFileName (ConfigurationAppSettingKey, DefaultConfigurationFile), true)
    {
    }
  }
}