using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Security.Configuration;

namespace Rubicon.Security
{
  public class SecurityStrategy : ISecurityStrategy
  {
    private IAccessTypeCache<string> _localCache;
    private IGlobalAccessTypeCacheProvider _globalCacheProvider;

    public SecurityStrategy ()
      : this (new AccessTypeCache<string> (), SecurityConfiguration.Current.GlobalAccessTypeCacheProvider)
    {
    }

    public SecurityStrategy (IAccessTypeCache<string> localCache, IGlobalAccessTypeCacheProvider globalCacheProvider)
    {
      ArgumentUtility.CheckNotNull ("localCache", localCache);
      ArgumentUtility.CheckNotNull ("globalCacheProvider", globalCacheProvider);

      _localCache = localCache;
      _globalCacheProvider = globalCacheProvider;
    }

    public IAccessTypeCache<string> LocalCache
    {
      get { return _localCache; }
    }

    public IGlobalAccessTypeCacheProvider GlobalCacheProvider
    {
      get { return _globalCacheProvider; }
    }

    public void InvalidateLocalCache ()
    {
      _localCache.Clear ();
    }

    public bool HasAccess (ISecurityContextFactory factory, ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("factory", factory);
      ArgumentUtility.CheckNotNull ("securityService", securityService);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      AccessType[] actualAccessTypes = GetAccessFromLocalCache (factory, securityService, user);

      foreach (AccessType requiredAccessType in requiredAccessTypes)
      {
        if (Array.IndexOf<AccessType> (actualAccessTypes, requiredAccessType) < 0)
          return false;
      }

      return true;
    }

    private AccessType[] GetAccessFromLocalCache (ISecurityContextFactory factory, ISecurityService securityService, IPrincipal user)
    {
      AccessType[] actualAccessTypes = _localCache.Get (user.Identity.Name);
      if (actualAccessTypes == null)
      {
        actualAccessTypes = GetAccessFromGlobalCache (factory, securityService, user);
        _localCache.Add (user.Identity.Name, actualAccessTypes);
      }
      return actualAccessTypes;
    }

    private AccessType[] GetAccessFromGlobalCache (ISecurityContextFactory factory, ISecurityService securityService, IPrincipal user)
    {
      IAccessTypeCache<GlobalAccessTypeCacheKey> globalAccessTypeCache = _globalCacheProvider.GetAccessTypeCache ();
      if (globalAccessTypeCache == null)
        throw new InvalidOperationException ("IGlobalAccesTypeCacheProvider.GetAccessTypeCache() evaluated and returned null.");

      SecurityContext context = factory.CreateSecurityContext ();
      if (context == null)
        throw new InvalidOperationException ("ISecurityContextFactory.CreateSecurityContext() evaluated and returned null.");

      GlobalAccessTypeCacheKey key = new GlobalAccessTypeCacheKey (context, user);
      AccessType[] actualAccessTypes = globalAccessTypeCache.Get (key);
      if (actualAccessTypes == null)
      {
        actualAccessTypes = GetAccessFromSecurityService (securityService, context, user);
        globalAccessTypeCache.Add (key, actualAccessTypes);
      }
      return actualAccessTypes;
    }

    private AccessType[] GetAccessFromSecurityService (ISecurityService securityService, SecurityContext context, IPrincipal user)
    {
      AccessType[] actualAccessTypes = securityService.GetAccess (context, user);
      if (actualAccessTypes == null)
        actualAccessTypes = new AccessType[0];
      return actualAccessTypes;
    }
  }
}
