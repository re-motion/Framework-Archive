using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public class ObjectSecurityStrategy : IObjectSecurityStrategy
  {
    private ISecurityStrategy _securityStrategy;
    private ISecurityContextFactory _securityContextFactory;

    public ObjectSecurityStrategy (ISecurityContextFactory securityContextFactory, ISecurityStrategy securityStrategy)
    {
      ArgumentUtility.CheckNotNull ("securityContextFactory", securityContextFactory);
      ArgumentUtility.CheckNotNull ("securityStrategy", securityStrategy);
      
      _securityContextFactory = securityContextFactory;
      _securityStrategy = securityStrategy;
    }

    public ObjectSecurityStrategy (ISecurityContextFactory securityContextFactory)
      : this (securityContextFactory, new SecurityStrategy (new NullAccessTypeCache<string> (), new NullGlobalAccessTypeCacheProvider()))
    {
    }

    public ISecurityStrategy SecurityStrategy
    {
      get { return _securityStrategy; }
    }

    public ISecurityContextFactory SecurityContextFactory
    {
      get { return _securityContextFactory; }
    }

    public bool HasAccess (ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securityService", securityService);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      return _securityStrategy.HasAccess (_securityContextFactory, securityService, user, requiredAccessTypes);
    }
  }
}
