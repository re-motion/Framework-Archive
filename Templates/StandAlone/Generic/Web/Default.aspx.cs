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

using Rubicon.Templates.Generic.Web.Classes;

namespace Rubicon.Templates.Generic.Web
{

public class Default : BasePage
{
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected System.Web.UI.WebControls.LinkButton StartButton;

	private void Page_Load(object sender, System.EventArgs e)
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
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void StartButton_Click(object sender, System.EventArgs e)
  {
    Session.Abandon();
  }
}

}
