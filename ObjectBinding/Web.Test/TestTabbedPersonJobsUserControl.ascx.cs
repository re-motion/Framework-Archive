using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI;
using Rubicon.Collections;

namespace OBWTest
{
public class TestTabbedPersonJobsUserControl : 
    DataEditUserControl, IControl, IFormGridRowProvider
{
  protected Rubicon.ObjectBinding.Web.Controls.BocList ListField;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue MultilineTextField;

  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  private AutoInitHashtable _listOfFormGridRowInfos =
      new AutoInitHashtable (typeof (FormGridRowInfoCollection));
  private AutoInitHashtable _listOfHiddenRows = 
      new AutoInitHashtable (typeof (StringCollection));
  private Control _incomeField;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad (e);
    _incomeField.Visible = false;
  }

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
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    FormGridRowInfoCollection newRows = (FormGridRowInfoCollection)_listOfFormGridRowInfos[FormGrid];

    BocTextValue incomeField = new BocTextValue();
    incomeField.ID = "IncomeField";
    incomeField.DataSourceControl = ReflectionBusinessObjectDataSourceControl.ID;
    incomeField.PropertyIdentifier = "Income";
    _incomeField = incomeField;

    //  A new row
    newRows.Add (new FormGridRowInfo(
        incomeField, 
        FormGridRowInfo.RowType.ControlInRowWithLabel, 
        MultilineTextField.ID, 
        FormGridRowInfo.RowPosition.AfterRowWithID));
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
