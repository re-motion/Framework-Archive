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
	/// <summary>
	/// Summary description for TestUserControlBinding.
	/// </summary>
	public class TestUserControlBinding : System.Web.UI.Page
	{
    protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
    protected Rubicon.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl DataSource;
    protected System.Web.UI.HtmlControls.HtmlTable NameFormGrid;
    protected System.Web.UI.WebControls.Label Label1;
    protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue1;
    protected Rubicon.ObjectBinding.Web.UI.Controls.UserControlBinding UserControlBinding1;
    protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  
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
