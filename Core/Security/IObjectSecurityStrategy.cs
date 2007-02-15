using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  public interface IObjectSecurityStrategy
  {
    bool HasAccess (ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes);
  }
}
