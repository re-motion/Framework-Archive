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

  // IHttpHandler Members


  public void ProcessRequest (HttpContext context)
  {
    WxeWindowState windowState = null;
    string windowToken = context.Request.Params["WxeWindowToken"];
    WxeWindowStateCollection windowStates = WxeWindowStateCollection.Instance;
    if (windowToken != null)
    {
      if (windowStates == null)
        throw new ApplicationException ("Session timeout."); // TODO: display error message

      windowState = windowStates.GetItem (windowToken);
      if (windowState == null)
        throw new ApplicationException ("Page timeout."); // TODO: display error message
      windowState.Touch();

      windowStates.DisposeExpired();

      _currentFunction = windowState.Function;
      if (_currentFunction == null)
        throw new ApplicationException ("Function missing in WxeWindowState {0}." + windowState.WindowToken);
      string action = context.Request["WxeAction"];
      if (action == "cancel")
      {
        windowStates.Remove (windowState);
        return;
      }
    }
    else
    {
      string typeName = context.Request.Params["WxeFunctionType"];
      if (typeName == null)
        throw new HttpException ("Missing URL parameter 'WxeFunctionType'");

      Type type = TypeUtility.GetType (typeName, true, false);
      _currentFunction = (WxeFunction) Activator.CreateInstance (type);

      if (windowStates == null)
      {
        windowStates = new WxeWindowStateCollection();
        WxeWindowStateCollection.Instance = windowStates;
      }
      windowState = new WxeWindowState (_currentFunction, 20); // TODO: make lifetime configurable
      windowStates.Add (windowState);

      _currentFunction.InitializeParameters (context.Request.Params);
    }

    WxeContext wxeContext = new WxeContext (context); 
    wxeContext.WindowToken = windowState.WindowToken;
    WxeContext.SetCurrent (wxeContext);

    _currentFunction.Execute (wxeContext);
  }

  public bool IsReusable
  {
    get { return false; }
  }
}

}