using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class DerivedSecurableObjectWithSecuredInstanceMethods : SecurableObjectWithSecuredInstanceMethods
  {
    public DerivedSecurableObjectWithSecuredInstanceMethods ()
    {
    }

    [RequiredMethodPermission (TestAccessType.Fourth)]
    public void InstanceMethod ()
    {
    }
  }
}
