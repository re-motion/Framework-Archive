using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{
  [RequiredWxeFunctionPermission (typeof (SecurableClass))]
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