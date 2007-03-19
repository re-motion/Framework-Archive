using System;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Public interface for a factory creating instances of domain objects.
  /// </summary>
  /// <remarks>
  /// This interface is used internally by <see cref="DomainObject.Create"/> and should likely not be used directly. If a factory needs to be
  /// accessed directly, <see cref="Rubicon.Data.DomainObjects.Configuration.DomainObjectsConfiguration"/> can be used to access the current factory.
  /// </remarks>
  public interface IDomainObjectFactory
  {
    object Create (Type type, params object[] args);
    bool WasCreatedByFactory (object o);
  }
}
