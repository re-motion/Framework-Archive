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

    [DemandMethodPermission (TestAccessType.First)]
    public void InstanceMethod ()
    {
    }

    [DemandMethodPermission (TestAccessType.Second)]
    public void InstanceMethod (string value)
    {
    }

    [DemandMethodPermission (TestAccessType.Third)]
    public void OtherInstanceMethod (string value)
    {
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
