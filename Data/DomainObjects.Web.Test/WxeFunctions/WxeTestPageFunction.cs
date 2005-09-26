using System;

using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.Web.Test.WxeFunctions
{
public class WxeTestPageFunction : WxeFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public WxeTestPageFunction ()
  {
  }

  // methods and properties

  public ClientTransaction CurrentClientTransaction
  {
    get { return (ClientTransaction) Variables["CurrentClientTransaction"]; }
    set { Variables["CurrentClientTransaction"] = value;}
  }

  private WxePageStep Step1 = new WxePageStep ("WxeTestPage.aspx");
}
}
