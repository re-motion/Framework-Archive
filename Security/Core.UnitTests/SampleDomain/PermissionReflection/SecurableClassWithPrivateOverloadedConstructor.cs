using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security.UnitTests.SampleDomain.PermissionReflection
{
  public class SecurableClassWithPrivateOverloadedConstructor : ISecurableType
  {
    [RequiredMethodPermission (GeneralAccessType.Find)]
    protected SecurableClassWithPrivateOverloadedConstructor ()
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Edit)]
    public SecurableClassWithPrivateOverloadedConstructor (string filename)
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
