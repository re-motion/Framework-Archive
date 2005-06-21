using System;
using Rubicon.Web.ExecutionEngine;

namespace OBWTest
{

public class ViewPersonDetailsWxeFunction: WxeFunction
{
  static readonly WxeParameterDeclaration[] s_parameters =  { 
      new WxeParameterDeclaration ("id", false, WxeParameterDirection.In, typeof (string))};

  // parameters and local variables
  public override WxeParameterDeclaration[] ParameterDeclarations
  {
    get { return s_parameters; }
  }

  [WxeParameter (1, false, WxeParameterDirection.In)]
  public string ID
  {
    get { return (string) Variables["id"]; }
    set { Variables["id"] = value; }
  }

  // steps

  private WxeStep Step1 = new WxePageStep ("PersonDetailsForm.aspx");
}
}
