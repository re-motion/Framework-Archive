using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{
  public class SecurableClass : ISecurableType
  {
    [RequiredMethodPermission (GeneralAccessType.Create)]
    public static void Create ()
    {
    }

    private ISecurityContextFactory _securityContextFactory;

    [RequiredMethodPermission (GeneralAccessType.Create)]
    public SecurableClass (ISecurityContextFactory securityContextFactory)
    {
      _securityContextFactory = securityContextFactory;
    }

    [RequiredMethodPermission (GeneralAccessType.Edit)]
    [RequiredMethodPermission (GeneralAccessType.Read)]
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
