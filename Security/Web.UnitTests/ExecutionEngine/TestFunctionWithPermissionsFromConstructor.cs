using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UnitTests.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.ExecutionEngine
{
  [WxeDemandMethodPermission (MethodType.Constructor, SecurableClass=typeof (SecurableObject))]
  public class TestFunctionWithPermissionsFromConstructor : WxeFunction
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public TestFunctionWithPermissionsFromConstructor ()
      : base ()
    {
    }

    // methods and properties
  }
}