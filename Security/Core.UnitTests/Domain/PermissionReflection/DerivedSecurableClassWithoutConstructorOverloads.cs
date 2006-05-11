using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security.UnitTests.Domain.PermissionReflection
{
  public class DerivedSecurableClassWithoutConstructorOverloads : SecurableClassWithoutConstructorOverloads
  {
    [RequiredMethodPermission (GeneralAccessType.Find)]
    public DerivedSecurableClassWithoutConstructorOverloads ()
    {
    }
  }
}
