using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Reflection;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  public interface IInterceptableObjectGenerator<TInterceptorTarget>
  {
    /// <summary>
    /// Used for deciding which members should be interceptable and what interception code should be executed for them.
    /// </summary>
    IInterceptorSelector<TInterceptorTarget> Selector { get; set; }

    /// <summary>
    /// Gets an interceptable type compatible with a given base type.
    /// </summary>
    /// <param name="baseType">The base type which the interceptable type must be compatible to.</param>
    /// <returns>An interceptable type compatible with <paramref name="baseType"/>.</returns>
    /// <remarks><para>This method can dynamically create subclasses or proxies which add the ability to intercept method calls. It uses the 
    /// <see cref="Selector"/> to determine which methods should be intercepted and to assign interceptors to methods.</para>
    ///</remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="baseType"/> argument is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="baseType"/> cannot be intercepted for some reason or it isn't at least of type
    /// <typeparamref name="TInterceptorTarget"/>.</exception>
    Type GetInterceptableType(Type baseType);

    /// <summary>
    /// Checks whether the given type was created by this generator implementation (but not necessarily by the same generator instance).
    /// </summary>
    /// <param name="type">The type to be checked.</param>
    /// <returns>True if <paramref name="type"/> was created by this generator implementation, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter was <see langword="null"/></exception>
    bool WasCreatedByGenerator (Type type);

    /// <summary>
    /// Returns a construction object that can be used to instantiate objects of a given interceptable type.
    /// </summary>
    /// <typeparam name="TMinimal">The type statically returned by the construction object.</typeparam>
    /// <param name="concreteType">The exatct interceptable type to be constructed; this must be a type returned by <see cref="GetInterceptableType"/>.
    /// <typeparamref name="TMinimal"/> must be assignable from this type.</param>
    /// <returns>A construction object, which instantiates <paramref name="concreteType"/> and returns <typeparamref name="TMinimal"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="concreteType"/> argument is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="concreteType"/> is not the same or a subtype of <typeparamref name="TMinimal"/> or
    /// <paramref name="concreteType"/> wasn't created by this kind of factory.</exception>
    IInvokeWith<TMinimal> MakeTypesafeConstructorInvoker<TMinimal> (Type concreteType);

    /// <summary>
    /// Prepares an instance which has not been created by construction via <see cref="MakeTypesafeConstructorInvoker"/> for use.
    /// </summary>
    /// <param name="instance">The instance to be prepared</param>
    /// <remarks>
    /// If an instance is constructed without a constructor call, e.g. by using
    /// <see cref="System.Runtime.Serialization.FormatterServices.GetSafeUninitializedObject"/> instead of <see cref="MakeTypesafeConstructorInvoker"/>,
    /// this method can be used to have the factory perform any initialization work it would otherwise have performed via the constructor.
    /// </remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="instance"/> argument is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type created by this kind of factory.</exception>
    void PrepareUnconstructedInstance (DomainObject instance);
  }
}
