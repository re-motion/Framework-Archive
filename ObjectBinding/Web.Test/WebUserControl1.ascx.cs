namespace OBWTest
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
  using Rubicon.ObjectBinding.Web;

	/// <summary>
	///		Summary description for WebUserControl1.
	/// </summary>
	public class WebUserControl1 : DataSourceUserControl
	{
    protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator1;
    protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue1;
    protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSource CurrentObject;
    protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSource reflectionBusinessObjectDataSource1;

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here


		}
    protected override Rubicon.ObjectBinding.IBusinessObjectDataSource DataSource
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
      this.CurrentObject = new Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSource();
      // 
      // CurrentObject
      // 
      this.CurrentObject.BusinessObject = null;
      this.CurrentObject.EditMode = true;
      this.CurrentObject.TypeName = null;
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion
	}
}
