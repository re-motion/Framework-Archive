using System;
using System.Web;
using System.Web.UI.WebControls;

using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Utilities;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;

using OBRTest;

namespace OBWTest.IndividualControlTests
{

public class BocDateTimeValueUserControl : BaseUserControl
{
  protected System.Web.UI.HtmlControls.HtmlGenericControl NonVisualControls;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue LastNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue BirthdayField;
  protected System.Web.UI.WebControls.Label BirthdayFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue ReadOnlyBirthdayField;
  protected System.Web.UI.WebControls.Label ReadOnlyBirthdayFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue UnboundBirthdayField;
  protected System.Web.UI.WebControls.Label UnboundBirthdayFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue UnboundRequiredBirthdayField;
  protected System.Web.UI.WebControls.Label UnboundRequiredBirthdayFieldLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue UnboundReadOnlyBirthdayField;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyBirthdayFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue DateOfDeathField;
  protected System.Web.UI.WebControls.Label DateOfDeathFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue ReadOnlyDateOfDeathField;
  protected System.Web.UI.WebControls.Label ReadOnlyDateOfDeathFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue UnboundDateOfDeathField;
  protected System.Web.UI.WebControls.Label UnboundDateOfDeathFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue UnboundReadOnlyDateOfDeathField;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyDateOfDeathFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue DirectlySetBocDateTimeValueField;
  protected System.Web.UI.WebControls.Label DirectlySetBocDateTimeValueFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue ReadOnlyDirectlySetBocDateTimeValueField;
  protected System.Web.UI.WebControls.Label ReadOnlyDirectlySetBocDateTimeValueFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue DisabledBirthdayField;
  protected System.Web.UI.WebControls.Label DisabledBirthdayFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue DisabledReadOnlyBirthdayField;
  protected System.Web.UI.WebControls.Label DisabledReadOnlyBirthdayFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue DisabledUnboundBirthdayField;
  protected System.Web.UI.WebControls.Label DisabledUnboundBirthdayFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue DisabledUnboundReadOnlyBirthdayField;
  protected System.Web.UI.WebControls.Label DisabledUnboundReadOnlyBirthdayFieldValueLabel;
  protected Rubicon.Web.UI.Controls.WebButton BirthdayTestSetNullButton;
  protected Rubicon.Web.UI.Controls.WebButton BirthdayTestSetNewValueButton;
  protected System.Web.UI.WebControls.Label BirthdayFieldDateTimeChangedLabel;
  protected Rubicon.Web.UI.Controls.WebButton ReadOnlyBirthdayTestSetNullButton;
  protected Rubicon.Web.UI.Controls.WebButton ReadOnlyBirthdayTestSetNewValueButton;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl CurrentObject;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    this.BirthdayField.DateTimeChanged += new System.EventHandler(this.BirthdayField_DateTimeChanged);
    this.BirthdayTestSetNullButton.Click += new System.EventHandler(this.BirthdayTestSetNullButton_Click);
    this.BirthdayTestSetNewValueButton.Click += new System.EventHandler(this.BirthdayTestSetNewValueButton_Click);
    this.ReadOnlyBirthdayTestSetNullButton.Click += new System.EventHandler(this.ReadOnlyBirthdayTestSetNullButton_Click);
    this.ReadOnlyBirthdayTestSetNewValueButton.Click += new System.EventHandler(this.ReadOnlyBirthdayTestSetNewValueButton_Click);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    Person person = (Person) CurrentObject.BusinessObject;

    UnboundBirthdayField.Property = (Rubicon.ObjectBinding.IBusinessObjectDateTimeProperty) person.GetBusinessObjectProperty("DateOfBirth");
    //UnboundBirthdayField.LoadUnboundValue (person.DateOFBirth, IsPostBack);
    UnboundReadOnlyBirthdayField.Property = (Rubicon.ObjectBinding.IBusinessObjectDateTimeProperty) person.GetBusinessObjectProperty("DateOfBirth");
    UnboundReadOnlyBirthdayField.LoadUnboundValue (person.DateOfBirth, IsPostBack);

    UnboundDateOfDeathField.Property = (Rubicon.ObjectBinding.IBusinessObjectDateProperty) person.GetBusinessObjectProperty("DateOfDeath");
    UnboundDateOfDeathField.LoadUnboundValue (person.DateOfDeath, IsPostBack);
    //UnboundReadOnlyDateOfDeathField.Property = (Rubicon.ObjectBinding.IBusinessObjectDateProperty) person.GetBusinessObjectProperty("DateOfDeath");
    UnboundReadOnlyDateOfDeathField.LoadUnboundValue (person.DateOfDeath, IsPostBack);

    DisabledUnboundBirthdayField.Property = (Rubicon.ObjectBinding.IBusinessObjectDateTimeProperty) person.GetBusinessObjectProperty("DateOfBirth");
    DisabledUnboundBirthdayField.LoadUnboundValue (person.DateOfBirth, IsPostBack);
    DisabledUnboundReadOnlyBirthdayField.Property = (Rubicon.ObjectBinding.IBusinessObjectDateTimeProperty) person.GetBusinessObjectProperty("DateOfBirth");
    DisabledUnboundReadOnlyBirthdayField.LoadUnboundValue (person.DateOfBirth, IsPostBack);

    DirectlySetBocDateTimeValueField.LoadUnboundValue (DateTime.Now, IsPostBack);
    ReadOnlyDirectlySetBocDateTimeValueField.LoadUnboundValue (DateTime.Now, IsPostBack);

    if (!IsPostBack)
    {
      if (Page is ISmartNavigablePage)
        ((ISmartNavigablePage) Page).SetFocus (BirthdayField);
    }
  }

  override protected void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    SetDebugLabel (BirthdayField, BirthdayFieldValueLabel);
    SetDebugLabel (ReadOnlyBirthdayField, ReadOnlyBirthdayFieldValueLabel);
    SetDebugLabel (UnboundBirthdayField, UnboundBirthdayFieldValueLabel);
    SetDebugLabel (UnboundReadOnlyBirthdayField, UnboundReadOnlyBirthdayFieldValueLabel);
    SetDebugLabel (DateOfDeathField, DateOfDeathFieldValueLabel);
    SetDebugLabel (ReadOnlyDateOfDeathField, ReadOnlyDateOfDeathFieldValueLabel);
    SetDebugLabel (UnboundDateOfDeathField, UnboundDateOfDeathFieldValueLabel);
    SetDebugLabel (UnboundReadOnlyDateOfDeathField, UnboundReadOnlyDateOfDeathFieldValueLabel);
    SetDebugLabel (DirectlySetBocDateTimeValueField, DirectlySetBocDateTimeValueFieldValueLabel);
    SetDebugLabel (ReadOnlyDirectlySetBocDateTimeValueField, ReadOnlyDirectlySetBocDateTimeValueFieldValueLabel);
    SetDebugLabel (DisabledBirthdayField, DisabledBirthdayFieldValueLabel);
    SetDebugLabel (DisabledReadOnlyBirthdayField, DisabledReadOnlyBirthdayFieldValueLabel);
    SetDebugLabel (DisabledUnboundBirthdayField, DisabledUnboundBirthdayFieldValueLabel);
    SetDebugLabel (DisabledUnboundReadOnlyBirthdayField, DisabledUnboundReadOnlyBirthdayFieldValueLabel);
  }

  private void SetDebugLabel (IBusinessObjectBoundWebControl control, Label label)
  {
   if (control.Value != null)
      label.Text = control.Value.ToString();
    else
      label.Text = "not set";
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
    if (BirthdayField.Value != null)
      BirthdayFieldDateTimeChangedLabel.Text = BirthdayField.Value.ToString();
    else
      BirthdayFieldDateTimeChangedLabel.Text = "not set";
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
