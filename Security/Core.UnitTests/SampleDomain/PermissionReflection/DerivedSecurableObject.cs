using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security.UnitTests.SampleDomain.PermissionReflection
{
  public class DerivedSecurableObject : SecurableObject
  {
    [RequiredMethodPermission (GeneralAccessType.Find)]
    public DerivedSecurableObject ()
    {
    }

    public DerivedSecurableObject (ISecurityContextFactory contextFactory)
      : base (contextFactory)
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Read)]
    public new void Send ()
    {
    }

    [RequiredMethodPermission (GeneralAccessType.Create)]
    public override void Print ()
    {
      base.Print ();
    }
  }
}
