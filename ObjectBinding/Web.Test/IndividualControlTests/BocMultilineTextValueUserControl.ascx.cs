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

public class BocMultilineTextValueUserControl : BaseUserControl
{
  protected System.Web.UI.HtmlControls.HtmlGenericControl NonVisualControls;
  protected Remotion.Web.UI.Controls.FormGridManager FormGridManager;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue LastNameField;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue CVField;
  protected System.Web.UI.WebControls.Label CVFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue ReadOnlyCVField;
  protected System.Web.UI.WebControls.Label ReadOnlyCVFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue UnboundCVField;
  protected System.Web.UI.WebControls.Label UnboundCVFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue UnboundReadOnlyCVField;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyCVFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue DisabledCVField;
  protected System.Web.UI.WebControls.Label DisabledCVFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue DisabledReadOnlyCVField;
  protected System.Web.UI.WebControls.Label DisabledReadOnlyCVFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue DisabledUnboundCVField;
  protected System.Web.UI.WebControls.Label DisabledUnboundCVFieldValueLabel;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue DisabledUnboundReadOnlyCVField;
  protected System.Web.UI.WebControls.Label DisabledUnboundReadOnlyCVFieldValueLabel;
  protected System.Web.UI.WebControls.Label CVFieldTextChangedLabel;
  protected Remotion.Web.UI.Controls.WebButton CVTestSetNullButton;
  protected Remotion.Web.UI.Controls.WebButton CVTestSetNewValueButton;
  protected Remotion.Web.UI.Controls.WebButton ReadOnlyCVTestSetNullButton;
  protected Remotion.Web.UI.Controls.WebButton ReadOnlyCVTestSetNewValueButton;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();
 
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

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    Person person = (Person) CurrentObject.BusinessObject;

    UnboundCVField.Property = (Remotion.ObjectBinding.IBusinessObjectStringProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("CV");
    //UnboundCVField.LoadUnboundValue (person.CV, IsPostBack);
    UnboundReadOnlyCVField.Property = (Remotion.ObjectBinding.IBusinessObjectStringProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("CV");
    UnboundReadOnlyCVField.LoadUnboundValue (person.CV, IsPostBack);
    DisabledUnboundCVField.Property = (Remotion.ObjectBinding.IBusinessObjectStringProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("CV");
    DisabledUnboundCVField.LoadUnboundValue (person.CV, IsPostBack);
    DisabledUnboundReadOnlyCVField.Property = (Remotion.ObjectBinding.IBusinessObjectStringProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("CV");
    DisabledUnboundReadOnlyCVField.LoadUnboundValue (person.CV, IsPostBack);
    
    if (!IsPostBack)
    {
      if (Page is ISmartNavigablePage)
        ((ISmartNavigablePage) Page).SetFocus (CVField);
    }
  }

  override protected void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    SetDebugLabel (CVField, CVFieldValueLabel);
    SetDebugLabel (ReadOnlyCVField, ReadOnlyCVFieldValueLabel);
    SetDebugLabel (UnboundCVField, UnboundCVFieldValueLabel);
    SetDebugLabel (UnboundReadOnlyCVField, UnboundReadOnlyCVFieldValueLabel);
    SetDebugLabel (DisabledCVField, DisabledCVFieldValueLabel);
    SetDebugLabel (DisabledReadOnlyCVField, DisabledReadOnlyCVFieldValueLabel);
    SetDebugLabel (DisabledUnboundCVField, DisabledUnboundCVFieldValueLabel);
    SetDebugLabel (DisabledUnboundReadOnlyCVField, DisabledUnboundReadOnlyCVFieldValueLabel);
  }

  private void SetDebugLabel (BocMultilineTextValue control, Label label)
  {
   if (control.Value != null)
      label.Text = StringUtility.ConcatWithSeparator (control.Value, "<br>");
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
