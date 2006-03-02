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

public class BocCheckBoxUserControl : BaseUserControl
{
  protected System.Web.UI.HtmlControls.HtmlGenericControl NonVisualControls;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue LastNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocCheckBox DeceasedField;
  protected System.Web.UI.WebControls.Label DeceasedFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocCheckBox ReadOnlyDeceasedField;
  protected System.Web.UI.WebControls.Label ReadOnlyDeceasedFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocCheckBox UnboundDeceasedField;
  protected System.Web.UI.WebControls.Label UnboundDeceasedFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocCheckBox UnboundReadOnlyDeceasedField;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyDeceasedFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocCheckBox DisabledDeceasedField;
  protected System.Web.UI.WebControls.Label DisabledDeceasedFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocCheckBox DisabledReadOnlyDeceasedField;
  protected System.Web.UI.WebControls.Label DisabledReadOnlyDeceasedFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocCheckBox DisabledUnboundDeceasedField;
  protected System.Web.UI.WebControls.Label DisabledUnboundDeceasedFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocCheckBox DisabledUnboundReadOnlyDeceasedField;
  protected System.Web.UI.WebControls.Label DisabledUnboundReadOnlyDeceasedFieldValueLabel;
  protected System.Web.UI.WebControls.Label DeceasedFieldCheckedChangedLabel;
  protected Rubicon.Web.UI.Controls.WebButton DeceasedTestSetNullButton;
  protected Rubicon.Web.UI.Controls.WebButton DeceasedTestToggleValueButton;
  protected Rubicon.Web.UI.Controls.WebButton ReadOnlyDeceasedTestSetNullButton;
  protected Rubicon.Web.UI.Controls.WebButton ReadOnlyDeceasedTestToggleValueButton;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl CurrentObject;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    this.DeceasedField.CheckedChanged += new System.EventHandler(this.DeceasedField_CheckedChanged);
    this.DeceasedTestSetNullButton.Click += new System.EventHandler(this.DeceasedTestSetNullButton_Click);
    this.DeceasedTestToggleValueButton.Click += new System.EventHandler(this.DeceasedTestToggleValueButton_Click);
    this.ReadOnlyDeceasedTestSetNullButton.Click += new System.EventHandler(this.ReadOnlyDeceasedTestSetNullButton_Click);
    this.ReadOnlyDeceasedTestToggleValueButton.Click += new System.EventHandler(this.ReadOnlyDeceasedTestToggleValueButton_Click);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    Person person = (Person) CurrentObject.BusinessObject;

    UnboundDeceasedField.Property = (Rubicon.ObjectBinding.IBusinessObjectBooleanProperty) person.GetBusinessObjectProperty("Deceased");
    //UnboundDeceasedField.LoadUnboundValue (person.Deceased, IsPostBack);
    UnboundReadOnlyDeceasedField.Property = (Rubicon.ObjectBinding.IBusinessObjectBooleanProperty) person.GetBusinessObjectProperty("Deceased");
    UnboundReadOnlyDeceasedField.LoadUnboundValue (person.Deceased, IsPostBack);
    DisabledUnboundDeceasedField.Property = (Rubicon.ObjectBinding.IBusinessObjectBooleanProperty) person.GetBusinessObjectProperty("Deceased");
    DisabledUnboundDeceasedField.LoadUnboundValue (person.Deceased, IsPostBack);
    DisabledUnboundReadOnlyDeceasedField.Property = (Rubicon.ObjectBinding.IBusinessObjectBooleanProperty) person.GetBusinessObjectProperty("Deceased");
    DisabledUnboundReadOnlyDeceasedField.LoadUnboundValue (person.Deceased, IsPostBack);

    if (! IsPostBack)
    {
      if (Page is ISmartNavigablePage)
        ((ISmartNavigablePage) Page).SetFocus (DeceasedField);
    }
  }

  override protected void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    SetDebugLabel (DeceasedField, DeceasedFieldValueLabel);
    SetDebugLabel (ReadOnlyDeceasedField, ReadOnlyDeceasedFieldValueLabel);
    SetDebugLabel (UnboundDeceasedField, UnboundDeceasedFieldValueLabel);
    SetDebugLabel (UnboundReadOnlyDeceasedField, UnboundReadOnlyDeceasedFieldValueLabel);
    SetDebugLabel (DisabledDeceasedField, DisabledDeceasedFieldValueLabel);
    SetDebugLabel (DisabledReadOnlyDeceasedField, DisabledReadOnlyDeceasedFieldValueLabel);
    SetDebugLabel (DisabledUnboundDeceasedField, DisabledUnboundDeceasedFieldValueLabel);
    SetDebugLabel (DisabledUnboundReadOnlyDeceasedField, DisabledUnboundReadOnlyDeceasedFieldValueLabel);
  }

  private void SetDebugLabel (IBusinessObjectBoundWebControl control, Label label)
  {
   if (control.Value != null)
      label.Text = control.Value.ToString();
    else
      label.Text = "not set";
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
