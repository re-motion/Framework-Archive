using System;

using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Templates.Generic.Web.WxeFunctions
{
public class BaseFunction : WxeFunction
{
  public BaseFunction ()
  {
    Initialize ();
  }

  public BaseFunction (params object[] args) : base (args)
  {
    Initialize ();
  }

  private void Initialize ()
  {
    ReturnUrl = "WxeHandler.ashx?WxeFunctionType=Rubicon.Templates.Generic.Web.WxeFunctions.TemplateFunction,Rubicon.Templates.Generic.Web";
  }

  // methods and properties

  [WxeParameter (1, false, WxeParameterDirection.In)]
  public virtual string NavSelectedTab
  {
    get { return (string) Variables["navSelectedTab"]; }
    set { Variables["navSelectedTab"] = value; }
  }

  [WxeParameter (2, false, WxeParameterDirection.In)]
  public virtual string NavSelectedMenu
  {
    get { return (string) Variables["navSelectedMenu"]; }
    set { Variables["navSelectedMenu"] = value; }
  }
}
}
