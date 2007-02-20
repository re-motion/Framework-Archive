using System;

namespace Rubicon.Security
{
  /// <summary>The base interface for all business objects that need security features.</summary>
  public interface ISecurableObject
  {
    /// <summary>Gets the <see cref="IObjectSecurityStrategy"/> used by that business object.</summary>
    /// <remarks>Primarily used by a <see cref="T:Rubicon.Security.SecurityClient"/> to dispatch security checks.</remarks>
    /// <returns>Returns the <see cref="IObjectSecurityStrategy"/>.</returns>
    IObjectSecurityStrategy GetSecurityStrategy ();
  }
}
