using System;
using Remotion.Security.UnitTests.Web.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Security.UnitTests.Web.ExecutionEngine
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