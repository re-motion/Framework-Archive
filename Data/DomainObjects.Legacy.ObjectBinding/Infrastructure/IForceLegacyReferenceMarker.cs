using System;

namespace Remotion.Data.DomainObjects.ObjectBinding.Infrastructure
{
  /// <summary>
  /// Marker interface forcing users of controls to explicitly reference <c>Remotion.Data.DomainObjects.Legacy.ObjectBinding.dll</c>.
  /// </summary>
  /// <remarks>
  /// When implementing a control based on this legacy assembly, implement this marker interface to force clients of your control to
  /// explicitly reference the assembly.
  /// </remarks>
  public interface IForceLegacyReferenceMarker
  {
  }
}
