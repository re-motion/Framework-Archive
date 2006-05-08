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
      Enum[] actualAccessTypes = _securityService.GetAccess (context, userName);

      if (actualAccessTypes == null)
        return false;

      foreach (Enum requiredAccessType in requiredAccessTypes)
      {
        if (Array.IndexOf<Enum> (actualAccessTypes, requiredAccessType) >= 0)
          return true;
      }

      return false;
    }
  }
}
