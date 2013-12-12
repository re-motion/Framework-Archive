using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;

namespace Remotion.ServiceLocation
{
  public interface IServiceConfigurationDiscoveryService
  {
    /// <summary>
    /// Gets the default service configuration for the types returned by the given <see cref="ITypeDiscoveryService"/>.
    /// </summary>
    /// <param name="typeDiscoveryService">The type discovery service.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> for each type returned by the <paramref name="typeDiscoveryService"/> that has the
    /// <see cref="ConcreteImplementationAttribute"/> applied. Types without the attribute are ignored.</returns>
    IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (ITypeDiscoveryService typeDiscoveryService);

    /// <summary>
    /// Gets the default service configuration for the given types.
    /// </summary>
    /// <param name="types">The types to get the default service configuration for.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> for each type that has the <see cref="ConcreteImplementationAttribute"/> applied. 
    /// Types without the attribute are ignored.</returns>
    IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (IEnumerable<Type> types);

    /// <summary>
    /// Gets the default service configuration for the types in the given assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies for whose types to get the default service configuration.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> for each type that has the <see cref="ConcreteImplementationAttribute"/> applied. 
    /// Types without the attribute are ignored.</returns>
    IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (IEnumerable<Assembly> assemblies);
  }
}