using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class SecurableClassWithSecuredConstructors : ISecurableType
  {
    [RequiredMethodPermission (TestAccessType.First)]
    public SecurableClassWithSecuredConstructors ()
    {
    }

    [RequiredMethodPermission (TestAccessType.Second)]
    public SecurableClassWithSecuredConstructors (string value)
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new NotImplementedException ("The method or operation is not implemented.");
    }
  }
}
