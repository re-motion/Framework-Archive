using System;
using Remotion.Security.Web.ExecutionEngine;
using Remotion.Security.UnitTests.Web.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Security.UnitTests.Web.ExecutionEngine
{
  [WxeDemandTargetMethodPermission ("Show")]
  public class TestFunctionWithPermissionsFromInstanceMethod : WxeFunction
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public TestFunctionWithPermissionsFromInstanceMethod (SecurableObject thisObject)
      : base (thisObject)
    {
    }

    // methods and properties

    [WxeParameter (0, true, WxeParameterDirection.In)]
    public SecurableObject ThisObject
    {
      get
      {
        return (SecurableObject) Variables["ThisObject"];
      }
      set
      {
        Variables["ThisObject"] = value;
      }
    }
  }
}