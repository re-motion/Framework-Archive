using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class DerivedSecurableClassWithSecuredStaticMethods : SecurableClassWithSecuredStaticMethods
  {
    [RequiredMethodPermission (TestAccessType.Fourth)]
    public static void DerivedStaticMethod ()
    {
    }

    public DerivedSecurableClassWithSecuredStaticMethods ()
    {
    }
  }
}
