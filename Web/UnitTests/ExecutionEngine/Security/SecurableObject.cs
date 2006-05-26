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

    private ISecurityContextFactory _securityContextFactory;

    public SecurableObject (ISecurityContextFactory securityContextFactory)
    {
      _securityContextFactory = securityContextFactory;
    }

    [DemandMethodPermission (GeneralAccessType.Edit, GeneralAccessType.Read)]
    public void Show ()
    {
    }

    [DemandMethodPermission (GeneralAccessType.Delete)]
    public void Delete ()
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      return _securityContextFactory;
    }
  }
}
