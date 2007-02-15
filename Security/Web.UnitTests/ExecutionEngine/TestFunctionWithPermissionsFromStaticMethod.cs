using System;
using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Security.Web.UnitTests.Domain;
using Rubicon.Web.ExecutionEngine;

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