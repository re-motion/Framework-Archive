using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest
{
	/// <summary>
	/// Summary description for TestUserControlBinding.
	/// </summary>
	public class TestUserControlBinding : Page
	{
    protected FormGridManager FormGridManager;
    protected BindableObjectDataSourceControl DataSource;
    protected HtmlTable NameFormGrid;
    protected Label Label1;
    protected BocTextValue BocTextValue1;
    protected UserControlBinding UserControlBinding1;
    protected HtmlHeadContents HtmlHeadContents;
  
		private void Page_Load(object sender, EventArgs e)
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
