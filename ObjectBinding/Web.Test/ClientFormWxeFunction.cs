using System;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.Web.ExecutionEngine;
using OBRTest;

namespace OBWTest
{

public class ClientFormWxeFunction: WxeFunction
{
  public ClientFormWxeFunction ()
  {
    Object = Person.GetObject (new Guid (0,0,0,0,0,0,0,0,0,0,1));
    ReturnUrl = "javascript:window.close();";
  }

  // parameters
  public ReflectionBusinessObject Object 
  {
    get { return (ReflectionBusinessObject) Variables["Object"]; }
    set { Variables["Object"] = value; }
  }

  [WxeParameter (1, true)]
  public bool ReadOnly
  {
    get { return (bool) Variables["ReadOnly"]; }
    set { Variables["ReadOnly"] = value; }
  }

  // steps

  class Step1: WxeStepList
  {
    ClientFormWxeFunction Function { get { return (ClientFormWxeFunction) ParentFunction; } }

    WxeStep Step1_ = new WxePageStep ("ClientForm.aspx");
  }

  class Step2: WxeStepList
  {
    ClientFormWxeFunction Function { get { return (ClientFormWxeFunction) ParentFunction; } }

    WxeStep Step1_ = new WxePageStep ("ClientForm.aspx");
  }
}

public class ClientFormClosingWxeFunction: WxeFunction
{
  void Step1()
  {
  }
}

public class ClientFormKeepAliveWxeFunction: WxeFunction
{
  void Step1()
  {
  }
}

}
