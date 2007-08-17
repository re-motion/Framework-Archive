using System;

namespace Rubicon.Security.UnitTests.Core.SampleDomain
{
  public class DerivedSecurableObjectWithSecuredStaticMethods : SecurableObjectWithSecuredStaticMethods
  {
    [DemandMethodPermission (TestAccessTypes.Fourth)]
    public static void DerivedStaticMethod ()
    {
    }

    public DerivedSecurableObjectWithSecuredStaticMethods ()
    {
    }
  }
}
