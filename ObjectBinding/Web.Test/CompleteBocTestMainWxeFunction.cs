using System;
using Rubicon.Web.ExecutionEngine;

namespace OBWTest
{

public class IntegrationTestMainWxeFunction: WxeFunction
{
  public IntegrationTestMainWxeFunction ()
  {
    ReturnUrl = "StartForm.aspx";
  }

  // steps

  private WxeStep Step1 = new WxePageStep ("IntegrationTestForm.aspx");
  private WxeStep Step2 = new WxePageStep ("IntegrationTestUserControlForm.aspx");
}

}
