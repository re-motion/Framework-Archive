using System;

namespace Rubicon.Security
{
  /// <summary>Defines the signature for a factory method to create a <see cref="SecurityContext"/> for a buiness object.</summary>
  /// <remarks><note type="implementnotes">Typically implemented by business objects (hence acting as their own <see cref="SecurityContext"/> factory).</note></remarks>
  public interface ISecurityContextFactory
  {
    SecurityContext CreateSecurityContext ();
  }
}
