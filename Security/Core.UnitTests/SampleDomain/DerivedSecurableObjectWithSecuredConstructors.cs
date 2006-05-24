using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class DerivedSecurableObjectWithSecuredConstructors : SecurableObjectWithSecuredConstructors
  {
    [RequiredMethodPermission (TestAccessType.Fourth)]
    public DerivedSecurableObjectWithSecuredConstructors ()
    {
    }
  }
}
