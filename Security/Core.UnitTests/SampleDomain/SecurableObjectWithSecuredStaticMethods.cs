using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class SecurableObjectWithSecuredStaticMethods : ISecurableObject
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

    public SecurableObjectWithSecuredStaticMethods ()
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new NotImplementedException ("The method or operation is not implemented.");
    }
  }
}
