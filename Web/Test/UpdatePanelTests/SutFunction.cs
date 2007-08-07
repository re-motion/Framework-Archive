using System;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.Test.UpdatePanelTests
{
  public class SutFunction : WxeFunction
  {
    private WxeStep Step1 = new WxePageStep ("~/UpdatePanelTests/SutForm.aspx");
  }
}