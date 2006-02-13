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
  [WxePageFunction ("AutoPage.aspx", typeof (WxeFunction))]
  [WxePageParameter (1, "InArg", typeof (string), true)]
  [WxePageParameter (2, "InOutArg", typeof (string), true, WxeParameterDirection.InOut)]
  [WxePageParameter (3, "OutArg", typeof (string), WxeParameterDirection.Out)]
  [WxePageVariable ("LocalVariable", typeof (string))]
  public partial class AutoPage : WxePage
  {
    // for ASP.NET 1.1
    // new AutoPageVariables Variables { get { return ((AutoPageFunction) CurrentFunction).PageVariables; } }

    protected void Page_Load (object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        // use input parameters for control initialization

        // for ASP.NET 2.0
        InArgField.Text = InArg;
        InOutArgField.Text = InOutArg;

        // for ASP.NET 1.1
        //InArgField.Text = Variables.InArg;
        //InOutArgField.Text = Variables.InOutArg;
      }
    }

    protected void ExecSelfButton_Click (object sender, EventArgs e)
    {
      string inOutParam = InOutArgField.Text + "'";
      OutArgField.Text = AutoPageFunction.Call (this, InArgField.Text + "'", ref inOutParam);
      InOutArgField.Text = inOutParam;

      if (!IsReturningPostBack)
      {
        // call function recursively
        ExecuteFunction (new AutoPageFunction (
            InArgField.Text + "'",
            InOutArgField.Text + "'"));
      }
      else
      {
        // when call returns, use output parameters 
        AutoPageFunction function = (AutoPageFunction) ReturningFunction;
        OutArgField.Text = function.OutArg;
        InOutArgField.Text = function.InOutArg;
      }
    }

    protected void ReturnButton_Click (object sender, EventArgs e)
    {
      // set output parameters and return

      // for ASP.NET 2.0
      InOutArg = InOutArgField.Text + "'";
      OutArg = OutArgField.Text + "'";
      Return ();

      // obsolete
      // Return (InOutArgField.Text + "'", OutArgField.Text + "'");

      // for ASP.NET 1.1
      //Variables.InOutArg = InOutArgField.Text + "'";
      //Variables.OutArg = OutArgField.Text + "'";
      //ExecuteNextStep ();
    }
  }

  //internal struct AutoPageVariables
  //{
  //  private /*readonly*/ Rubicon.Collections.NameObjectCollection Variables;

  //  public AutoPageVariables (Rubicon.Collections.NameObjectCollection variables)
  //  {
  //    Variables = variables;
  //  }

  //  public string InArg
  //  {
  //    get { return (string) Variables["InArg"]; }
  //  }

  //  public string InOutArg
  //  {
  //    get { return (string) Variables["InOutArg"]; }
  //    set { Variables["InOutArg"] = value; }
  //  }

  //  public string OutArg
  //  {
  //    set { Variables["OutArg"] = value; }
  //  }
  //}

  //public class AutoPageFunction: WxeFunction
  //{
  //  public AutoPageVariables PageVariables
  //  {
  //    get { return new AutoPageVariables (Variables); } 
  //  }
  //}


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
