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
    private ISecurityContextFactory _contextFactory;

    public ObjectSecurityStrategy (ISecurityContextFactory securityContextFactory, ISecurityStrategy securityStrategy)
    {
      ArgumentUtility.CheckNotNull ("securityContextFactory", securityContextFactory);
      ArgumentUtility.CheckNotNull ("securityStrategy", securityStrategy);
      
      _contextFactory = securityContextFactory;
      _securityStrategy = securityStrategy;
    }

    public ObjectSecurityStrategy (ISecurityContextFactory securityContextFactory)
      : this (securityContextFactory, new SecurityStrategy (new NullAccessTypeCache ()))
    {
    }

    public bool HasAccess (ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securityService", securityService);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      return _securityStrategy.HasAccess (_contextFactory.GetSecurityContext (), securityService, user, requiredAccessTypes);
    }
  }
}
