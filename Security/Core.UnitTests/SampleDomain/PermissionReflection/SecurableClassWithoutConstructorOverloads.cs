using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain.PermissionReflection
{
  public class SecurableClassWithoutConstructorOverloads : ISecurableType
  {
    public SecurableClassWithoutConstructorOverloads ()
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
