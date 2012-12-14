using System;
using Rubicon.Web.ExecutionEngine;

namespace OBWTest
{

public class SingleBocTestMainWxeFunction: WxeFunction
{
  public SingleBocTestMainWxeFunction ()
  {
    ReturnUrl = "StartForm.aspx";
  }

  // steps

  private WxeStep Step1 = new WxePageStep ("SingleBocTestListForm.aspx");
  private WxeStep Step2 = new WxePageStep ("SingleBocTestReferenceValueForm.aspx");
  private WxeStep Step3 = new WxePageStep ("SingleBocTestBooleanValueForm.aspx");
  private WxeStep Step4 = new WxePageStep ("SingleBocTestMultilineTextValueForm.aspx");
  private WxeStep Step5 = new WxePageStep ("SingleBocTestDateTimeValueForm.aspx");
  private WxeStep Step6 = new WxePageStep ("SingleBocTestEnumValueForm.aspx");
  private WxeStep Step7 = new WxePageStep ("SingleBocTestTextValueForm.aspx");
}

}
