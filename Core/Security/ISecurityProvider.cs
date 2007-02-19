using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  [Obsolete ("Use ISecurityProvider instead. (Version: 1.7.41)")]
  public interface ISecurityService : ISecurityProvider
  {
  }

  /// <summary>Provides access to the permission management functionality.</summary>
  /// <remarks>This service interface enables a plugable security system architecture, acting as single point of access to the permission management functionality.</remarks>
  public interface ISecurityProvider : INullableObject
  {
    /// <summary>Determines permission for a user.</summary>
    /// <param name="context">The <see cref="SecurityContext"/> gouping all object-specific security information of the current permission check.</param>
    /// <param name="user">The <see cref="IPrincipal"/> on whose behalf the permissions are evaluated.</param>
    /// <returns></returns>
    AccessType[] GetAccess (SecurityContext context, IPrincipal user);

    /// <summary>Get the actual revison number.</summary>
    /// <returns>The actual revison number.</returns>
    /// <remarks>The revison number is incremented when any cached permission becomes outdated; an incremented revision number indicates that the cache must be discared.</remarks>
    int GetRevision ();
  }
}