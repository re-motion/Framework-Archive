﻿using System;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest
{
  public class WindowFunction : WxeFunction
  {
    public WindowFunction ()
        : base (new NoneTransactionMode())
    {
    }
    
    // Steps
    private void Step1 ()
    {
      ExceptionHandler.AppendCatchExceptionTypes (typeof (WxeUserCancelException));
    }

    private WxeStep Step2 = new WxePageStep ("MultiWindowTest/Window.aspx");
  }
}