using System;
using Rubicon.Web.ExecutionEngine;

namespace OBWTest
{

public class ViewPersonDetailsWxeFunction: WxeFunction
{
  public ViewPersonDetailsWxeFunction (object id)
    : base (id)
  {
  }

  // parameters and local variables

  [WxeParameter (1, true, WxeParameterDirection.In)]
  public string ID
  {
    get { return (string) Variables["ID"]; }
    set { Variables["ID"] = value; }
  }

  // steps

  private WxeStep Step1 = new WxePageStep ("PersonDetailsPage.aspx");
}
}
