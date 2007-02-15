using System;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class DerivedSecurableObjectWithSecuredInstanceMethods : SecurableObjectWithSecuredInstanceMethods
  {
    public DerivedSecurableObjectWithSecuredInstanceMethods ()
    {
    }

    [DemandMethodPermission (TestAccessTypes.Fourth)]
    public new void InstanceMethod ()
    {
    }
  }
}
