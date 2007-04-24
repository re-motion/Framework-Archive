using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Design
{
  /// <summary>
  /// Design mode version of the <see cref="MappingReflector"/> type. Associated with the <see cref="MappingReflector"/> by use of the 
  /// <see cref="DesignModeMappingLoaderAttribute"/>.
  /// </summary>
  public class DesignModeMappingReflector : MappingReflectorBase
  {
    private ITypeDiscoveryService _typeDiscoveryService;

    public DesignModeMappingReflector (ISite site)
    {
      ArgumentUtility.CheckNotNull ("site", site);

      _typeDiscoveryService = (ITypeDiscoveryService) site.GetService (typeof (ITypeDiscoveryService));
      Assertion.Assert (_typeDiscoveryService != null, "Look-up of 'ITypeDiscoveryService' via site.GetService(...) failed.");
    }

    protected override ICollection GetDomainObjectClasses()
    {
      return _typeDiscoveryService.GetTypes (typeof (DomainObject), false);
    }
  }
}