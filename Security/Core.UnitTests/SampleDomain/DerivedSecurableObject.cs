using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  public class DerivedSecurableObject : SecurableObject
  {
    //[RequiredMethodPermission (GeneralAccessType.Edit)]
    //public static new string GetObjectName (SecurableObject securableObject)
    //{
    //  return null;
    //}

    public DerivedSecurableObject ()
    {
    }

    public DerivedSecurableObject (ISecurityContextFactory contextFactory)
      : base (contextFactory)
    {
    }

    [DemandMethodPermission (GeneralAccessType.Read)]
    public new void Send ()
    {
    }

    [DemandMethodPermission (GeneralAccessType.Create)]
    public override void Print ()
    {
      base.Print ();
    }
  }
}
