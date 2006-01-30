using System;
using System.Web;
using System.Web.SessionState;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.PageTransition
{

public class ProfilingWxeFunction: WxeFunction
{
  private DateTime _start;
  private DateTime _end;

  public ProfilingWxeFunction ()
  {
    ReturnUrl = "Start.aspx";
  }

  public ProfilingWxeFunction (params object[] args)
    : base (args)
  {
    ReturnUrl = "Start.aspx";
  }

  // steps

  void Step10()
  {
    _start = DateTime.Now;
  }

  WxeStep Step21 = new WxePageStep ("ProfilingForm.aspx");
  WxeStep Step22 = new WxePageStep ("ProfilingForm.aspx");
  WxeStep Step23 = new WxePageStep ("ProfilingForm.aspx");
  WxeStep Step24 = new WxePageStep ("ProfilingForm.aspx");
  WxeStep Step25 = new WxePageStep ("ProfilingForm.aspx");
  WxeStep Step26 = new WxePageStep ("ProfilingForm.aspx");
  WxeStep Step27 = new WxePageStep ("ProfilingForm.aspx");
  WxeStep Step28 = new WxePageStep ("ProfilingForm.aspx");
  WxeStep Step29 = new WxePageStep ("ProfilingForm.aspx");

  void Step30()
  {
    _end = DateTime.Now;
    TimeSpan diff = _end - _start;
    System.Diagnostics.Debug.WriteLine (string.Format ("Runtime: {0} ms", diff.Ticks / 10000));
  }
}

}
