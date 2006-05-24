using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security.UnitTests.SampleDomain.PermissionReflection
{
  public class SecurableObjectWithPrivateOverloadedConstructor : ISecurableObject
  {
    [RequiredMethodPermission (GeneralAccessType.Find)]
    protected SecurableObjectWithPrivateOverloadedConstructor ()
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Edit)]
    public SecurableObjectWithPrivateOverloadedConstructor (string filename)
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
