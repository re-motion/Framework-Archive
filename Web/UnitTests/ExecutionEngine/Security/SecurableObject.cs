using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;
using Rubicon.Web.UnitTests.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine.Security
{
  public class SecurableObject : ISecurableObject
  {
    [RequiredMethodPermission (GeneralAccessType.Create)]
    public static void Create ()
    {
    }

    private ISecurityContextFactory _securityContextFactory;

    public SecurableObject (ISecurityContextFactory securityContextFactory)
    {
      _securityContextFactory = securityContextFactory;
    }

    [RequiredMethodPermission (GeneralAccessType.Edit, GeneralAccessType.Read)]
    public void Show ()
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Delete)]
    public void Delete ()
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      return _securityContextFactory;
    }
  }
}
