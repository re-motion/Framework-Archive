using System;
using Rubicon.Web.ExecutionEngine;

namespace OBWTest
{

public class CompleteBocTestMainWxeFunction: WxeFunction
{
  public CompleteBocTestMainWxeFunction ()
  {
    ReturnUrl = "StartForm.aspx";
  }

  // steps

  private WxeStep Step1 = new WxePageStep ("CompleteBocTestForm.aspx");
  private WxeStep Step2 = new WxePageStep ("CompleteBocTestUserControlForm.aspx");
}

}
