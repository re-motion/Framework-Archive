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
    ReturnUrl = "javascript:window.close();";
  }

  // parameters

  // steps

  WxeStep Step1 = new WxePageStep ("ClientForm.aspx");
  WxeStep Step2 = new WxePageStep ("ClientForm.aspx");
}

}
