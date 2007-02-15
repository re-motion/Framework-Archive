using System;
using System.Security.Principal;
using Rubicon.Collections;
using Rubicon.Security.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Security
{
  public class SecurityStrategy : ISecurityStrategy
  {
    private ICache<string, AccessType[]> _localCache;
    private IGlobalAccessTypeCacheProvider _globalCacheProvider;

    public SecurityStrategy ()
      : this (new Cache<string, AccessType[]> (), SecurityConfiguration.Current.GlobalAccessTypeCacheProvider)
    {
    }

    public SecurityStrategy (ICache<string, AccessType[]> localCache, IGlobalAccessTypeCacheProvider globalCacheProvider)
    {
      ArgumentUtility.CheckNotNull ("localCache", localCache);
      ArgumentUtility.CheckNotNull ("globalCacheProvider", globalCacheProvider);

      _localCache = localCache;
      _globalCacheProvider = globalCacheProvider;
    }

    public ICache<string, AccessType[]> LocalCache
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
      return _localCache.GetOrCreateValue (user.Identity.Name, delegate { return GetAccessFromGlobalCache (factory, securityService, user); });
    }

    private AccessType[] GetAccessFromGlobalCache (ISecurityContextFactory factory, ISecurityService securityService, IPrincipal user)
    {
      ICache<Tuple<SecurityContext, string>, AccessType[]> globalAccessTypeCache = _globalCacheProvider.GetCache ();
      if (globalAccessTypeCache == null)
        throw new InvalidOperationException ("IGlobalAccesTypeCacheProvider.GetAccessTypeCache() evaluated and returned null.");

      SecurityContext context = factory.CreateSecurityContext ();
      if (context == null)
        throw new InvalidOperationException ("ISecurityContextFactory.CreateSecurityContext() evaluated and returned null.");

      Tuple<SecurityContext, string> key = new Tuple<SecurityContext, string> (context, user.Identity.Name);
      return globalAccessTypeCache.GetOrCreateValue (key, delegate { return GetAccessFromSecurityService (securityService, context, user); });
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
