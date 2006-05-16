using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class SecurableClassWithSecuredStaticMethods : ISecurableType
  {
    [RequiredMethodPermission (TestAccessType.First)]
    public static void StaticMethod ()
    {
    }

    [RequiredMethodPermission (TestAccessType.Second)]
    public static void StaticMethod (string value)
    {
    }

    [RequiredMethodPermission (TestAccessType.Third)]
    public static void OtherStaticMethod (string value)
    {
    }

    public SecurableClassWithSecuredStaticMethods ()
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new NotImplementedException ("The method or operation is not implemented.");
    }
  }
}
