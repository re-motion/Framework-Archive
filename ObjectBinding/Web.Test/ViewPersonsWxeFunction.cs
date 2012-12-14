using System;
using Rubicon.Web.ExecutionEngine;
using Rubicon.ObjectBinding;

namespace OBWTest
{

public class ViewPersonsWxeFunction: WxeFunction
{
  static readonly WxeParameterDeclaration[] s_parameters =  { 
      new WxeParameterDeclaration ("objects", true, WxeParameterDirection.In, typeof (IBusinessObject[]))};

  // parameters and local variables
  public override WxeParameterDeclaration[] ParameterDeclarations
  {
    get { return s_parameters; }
  }

  [WxeParameter (1, true, WxeParameterDirection.In)]
  public IBusinessObject[] Objects
  {
    get { return (IBusinessObject[]) Variables["objects"]; }
    set { Variables["objects"] = value; }
  }

  // steps

  private WxeStep Step1 = new WxePageStep ("PersonsForm.aspx");
}
}
