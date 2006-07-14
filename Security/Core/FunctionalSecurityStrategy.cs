using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Security.Configuration;

namespace Rubicon.Security
{
  public class FunctionalSecurityStrategy : IFunctionalSecurityStrategy
  {
    private ISecurityStrategy _securityStrategy;

    public FunctionalSecurityStrategy (ISecurityStrategy securityStrategy)
    {
      ArgumentUtility.CheckNotNull ("securityStrategy", securityStrategy);

      _securityStrategy = securityStrategy;
    }

    public FunctionalSecurityStrategy ()
      : this (new SecurityStrategy (new NullAccessTypeCache<string> (), SecurityConfiguration.Current.GlobalAccessTypeCacheProvider))
    {
    }

    public ISecurityStrategy SecurityStrategy
    {
      get { return _securityStrategy; }
    }

    public bool HasAccess (Type type, ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableObject));
      ArgumentUtility.CheckNotNull ("securityService", securityService);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      return _securityStrategy.HasAccess (new FunctionalSecurityContextFactory (type), securityService, user, requiredAccessTypes);
    }
  }
}
