using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Reflection;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding.Reflection;
using OBRTest;

namespace OBWTest
{

public class SingleBocDateTimeValueForm : SingleBocTestWxeBasePage
{
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected System.Web.UI.WebControls.Button BirthdayTestSetNullButton;
  protected System.Web.UI.WebControls.Button BirthdayTestSetFemaleButton;
  protected System.Web.UI.WebControls.Button ReadOnlyBirthdayTestSetNullButton;
  protected System.Web.UI.WebControls.Button ReadOnlyBirthdayTestSetFemaleButton;
  protected System.Web.UI.WebControls.Button SetBirthdayButton;
  protected System.Web.UI.WebControls.Button BirthdayTestSetNewValueButton;
  protected System.Web.UI.WebControls.Button ReadOnlyBirthdayTestSetNewValueButton;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue LastNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue UnboundDateOfDeathField;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected System.Web.UI.WebControls.Label DeceasedFieldCheckedChangedLabel;
  protected System.Web.UI.WebControls.Label ReadOnlyBirthdayFieldValueLabel;
  protected System.Web.UI.WebControls.Label UnboundBirthdayFieldValueLabel;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyBirthdayFieldValueLabel;
  protected System.Web.UI.WebControls.Label DateOfDeathFieldValueLabel;
  protected System.Web.UI.WebControls.Label ReadOnlyDateOfDeathFieldValueLabel;
  protected System.Web.UI.WebControls.Label DirectlySetBocDateTimeValueFieldValueLabel;
  protected System.Web.UI.WebControls.Label ReadOnlyDirectlySetBocDateTimeValueFieldValueLabel;
  protected System.Web.UI.WebControls.Label BirthdayFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue ReadOnlyBirthdayField;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue UnboundBirthdayField;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue UnboundReadOnlyBirthdayField;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue DateOfDeathField;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue ReadOnlyDateOfDeathField;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue UnboundReadOnlyDateOfDeathField;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue DirectlySetBocDateTimeValueField;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue ReadOnlyDirectlySetBocDateTimeValueField;
  protected System.Web.UI.WebControls.Label BirthdayFieldDateTimeChangedLabel;
  protected System.Web.UI.WebControls.Label UnboundDateOfDeathFieldValueLabel;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyDateOfDeathFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BirthdayField;

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

    this.UnboundBirthdayField.Property = (Rubicon.ObjectBinding.IBusinessObjectDateTimeProperty) person.GetBusinessObjectProperty("DateOfBirth");
    //this.UnboundBirthdayField.Value = person.DateOFBirth;
    this.UnboundReadOnlyBirthdayField.Property = (Rubicon.ObjectBinding.IBusinessObjectDateTimeProperty) person.GetBusinessObjectProperty("DateOfBirth");
    this.UnboundReadOnlyBirthdayField.Value = person.DateOfBirth;

    this.UnboundDateOfDeathField.Property = (Rubicon.ObjectBinding.IBusinessObjectDateProperty) person.GetBusinessObjectProperty("DateOfDeath");
    //this.UnboundDateOfDeathField.Value = person.DateOfDeath;
    this.UnboundReadOnlyDateOfDeathField.Property = (Rubicon.ObjectBinding.IBusinessObjectDateProperty) person.GetBusinessObjectProperty("DateOfDeath");
    this.UnboundReadOnlyDateOfDeathField.Value = person.DateOfDeath;

    if (!IsPostBack)
    {
      this.DirectlySetBocDateTimeValueField.Value = DateTime.Now;
      this.ReadOnlyDirectlySetBocDateTimeValueField.Value = DateTime.Now;
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
    this.BirthdayField.DateTimeChanged += new System.EventHandler(this.BirthdayField_DateTimeChanged);
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.BirthdayTestSetNullButton.Click += new System.EventHandler(this.BirthdayTestSetNullButton_Click);
    this.BirthdayTestSetNewValueButton.Click += new System.EventHandler(this.BirthdayTestSetNewValueButton_Click);
    this.ReadOnlyBirthdayTestSetNullButton.Click += new System.EventHandler(this.ReadOnlyBirthdayTestSetNullButton_Click);
    this.ReadOnlyBirthdayTestSetNewValueButton.Click += new System.EventHandler(this.ReadOnlyBirthdayTestSetNewValueButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  protected override void OnPreRender(EventArgs e)
  {
    if (! BirthdayField.IsValidValue)
      BirthdayFieldDateTimeChangedLabel.Text = "invalid data";
    else if (BirthdayField.Value != null)
      BirthdayFieldValueLabel.Text = BirthdayField.Value.ToString();
    else
      BirthdayFieldValueLabel.Text = "not set";
    
    if (! ReadOnlyBirthdayField.IsValidValue)
      BirthdayFieldDateTimeChangedLabel.Text = "invalid data";
    else if (ReadOnlyBirthdayField.Value != null)
      ReadOnlyBirthdayFieldValueLabel.Text = ReadOnlyBirthdayField.Value.ToString();
    else
      ReadOnlyBirthdayFieldValueLabel.Text = "not set";

    if (! UnboundBirthdayField.IsValidValue)
      BirthdayFieldDateTimeChangedLabel.Text = "invalid data";
    else if (UnboundBirthdayField.Value != null)
      UnboundBirthdayFieldValueLabel.Text = UnboundBirthdayField.Value.ToString();
    else
      UnboundBirthdayFieldValueLabel.Text = "not set";

    if (! UnboundReadOnlyBirthdayField.IsValidValue)
      BirthdayFieldDateTimeChangedLabel.Text = "invalid data";
    else if (UnboundReadOnlyBirthdayField.Value != null)
      UnboundReadOnlyBirthdayFieldValueLabel.Text = UnboundReadOnlyBirthdayField.Value.ToString();
    else
      UnboundReadOnlyBirthdayFieldValueLabel.Text = "not set";

    if (! DateOfDeathField.IsValidValue)
      BirthdayFieldDateTimeChangedLabel.Text = "invalid data";
    else if (DateOfDeathField.Value != null)
      DateOfDeathFieldValueLabel.Text = DateOfDeathField.Value.ToString();
    else
      DateOfDeathFieldValueLabel.Text = "not set";
    
    if (! ReadOnlyDateOfDeathField.IsValidValue)
      BirthdayFieldDateTimeChangedLabel.Text = "invalid data";
    else if (ReadOnlyDateOfDeathField.Value != null)
      ReadOnlyDateOfDeathFieldValueLabel.Text = ReadOnlyDateOfDeathField.Value.ToString();
    else
      ReadOnlyDateOfDeathFieldValueLabel.Text = "not set";

    if (! UnboundDateOfDeathField.IsValidValue)
      BirthdayFieldDateTimeChangedLabel.Text = "invalid data";
    else if (UnboundDateOfDeathField.Value != null)
      UnboundDateOfDeathFieldValueLabel.Text = UnboundDateOfDeathField.Value.ToString();
    else
      UnboundDateOfDeathFieldValueLabel.Text = "not set";

    if (! UnboundReadOnlyDateOfDeathField.IsValidValue)
      BirthdayFieldDateTimeChangedLabel.Text = "invalid data";
    else if (UnboundReadOnlyDateOfDeathField.Value != null)
      UnboundReadOnlyDateOfDeathFieldValueLabel.Text = UnboundReadOnlyDateOfDeathField.Value.ToString();
    else
      UnboundReadOnlyDateOfDeathFieldValueLabel.Text = "not set";

    if (! BirthdayField.IsValidValue)
      BirthdayFieldDateTimeChangedLabel.Text = "invalid data";
    else if (DirectlySetBocDateTimeValueField.Value != null)
      DirectlySetBocDateTimeValueFieldValueLabel.Text = DirectlySetBocDateTimeValueField.Value.ToString();
    else
      DirectlySetBocDateTimeValueFieldValueLabel.Text = "not set";

    if (! ReadOnlyDirectlySetBocDateTimeValueField.IsValidValue)
      BirthdayFieldDateTimeChangedLabel.Text = "invalid data";
    else if (ReadOnlyDirectlySetBocDateTimeValueField.Value != null)
      ReadOnlyDirectlySetBocDateTimeValueFieldValueLabel.Text = ReadOnlyDirectlySetBocDateTimeValueField.Value.ToString();
    else
      ReadOnlyDirectlySetBocDateTimeValueFieldValueLabel.Text = "not set";

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

  private void BirthdayTestSetNullButton_Click(object sender, System.EventArgs e)
  {
    BirthdayField.Value = null;
  }

  private void BirthdayTestSetNewValueButton_Click(object sender, System.EventArgs e)
  {
    BirthdayField.Value = new DateTime (1950, 1, 1);
  }

  private void ReadOnlyBirthdayTestSetNullButton_Click(object sender, System.EventArgs e)
  {
    ReadOnlyBirthdayField.Value = null;
  }

  private void ReadOnlyBirthdayTestSetNewValueButton_Click(object sender, System.EventArgs e)
  {
    ReadOnlyBirthdayField.Value = new DateTime (1950, 1, 1);;
  }

  private void BirthdayField_DateTimeChanged (object sender, System.EventArgs e)
  {
    if (! BirthdayField.IsValidValue)
      BirthdayFieldDateTimeChangedLabel.Text = "invalid data";
    else if (BirthdayField.Value != null)
      BirthdayFieldDateTimeChangedLabel.Text = BirthdayField.Value.ToString();
    else
      BirthdayFieldDateTimeChangedLabel.Text = "not set";
  }
}

}
