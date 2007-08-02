using System;
using System.Web;
using System.Web.SessionState;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.Test.ExecutionEngine
{
  public class RedirectedSubWxeFunction: WxeFunction
  {
    public RedirectedSubWxeFunction ()
    {
    }

    void Step1 (WxeContext context)
    {
      PageUtility.Redirect (context.HttpContext.Response, "~/Start.aspx?Redirected");
    }
  }

  public class RedirectedWxeFunction: WxeFunction
  {
    public RedirectedWxeFunction ()
    {
    }

    public RedirectedWxeFunction (params object[] args)
        : base (args)
    {
    }

    // steps

    WxeStep Step1 = new RedirectedSubWxeFunction();

    WxeStep Step2 = new WxePageStep ("~/ExecutionEngine/SessionForm.aspx");
  }
}