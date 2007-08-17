using System;
using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Security.UnitTests.Web.Domain;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Security.UnitTests.Web.ExecutionEngine
{
  [WxeDemandCreatePermission (typeof (SecurableObject))]
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