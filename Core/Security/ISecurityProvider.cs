using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  [Obsolete ("Use ISecurityProvider instead. (Version: 1.7.41)")]
  public interface ISecurityService : ISecurityProvider
  {
  }

  public interface ISecurityProvider : INullableObject
  {
    AccessType[] GetAccess (SecurityContext context, IPrincipal user);

    int GetRevision ();
  }
}