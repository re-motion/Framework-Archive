using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class DerivedSecurableClassWithSecuredInstanceMethods : SecurableClassWithSecuredInstanceMethods
  {
    public DerivedSecurableClassWithSecuredInstanceMethods ()
    {
    }

    [RequiredMethodPermission (TestAccessType.Fourth)]
    public void InstanceMethod ()
    {
    }
  }
}
