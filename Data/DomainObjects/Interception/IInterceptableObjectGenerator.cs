using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects.Interception
{
  public interface IInterceptableObjectGenerator<TInterceptorTarget>
  {
    IInterceptorSelector<TInterceptorTarget> Selector { get; set; }

    /// <summary>
    /// Creates a new interceptable instance of a type.
    /// </summary>
    /// <param name="type">The type which the object must support.</param>
    /// <param name="args">The arguments to be passed to the object's constructor.</param>
    /// <returns>A new interceptable object instance.</returns>
    /// <remarks><para>This method does not have to directly instantiate the given <paramref name="type"/>, but can instead dynamically create
    /// subclasses or proxies which add the ability to intercept method calls. It uses the <see cref="Selector"/> to determine which methods
    /// should be intercepted and to assign interceptors to methods.</para>
    /// <para>The given <paramref name="type"/> must implement a constructor whose signature matches the arguments passed via <paramref name="args"/>.
    /// Avoid passing ambiguous argument arrays to this method. Or better: Avoid writing objects with such construcors.</para>
    ///</remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> or <paramref name="args"/> argument is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="type"/> cannot be intercepted for some reason or it isn't at least of type
    /// <typeparamref name="TInterceptorTarget"/>.</exception>
    /// <exception cref="MissingMethodException">The given <paramref name="type"/> does not implement a corresponding public or protected constructor.
    /// </exception>
    /// <exception cref="System.Reflection.TargetInvocationException">The constructor of the given <paramref name="type"/> threw an exception. See
    /// <see cref="Exception.InnerException"/>.</exception>
    object CreateInterceptableObject(Type type, object[] args);

    /// <summary>
    /// Checks whether the given object was created by this generator implementation (but not necessarily by the same generator instance).
    /// </summary>
    /// <param name="o">The object to be checked.</param>
    /// <returns>True if <paramref name="o"/> was created by this generator implementation, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="o"/> parameter was <see langword="null"/></exception>
    bool WasCreatedByGenerator (object o);
  }
}
