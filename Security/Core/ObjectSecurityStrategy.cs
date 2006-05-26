using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public class ObjectSecurityStrategy : BaseSecurityStrategy, IObjectSecurityStrategy
  {
    private ISecurityContextFactory _contextFactory;

    public ObjectSecurityStrategy (ISecurityContextFactory securityContextFactory)
    {
      ArgumentUtility.CheckNotNull ("securityContextFactory", securityContextFactory);
      _contextFactory = securityContextFactory;
    }

    public bool HasAccess (ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securityService", securityService);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      return HasAccess (_contextFactory.GetSecurityContext (), securityService, user, requiredAccessTypes);
    }
  }
}
