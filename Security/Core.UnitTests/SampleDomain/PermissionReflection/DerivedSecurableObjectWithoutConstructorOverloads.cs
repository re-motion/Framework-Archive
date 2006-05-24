using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security.UnitTests.SampleDomain.PermissionReflection
{
  public class DerivedSecurableObjectWithoutConstructorOverloads : SecurableObjectWithoutConstructorOverloads
  {
    [RequiredMethodPermission (GeneralAccessType.Find)]
    public DerivedSecurableObjectWithoutConstructorOverloads ()
    {
    }
  }
}
