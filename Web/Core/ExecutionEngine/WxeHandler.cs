using System;
using System.Web;
using System.Collections;
using System.Web.UI;
using System.Web.SessionState;
using System.Reflection;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

public class WxeHandler: IHttpHandler, IRequiresSessionState
{
  WxeFunction _currentFunction;

  public WxeFunction CurrentFunction
  {
    get { return _currentFunction; }
  }

  public void ProcessRequest (HttpContext context)
  {
    WxeFunctionState functionState = null;
    string functionToken = context.Request.Params["WxeFunctionToken"];
    WxeFunctionStateCollection functionStates = WxeFunctionStateCollection.Instance;

    string typeName = context.Request.Params["WxeFunctionType"];
    if (typeName != null)
    {
      Type type = TypeUtility.GetType (typeName, true, false);
      _currentFunction = (WxeFunction) Activator.CreateInstance (type);

      if (functionStates == null)
      {
        functionStates = new WxeFunctionStateCollection();
        WxeFunctionStateCollection.Instance = functionStates;
      }

      if (functionToken != null)
        functionState = new WxeFunctionState (_currentFunction, functionToken, 20); // TODO: make lifetime configurable
      else
        functionState = new WxeFunctionState (_currentFunction, 20);
      functionStates.Add (functionState);

      _currentFunction.InitializeParameters (context.Request.Params);
      string returnUrlArg = context.Request.Params["ReturnUrl"];
      if (returnUrlArg != null)
        _currentFunction.ReturnUrl = returnUrlArg;
    }
    else if (functionToken != null)
    {
      if (functionStates == null)
        throw new ApplicationException ("Session timeout."); // TODO: display error message

      functionState = functionStates.GetItem (functionToken);
      if (functionState == null)
        throw new ApplicationException ("Page timeout."); // TODO: display error message
      functionState.Touch();

      functionStates.DisposeExpired();

      _currentFunction = functionState.Function;
      if (_currentFunction == null)
        throw new ApplicationException ("Function missing in WxeFunctionState {0}." + functionState.FunctionToken);
      string action = context.Request["WxeAction"];
      if (action == "cancel")
      {
        functionStates.Remove (functionState);
        return;
      }
    }
    else
    {
      throw new HttpException ("Missing URL parameter 'WxeFunctionType'");
    }

    WxeContext wxeContext = new WxeContext (context); 
    wxeContext.FunctionToken = functionState.FunctionToken;
    WxeContext.SetCurrent (wxeContext);

    _currentFunction.Execute (wxeContext);

    string returnUrl = _currentFunction.ReturnUrl;
    if (returnUrl != null)
    {
      // Variables.Clear();
      if (returnUrl.StartsWith ("javascript:"))
      {
        context.Response.Clear();
        string script = returnUrl.Substring ("javascript:".Length);
        context.Response.Write ("<html><script language=\"JavaScript\">" + script + "</script></html>");
      }
      else
      {
        context.Response.Redirect (returnUrl);
      }
    }
  }

  public bool IsReusable
  {
    get { return false; }
  }
}

}