using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security.UnitTests.SampleDomain.PermissionReflection
{
  public class DerivedSecurableClass : SecurableClass
  {
    [RequiredMethodPermission (GeneralAccessType.Find)]
    public DerivedSecurableClass ()
    {
    }

    public DerivedSecurableClass (ISecurityContextFactory contextFactory)
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
