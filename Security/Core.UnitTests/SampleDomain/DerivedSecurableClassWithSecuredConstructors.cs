using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class DerivedSecurableClassWithSecuredConstructors : SecurableClassWithSecuredConstructors
  {
    [RequiredMethodPermission (TestAccessType.Fourth)]
    public DerivedSecurableClassWithSecuredConstructors ()
    {
    }
  }
}
