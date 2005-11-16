using System;
using System.Web;
using System.Web.SessionState;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.PageTransition
{

public class MultiplePostbackCatcherWxeFunction: WxeFunction
{
  public MultiplePostbackCatcherWxeFunction ()
  {
  }

  public MultiplePostbackCatcherWxeFunction (params object[] args)
    : base (args)
  {
  }

  // steps

  WxeStep Step1 = new WxePageStep ("MultiplePostbackCatcherForm.aspx");
}

}
