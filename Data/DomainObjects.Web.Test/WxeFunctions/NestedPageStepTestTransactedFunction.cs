using System;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.Web.Test.WxeFunctions
{
  public class NestedPageStepTestTransactedFunction : WxeTransactedFunction
  {
    private WxePageStep Step1 = new WxePageStep ("ImmediatelyReturningPage.aspx");
  }
}
