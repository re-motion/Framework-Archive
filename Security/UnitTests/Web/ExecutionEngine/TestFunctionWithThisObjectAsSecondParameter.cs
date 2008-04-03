using System;
using Remotion.Security.UnitTests.Web.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Security.UnitTests.Web.ExecutionEngine
{
  public class TestFunctionWithThisObjectAsSecondParameter : WxeFunction
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public TestFunctionWithThisObjectAsSecondParameter (object someObject, SecurableObject thisObject)
      : base (someObject, thisObject)
    {
    }

    // methods and properties

    [WxeParameter (0, true, WxeParameterDirection.In)]
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

    [WxeParameter (1, true, WxeParameterDirection.In)]
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