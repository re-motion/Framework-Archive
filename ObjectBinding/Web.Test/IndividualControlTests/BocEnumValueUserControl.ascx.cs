using System;
using System.Web;
using System.Web.UI.WebControls;

using Remotion.ObjectBinding;

using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace OBWTest.IndividualControlTests
{

public class BocEnumValueUserControl : BaseUserControl
{
  protected System.Web.UI.HtmlControls.HtmlGenericControl NonVisualControls;
  protected Remotion.Web.UI.Controls.FormGridManager FormGridManager;
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue LastNameField;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue GenderField;
  protected System.Web.UI.WebControls.Label GenderFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue ReadOnlyGenderField;
  protected System.Web.UI.WebControls.Label ReadOnlyGenderFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue MarriageStatusField;
  protected System.Web.UI.WebControls.Label MarriageStatusFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue UnboundMarriageStatusField;
  protected System.Web.UI.WebControls.Label UnboundMarriageStatusFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue UnboundReadOnlyMarriageStatusField;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyMarriageStatusFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue DeceasedAsEnumField;
  protected System.Web.UI.WebControls.Label DeceasedAsEnumFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue DisabledGenderField;
  protected System.Web.UI.WebControls.Label DisabledGenderFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue DisabledReadOnlyGenderField;
  protected System.Web.UI.WebControls.Label DisabledReadOnlyGenderFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue DisabledMarriageStatusField;
  protected System.Web.UI.WebControls.Label DisabledMarriageStatusFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue DisabledUnboundMarriageStatusField;
  protected System.Web.UI.WebControls.Label DisabledUnboundMarriageStatusFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue DisabledUnboundReadOnlyMarriageStatusField;
  protected System.Web.UI.WebControls.Label DisabledUnboundReadOnlyMarriageStatusFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue InstanceEnumField;
  protected System.Web.UI.WebControls.Label InstanceEnumFieldValueLabel;
  protected System.Web.UI.WebControls.Label GenderFieldSelectionChangedLabel;
  protected Remotion.Web.UI.Controls.WebButton GenderTestSetNullButton;
  protected Remotion.Web.UI.Controls.WebButton GenderTestSetDisabledGenderButton;
  protected Remotion.Web.UI.Controls.WebButton GenderTestSetMarriedButton;
  protected Remotion.Web.UI.Controls.WebButton ReadOnlyGenderTestSetNullButton;
  protected Remotion.Web.UI.Controls.WebButton ReadOnlyGenderTestSetNewItemButton;
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

    UnboundMarriageStatusField.Property = (Remotion.ObjectBinding.IBusinessObjectEnumerationProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("MarriageStatus");
    //UnboundMarriageStatusField.LoadUnboundValue (person.MarriageStatus, IsPostBack);
    UnboundReadOnlyMarriageStatusField.Property = (Remotion.ObjectBinding.IBusinessObjectEnumerationProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("MarriageStatus");
    UnboundReadOnlyMarriageStatusField.LoadUnboundValue (person.MarriageStatus, IsPostBack);
    DisabledUnboundMarriageStatusField.Property = (Remotion.ObjectBinding.IBusinessObjectEnumerationProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("MarriageStatus");
    DisabledUnboundMarriageStatusField.LoadUnboundValue (person.MarriageStatus, IsPostBack);
    DisabledUnboundReadOnlyMarriageStatusField.Property = (Remotion.ObjectBinding.IBusinessObjectEnumerationProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("MarriageStatus");
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
    GenderField.Value = Gender.UnknownGender;
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
