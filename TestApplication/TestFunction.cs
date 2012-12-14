using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using Rubicon.Web.ExecutionEngine;

namespace TestApplication
{
    public class TestFunction : WxeFunction
    {
        public TestFunction ()
        {
            
        }

        private void Step1 (WxeContext context)
        {
            Debug.WriteLine("hello");
        }

        private WxePageStep Step2 = new WxePageStep("~/Step2.aspx");
        private WxePageStep Step3 = new WxePageStep("~/Step3.aspx");

      
    }
}