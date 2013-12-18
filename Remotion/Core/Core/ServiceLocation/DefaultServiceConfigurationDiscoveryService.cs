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

    public static DefaultServiceConfigurationDiscoveryService Create ()
    {
      return new DefaultServiceConfigurationDiscoveryService(ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService());
    }

    public DefaultServiceConfigurationDiscoveryService (ITypeDiscoveryService typeDiscoveryService)
    {
      _typeDiscoveryService = typeDiscoveryService;
    }

    //TODO RM-5560: change to instance implementation and pass ITypeDiscoveryService via ctor. 
    // Optionally, provide a factory method that depends on ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService()
    // Drop ITypeDiscoveryService from method signatures.

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
      var derivedTypes = _typeDiscoveryService.GetTypes (serviceType, excludeGlobalTypes);
      
      // TODO RM-5560: Refactor to ask for each type the derived types from TIypeDiscoverySerivce
      // determine flag excludeGlobalTypes on GetTypes by checking type.Assembly.GlobalAssemblyCache
      // TBD: Caching-Decorator for ITypeDiscoveryService?
      // Cache for each derived type if it contains a ConcreteImplementationAttribute, but result of GetCustomAttributes must not be cached.
      // Only cache if it is actually sensible.

      // TODO RM-5506: caching 
      var attributes = derivedTypes
          .Cast<Type>()
          .SelectMany (
              type => AttributeUtility.GetCustomAttributes<ImplementationForAttribute> (type, false)
                  .Select (attribute => Tuple.Create (type, attribute)))
          .Where (tuple => tuple.Item2.ServiceType == serviceType)
          .ToArray();

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
  }
}