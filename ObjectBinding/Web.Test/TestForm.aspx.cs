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
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Utilities;
using Rubicon.NullableValueTypes;

namespace OBWTest
{

/// <summary>
/// Summary description for WebFormMK.
/// </summary>
public class WebFormMK : WebFormBase

{
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSource reflectionBusinessObjectDataSource;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator1;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue GenderField;
  protected Rubicon.ObjectBinding.Web.Controls.BocReferenceValue PartnerField;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BirthdayField;
  protected System.Web.UI.WebControls.Button PostBackButton;

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

    reflectionBusinessObjectDataSource.BusinessObject = person;

    this.DataBind();
    if (!IsPostBack)
    {
      reflectionBusinessObjectDataSource.LoadValues (false);
    }
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
	}

	#region Web Form Designer generated code
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.reflectionBusinessObjectDataSource = new Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSource();
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    // 
    // reflectionBusinessObjectDataSource
    // 
    this.reflectionBusinessObjectDataSource.BusinessObject = null;
    this.reflectionBusinessObjectDataSource.EditMode = true;
    this.reflectionBusinessObjectDataSource.TypeName = "OBWTest.Person, OBWTest";
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SaveButton_Click(object sender, System.EventArgs e)
  {
    bool isValid = FormGridManager.Validate();
    if (isValid)
    {
      reflectionBusinessObjectDataSource.SaveValues (false);
      Person person = (Person) reflectionBusinessObjectDataSource.BusinessObject;
      person.SaveObject();
      if (person.Partner != null)
        person.Partner.SaveObject();
    }
  }
}

}