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
    WxePageSession pageSession = null;
    string pageToken = context.Request.Params["WxePageToken"];
    if (pageToken != null)
    {
      ArrayList pages = (ArrayList) context.Session ["WxePages"];
      if (pages == null)
        throw new ApplicationException ("Session timeout."); // TODO: display error message

      // find page session for current token and dispose expired page sessions
      foreach (WxePageSession page in pages)
      {
        if (pageToken == page.PageToken)
        {
          pageSession = page;
        }
        else if (page.IsExpired)
        {
          pages.Remove (page);
          page.Dispose();
        }
      }

      if (pageSession == null)
        throw new ApplicationException ("Page timeout."); // TODO: display error message

      pageSession.Touch();
      _currentFunction = pageSession.Function;
      if (_currentFunction == null)
        throw new ApplicationException ("Function missing in WxePageSession {0}." + pageSession.PageToken);
      string action = context.Request["WxeAction"];
      if (action == "cancel")
      {
        pages.Remove (pageSession);
        pageSession.Dispose();
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

      ArrayList pages = (ArrayList) context.Session ["WxePages"];
      if (pages == null)
      {
        pages = new ArrayList(1);
        context.Session["WxePages"] = pages;
      }
      pageSession = new WxePageSession (_currentFunction, 20); // TODO: make lifetime configurable
      pages.Add (pageSession);

      WxeParameterDeclaration.CopyToCallee (_currentFunction.ParameterDeclarations, context.Request.Params, _currentFunction.Variables, CultureInfo.InvariantCulture);
    }

    WxeContext wxeContext = new WxeContext (context); 
    wxeContext.PageToken = pageSession.PageToken;
    WxeContext.SetCurrent (wxeContext);

    _currentFunction.Execute (wxeContext);
  }

  public bool IsReusable
  {
    get { return false; }
  }
}

}