using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  /// <summary>Provider to determine the <see cref="IPrincipal"/> on whose behalf permissions are evaluated.</summary>
  public interface IUserProvider : INullableObject
  {
    /// <summary>Get the <see cref="IPrincipal"/> on whose behalf permissions are evaluated.</summary>
    /// <returns>The <see cref="IPrincipal"/> on whose behalf permissions are evaluated.</returns>
    IPrincipal GetUser();
  }
}