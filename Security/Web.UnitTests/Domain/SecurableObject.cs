using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;
using Rubicon.Web.UnitTests.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.Domain
{
  public class SecurableObject : ISecurableObject
  {
    public enum Methods
    {
      Delete,
      Show,
      Search
    }

    [DemandMethodPermission (GeneralAccessType.Search)]
    public static void Search ()
    {
    }

    private IObjectSecurityStrategy _securityStrategy;

    public SecurableObject (IObjectSecurityStrategy securityStrategy)
    {
      _securityStrategy = securityStrategy;
    }

    [DemandMethodPermission (GeneralAccessType.Read)]
    public void Show ()
    {
    }

    [DemandMethodPermission (GeneralAccessType.Delete)]
    public void Delete ()
    {
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _securityStrategy;
    }
  }
}
