using System;
using Rubicon.Reflection;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Public interface for a factory creating instances of domain objects.
  /// </summary>
  /// <remarks>
  /// This interface is used internally by <see cref="DomainObject.NewObject"/> (indirectly via <see cref="NewStyleDomainObjectCreator"/>
  /// and should likely not be used directly. If a factory really needs to be accessed directly,
  /// <see cref="Rubicon.Data.DomainObjects.Configuration.DomainObjectsConfiguration"/> can be used to access the current factory.
  /// </remarks>
  public interface IDomainObjectFactory
  {
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
    Type GetConcreteDomainObjectType (Type baseType);

    /// <summary>
    /// Checkes whether a given domain object type was created by this factory implementation (but not necessarily the same factory instance).
    /// </summary>
    /// <param name="type">The type to be checked.</param>
    /// <returns>True if <paramref name="type"/> was created by a factory of the same implementation, else false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter was null.</exception>
    bool WasCreatedByFactory (Type type);

    /// <summary>
    /// Returns a construction object that can be used to instantiate objects of a given interceptable type.
    /// </summary>
    /// <typeparam name="TMinimal">The type statically returned by the construction object.</typeparam>
    /// <param name="type">The exatct interceptable type to be constructed; this must be a type returned by <see cref="GetConcreteDomainObjectType"/>.
    /// <typeparamref name="TMinimal"/> must be assignable from this type.</param>
    /// <returns>A construction object, which instantiates <paramref name="type"/> and returns <typeparamref name="TMinimal"/>.</returns>
    IInvokeWith<TMinimal> GetTypesafeConstructorInvoker<TMinimal> (Type type);
  }
}
