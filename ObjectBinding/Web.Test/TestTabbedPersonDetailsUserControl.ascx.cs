using System;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding;

namespace OBWTest
{
public class TestTabbedPersonDetailsUserControl : 
    DataEditUserControl
{
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue LastNameField;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue DateOfBirthField;
  protected Rubicon.ObjectBinding.Web.Controls.BocReferenceValue PartnerField;
  protected Rubicon.ObjectBinding.Web.Controls.BocBooleanValue DeceasedField;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue DateOfDeathField;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue MarriageStatusField;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;

  public override IBusinessObjectDataSource DataSource
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
