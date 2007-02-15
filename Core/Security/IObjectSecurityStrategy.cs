using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  public interface IObjectSecurityStrategy
  {
    bool HasAccess (ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes);
  }
}
