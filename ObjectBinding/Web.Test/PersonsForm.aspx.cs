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
using Remotion.ObjectBinding.Sample;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;
using Remotion.NullableValueTypes;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.ObjectBinding;


namespace OBWTest
{

public class PersonsForm : SingleBocTestWxeBasePage
{
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Remotion.Web.UI.Controls.FormGridManager FormGridManager;
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
  protected Remotion.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocList PersonList;
  protected System.Web.UI.WebControls.Button PostBackButton;

	private void Page_Load(object sender, System.EventArgs e)
	{
    PersonList.Value = (IBusinessObject[]) Variables["objects"];
  }

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    if (!IsPostBack)
      XmlReflectionBusinessObjectStorageProvider.Current.Reset();
	}

	#region Web Form Designer generated code
	
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