using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UnitTests.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.ExecutionEngine
{
  public class TestFunctionWithThisObject : WxeFunction
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public TestFunctionWithThisObject (SecurableObject thisObject, object someObject)
      : base (thisObject, someObject)
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

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public object SomeObject
    {
      get
      {
        return Variables["SomeObject"];
      }
      set
      {
        Variables["SomeObject"] = value;
      }
    }
  }
}