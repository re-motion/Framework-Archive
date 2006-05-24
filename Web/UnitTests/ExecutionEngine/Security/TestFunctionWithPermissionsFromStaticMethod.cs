using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UnitTests.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine.Security
{
  [WxeDemandMethodPermission (MethodType.Static, SecurableClass=typeof (SecurableObject), Method="Create")]
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