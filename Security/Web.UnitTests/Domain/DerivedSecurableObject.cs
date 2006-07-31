using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security.Web.UnitTests.Domain
{
  public class DerivedSecurableObject : SecurableObject
  {
    // types

    public new enum Method
    {
      Create,
      Delete,
      Show,
      Search
    }

    // static members

    // member fields

    // construction and disposing

    public DerivedSecurableObject (IObjectSecurityStrategy securityStrategy)
      : base (securityStrategy)
    {
    }

    // methods and properties

    [DemandMethodPermission (GeneralAccessTypes.Read)]
    public void ShowSpecial ()
    {
    }
  }
}