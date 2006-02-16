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

public class BocMultilineTextValueUserControl : BaseUserControl
{
  protected System.Web.UI.HtmlControls.HtmlGenericControl NonVisualControls;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue LastNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocMultilineTextValue CVField;
  protected System.Web.UI.WebControls.Label CVFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocMultilineTextValue ReadOnlyCVField;
  protected System.Web.UI.WebControls.Label ReadOnlyCVFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocMultilineTextValue UnboundCVField;
  protected System.Web.UI.WebControls.Label UnboundCVFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocMultilineTextValue UnboundReadOnlyCVField;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyCVFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocMultilineTextValue DisabledCVField;
  protected System.Web.UI.WebControls.Label DisabledCVFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocMultilineTextValue DisabledReadOnlyCVField;
  protected System.Web.UI.WebControls.Label DisabledReadOnlyCVFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocMultilineTextValue DisabledUnboundCVField;
  protected System.Web.UI.WebControls.Label DisabledUnboundCVFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocMultilineTextValue DisabledUnboundReadOnlyCVField;
  protected System.Web.UI.WebControls.Label DisabledUnboundReadOnlyCVFieldValueLabel;
  protected System.Web.UI.WebControls.Label CVFieldTextChangedLabel;
  protected Rubicon.Web.UI.Controls.WebButton CVTestSetNullButton;
  protected Rubicon.Web.UI.Controls.WebButton CVTestSetNewValueButton;
  protected Rubicon.Web.UI.Controls.WebButton ReadOnlyCVTestSetNullButton;
  protected Rubicon.Web.UI.Controls.WebButton ReadOnlyCVTestSetNewValueButton;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl CurrentObject;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    Load += new EventHandler(BocMultilineTextValueUserControl_Load);
    PreRender += new EventHandler(BocMultilineTextValueUserControl_PreRender);
 
    this.CVField.TextChanged += new System.EventHandler(this.CVField_TextChanged);
    this.CVTestSetNullButton.Click += new System.EventHandler(this.CVTestSetNullButton_Click);
    this.CVTestSetNewValueButton.Click += new System.EventHandler(this.CVTestSetNewValueButton_Click);
    this.ReadOnlyCVTestSetNullButton.Click += new System.EventHandler(this.ReadOnlyCVTestSetNullButton_Click);
    this.ReadOnlyCVTestSetNewValueButton.Click += new System.EventHandler(this.ReadOnlyCVTestSetNewValueButton_Click);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  private void BocMultilineTextValueUserControl_Load(object sender, EventArgs e)
  {
    Person person = (Person) CurrentObject.BusinessObject;

    UnboundCVField.Property = (Rubicon.ObjectBinding.IBusinessObjectStringProperty) person.GetBusinessObjectProperty("CV");
    //UnboundCVField.LoadUnboundValue (person.CV, IsPostBack);
    UnboundReadOnlyCVField.Property = (Rubicon.ObjectBinding.IBusinessObjectStringProperty) person.GetBusinessObjectProperty("CV");
    UnboundReadOnlyCVField.LoadUnboundValue (person.CV, IsPostBack);
    DisabledUnboundCVField.Property = (Rubicon.ObjectBinding.IBusinessObjectStringProperty) person.GetBusinessObjectProperty("CV");
    DisabledUnboundCVField.LoadUnboundValue (person.CV, IsPostBack);
    DisabledUnboundReadOnlyCVField.Property = (Rubicon.ObjectBinding.IBusinessObjectStringProperty) person.GetBusinessObjectProperty("CV");
    DisabledUnboundReadOnlyCVField.LoadUnboundValue (person.CV, IsPostBack);
    
    if (!IsPostBack)
    {
      if (Page is ISmartNavigablePage)
        ((ISmartNavigablePage) Page).SetFocus (CVField);
    }
  }

  private void BocMultilineTextValueUserControl_PreRender(object sender, EventArgs e)
  {
    SetDebugLabel (CVField, CVFieldValueLabel);
    SetDebugLabel (ReadOnlyCVField, ReadOnlyCVFieldValueLabel);
    SetDebugLabel (UnboundCVField, UnboundCVFieldValueLabel);
    SetDebugLabel (UnboundReadOnlyCVField, UnboundReadOnlyCVFieldValueLabel);
    SetDebugLabel (DisabledCVField, DisabledCVFieldValueLabel);
    SetDebugLabel (DisabledReadOnlyCVField, DisabledReadOnlyCVFieldValueLabel);
    SetDebugLabel (DisabledUnboundCVField, DisabledUnboundCVFieldValueLabel);
    SetDebugLabel (DisabledUnboundReadOnlyCVField, DisabledUnboundReadOnlyCVFieldValueLabel);
  }

  private void SetDebugLabel (IBusinessObjectBoundWebControl control, Label label)
  {
   if (control.Value != null)
      label.Text = control.Value.ToString();
    else
      label.Text = "not set";
  }

  private void CVTestSetNullButton_Click(object sender, System.EventArgs e)
  {
    CVField.Value = null;
  }

  private void CVTestSetNewValueButton_Click(object sender, System.EventArgs e)
  {
    CVField.Value = new string[] {"Foo", "Bar"};
  }

  private void ReadOnlyCVTestSetNullButton_Click(object sender, System.EventArgs e)
  {
    ReadOnlyCVField.Value = null;
  }

  private void ReadOnlyCVTestSetNewValueButton_Click(object sender, System.EventArgs e)
  {
    ReadOnlyCVField.Value = new string[] {"Foo", "Bar"};
  }

  private void CVField_TextChanged(object sender, System.EventArgs e)
  {
    if (CVField.Value != null)
      CVFieldTextChangedLabel.Text = StringUtility.ConcatWithSeparator (CVField.Value, "<br>");
    else
      CVFieldTextChangedLabel.Text = "not set";
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
