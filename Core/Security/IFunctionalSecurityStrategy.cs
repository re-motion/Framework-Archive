using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Rubicon.Security
{
  public interface IFunctionalSecurityStrategy
  {
    bool HasAccess (Type type, ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes);
  }
}
