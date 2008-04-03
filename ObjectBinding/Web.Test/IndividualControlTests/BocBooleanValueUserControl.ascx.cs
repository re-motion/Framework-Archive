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

public class BocBooleanValueUserControl : BaseUserControl
{
  protected System.Web.UI.HtmlControls.HtmlGenericControl NonVisualControls;
  protected Remotion.Web.UI.Controls.FormGridManager FormGridManager;
  protected System.Web.UI.WebControls.Label DeceasedFieldValueLabel;
  protected System.Web.UI.WebControls.Label ReadOnlyDeceasedFieldValueLabel;
  protected System.Web.UI.WebControls.Label UnboundDeceasedFieldValueLabel;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyDeceasedFieldValueLabel;
  protected System.Web.UI.WebControls.Label DisabledDeceasedFieldValueLabel;
  protected System.Web.UI.WebControls.Label DisabledReadOnlyDeceasedFieldValueLabel;
  protected System.Web.UI.WebControls.Label DisabledUnboundDeceasedFieldValueLabel;
  protected System.Web.UI.WebControls.Label DisabledUnboundReadOnlyDeceasedFieldValueLabel;
  protected System.Web.UI.WebControls.Label DeceasedFieldCheckedChangedLabel;
  protected Remotion.Web.UI.Controls.WebButton DeceasedTestSetNullButton;
  protected Remotion.Web.UI.Controls.WebButton DeceasedTestToggleValueButton;
  protected Remotion.Web.UI.Controls.WebButton ReadOnlyDeceasedTestSetNullButton;
  protected Remotion.Web.UI.Controls.WebButton ReadOnlyDeceasedTestToggleValueButton;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue LastNameField;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue DeceasedField;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue ReadOnlyDeceasedField;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue UnboundDeceasedField;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue UnboundReadOnlyDeceasedField;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue DisabledDeceasedField;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue DisabledReadOnlyDeceasedField;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue DisabledUnboundDeceasedField;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue DisabledUnboundReadOnlyDeceasedField;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    DeceasedField.CheckedChanged += new System.EventHandler (DeceasedField_CheckedChanged);
    DeceasedTestSetNullButton.Click += new System.EventHandler (DeceasedTestSetNullButton_Click);
    DeceasedTestToggleValueButton.Click += new System.EventHandler (DeceasedTestToggleValueButton_Click);
    ReadOnlyDeceasedTestSetNullButton.Click += new System.EventHandler (ReadOnlyDeceasedTestSetNullButton_Click);
    ReadOnlyDeceasedTestToggleValueButton.Click += new System.EventHandler (ReadOnlyDeceasedTestToggleValueButton_Click);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    Person person = (Person) CurrentObject.BusinessObject;

    UnboundDeceasedField.Property = (Remotion.ObjectBinding.IBusinessObjectBooleanProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Deceased");
    //UnboundDeceasedField.LoadUnboundValue (person.Deceased, IsPostBack);
    UnboundReadOnlyDeceasedField.Property = (Remotion.ObjectBinding.IBusinessObjectBooleanProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Deceased");
    UnboundReadOnlyDeceasedField.LoadUnboundValue (person.Deceased, IsPostBack);
    DisabledUnboundDeceasedField.Property = (Remotion.ObjectBinding.IBusinessObjectBooleanProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Deceased");
    DisabledUnboundDeceasedField.LoadUnboundValue (person.Deceased, IsPostBack);
    DisabledUnboundReadOnlyDeceasedField.Property = (Remotion.ObjectBinding.IBusinessObjectBooleanProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Deceased");
    DisabledUnboundReadOnlyDeceasedField.LoadUnboundValue (person.Deceased, IsPostBack);

    if (!IsPostBack)
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
