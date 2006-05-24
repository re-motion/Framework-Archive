using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class SecurableObjectWithSecuredConstructors : ISecurableObject
  {
    [RequiredMethodPermission (TestAccessType.First)]
    public SecurableObjectWithSecuredConstructors ()
    {
    }

    [RequiredMethodPermission (TestAccessType.Second)]
    public SecurableObjectWithSecuredConstructors (string value)
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new NotImplementedException ("The method or operation is not implemented.");
    }
  }
}
