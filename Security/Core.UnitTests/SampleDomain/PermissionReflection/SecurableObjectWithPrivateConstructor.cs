using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security.UnitTests.SampleDomain.PermissionReflection
{
  public class SecurableObjectWithPrivateConstructor : ISecurableObject
  {
    [RequiredMethodPermission (GeneralAccessType.Find)]
    protected SecurableObjectWithPrivateConstructor ()
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
