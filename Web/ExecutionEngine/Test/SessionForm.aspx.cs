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
  protected Rubicon.Web.UI.Controls.WebButton WebButton1;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;


	private void Page_Load(object sender, System.EventArgs e)
	{
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
    this.WebButton1.Click += new System.EventHandler(this.WebButton1_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void WebButton1_Click(object sender, System.EventArgs e)
  {
    if (!IsReturningPostBack)
      ExecuteFunction (new SampleWxeFunction (), "_blank", WebButton1, true);
  }

}

}

