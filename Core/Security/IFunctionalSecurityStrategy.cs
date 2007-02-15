using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  public interface IFunctionalSecurityStrategy
  {
    bool HasAccess (Type type, ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes);
  }
}
