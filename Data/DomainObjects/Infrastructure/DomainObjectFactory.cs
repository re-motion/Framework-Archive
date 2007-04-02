using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Infrastructure.Interception;
using Rubicon.Data.DomainObjects.Infrastructure.Interception.Castle;

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
    /// Creates a new interceptable instance of a domain object.
    /// </summary>
    /// <param name="type">The type which the object must support.</param>
    /// <param name="args">The arguments to be passed to the domain object's constructor.</param>
    /// <returns>A new domain object instance.</returns>
    /// <remarks><para>This method does not directly instantiate the given <paramref name="type"/>, but instead dynamically creates a subclass that
    /// intercepts certain method calls in order to perform management tasks.</para>
    /// <para>This method ensures that the created domain object supports the new property syntax.</para>
    /// <para>The given <paramref name="type"/> must implement a constructor whose signature matches the arguments passed via <paramref name="args"/>.
    /// Avoid passing ambiguous argument arrays to this method. Or better: Avoid writing domain objects with such construcors.
    /// If you need to call a constructor with exactly one <see langword="null"/> argument, you can either pass <c>null</c> or <c>new object[] { null }</c>
    /// to this method. If you need to call a constructor which takes a single argument of array type, wrap it in a dedicated
    /// <c>object[]</c> (e.g. <c>new object[] { new int[] { 1, 2, 3 } }</c>).</para></remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> argument is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="type"/> argument is sealed, contains abstract methods (apart from automatic
    /// properties), or is not derived from <see cref="DomainObject"/>.</exception>
    /// <exception cref="MissingMethodException">The given <paramref name="type"/> does not implement a corresponding public or protected constructor.
    /// </exception>
    /// <exception cref="System.Reflection.TargetInvocationException">The constructor of the given <paramref name="type"/> threw an exception. See
    /// <see cref="Exception.InnerException"/>.</exception>
    public object Create (Type type, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (!typeof (DomainObject).IsAssignableFrom (type))
      {
        string message = string.Format("Cannot instantiate type {0} as it is not derived from DomainObject.", type.FullName);
        throw new ArgumentException (message, "type");
      }
      if (type.IsSealed)
      {
        string message = string.Format ("Cannot instantiate type {0} as it is sealed.", type.FullName);
        throw new ArgumentException (message, "type");
      }
      if (type.IsAbstract && !type.IsDefined (typeof (NotAbstractAttribute), false))
      {
        string message = string.Format("Cannot instantiate type {0} as it is abstract; for automatic properties, NotAbstractAttribute must be used.",
            type.FullName);
        throw new ArgumentException (message, "type");
      }
      if (args == null)
      {
        args = new object[] { null };
      }

      return _generator.CreateInterceptableObject (type, args);
    }

    /// <summary>
    /// Checkes whether a given object instance was created by the factory or not.
    /// </summary>
    /// <param name="o">The object instance to be checked.</param>
    /// <returns>True if <paramref name="o"/> was created by the factory, else false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="o"/> parameter was null.</exception>
    public bool WasCreatedByFactory (object o)
    {
      ArgumentUtility.CheckNotNull ("o", o);
      return _generator.WasCreatedByGenerator (o);
    }
  }
}
