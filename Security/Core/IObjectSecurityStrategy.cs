using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Rubicon.Security
{
  public interface IObjectSecurityStrategy
  {
    bool HasAccess (ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes);
  }
}
