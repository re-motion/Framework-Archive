using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Infrastructure.Interception;
using Rubicon.Data.DomainObjects.Infrastructure.Interception.Castle;
using Rubicon.Reflection;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Default implementation of <see cref="IDomainObjectFactory"/>.
  /// </summary>
  public class DomainObjectFactory : IDomainObjectFactory
  {
    private readonly IInterceptableObjectGenerator<DomainObject> _generator =
        new CastleInterceptableObjectGenerator<DomainObject>(new DomainObjectInterceptorSelector());

    /// <summary>
    /// Creates an interceptable type compatible to a given domain object base type.
    /// </summary>
    /// <param name="baseType">The domain object type the interceptable type must be compatible to.</param>
    /// <returns>An interceptable type compatible to the <paramref name="baseType"/>.</returns>
    /// <remarks><para>This method dynamically creates a subclass that intercepts certain method calls in order to perform management tasks.
    /// Using it ensures that the created domain object supports the new property syntax.</para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="baseType"/> argument is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="baseType"/> argument is sealed, contains abstract methods (apart from automatic
    /// properties), or is not derived from <see cref="DomainObject"/>.</exception>
    public Type GetConcreteDomainObjectType (Type baseType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("baseType", baseType, typeof (DomainObject));

      if (baseType.IsSealed)
      {
        string message = string.Format ("Cannot instantiate type {0} as it is sealed.", baseType.FullName);
        throw new ArgumentException (message, "baseType");
      }
      
      if (MappingConfiguration.Current.ClassDefinitions.GetMandatory (baseType).IsAbstract)
      {
        string message = string.Format (
          "Cannot instantiate type {0} as it is abstract; for classes with automatic properties, InstantiableAttribute must be used.",
          baseType.FullName);
        throw new ArgumentException (message, "baseType");
      }

      return _generator.GetInterceptableType (baseType);
    }

    /// <summary>
    /// Checkes whether a given domain object type was created by this factory implementation (but not necessarily the same factory instance).
    /// </summary>
    /// <param name="type">The type to be checked.</param>
    /// <returns>True if <paramref name="type"/> was created by a factory of the same implementation, else false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter was null.</exception>
    public bool WasCreatedByFactory (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return _generator.WasCreatedByGenerator (type);
    }

    /// <summary>
    /// Returns a construction object that can be used to instantiate objects of a given interceptable type.
    /// </summary>
    /// <typeparam name="TMinimal">The type statically returned by the construction object.</typeparam>
    /// <param name="type">The exatct interceptable type to be constructed; this must be a type returned by <see cref="GetConcreteDomainObjectType"/>.
    /// <typeparamref name="TMinimal"/> must be assignable from this type.</param>
    /// <returns>A construction object, which instantiates <paramref name="type"/> and returns <typeparamref name="TMinimal"/>.</returns>
    public IInvokeWith<TMinimal> GetTypesafeConstructorInvoker<TMinimal> (Type type)
    {
      if (!typeof (TMinimal).IsAssignableFrom (type))
      {
        string message = string.Format ("The required minimal type {0} and concrete type {1} are not compatible.",
          typeof (TMinimal).FullName, type.FullName);
        throw new ArgumentException (message);
      }

      if (!WasCreatedByFactory (type))
      {
        string message = string.Format ("Type {0} is not an interceptable type created by the factory.",
          type.FullName);
        throw new ArgumentException (message);
      }

      return _generator.MakeTypesafeConstructorInvoker<TMinimal> (type);
    }
  }
}
