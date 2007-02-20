using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  /// <summary>Defines an interface for retrieving the current user.</summary>
  public interface IUserProvider : INullableObject
  {
    /// <summary>Gets the current user.</summary>
    /// <returns>The <see cref="IPrincipal"/> representing the current user.</returns>
    IPrincipal GetUser();
  }
}