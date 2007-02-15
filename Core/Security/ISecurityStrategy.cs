using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  public interface ISecurityStrategy
  {
    bool HasAccess (ISecurityContextFactory factory, ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes);
    void InvalidateLocalCache ();
  }
}
