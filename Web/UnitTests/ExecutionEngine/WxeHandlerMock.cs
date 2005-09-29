using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;
using System.Reflection;
using System.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.Configuration;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UntTests.ExecutionEngine
{

public class WxeHandlerMock: WxeHandler
{
  public new void CheckTimeoutConfiguration (HttpContext context)
  {
    base.CheckTimeoutConfiguration (context);
  }

  public new WxeFunctionState CreateNewFunctionState (HttpContext context, string typeName, string functionToken)
  {
    return base.CreateNewFunctionState (context, typeName, functionToken);
  }

  public new WxeFunctionState ResumeExistingFunctionState (HttpContext context, string functionToken)
  {
    return base.ResumeExistingFunctionState (context, functionToken);
  }

  public new void ProcessFunctionState (HttpContext context, WxeFunctionState functionState, bool isNewFunction)
  {
    base.ProcessFunctionState (context, functionState, isNewFunction);
  }

  public new void ExecuteFunctionState (HttpContext context, WxeFunctionState functionState, bool isNewFunction)
  {
    base.ExecuteFunctionState (context, functionState, isNewFunction);
  }

  public new virtual void ExecuteFunction (WxeFunction function, WxeContext context, bool isNew)
  {
    base.ExecuteFunction (function, context, isNew);
  }

  public new void CleanUpFunctionState (WxeFunctionState functionState)
  {
    base.CleanUpFunctionState (functionState);
  }

  public new void ProcessReturnUrl (HttpContext context, string returnUrl)
  {
    base.ProcessReturnUrl (context, returnUrl);
  }
}

}