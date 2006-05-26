using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;
using Rubicon.Web.UnitTests.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine.Security
{
  public class SecurableObject : ISecurableObject
  {
    [DemandMethodPermission (GeneralAccessType.Create)]
    public static void Create ()
    {
    }

    private IObjectSecurityStrategy _securityStrategy;

    public SecurableObject (ISecurityContextFactory securityContextFactory)
    {
      _securityStrategy = new ObjectSecurityStrategy (securityContextFactory);
    }

    [DemandMethodPermission (GeneralAccessType.Edit, GeneralAccessType.Read)]
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
