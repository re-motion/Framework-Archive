using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Collections.Specialized;
using Rubicon.PageTransition;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.Utilities;

namespace Rubicon.PageTransition
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public class WebForm1: WxePage
	{
    protected System.Web.UI.WebControls.TextBox TextBox1;
    protected System.Web.UI.WebControls.Button Stay;
    protected System.Web.UI.WebControls.Button Next;
    protected System.Web.UI.WebControls.CheckBox IsPostBackCheck;
    protected System.Web.UI.WebControls.Button Sub;
    protected System.Web.UI.WebControls.Label StackLabel;
    protected System.Web.UI.WebControls.Label Label2;
    protected System.Web.UI.WebControls.Label Label3;
    protected System.Web.UI.WebControls.Label Var1Label;
    protected System.Web.UI.WebControls.Label Label5;
    protected System.Web.UI.WebControls.Label Var2Label;
    protected System.Web.UI.WebControls.Button Throw;
    protected System.Web.UI.WebControls.Label Label1;
    protected System.Web.UI.WebControls.Label RetValLabel;
    protected System.Web.UI.WebControls.Button SubExtButton;

    public readonly WxeParameterDeclaration[] PageParameters = {
          new WxeParameterDeclaration ("text", true, WxeParameterDirection.InOut, typeof (string)),
          new WxeParameterDeclaration ("invocations", false, WxeParameterDirection.Out, typeof (int))
        };

    private ISampleFunctionVariables Function
    {
      get { return (ISampleFunctionVariables) CurrentFunction; }
    }

		private void Page_Load (object sender, System.EventArgs e)
		{
      NameValueCollection coll = this.GetPostBackCollection();
      if (coll != null)
      {
        object o = new LosFormatter().Deserialize (coll["__VIEWSTATE"]);
        o = null;
      }
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      for (WxeStep step = CurrentStep; step != null; step = step.ParentStep)
        sb.AppendFormat ("{0}<br>", step.ToString());      
      StackLabel.Text = sb.ToString();

			Var1Label.Text = Function.Var1;
      Var2Label.Text = Function.Var2;
      IsPostBackCheck.Checked = IsPostBack;

      GetPostBackEventReference (this); // force __doPostBack script
      PageUtility.RegisterClientScriptBlock (this, "doSubmit",
          "function doSubmit (button, pageToken) {"
          + "  var theForm = document." + Form.ClientID + ";"
          // + "  document.getElementById(button).value = button; "
          + "  theForm.returningToken.value = pageToken;"
          + "  document.getElementById(button).click();"
          + "  }");
      HtmlInputHidden returningTokenField = new HtmlInputHidden ();
      returningTokenField.ID = "returningToken";
      Form.Controls.Add (returningTokenField);

      NameValueCollection postBackCollection = GetPostBackCollection();
      if (postBackCollection != null)
      {
        string returningToken = postBackCollection["returningToken"];
        if (returningToken != null)
        {
          ArrayList pages = (ArrayList) Session["WxePages"];
          foreach (WxePageSession pageSession in pages)
          {
            if (pageSession.PageToken == returningToken)
            {
              WxeContext.Current.ReturningFunction = pageSession.Function;
              WxeContext.Current.IsReturningPostBack = true;
            }
          }
        }
      }
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
      this.Stay.Click += new System.EventHandler(this.Stay_Click);
      this.Next.Click += new System.EventHandler(this.Next_Click);
      this.Throw.Click += new System.EventHandler(this.Throw_Click);
      this.Sub.Click += new System.EventHandler(this.Sub_Click);
      this.SubExtButton.Click += new System.EventHandler(this.SubExtButton_Click);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion

    private void Stay_Click (object sender, System.EventArgs e)
    {
    
    }

    private void Next_Click (object sender, System.EventArgs e)
    {
      ExecuteNextStep ();
//      WxeFunction currentFunction = ((WxeFunction) Session["CurrentFunction"]);
//      currentFunction.ExecutingStep.ExecuteNextStep (Context);
    }

    private void Sub_Click (object sender, System.EventArgs e)
    {
      // CurrentStep.ExecuteFunction ((Control)sender, this, new SubFunction("call var1", "call var2"));
      if (! IsReturningPostBack)
      {
        SubFunction subFunction = new SubFunction ("call var 1!", "vall var2");
        ExecuteFunction (subFunction);
      }
      else
      {
        SubFunction subFunction = (SubFunction) ReturningFunction;
        RetValLabel.Text = subFunction.Var1;
      }
    }

    private void Throw_Click (object sender, System.EventArgs e)
    {
      throw new ApplicationException ("test exception");
    }

    private void SubExtButton_Click(object sender, System.EventArgs e)
    {
      if (! IsReturningPostBack)
      {
        SubFunction subFunction = new SubFunction ("call var 1!", "vall var2");
        ExecuteFunction (subFunction, "_blank", (Control) sender);
      }
      else
      {
        SubFunction subFunction = (SubFunction) ReturningFunction;
        RetValLabel.Text = subFunction.Var1;
      }
    }

    public class SubFunction: WxeFunction, ISampleFunctionVariables
    {
      public SubFunction (string var1, string var2)
        : base (var1, var2)
      {
      }

      public SubFunction (params object[] args)
        : base (args)
      {
      }

      [WxeParameter (1, true, WxeParameterDirection.InOut)]
      public string Var1
      {
        get { return (string) Variables["Var1"]; }
        set { Variables["Var1"] = value; }
      }

      [WxeParameter (2, WxeParameterDirection.InOut)]
      public string Var2
      {
        get { return (string) Variables["Var2"]; }
        set { Variables["Var2"] = value; }
      }

      private void Step1 (WxeContext context)
      {
        Var1 = "SubFunction Step1";
      }

      private WxeStep Step2 = new WxePageStep ("WebForm1.aspx");

      private void Step3 (WxeContext context)
      {
        Var1 = "SubFunction Step3";
      }
    }
	}
}
