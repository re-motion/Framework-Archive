using Remotion.ObjectBinding.Web.UI.Controls;

namespace OBWTest
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
  using Remotion.ObjectBinding;
  using Remotion.ObjectBinding.Web;

	/// <summary>
	///		Summary description for WebUserControl1.
	/// </summary>
	[Obsolete ("DataSourceUserControl is obsolete, rendering this implementation obsoelte as well.")]
	public class WebUserControl1 : DataSourceUserControl
	{
    protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator1;
    protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue1;
    protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
    protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl reflectionBusinessObjectDataSource1;

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here


		}
    protected override Remotion.ObjectBinding.IBusinessObjectDataSource DataSource
    {
      get { return base.DataSource;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
      this.CurrentObject = new BindableObjectDataSourceControl();
      // 
      // CurrentObject
      // 
      this.CurrentObject.BusinessObject = null;
      this.CurrentObject.Mode = DataSourceMode.Edit;
      this.CurrentObject.Type = null;
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion
	}
}
