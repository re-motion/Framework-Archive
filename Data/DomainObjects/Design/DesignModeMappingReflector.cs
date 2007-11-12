using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Mixins;
using Rubicon.Mixins.Context;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Design
{
  /// <summary>
  /// Design mode version of the <see cref="MappingReflector"/> type. Associated with the <see cref="MappingReflector"/> by use of the 
  /// <see cref="DesignModeMappingLoaderAttribute"/>.
  /// </summary>
  public class DesignModeMappingReflector : MappingReflectorBase
  {
    private readonly ITypeDiscoveryService _typeDiscoveryService;

    public DesignModeMappingReflector (ISite site)
    {
      ArgumentUtility.CheckNotNull ("site", site);

      _typeDiscoveryService = (ITypeDiscoveryService) site.GetService (typeof (ITypeDiscoveryService));
      Assertion.IsNotNull(_typeDiscoveryService, "Look-up of 'ITypeDiscoveryService' via site.GetService(...) failed.");
    }

    protected override Type[] GetDomainObjectTypes()
    {
      // In design mode, the AppDomain's base directory is not defined. Therefore, we must avoid loading the default mixin configuration,
      // so we set an empty default one.
      if (!MixinConfiguration.HasActiveContext)
        MixinConfiguration.SetActiveContext (new ApplicationContext ());

      List<Type> domainObjectsTypes = new List<Type>();
      foreach (Type domainObjectType in _typeDiscoveryService.GetTypes (typeof (DomainObject), false))
        domainObjectsTypes.Add (domainObjectType);

      return domainObjectsTypes.ToArray();

    }
  }
}