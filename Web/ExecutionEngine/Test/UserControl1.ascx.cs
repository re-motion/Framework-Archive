using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.PageTransition
{

public class UserControl1 : WxeUserControl
{
  protected System.Web.UI.WebControls.TextBox TextBox1;
  protected System.Web.UI.WebControls.Button Stay;
  protected System.Web.UI.WebControls.Button Sub;
  protected System.Web.UI.WebControls.Button Next;

	private void Page_Load (object sender, System.EventArgs e)
	{
		// Put user code to initialize the page here
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
	///		Required method for Designer support - do not modify
	///		the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
    this.Stay.Click += new System.EventHandler(this.Stay_Click);
    this.Sub.Click += new System.EventHandler(this.Sub_Click);
    this.Next.Click += new System.EventHandler(this.Next_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void Stay_Click (object sender, System.EventArgs e)
  {
  
  }

  private void Sub_Click (object sender, System.EventArgs e)
  {
    CurrentStep.ExecuteFunction (sender, Page, new WebForm1.SubFunction ("usercontrol var1", "usercontrol var2"));  
  }

  private void Next_Click (object sender, System.EventArgs e)
  {
    CurrentStep.ExecuteNextStep ();
  }

}

}
