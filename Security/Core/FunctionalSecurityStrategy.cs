using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public class FunctionalSecurityStrategy : BaseSecurityStrategy, IFunctionalSecurityStrategy
  {
    public bool HasAccess (Type type, ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableObject));
      ArgumentUtility.CheckNotNull ("securityService", securityService);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      return HasAccess (new SecurityContext (type), securityService, user, requiredAccessTypes);
    }
  }
}
