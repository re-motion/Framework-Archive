using System;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding;

namespace OBWTest
{
public class TestTabbedPersonJobsUserControl : 
    DataEditUserControl
{
  protected Rubicon.ObjectBinding.Web.Controls.BocList ListField;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue LastNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue MultilineTextField;

  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return ReflectionBusinessObjectDataSourceControl; }
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

  }
	#endregion
	}
}
