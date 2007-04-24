using System;
using System.ComponentModel;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Design;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Design
{
  /// <summary>
  /// The <see cref="DomainObjectsDesignModeHelper"/> is inteded to encapsulate design mode specific initialization code for <see cref="IComponent"/>
  /// implementations in the <see cref="N:Rubicon.Data.DomainObjects"/> namespace, such as data sources.
  /// </summary>
  public class DomainObjectsDesignModeHelper
  {
    private readonly IDesignModeHelper _designModeHelper;

    public DomainObjectsDesignModeHelper (IDesignModeHelper designModeHelper)
    {
      ArgumentUtility.CheckNotNull ("designModeHelper", designModeHelper);

      _designModeHelper = designModeHelper;
    }

    public void InitializeConfiguration ()
    {
      System.Configuration.Configuration configuration = _designModeHelper.GetConfiguration();
      if (configuration != null)
      {
        ConfigurationWrapper.SetCurrent (ConfigurationWrapper.CreateFromConfigurationObject (configuration));
        DomainObjectsConfiguration.SetCurrent (new DomainObjectsConfiguration());
        MappingConfiguration.SetCurrent (new MappingConfiguration (GetMappingLoader()));       
      }
    }

    private IMappingLoader GetMappingLoader()
    {
      Type mappingLoaderType = GetMappingLoaderType();
      DesignModeMappingLoaderAttribute designModeMappingLoaderAttribute = 
          AttributeUtility.GetCustomAttribute<DesignModeMappingLoaderAttribute> (mappingLoaderType, true);
      Assertion.Assert (
          designModeMappingLoaderAttribute != null, 
          "'{0}' does not have the '{1}' applied.", mappingLoaderType.FullName, typeof (DesignModeMappingLoaderAttribute).FullName);

      return designModeMappingLoaderAttribute.CreateInstance (_designModeHelper.Site);
    }

    private Type GetMappingLoaderType()
    {
      IDomainObjectsConfiguration domainObjectsConfiguration = DomainObjectsConfiguration.Current;
      Assertion.Assert (domainObjectsConfiguration != null, "DomainObjectsConfiguration.Current evaluated and returned null.");

      MappingLoaderConfiguration mappingLoaderConfiguration = domainObjectsConfiguration.MappingLoader;
      Assertion.Assert (mappingLoaderConfiguration != null, "DomainObjectsConfiguration.Current.MappingLoader evaluated and returned null.");
      
      Type mappingLoaderType = mappingLoaderConfiguration.MappingLoaderType;
      Assertion.Assert (mappingLoaderType != null, "DomainObjectsConfiguration.Current.MappingLoader.MappingLoaderType evaluated and returned null.");
      
      return mappingLoaderType;
    }
  }
}