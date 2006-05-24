using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class SecurableObjectWithSecuredInstanceMethods : ISecurableObject
  {
    public SecurableObjectWithSecuredInstanceMethods ()
    {
    }

    [RequiredMethodPermission (TestAccessType.First)]
    public void InstanceMethod ()
    {
    }

    [RequiredMethodPermission (TestAccessType.Second)]
    public void InstanceMethod (string value)
    {
    }

    [RequiredMethodPermission (TestAccessType.Third)]
    public void OtherInstanceMethod (string value)
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new NotImplementedException ("The method or operation is not implemented.");
    }
  }
}
