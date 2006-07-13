using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public interface IAccessTypeCache
  {
    void Add (string key, AccessType[] accessTypes);
    AccessType[] Get (string key);
  }

  public class NullAccessTypeCache : IAccessTypeCache
  {
    public void Add (string key, AccessType[] accessTypes)
    {
    }

    public AccessType[] Get (string key)
    {
      return null;
    }
  }

  public class SecurityStrategy : ISecurityStrategy
  {
    private IAccessTypeCache _accessTypeCache;

    public SecurityStrategy (IAccessTypeCache accessTypeCache)
    {
      ArgumentUtility.CheckNotNull ("accessTypeCache", accessTypeCache);
      _accessTypeCache = accessTypeCache;
    }

    public bool HasAccess (SecurityContext context, ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("securityService", securityService);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      AccessType[] actualAccessTypes = _accessTypeCache.Get (user.Identity.Name);
      if (actualAccessTypes == null)
      {
        actualAccessTypes = securityService.GetAccess (context, user);
        if (actualAccessTypes == null)
          actualAccessTypes = new AccessType[0];
        _accessTypeCache.Add (user.Identity.Name, actualAccessTypes);
      }

      foreach (AccessType requiredAccessType in requiredAccessTypes)
      {
        if (Array.IndexOf<AccessType> (actualAccessTypes, requiredAccessType) < 0)
          return false;
      }

      return true;
    }
  }
}
