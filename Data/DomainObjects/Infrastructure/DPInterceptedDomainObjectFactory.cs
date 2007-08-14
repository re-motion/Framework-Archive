using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Infrastructure.DPInterception;
using Rubicon.Data.DomainObjects.Infrastructure.DPInterception.Castle;
using Rubicon.Reflection;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Default implementation of <see cref="IDomainObjectFactory"/>.
  /// </summary>
  public class DPInterceptedDomainObjectFactory : IDomainObjectFactory
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
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> argument is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="type"/> is not the same or a subtype of <typeparamref name="TMinimal"/> or
    /// <paramref name="type"/> wasn't created by this kind of factory.</exception>
    public IFuncInvoker<TMinimal> GetTypesafeConstructorInvoker<TMinimal> (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return _generator.MakeTypesafeConstructorInvoker<TMinimal> (type);
    }

    /// <summary>
    /// Prepares an instance which has not been created by construction via <see cref="GetTypesafeConstructorInvoker"/> for use.
    /// </summary>
    /// <param name="instance">The instance to be prepared</param>
    /// <remarks>
    /// If an instance is constructed without a constructor call, e.g. by using
    /// <see cref="System.Runtime.Serialization.FormatterServices.GetSafeUninitializedObject"/> instead of <see cref="GetTypesafeConstructorInvoker"/>,
    /// this method can be used to have the factory perform any initialization work it would otherwise have performed via the constructor.
    /// </remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="instance"/> argument is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type created by this kind of factory.</exception>
    public void PrepareUnconstructedInstance (DomainObject instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      _generator.PrepareUnconstructedInstance (instance);
    }
  }
}
