using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain.PermissionReflection
{
  public class SecurableObjectWithoutConstructorOverloads : ISecurableObject
  {
    public SecurableObjectWithoutConstructorOverloads ()
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
