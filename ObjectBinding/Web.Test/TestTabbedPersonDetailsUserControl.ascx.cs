using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI;
using Rubicon.Collections;

namespace OBWTest
{
public class TestTabbedPersonDetailsUserControl : 
    DataEditUserControl, IControl, IFormGridRowProvider
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
  private AutoInitHashtable _listOfFormGridRowInfos =
      new AutoInitHashtable (typeof (FormGridRowInfoCollection));
  private AutoInitHashtable _listOfHiddenRows = 
      new AutoInitHashtable (typeof (StringCollection));

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return ReflectionBusinessObjectDataSourceControl; }
  }

  public virtual StringCollection GetHiddenRows (HtmlTable table)
  {
    return (StringCollection) _listOfHiddenRows[table];
  }

  public virtual FormGridRowInfoCollection GetAdditionalRows (HtmlTable table)
  {
    return (FormGridRowInfoCollection) _listOfFormGridRowInfos[table];
  }

	#region Web Form Designer generated code

	override protected void OnInit(EventArgs e)
	{
    StringCollection hiddenRows = (StringCollection)_listOfHiddenRows[FormGrid];
    FormGridRowInfoCollection newRows = (FormGridRowInfoCollection)_listOfFormGridRowInfos[FormGrid];

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
