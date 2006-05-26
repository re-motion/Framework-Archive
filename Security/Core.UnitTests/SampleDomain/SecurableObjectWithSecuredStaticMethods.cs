using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class SecurableObjectWithSecuredStaticMethods : ISecurableObject
  {
    [DemandMethodPermission (TestAccessType.First)]
    public static void StaticMethod ()
    {
    }

    [DemandMethodPermission (TestAccessType.Second)]
    public static void StaticMethod (string value)
    {
    }

    [DemandMethodPermission (TestAccessType.Third)]
    public static void OtherStaticMethod (string value)
    {
    }

    public SecurableObjectWithSecuredStaticMethods ()
    {
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
