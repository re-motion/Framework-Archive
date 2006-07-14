using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  public interface ISecurityStrategy
  {
    bool HasAccess (ISecurityContextFactory factory, ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes);
    void InvalidateLocalCache ();
  }
}
