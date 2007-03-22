using System;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Public interface for a factory creating instances of domain objects.
  /// </summary>
  /// <remarks>
  /// This interface is used internally by <see cref="DomainObject.Create"/> (indirectly via <see cref="NewStyleDomainObjectCreator"/> and should
  /// likely not be used directly. If a factory really needs to be accessed directly,
  /// <see cref="Rubicon.Data.DomainObjects.Configuration.DomainObjectsConfiguration"/> can be used to access the current factory.
  /// </remarks>
  public interface IDomainObjectFactory
  {
    object Create (Type type, params object[] args);
    bool WasCreatedByFactory (object o);
  }
}
