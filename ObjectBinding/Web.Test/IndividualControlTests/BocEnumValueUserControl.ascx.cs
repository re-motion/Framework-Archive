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

public class BocEnumValueUserControl : BaseUserControl
{
  protected System.Web.UI.HtmlControls.HtmlGenericControl NonVisualControls;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue LastNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue GenderField;
  protected System.Web.UI.WebControls.Label GenderFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue ReadOnlyGenderField;
  protected System.Web.UI.WebControls.Label ReadOnlyGenderFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue MarriageStatusField;
  protected System.Web.UI.WebControls.Label MarriageStatusFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue UnboundMarriageStatusField;
  protected System.Web.UI.WebControls.Label UnboundMarriageStatusFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue UnboundReadOnlyMarriageStatusField;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyMarriageStatusFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue DeceasedAsEnumField;
  protected System.Web.UI.WebControls.Label DeceasedAsEnumFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue DisabledGenderField;
  protected System.Web.UI.WebControls.Label DisabledGenderFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue DisabledReadOnlyGenderField;
  protected System.Web.UI.WebControls.Label DisabledReadOnlyGenderFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue DisabledMarriageStatusField;
  protected System.Web.UI.WebControls.Label DisabledMarriageStatusFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue DisabledUnboundMarriageStatusField;
  protected System.Web.UI.WebControls.Label DisabledUnboundMarriageStatusFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue DisabledUnboundReadOnlyMarriageStatusField;
  protected System.Web.UI.WebControls.Label DisabledUnboundReadOnlyMarriageStatusFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue InstanceEnumField;
  protected System.Web.UI.WebControls.Label InstanceEnumFieldValueLabel;
  protected System.Web.UI.WebControls.Label GenderFieldSelectionChangedLabel;
  protected Rubicon.Web.UI.Controls.WebButton GenderTestSetNullButton;
  protected Rubicon.Web.UI.Controls.WebButton GenderTestSetDisabledGenderButton;
  protected Rubicon.Web.UI.Controls.WebButton GenderTestSetMarriedButton;
  protected Rubicon.Web.UI.Controls.WebButton ReadOnlyGenderTestSetNullButton;
  protected Rubicon.Web.UI.Controls.WebButton ReadOnlyGenderTestSetNewItemButton;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  
  private string _instanceEnum;

  public string InstanceEnum
  {
    get { return _instanceEnum; }
    set { _instanceEnum = value; }
  }

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    this.GenderField.SelectionChanged += new System.EventHandler(this.GenderField_SelectionChanged);
    this.GenderTestSetNullButton.Click += new System.EventHandler(this.GenderTestSetNullButton_Click);
    this.GenderTestSetDisabledGenderButton.Click += new System.EventHandler(this.GenderTestSetDisabledGenderButton_Click);
    this.GenderTestSetMarriedButton.Click += new System.EventHandler(this.GenderTestSetMarriedButton_Click);
    this.ReadOnlyGenderTestSetNullButton.Click += new System.EventHandler(this.ReadOnlyGenderTestSetNullButton_Click);
    this.ReadOnlyGenderTestSetNewItemButton.Click += new System.EventHandler(this.ReadOnlyGenderTestSetFemaleButton_Click);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    Person person = (Person) CurrentObject.BusinessObject;

    GenderField.LoadUnboundValue ((Gender?)null, IsPostBack);

    UnboundMarriageStatusField.Property = (Rubicon.ObjectBinding.IBusinessObjectEnumerationProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("MarriageStatus");
    //UnboundMarriageStatusField.LoadUnboundValue (person.MarriageStatus, IsPostBack);
    UnboundReadOnlyMarriageStatusField.Property = (Rubicon.ObjectBinding.IBusinessObjectEnumerationProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("MarriageStatus");
    UnboundReadOnlyMarriageStatusField.LoadUnboundValue (person.MarriageStatus, IsPostBack);
    DisabledUnboundMarriageStatusField.Property = (Rubicon.ObjectBinding.IBusinessObjectEnumerationProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("MarriageStatus");
    DisabledUnboundMarriageStatusField.LoadUnboundValue (person.MarriageStatus, IsPostBack);
    DisabledUnboundReadOnlyMarriageStatusField.Property = (Rubicon.ObjectBinding.IBusinessObjectEnumerationProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("MarriageStatus");
    DisabledUnboundReadOnlyMarriageStatusField.LoadUnboundValue (person.MarriageStatus, IsPostBack);

    if (!IsPostBack)
    {
      if (Page is ISmartNavigablePage)
        ((ISmartNavigablePage) Page).SetFocus (MarriageStatusField);
    }
  }

  override protected void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    SetDebugLabel (GenderField, GenderFieldValueLabel);
    SetDebugLabel (ReadOnlyGenderField, ReadOnlyGenderFieldValueLabel);
    SetDebugLabel (MarriageStatusField, MarriageStatusFieldValueLabel);
    SetDebugLabel (UnboundMarriageStatusField, UnboundMarriageStatusFieldValueLabel);
    SetDebugLabel (UnboundReadOnlyMarriageStatusField, UnboundReadOnlyMarriageStatusFieldValueLabel);
    SetDebugLabel (DeceasedAsEnumField, DeceasedAsEnumFieldValueLabel);
    SetDebugLabel (DisabledGenderField, DisabledGenderFieldValueLabel);
    SetDebugLabel (DisabledReadOnlyGenderField, DisabledReadOnlyGenderFieldValueLabel);
    SetDebugLabel (DisabledMarriageStatusField, DisabledMarriageStatusFieldValueLabel);
    SetDebugLabel (DisabledUnboundMarriageStatusField, DisabledUnboundMarriageStatusFieldValueLabel);
    SetDebugLabel (DisabledUnboundReadOnlyMarriageStatusField, DisabledUnboundReadOnlyMarriageStatusFieldValueLabel);
    SetDebugLabel (InstanceEnumField, InstanceEnumFieldValueLabel);
  }

  private void SetDebugLabel (IBusinessObjectBoundWebControl control, Label label)
  {
   if (control.Value != null)
      label.Text = control.Value.ToString();
    else
      label.Text = "not set";
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
