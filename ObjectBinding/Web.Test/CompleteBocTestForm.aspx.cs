using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;
using Rubicon.Utilities;
using Rubicon.Collections;
using Rubicon.ObjectBinding.Web.Controls;

namespace OBWTest
{

  public class TestForm : 
      WebFormBase,
      IFormGridRowProvider //  Provides new rows and rows to hide to the FormGridManager
{

  private AutoInitHashtable _listOfFormGridRowInfos = new AutoInitHashtable (typeof (FormGridRowInfoCollection));
  private AutoInitHashtable _listOfHiddenRows = new AutoInitHashtable (typeof (StringCollection));

    protected System.Web.UI.WebControls.Button PostBackButton;
    protected System.Web.UI.WebControls.Button SaveButton;
    protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
    protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
    protected Rubicon.ObjectBinding.Web.Controls.BocTextValue LastNameField;
    protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
    protected Rubicon.ObjectBinding.Web.Controls.BocTextValue TextField;
    protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue MultilineTextField;
    protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue DateTimeField;
    protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue EnumField;
    protected Rubicon.ObjectBinding.Web.Controls.BocReferenceValue ReferenceField;
    protected Rubicon.ObjectBinding.Web.Controls.BocBooleanValue BooleanField;
    protected Rubicon.ObjectBinding.Web.Controls.BocList ListField;
    protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
    protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

  private void Page_Load(object sender, System.EventArgs e)
	{
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject (personID);
    Person partner;
    if (person == null)
    {
      person = Person.CreateObject (personID);
      person.FirstName = "Hugo";
      person.LastName = "Meier";
      person.DateOfBirth = new DateTime (1959, 4, 15);
      person.Height = 179;
      person.Income = 2000;

      partner = person.Partner = Person.CreateObject();
      partner.FirstName = "Sepp";
      partner.LastName = "Forcher";
    }
    else
    {
      partner = person.Partner;
    }

    ReflectionBusinessObjectDataSourceControl.BusinessObject = person;
    
    ReflectionBusinessObjectDataSourceControl.LoadValues (IsPostBack);
	}

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    if (!IsPostBack)
      Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectStorage.Reset();
  
    FormGridRowInfoCollection newRows = (FormGridRowInfoCollection)_listOfFormGridRowInfos[FormGrid.ID];

    BocTextValue incomeField = new BocTextValue();
    incomeField.ID = "IncomeField";
    incomeField.DataSourceControl = ReflectionBusinessObjectDataSourceControl.ID;
    incomeField.PropertyIdentifier = "Income";

    //  A new row
    newRows.Add (new FormGridRowInfo(
        incomeField, 
        FormGridRowInfo.RowType.ControlInRowWithLabel, 
        BooleanField.ID, 
        FormGridRowInfo.RowPosition.AfterRowWithID));
	}

	#region Web Form Designer generated code
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SaveButton_Click(object sender, System.EventArgs e)
  {
    bool isValid = FormGridManager.Validate();
    if (isValid)
    {
      ReflectionBusinessObjectDataSourceControl.SaveValues (false);
      Person person = (Person) ReflectionBusinessObjectDataSourceControl.BusinessObject;
      person.SaveObject();
    }
  }

  public virtual StringCollection GetHiddenRows (string table)
  {
    return (StringCollection) _listOfHiddenRows[table];
  }

  public virtual FormGridRowInfoCollection GetAdditionalRows (string table)
  {
    return (FormGridRowInfoCollection) _listOfFormGridRowInfos[table];
  }
}

}
