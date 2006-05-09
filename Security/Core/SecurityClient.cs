using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Rubicon.Utilities;
using Rubicon.Security.Configuration;

namespace Rubicon.Security
{
  public class SecurityClient
  {
    private ISecurityService _securityService;

    public SecurityClient ()
        : this (SecurityConfiguration.Current.SecurityService)
    {
    }

    public SecurityClient (ISecurityService securityService)
    {
      ArgumentUtility.CheckNotNull ("securityService", securityService);
      _securityService = securityService;
    }

    public bool HasAccess (SecurityContext context, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmpty ("requiredAccessTypes", requiredAccessTypes);

      AccessType[] actualAccessTypes = _securityService.GetAccess (context, user);

      if (actualAccessTypes == null)
        return false;

      foreach (AccessType requiredAccessType in requiredAccessTypes)
      {
        if (Array.IndexOf<AccessType> (actualAccessTypes, requiredAccessType) < 0)
          return false;
      }

      return true;
    }

    public bool HasAccess (ISecurableType securableType, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableType", securableType);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmpty ("requiredAccessTypes", requiredAccessTypes);

      ISecurityContextFactory contextFactory = securableType.GetSecurityContextFactory ();
      if (contextFactory == null)
        throw new ArgumentException ("The securable type did not return a ISecurityContextFactory.", "securableType");

      return HasAccess (contextFactory.GetSecurityContext (), user, requiredAccessTypes);
    }

    public bool HasAccess (SecurityContext context, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNullOrEmpty ("requiredAccessTypes", requiredAccessTypes);

      return HasAccess (context, GetCurrentUser (), requiredAccessTypes);
    }

    public bool HasAccess (ISecurableType securableType, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableType", securableType);
      ArgumentUtility.CheckNotNullOrEmpty ("requiredAccessTypes", requiredAccessTypes);

      return HasAccess (securableType, GetCurrentUser (), requiredAccessTypes);
    }

    private IPrincipal GetCurrentUser ()
    {
      return SecurityConfiguration.Current.UserProvider.GetUser ();
    }
  }
}
