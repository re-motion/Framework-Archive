using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Design;
using Remotion.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects.Legacy.Design
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