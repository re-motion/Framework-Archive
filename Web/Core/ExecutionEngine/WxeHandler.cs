using System;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;
using System.Reflection;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Rubicon.NullableValueTypes;

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
    _currentFunction = (WxeFunction) context.Session ["CurrentFunction"];
    if (_currentFunction == null)
    {
      string typeName = context.Request.Params["WxeFunctionType"];
      if (typeName == null)
        throw new HttpException ("Missing URL parameter 'WxeFunctionType'");

      Type type = Type.GetType (typeName, true);
      _currentFunction = (WxeFunction) Activator.CreateInstance (type);

      WxeParameterDeclaration.CopyToCallee (_currentFunction.ParameterDeclarations, context.Request.Params, _currentFunction.Variables, CultureInfo.InvariantCulture);

      context.Session["CurrentFunction"] = _currentFunction;
    }

    WxeContext wxeContext = WxeContext.Current;
    if (wxeContext == null)
    {
      wxeContext = new WxeContext (context); 
      WxeContext.SetCurrent (wxeContext);
    }

    _currentFunction.Execute (wxeContext);
  }

  public bool IsReusable
  {
    get { return false; }
  }
}

}