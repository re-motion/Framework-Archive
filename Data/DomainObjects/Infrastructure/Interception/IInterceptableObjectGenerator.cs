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

    IInvokeWith<T> MakeTypesafeConstructorInvoker<T> (Type concreteType);
  }
}
