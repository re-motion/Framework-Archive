using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace OBWTest
{
	using System;

  /// <summary>
	///		Summary description for WebUserControl1.
	/// </summary>
	[Obsolete ("DataSourceUserControl is obsolete, rendering this implementation obsoelte as well.")]
	public class WebUserControl1 : DataSourceUserControl
	{
    protected BocTextValueValidator BocTextValueValidator1;
    protected BocTextValue BocTextValue1;
    protected BindableObjectDataSourceControl CurrentObject;
    protected BindableObjectDataSourceControl reflectionBusinessObjectDataSource1;

		private void Page_Load(object sender, EventArgs e)
		{
			// Put user code to initialize the page here


		}
    protected override IBusinessObjectDataSource DataSource
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
