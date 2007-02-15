using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  public interface IFunctionalSecurityStrategy
  {
    bool HasAccess (Type type, ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes);
  }
}
