using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{
  [RequiredWxeFunctionPermission (typeof (SecurableClass), "Create")]
  public class TestFunctionWithPermissionsFromStaticMethods : WxeFunction
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public TestFunctionWithPermissionsFromStaticMethods ()
      : base ()
    {
    }

    // methods and properties
  }
}