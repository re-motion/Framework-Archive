using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  public interface ISecurityService : INullableObject
  {
    AccessType[] GetAccess (SecurityContext context, IPrincipal user);

    int GetRevision ();
  }
}