using System;

using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Templates.Generic.Web.WxeFunctions
{
public class TemplateFunction : BaseFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public TemplateFunction ()
  {
  }

  public TemplateFunction (string navSelectedTab, string navSelectedMenu) 
      : base (navSelectedTab, navSelectedMenu)
  {
  }

  // methods and properties

  private WxePageStep Step1 = new WxePageStep ("UI/TemplateForm.aspx");
}

}
