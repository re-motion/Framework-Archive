using System;
using Rubicon.Data.DomainObjects.Infrastructure.Interception;
using Rubicon.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides functionality for creating instances of DomainObjects which intercept property calls.
  /// </summary>
  public class InterceptedDomainObjectFactory
  {
    private readonly ModuleManager _scope = new ModuleManager();

    /// <summary>
    /// Saves the assemblies generated by the factory and returns the paths of the saved manifest modules.
    /// </summary>
    /// <returns>The paths of the manifest modules of the saved assemblies.</returns>
    public string[] SaveGeneratedAssemblies ()
    {
      return _scope.SaveAssemblies ();
    }

    /// <summary>
    /// Gets a domain object type assignable to the given base type which intercepts property calls.
    /// </summary>
    /// <param name="baseType">The base domain object type whose properties should be intercepted.</param>
    /// <returns>A domain object type which intercepts property calls.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="baseType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentTypeException"><paramref name="baseType"/> cannot be assigned to <see cref="DomainObject"/>.</exception>
    public Type GetConcreteDomainObjectType (Type baseType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("baseType", baseType, typeof (DomainObject));

      TypeGenerator generator = _scope.CreateTypeGenerator (baseType);
      return generator.BuildType (baseType);
    }
  }
}