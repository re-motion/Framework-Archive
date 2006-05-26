using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public abstract class BaseSecurityStrategy
  {
    protected virtual bool HasAccess (SecurityContext context, ISecurityService securityService, IPrincipal user, AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("securityService", securityService);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      AccessType[] actualAccessTypes = securityService.GetAccess (context, user);

      if (actualAccessTypes == null)
        return false;

      foreach (AccessType requiredAccessType in requiredAccessTypes)
      {
        if (Array.IndexOf<AccessType> (actualAccessTypes, requiredAccessType) < 0)
          return false;
      }

      return true;
    }
  }
}
