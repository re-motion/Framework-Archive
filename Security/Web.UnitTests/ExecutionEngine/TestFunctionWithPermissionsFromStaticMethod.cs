using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Security.Web.UnitTests.Domain;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UnitTests.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.ExecutionEngine
{
  [WxeDemandTargetStaticMethodPermission ("Search", typeof (SecurableObject))]
  public class TestFunctionWithPermissionsFromStaticMethod : WxeFunction
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public TestFunctionWithPermissionsFromStaticMethod ()
      : base ()
    {
    }

    // methods and properties
  }
}