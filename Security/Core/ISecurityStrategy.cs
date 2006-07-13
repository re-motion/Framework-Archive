using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  public interface ISecurityStrategy
  {
    bool HasAccess (SecurityContext context, ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes);
  }
}
