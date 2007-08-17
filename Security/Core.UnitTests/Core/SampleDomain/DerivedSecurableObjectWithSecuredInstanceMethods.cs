using System;

namespace Rubicon.Security.UnitTests.Core.SampleDomain
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
