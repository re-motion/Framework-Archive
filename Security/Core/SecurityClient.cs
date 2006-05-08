using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Security
{
  public class SecurityClient
  {
    private ISecurityService _securityService;

    public SecurityClient (ISecurityService securityService)
    {
      ArgumentUtility.CheckNotNull ("securityService", securityService);
      _securityService = securityService;
    }

    public bool HasAccess (SecurityContext context, string userName, Enum[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNullOrEmpty ("userName", userName);
      ArgumentUtility.CheckNotNullOrEmpty ("requiredAccessTypes", requiredAccessTypes);

      Enum[] actualAccessTypes = _securityService.GetAccess (context, userName);

      if (actualAccessTypes == null)
        return false;

      foreach (Enum requiredAccessType in requiredAccessTypes)
      {
        if (Array.IndexOf<Enum> (actualAccessTypes, requiredAccessType) < 0)
          return false;
      }

      return true;
    }

    public bool HasAccess (ISecurableType securableType, string userName, Enum[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableType", securableType);
      ArgumentUtility.CheckNotNullOrEmpty ("userName", userName);
      ArgumentUtility.CheckNotNullOrEmpty ("requiredAccessTypes", requiredAccessTypes);

      ISecurityContextFactory contextFactory = securableType.GetSecurityContextFactory ();
      return HasAccess (contextFactory.GetSecurityContext (), userName, requiredAccessTypes);
    }
  }
}
