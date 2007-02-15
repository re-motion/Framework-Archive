using System;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class SecurableObjectWithSecuredStaticMethods : ISecurableObject
  {
    [DemandMethodPermission (TestAccessTypes.First)]
    public static void StaticMethod ()
    {
    }

    [DemandMethodPermission (TestAccessTypes.Second)]
    public static void StaticMethod (string value)
    {
    }

    [DemandMethodPermission (TestAccessTypes.Third)]
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
