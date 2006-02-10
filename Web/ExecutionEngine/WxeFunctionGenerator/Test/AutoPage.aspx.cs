using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.Web.ExecutionEngine;

namespace Test
{
  [WxeFunctionPage ("AutoPage.aspx", typeof (WxeFunction))]
  [WxePageParameter (1, "InArg", typeof (string), true, WxeParameterDirection.In)]
  [WxePageParameter (2, "InOutArg", typeof (string), true, WxeParameterDirection.InOut)]
  [WxePageParameter (3, "OutArg", typeof (string), WxeParameterDirection.Out)]
  public partial class AutoPage : WxePage
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      InArgField.Text = InArg;
      InOutArgField.Text = InOutArg;
    }

    protected void ExecSelf_Click (object sender, EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        AutoPageFunction function = new AutoPageFunction (InArg + "'", InOutArg + "'");
        ExecuteFunction (function);
      }
      else
      {
        AutoPageFunction function = (AutoPageFunction) ReturningFunction;
        OutArgField.Text = function.OutArg + "'";
      }
    }
  }

  //public partial class AutoPage
  //{
  //  public string InArg
  //  {
  //    get { return (string)Variables["InArg"]; }
  //  }

  //  public string InOutArg
  //  {
  //    get { return (string)Variables["InOutArg"]; }
  //    set { Variables["InOutArg"] = value; }
  //  }

  //  public string OutArg
  //  {
  //    set { Variables["OutArg"] = value; }
  //  }
  //}
  
  //public class AutoPageFunction : WxeFunction
  //{
  //  public AutoPageFunction()
  //  {
  //  }

  //  public AutoPageFunction(params object[] args)
  //    : base(args)
  //  {
  //  }

  //  public AutoPageFunction(string InArg, string InOutArg)
  //    : base(InArg, InOutArg)
  //  {
  //  }

  //  [WxeParameter(1, true, WxeParameterDirection.In)]
  //  public string InArg
  //  {
  //    set { Variables["InArg"] = value; }
  //  }

  //  [WxeParameter(2, true, WxeParameterDirection.InOut)]
  //  public string InOutArg
  //  {
  //    get { return (string)Variables["InOutArg"]; }
  //    set { Variables["InOutArg"] = value; }
  //  }

  //  [WxeParameter(3, WxeParameterDirection.Out)]
  //  public string OutArg
  //  {
  //    get { return (string)Variables["OutArg"]; }
  //  }

  //  WxeStep Step1 = new WxePageStep ("AutoPage.aspx");
  //}
}
