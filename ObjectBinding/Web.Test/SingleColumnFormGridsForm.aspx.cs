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

namespace OBWTest
{
public class SingleColumnFormGridsForm : System.Web.UI.Page
{
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected Rubicon.Web.UI.Controls.SmartLabel SmartLabel1;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue1;
  protected System.Web.UI.HtmlControls.HtmlTable LeftFormGrid;
  protected Rubicon.Web.UI.Controls.SmartLabel Smartlabel2;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue2;
  protected Rubicon.Web.UI.Controls.FormGridManager FormgridManager;
  protected System.Web.UI.HtmlControls.HtmlTable RightFormGrid;
  protected Rubicon.Web.UI.Controls.SmartLabel Smartlabel3;
  protected Rubicon.ObjectBinding.Web.Controls.BocList BocList1;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;

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
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion
}
}
