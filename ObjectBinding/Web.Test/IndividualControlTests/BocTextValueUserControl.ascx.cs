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

public class BocTextValueUserControl : BaseUserControl
{
  protected System.Web.UI.HtmlControls.HtmlGenericControl NonVisualControls;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected System.Web.UI.WebControls.Label FirstNameFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue ReadOnlyFirstNameField;
  protected System.Web.UI.WebControls.Label ReadOnlyFirstNameFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue UnboundFirstNameField;
  protected System.Web.UI.WebControls.Label UnboundFirstNameFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue UnboundReadOnlyFirstNameField;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyFirstNameFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue IncomeField;
  protected System.Web.UI.WebControls.Label Label1;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue HeightField;
  protected System.Web.UI.WebControls.Label Label4;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue DateOfBirthField;
  protected System.Web.UI.WebControls.Label Label2;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue DateOfDeathField;
  protected System.Web.UI.WebControls.Label Label3;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue DisabledFirstNameField;
  protected System.Web.UI.WebControls.Label DisabledFirstNameFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue DisabledReadOnlyFirstNameField;
  protected System.Web.UI.WebControls.Label DisabledReadOnlyFirstNameFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue DisabledUnboundFirstNameField;
  protected System.Web.UI.WebControls.Label DisabledUnboundFirstNameFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue DisabledUnboundReadOnlyFirstNameField;
  protected System.Web.UI.WebControls.Label DisabledUnboundReadOnlyFirstNameFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue1;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue2;
  protected Rubicon.Web.UI.Controls.WebButton FirstNameTestSetNullButton;
  protected Rubicon.Web.UI.Controls.WebButton FirstNameTestSetNewValueButton;
  protected System.Web.UI.WebControls.Label FirstNameFieldTextChangedLabel;
  protected Rubicon.Web.UI.Controls.WebButton ReadOnlyFirstNameTestSetNullButton;
  protected Rubicon.Web.UI.Controls.WebButton ReadOnlyFirstNameTestSetNewValueButton;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();
 
    this.FirstNameField.TextChanged += new System.EventHandler(this.FirstNameField_TextChanged);
    this.FirstNameTestSetNullButton.Click += new System.EventHandler(this.FirstNameTestSetNullButton_Click);
    this.FirstNameTestSetNewValueButton.Click += new System.EventHandler(this.FirstNameTestSetNewValueButton_Click);
    this.ReadOnlyFirstNameTestSetNullButton.Click += new System.EventHandler(this.ReadOnlyFirstNameTestSetNullButton_Click);
    this.ReadOnlyFirstNameTestSetNewValueButton.Click += new System.EventHandler(this.ReadOnlyFirstNameTestSetNewValueButton_Click);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
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

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    Person person = (Person) CurrentObject.BusinessObject;

    UnboundFirstNameField.Property = (Rubicon.ObjectBinding.IBusinessObjectStringProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("FirstName");
    //UnboundFirstNameField.LoadUnboundValue (person.FirstName, IsPostBack);
    UnboundReadOnlyFirstNameField.Property = (Rubicon.ObjectBinding.IBusinessObjectStringProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("FirstName");
    UnboundReadOnlyFirstNameField.LoadUnboundValue (person.FirstName, IsPostBack);
    DisabledUnboundFirstNameField.Property = (Rubicon.ObjectBinding.IBusinessObjectStringProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("FirstName");
    DisabledUnboundFirstNameField.LoadUnboundValue (person.FirstName, IsPostBack);
    DisabledUnboundReadOnlyFirstNameField.Property = (Rubicon.ObjectBinding.IBusinessObjectStringProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("FirstName");
    DisabledUnboundReadOnlyFirstNameField.LoadUnboundValue (person.FirstName, IsPostBack);

    if (!IsPostBack)
    {
      if (Page is ISmartNavigablePage)
        ((ISmartNavigablePage) Page).SetFocus (FirstNameField);
    }
  }

  override protected void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    SetDebugLabel (FirstNameField, FirstNameFieldValueLabel);
    SetDebugLabel (ReadOnlyFirstNameField, ReadOnlyFirstNameFieldValueLabel);
    SetDebugLabel (UnboundFirstNameField, UnboundFirstNameFieldValueLabel);
    SetDebugLabel (UnboundReadOnlyFirstNameField, UnboundReadOnlyFirstNameFieldValueLabel);
    SetDebugLabel (DisabledFirstNameField, DisabledFirstNameFieldValueLabel);
    SetDebugLabel (DisabledReadOnlyFirstNameField, DisabledReadOnlyFirstNameFieldValueLabel);
    SetDebugLabel (DisabledUnboundFirstNameField, DisabledUnboundFirstNameFieldValueLabel);
    SetDebugLabel (DisabledUnboundReadOnlyFirstNameField, DisabledUnboundReadOnlyFirstNameFieldValueLabel);
  }

  private void SetDebugLabel (IBusinessObjectBoundWebControl control, Label label)
  {
   if (control.Value != null)
      label.Text = control.Value.ToString();
    else
      label.Text = "not set";
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
