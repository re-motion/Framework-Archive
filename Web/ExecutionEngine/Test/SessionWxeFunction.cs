using System;
using System.Web;
using System.Web.SessionState;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.PageTransition
{

public class SessionWxeFunction: WxeFunction
{
  public SessionWxeFunction ()
  {
    ReturnUrl = "javascript:window.close();";
  }

  [WxeParameter (0, true)]
  public bool ReadOnly
  {
    get { return (bool) Variables["ReadOnly"]; }
    set { Variables["ReadOnly"] = value; }
  }

  // steps

  void Step1()
  {
    HttpContext.Current.Session["key"] = 123456789;
  }

  class Step2: WxeStepList
  {
    SessionWxeFunction Function { get { return (SessionWxeFunction) ParentFunction; } }
    WxeStep Step1_ = new WxePageStep ("SessionForm.aspx");
  }

  class Step3: WxeStepList
  {
    SessionWxeFunction Function { get { return (SessionWxeFunction) ParentFunction; } }
    WxeStep Step1_ = new WxePageStep ("SessionForm.aspx");
  }
}

public class ClientFormClosingWxeFunction: WxeFunction
{
  void Step1()
  {
    object val = HttpContext.Current.Session["key"];
    if (val != null)
    {
      int i = (int) val;
    }
  }
}

public class ClientFormKeepAliveWxeFunction: WxeFunction
{
  void Step1()
  {
    object val = HttpContext.Current.Session["key"];
    if (val != null)
    {
      int i = (int) val;
    }
  }
}

}
