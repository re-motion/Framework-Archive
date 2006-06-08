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