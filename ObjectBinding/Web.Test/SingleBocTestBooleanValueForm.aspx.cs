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
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;
using OBRTest;

namespace OBWTest
{
public class SingleBocBooleanValueForm : SingleBocTestWxeBasePage
{
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocList ChildrenList;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected System.Web.UI.WebControls.Label ChildrenListEventArgsLabel;
  protected System.Web.UI.WebControls.CheckBox ChildrenListEventCheckBox;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.ObjectBinding.Web.Controls.BocBooleanValue DeceasedField;
  protected System.Web.UI.WebControls.Button DeceasedTestSetNullButton;
  protected System.Web.UI.WebControls.Button ReadOnlyDeceasedTestSetNullButton;
  protected System.Web.UI.WebControls.Button DeceasedTestToggleValueButton;
  protected System.Web.UI.WebControls.Button ReadOnlyDeceasedTestToggleValueButton;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected System.Web.UI.WebControls.Label ReadOnlyDeceasedFieldValueLabel;
  protected System.Web.UI.WebControls.Label UnboundDeceasedFieldValueLabel;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyDeceasedFieldValueLabel;
  protected System.Web.UI.WebControls.Label DeceasedFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.Controls.BocBooleanValue ReadOnlyDeceasedField;
  protected Rubicon.ObjectBinding.Web.Controls.BocBooleanValue UnboundDeceasedField;
  protected Rubicon.ObjectBinding.Web.Controls.BocBooleanValue UnboundReadOnlyDeceasedField;
  protected System.Web.UI.WebControls.Label DeceasedFieldCheckedChangedLabel;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue LastNameField;

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
    this.UnboundDeceasedField.Property = (Rubicon.ObjectBinding.IBusinessObjectBooleanProperty) person.GetBusinessObjectProperty("Deceased");
    //this.UnboundDeceasedField.Value = person.Deceased;
    this.UnboundReadOnlyDeceasedField.Property = (Rubicon.ObjectBinding.IBusinessObjectBooleanProperty) person.GetBusinessObjectProperty("Deceased");
    this.UnboundReadOnlyDeceasedField.Value = person.Deceased;
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
    this.DeceasedField.CheckedChanged += new System.EventHandler(this.DeceasedField_CheckedChanged);
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.DeceasedTestSetNullButton.Click += new System.EventHandler(this.DeceasedTestSetNullButton_Click);
    this.DeceasedTestToggleValueButton.Click += new System.EventHandler(this.DeceasedTestToggleValueButton_Click);
    this.ReadOnlyDeceasedTestSetNullButton.Click += new System.EventHandler(this.ReadOnlyDeceasedTestSetNullButton_Click);
    this.ReadOnlyDeceasedTestToggleValueButton.Click += new System.EventHandler(this.ReadOnlyDeceasedTestToggleValueButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  protected override void OnPreRender(EventArgs e)
  {
    if (DeceasedField.Value != null)
      DeceasedFieldValueLabel.Text = DeceasedField.Value.ToString();
    else
      DeceasedFieldValueLabel.Text = "not set";
    
    if (ReadOnlyDeceasedField.Value != null)
      ReadOnlyDeceasedFieldValueLabel.Text = ReadOnlyDeceasedField.Value.ToString();
    else
      ReadOnlyDeceasedFieldValueLabel.Text = "not set";

    if (UnboundDeceasedField.Value != null)
      UnboundDeceasedFieldValueLabel.Text = UnboundDeceasedField.Value.ToString();
    else
      UnboundDeceasedFieldValueLabel.Text = "not set";

   if (UnboundReadOnlyDeceasedField.Value != null)
      UnboundReadOnlyDeceasedFieldValueLabel.Text = UnboundReadOnlyDeceasedField.Value.ToString();
    else
      UnboundReadOnlyDeceasedFieldValueLabel.Text = "not set";

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

  private void DeceasedTestSetNullButton_Click(object sender, System.EventArgs e)
  {
    DeceasedField.Value = null;
  }

  private void DeceasedTestToggleValueButton_Click(object sender, System.EventArgs e)
  {
    if (DeceasedField.Value != null)
      DeceasedField.Value = ! (bool) DeceasedField.Value;
    else
      DeceasedField.Value = false;
  }

  private void ReadOnlyDeceasedTestSetNullButton_Click(object sender, System.EventArgs e)
  {
    ReadOnlyDeceasedField.Value = null;
  }

  private void ReadOnlyDeceasedTestToggleValueButton_Click(object sender, System.EventArgs e)
  {
    if (ReadOnlyDeceasedField.Value != null)
      ReadOnlyDeceasedField.Value = ! (bool) ReadOnlyDeceasedField.Value;
    else
      ReadOnlyDeceasedField.Value = false;
  }

  private void DeceasedField_CheckedChanged(object sender, System.EventArgs e)
  {
    if (DeceasedField.Value != null)
      DeceasedFieldCheckedChangedLabel.Text = DeceasedField.Value.ToString();
    else
      DeceasedFieldCheckedChangedLabel.Text = "not set";
  }
}

}
