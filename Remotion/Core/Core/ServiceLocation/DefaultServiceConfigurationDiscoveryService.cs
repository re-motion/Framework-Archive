// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Remotion.Collections;
using Remotion.Reflection;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Provides services for scanning a range of types for default service configuration settings, as they would be applied by 
  /// <see cref="DefaultServiceLocator"/>. Use this class in order to configure a specific service locator with <see cref="DefaultServiceLocator"/>'s
  /// defaults.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="DefaultServiceConfigurationDiscoveryService"/> uses the same logic as <see cref="DefaultServiceLocator"/> in order to find the
  /// default concrete implementation of service types configured via the <see cref="ImplementationForAttribute"/>. See 
  /// <see cref="DefaultServiceLocator"/> for more information about this.
  /// </para>
  /// <para>
  /// Concrete implementations registered with a specific <see cref="DefaultServiceLocator"/> using its <see cref="DefaultServiceLocator.Register(ServiceConfigurationEntry)"/>
  /// methods are not returned by this class.
  /// </para>
  /// </remarks>
  public class DefaultServiceConfigurationDiscoveryService : IServiceConfigurationDiscoveryService
  {
    private readonly ITypeDiscoveryService _typeDiscoveryService;
    private readonly ICache<Type, IEnumerable<Type>> _implementingTypeCache = CacheFactory.CreateWithLocking<Type, IEnumerable<Type>>();

    public static DefaultServiceConfigurationDiscoveryService Create ()
    {
      return new DefaultServiceConfigurationDiscoveryService(ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService());
    }

    public DefaultServiceConfigurationDiscoveryService (ITypeDiscoveryService typeDiscoveryService)
    {
      _typeDiscoveryService = typeDiscoveryService;
    }

    /// <summary>
    /// Gets the default service configuration for the types returned by the given <see cref="ITypeDiscoveryService"/>.
    /// </summary>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> for each serviceType that has implementations with a <see cref="ImplementationForAttribute"/> applied. 
    /// Types without the attribute are ignored.</returns>
    public IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration ()
    {
      return GetDefaultConfiguration (_typeDiscoveryService.GetTypes (null, false).Cast<Type>());
    }

    /// <summary>
    /// Gets the default service configuration for the given types.
    /// </summary>
    /// <param name="types">The types to get the default service configuration for.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> for each serviceType that has implementations with a <see cref="ImplementationForAttribute"/> applied. 
    /// Types without the attribute are ignored.</returns>
    public IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      return types.Select (
          type =>
          {
            try
            {
              return GetDefaultConfiguration (type);
            }
            catch (InvalidOperationException ex)
            {
              var message = string.Format ("Invalid configuration of service type '{0}'. {1}", type, ex.Message);
              throw new InvalidOperationException (message, ex);
            }
          }).Where (configuration => configuration.ImplementationInfos.Any());
    }

    public ServiceConfigurationEntry GetDefaultConfiguration (Type serviceType)
    {
      var excludeGlobalTypes = !serviceType.Assembly.GlobalAssemblyCache;

      var implementingTypes = _implementingTypeCache.GetOrCreateValue (serviceType, type => GetImplementingTypes (type, excludeGlobalTypes));

      var attributes = implementingTypes.SelectMany (
          type => AttributeUtility.GetCustomAttributes<ImplementationForAttribute> (type, false)
              .Where (attribute => attribute.ServiceType == serviceType)
              .Select (attribute => Tuple.Create (type, attribute)));

      var registrationTypes = attributes.Select (a => a.Item2.RegistrationType).Distinct().ToArray();
          
      if (registrationTypes.Contains(RegistrationType.Compound) && registrationTypes.Contains(RegistrationType.Single))
        throw new InvalidOperationException (
            "RegistrationTypes compound and Single cannot be used together.");
      
      if (registrationTypes.Contains(RegistrationType.Single) && registrationTypes.Contains(RegistrationType.Multiple))
        throw new InvalidOperationException (
            "RegistrationTypes Single and Multiple must not be mixed. All service implementations have to have the same registration type.");

      return ServiceConfigurationEntry.CreateFromAttributes (serviceType, attributes);
    }

    /// <summary>
    /// Gets the default service configuration for the types in the given assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies for whose types to get the default service configuration.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> for each type that has the <see cref="ImplementationForAttribute"/> applied. 
    /// Types without the attribute are ignored.</returns>
    public IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (IEnumerable<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return assemblies.SelectMany (a => GetDefaultConfiguration (AssemblyTypeCache.GetTypes (a)));
    }

    private IEnumerable<Type> GetImplementingTypes (Type serviceType, bool excludeGlobalTypes)
    {
      var derivedTypes = _typeDiscoveryService.GetTypes (serviceType, excludeGlobalTypes);

      var implementingTypes = new List<Type>();
      foreach (Type derivedType in derivedTypes)
      {
        foreach (var attribute in AttributeUtility.GetCustomAttributes<ImplementationForAttribute> (derivedType, false))
        {
          if (attribute.ServiceType == serviceType)
            implementingTypes.Add (derivedType);
        }
      }
      return implementingTypes.ToArray();
    }
  }
}