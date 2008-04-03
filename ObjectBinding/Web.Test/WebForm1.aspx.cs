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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls;


namespace OBWTest
{

public class WebForm1 : System.Web.UI.Page
{
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected Remotion.Web.UI.Controls.SmartLabel BocPropertyLabel1;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue LastNameField;
  protected Remotion.Web.UI.Controls.SmartLabel BocPropertyLabel2;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue DateOfBirthField;
  protected Remotion.Web.UI.Controls.SmartLabel BocPropertyLabel3;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator1;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue HeightField;
  protected Remotion.Web.UI.Controls.SmartLabel BocPropertyLabel4;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator2;
  protected System.Web.UI.WebControls.Label Label1;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue GenderField;
  protected Remotion.Web.UI.Controls.SmartLabel BocPropertyLabel5;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue MarriageStatusField;
  protected Remotion.Web.UI.Controls.SmartLabel SmartLabel1;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue PartnerFirstNameField;
  protected System.Web.UI.WebControls.Label Label2;
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObjectDataSource;
  protected Remotion.ObjectBinding.Web.UI.Controls.BusinessObjectReferenceDataSourceControl PartnerDataSource;
  protected Remotion.Web.UI.Controls.SmartLabel SmartLabel2;

	private void Page_Load(object sender, System.EventArgs e)
	{
    XmlReflectionBusinessObjectStorageProvider.Current.Reset ();
    Guid personID = new Guid (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
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

      person.SaveObject();
      partner.SaveObject();
    }
    else
    {
      partner = person.Partner;
    }

    CurrentObjectDataSource.BusinessObject = (IBusinessObject) person;

    this.DataBind();
    CurrentObjectDataSource.LoadValues (IsPostBack);
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
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SaveButton_Click (object sender, System.EventArgs e)
  {
    if (Page.IsValid)
    {
      CurrentObjectDataSource.SaveValues (false);
      Person person = (Person) CurrentObjectDataSource.BusinessObject;
      person.SaveObject();
      if (person.Partner != null)
        person.Partner.SaveObject();
    }
  }

}

}
