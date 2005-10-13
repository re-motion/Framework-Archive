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
using System.Text;
using Rubicon.Web.Utilities;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Collections;
using Rubicon.Web.UI.Controls;

namespace Rubicon.PageTransition
{

public class SessionForm : WxePage
{
  protected Rubicon.Web.UI.Controls.WebButton PostBackButton;
  protected System.Web.UI.WebControls.LinkButton LinkButton1;
  protected Rubicon.Web.UI.Controls.WebButton OpenSelfButton;
  protected System.Web.UI.WebControls.Button Button1;
  protected Rubicon.Web.UI.Controls.WebButton Button1Button;
  protected Rubicon.Web.UI.Controls.WebButton Submit1Button;
  protected Rubicon.Web.UI.Controls.WebButton Button2Button;
  protected Rubicon.Web.UI.Controls.WebButton ExecuteButton;
  protected Rubicon.Web.UI.Controls.WebButton ExecuteNoRepostButton;
  protected System.Web.UI.WebControls.Label FunctionTokenLabel;
  protected System.Web.UI.WebControls.Label PostBackIDLabel;
  protected Rubicon.Web.UI.Controls.WebButton OpenSampleFunctionWithMappedPathInNewWindowButton;
  protected Rubicon.Web.UI.Controls.WebButton OpenSampleFunctionInNewWindowButton;
  protected Rubicon.Web.UI.Controls.WebButton OpenSampleFunctionWithMappedPathButton;
  protected Rubicon.Web.UI.Controls.WebButton OpenSampleFunctionButton;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;


	private void Page_Load(object sender, System.EventArgs e)
	{
    RegisterClientSidePageEventHandler (WxePageEvents.OnPostBack, "Page_PostBack", "Page_PostBack");
    RegisterClientSidePageEventHandler (WxePageEvents.OnPostBack, "Page_Abort", "Page_Abort");
    FunctionTokenLabel.Text = "Token = " + WxeContext.Current.FunctionToken;
    PostBackIDLabel.Text = "PostBackID = " + WxeContext.Current.PostBackID.ToString();
  }

  override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
    base.OnInit(e);
	}
	#region Web Form Designer generated code

	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.OpenSampleFunctionWithMappedPathInNewWindowButton.Click += new System.EventHandler(this.OpenSampleFunctionWithMappedPathInNewWindowButton_Click);
    this.OpenSelfButton.Click += new System.EventHandler(this.OpenSelfButton_Click);
    this.Button1.Click += new System.EventHandler(this.Button1_Click);
    this.Button1Button.Click += new System.EventHandler(this.Button1Button_Click);
    this.Submit1Button.Click += new System.EventHandler(this.Submit1Button_Click);
    this.ExecuteButton.Click += new System.EventHandler(this.ExecuteButton_Click);
    this.ExecuteNoRepostButton.Click += new System.EventHandler(this.ExecuteNoRepostButton_Click);
    this.Button2Button.Click += new System.EventHandler(this.Button2Button_Click);
    this.OpenSampleFunctionWithMappedPathButton.Click += new System.EventHandler(this.OpenSampleFunctionWithMappedPathButton_Click);
    this.OpenSampleFunctionInNewWindowButton.Click += new System.EventHandler(this.OpenSampleFunctionInNewWindowButton_Click);
    this.OpenSampleFunctionButton.Click += new System.EventHandler(this.OpenSampleFunctionButton_Click);
    this.EnableAbortConfirmation = Rubicon.NullableValueTypes.NaBooleanEnum.True;
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void OpenSelfButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
      ExecuteFunction (new SessionWxeFunction (true), "_blank", OpenSelfButton, true);
  }

  private void Button1_Click(object sender, System.EventArgs e)
  {
  
  }

  private void Button1Button_Click(object sender, System.EventArgs e)
  {
  
  }

  private void Submit1Button_Click(object sender, System.EventArgs e)
  {
  
  }

  private void Button2Button_Click(object sender, System.EventArgs e)
  {
  
  }

  private void ExecuteButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
      ExecuteFunction (new SampleWxeFunction ());
  }

  private void ExecuteNoRepostButton_Click(object sender, System.EventArgs e)
  {
    ExecuteFunctionNoRepost (new SampleWxeFunction (), (Control) sender);
  }

  private void OpenSampleFunctionButton_Click(object sender, System.EventArgs e)
  {
    if (! IsReturningPostBack)
      ExecuteFunction (new SampleWxeFunction());
  }

  private void OpenSampleFunctionWithMappedPathButton_Click(object sender, System.EventArgs e)
  {
    if (! IsReturningPostBack)
      ExecuteFunctionWithMappedPath (new SampleWxeFunction());
  }

  private void OpenSampleFunctionInNewWindowButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
      ExecuteFunction (new SampleWxeFunction (), "_blank", OpenSampleFunctionButton, true);
  }

  private void OpenSampleFunctionWithMappedPathInNewWindowButton_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
      ExecuteFunctionWithMappedPath (new SampleWxeFunction (), "_blank", OpenSampleFunctionButton, true);
  }

}

}

