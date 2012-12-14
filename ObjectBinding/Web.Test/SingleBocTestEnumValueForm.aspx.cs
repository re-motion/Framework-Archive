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

public class SingleBocEnumValueForm : SingleBocTestWxeBasePage
{
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected System.Web.UI.WebControls.Button GenderTestSetNullButton;
  protected System.Web.UI.WebControls.Button GenderTestSetFemaleButton;
  protected System.Web.UI.WebControls.Button ReadOnlyGenderTestSetNullButton;
  protected System.Web.UI.WebControls.Button ReadOnlyGenderTestSetFemaleButton;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue LastNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue GenderField;
  protected System.Web.UI.WebControls.Button ReadOnlyGenderTestSetNewItemButton;
  protected System.Web.UI.WebControls.Button GenderTestSetDisabledGenderButton;
  protected System.Web.UI.WebControls.Button GenderTestSetMarriedButton;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected System.Web.UI.WebControls.Label ReadOnlyGenderFieldValueLabel;
  protected System.Web.UI.WebControls.Label MarriageStatusFieldValueLabel;
  protected System.Web.UI.WebControls.Label UnboundMarriageStatusFieldValueLabel;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyMarriageStatusFieldValueLabel;
  protected System.Web.UI.WebControls.Label GenderFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue ReadOnlyGenderField;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue MarriageStatusField;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue UnboundMarriageStatusField;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue UnboundReadOnlyMarriageStatusField;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue DeceasedAsEnumField;
  protected System.Web.UI.WebControls.Label DeceasedAsEnumFieldValueLabel;
  protected System.Web.UI.WebControls.Label GenderFieldSelectionChangedLabel;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;

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
    this.UnboundMarriageStatusField.Property = (Rubicon.ObjectBinding.IBusinessObjectEnumerationProperty) person.GetBusinessObjectProperty("MarriageStatus");
    //this.UnboundMarriageStatusField.Value = person.MarriageStatus;
    this.UnboundReadOnlyMarriageStatusField.Property = (Rubicon.ObjectBinding.IBusinessObjectEnumerationProperty) person.GetBusinessObjectProperty("MarriageStatus");
    this.UnboundReadOnlyMarriageStatusField.Value = person.MarriageStatus;
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
    this.GenderField.SelectionChanged += new System.EventHandler(this.GenderField_SelectionChanged);
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.GenderTestSetNullButton.Click += new System.EventHandler(this.GenderTestSetNullButton_Click);
    this.GenderTestSetDisabledGenderButton.Click += new System.EventHandler(this.GenderTestSetDisabledGenderButton_Click);
    this.GenderTestSetMarriedButton.Click += new System.EventHandler(this.GenderTestSetMarriedButton_Click);
    this.ReadOnlyGenderTestSetNullButton.Click += new System.EventHandler(this.ReadOnlyGenderTestSetNullButton_Click);
    this.ReadOnlyGenderTestSetNewItemButton.Click += new System.EventHandler(this.ReadOnlyGenderTestSetFemaleButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  protected override void OnPreRender(EventArgs e)
  {
    if (GenderField.Value != null)
      GenderFieldValueLabel.Text = GenderField.Value.ToString();
    else
      GenderFieldValueLabel.Text = "not set";
    
    if (ReadOnlyGenderField.Value != null)
      ReadOnlyGenderFieldValueLabel.Text = ReadOnlyGenderField.Value.ToString();
    else
      ReadOnlyGenderFieldValueLabel.Text = "not set";

    if (MarriageStatusField.Value != null)
      MarriageStatusFieldValueLabel.Text = MarriageStatusField.Value.ToString();
    else
      MarriageStatusFieldValueLabel.Text = "not set";

   if (UnboundMarriageStatusField.Value != null)
      UnboundMarriageStatusFieldValueLabel.Text = UnboundMarriageStatusField.Value.ToString();
    else
      UnboundMarriageStatusFieldValueLabel.Text = "not set";

   if (UnboundReadOnlyMarriageStatusField.Value != null)
      UnboundReadOnlyMarriageStatusFieldValueLabel.Text = UnboundReadOnlyMarriageStatusField.Value.ToString();
    else
      UnboundReadOnlyMarriageStatusFieldValueLabel.Text = "not set";

   if (DeceasedAsEnumField.Value != null)
      DeceasedAsEnumFieldValueLabel.Text = DeceasedAsEnumField.Value.ToString();
    else
      DeceasedAsEnumFieldValueLabel.Text = "not set";

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

  private void GenderTestSetNullButton_Click(object sender, System.EventArgs e)
  {
    GenderField.Value = null;
  }

  private void GenderTestSetDisabledGenderButton_Click(object sender, System.EventArgs e)
  {
    GenderField.Value = Gender.Disabled_UnknownGender;
  }

  private void GenderTestSetMarriedButton_Click(object sender, System.EventArgs e)
  {
    GenderField.Value = MarriageStatus.Married;
  }

  private void ReadOnlyGenderTestSetNullButton_Click(object sender, System.EventArgs e)
  {
    ReadOnlyGenderField.Value = null;
  }

  private void ReadOnlyGenderTestSetFemaleButton_Click(object sender, System.EventArgs e)
  {
    ReadOnlyGenderField.Value = Gender.Female;
  }

  private void GenderField_SelectionChanged(object sender, System.EventArgs e)
  {
    if (GenderField.Value != null)
      GenderFieldSelectionChangedLabel.Text = GenderField.Value.ToString();
    else
      GenderFieldSelectionChangedLabel.Text = "not set";
  }
}

}
