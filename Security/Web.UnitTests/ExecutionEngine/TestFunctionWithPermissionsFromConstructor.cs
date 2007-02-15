using System;
using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Security.Web.UnitTests.Domain;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.ExecutionEngine
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