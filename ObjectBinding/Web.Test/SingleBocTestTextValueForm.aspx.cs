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
using Rubicon.Web.Utilities;
using Rubicon.NullableValueTypes;
using OBRTest;

namespace OBWTest
{

  public class SingleBocTextValueForm : SingleBocTestWxeBasePage

{
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected System.Web.UI.WebControls.Button FirstNameTestSetNullButton;
  protected System.Web.UI.WebControls.Button FirstNameTestSetFemaleButton;
  protected System.Web.UI.WebControls.Button ReadOnlyFirstNameTestSetNullButton;
  protected System.Web.UI.WebControls.Button ReadOnlyFirstNameTestSetFemaleButton;
  protected System.Web.UI.WebControls.Button SetFirstNameButton;
  protected System.Web.UI.WebControls.Button FirstNameTestSetNewValueButton;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
  protected System.Web.UI.WebControls.Button BirthdayTestSetNullButton;
  protected System.Web.UI.WebControls.Button BirthdayTestSetNewValueButton;
  protected System.Web.UI.WebControls.Button BirthdayTestReadValueButton;
  protected System.Web.UI.WebControls.Label BirthdayReadValueLabel;
  protected System.Web.UI.WebControls.Button ReadOnlyBirthdayTestSetNullButton;
  protected System.Web.UI.WebControls.Button ReadOnlyBirthdayTestSetNewValueButton;
  protected System.Web.UI.WebControls.Button ReadOnlyBirthdayTestReadValueButton;
  protected System.Web.UI.WebControls.Label ReadOnlyBirthdayReadValueLabel;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected System.Web.UI.WebControls.Label ReadOnlyFirstNameFieldValueLabel;
  protected System.Web.UI.WebControls.Label UnboundFirstNameFieldValueLabel;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyFirstNameFieldValueLabel;
  protected System.Web.UI.WebControls.Label FirstNameFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue ReadOnlyFirstNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue UnboundFirstNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue UnboundReadOnlyFirstNameField;
  protected System.Web.UI.WebControls.Label FirstNameFieldTextChangedLabel;
  protected System.Web.UI.WebControls.Button ReadOnlyFirstNameTestSetNewValueButton;

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
    this.UnboundFirstNameField.Property = (Rubicon.ObjectBinding.IBusinessObjectStringProperty) person.GetBusinessObjectProperty("FirstName");
    //this.UnboundFirstNameField.Value = person.FirstName;
    this.UnboundReadOnlyFirstNameField.Property = (Rubicon.ObjectBinding.IBusinessObjectStringProperty) person.GetBusinessObjectProperty("FirstName");
    this.UnboundReadOnlyFirstNameField.Value = person.FirstName;
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
    this.FirstNameField.TextChanged += new System.EventHandler(this.FirstNameField_TextChanged);
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.FirstNameTestSetNullButton.Click += new System.EventHandler(this.FirstNameTestSetNullButton_Click);
    this.FirstNameTestSetNewValueButton.Click += new System.EventHandler(this.FirstNameTestSetNewValueButton_Click);
    this.ReadOnlyFirstNameTestSetNullButton.Click += new System.EventHandler(this.ReadOnlyFirstNameTestSetNullButton_Click);
    this.ReadOnlyFirstNameTestSetNewValueButton.Click += new System.EventHandler(this.ReadOnlyFirstNameTestSetNewValueButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  protected override void OnPreRender(EventArgs e)
  {
    if (FirstNameField.Value != null)
      FirstNameFieldValueLabel.Text = FirstNameField.Value.ToString();
    else
      FirstNameFieldValueLabel.Text = "not set";
    
    if (ReadOnlyFirstNameField.Value != null)
      ReadOnlyFirstNameFieldValueLabel.Text = ReadOnlyFirstNameField.Value.ToString();
    else
      ReadOnlyFirstNameFieldValueLabel.Text = "not set";

    if (UnboundFirstNameField.Value != null)
      UnboundFirstNameFieldValueLabel.Text = UnboundFirstNameField.Value.ToString();
    else
      UnboundFirstNameFieldValueLabel.Text = "not set";

   if (UnboundReadOnlyFirstNameField.Value != null)
      UnboundReadOnlyFirstNameFieldValueLabel.Text = UnboundReadOnlyFirstNameField.Value.ToString();
    else
      UnboundReadOnlyFirstNameFieldValueLabel.Text = "not set";

    base.OnPreRender (e);
  }

  private void SaveButton_Click(object sender, System.EventArgs e)
  {
    bool isValid = FormGridManager.Validate();
    if (isValid)
    {
      ReflectionBusinessObjectDataSourceControl.SaveValues (false);
      Person person = (Person) ReflectionBusinessObjectDataSourceControl.BusinessObject;
      person.SaveObject();
    }
    Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectStorage.Reset();
  }

  private void FirstNameTestSetNullButton_Click(object sender, System.EventArgs e)
  {
    FirstNameField.Value = null;
  }

  private void FirstNameTestSetNewValueButton_Click(object sender, System.EventArgs e)
  {
    FirstNameField.Value = "Foo Bar";
  }

  private void ReadOnlyFirstNameTestSetNullButton_Click(object sender, System.EventArgs e)
  {
    ReadOnlyFirstNameField.Value = null;
  }

  private void ReadOnlyFirstNameTestSetNewValueButton_Click(object sender, System.EventArgs e)
  {
    ReadOnlyFirstNameField.Value = "Foo Bar";
  }

  private void FirstNameField_TextChanged(object sender, System.EventArgs e)
  {
    if (FirstNameField.Value != null)
      FirstNameFieldTextChangedLabel.Text = FirstNameField.Value.ToString();
    else
      FirstNameFieldTextChangedLabel.Text = "not set";
  }
}

}
