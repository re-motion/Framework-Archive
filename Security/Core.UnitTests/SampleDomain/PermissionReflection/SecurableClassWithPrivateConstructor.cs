using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security.UnitTests.SampleDomain.PermissionReflection
{
  public class SecurableClassWithPrivateConstructor : ISecurableType
  {
    [RequiredMethodPermission (GeneralAccessType.Find)]
    protected SecurableClassWithPrivateConstructor ()
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
