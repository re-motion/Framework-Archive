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
using Rubicon.Utilities;
using OBRTest;

namespace OBWTest
{

  public class SingleBocMultilineTextValueForm : SingleBocTestWxeBasePage

{
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue CVField;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue ReadOnlyCVField;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue UnboundCVField;
  protected System.Web.UI.WebControls.Button CVTestSetNullButton;
  protected System.Web.UI.WebControls.Button CVTestSetNewValueButton;
  protected System.Web.UI.WebControls.Button ReadOnlyCVTestSetNullButton;
  protected System.Web.UI.WebControls.Button ReadOnlyCVTestSetNewValueButton;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue LastNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
    protected System.Web.UI.WebControls.Label UnboundCVFieldValueLabel;
    protected System.Web.UI.WebControls.Label CVFieldValueLabel;
    protected System.Web.UI.WebControls.Label ReadOnlyCVFieldValueLabel;
    protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue UnboundReadOnlyCVField;
    protected System.Web.UI.WebControls.Label UnboundReadOnlyCVFieldValueLabel;
    protected System.Web.UI.WebControls.Label CVFieldTextChangedLabel;
    protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

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
    this.UnboundCVField.Property = (Rubicon.ObjectBinding.IBusinessObjectStringProperty) person.GetBusinessObjectProperty("CV");
    //this.UnboundCVField.Value = person.CV;
    this.UnboundReadOnlyCVField.Property = (Rubicon.ObjectBinding.IBusinessObjectStringProperty) person.GetBusinessObjectProperty("CV");
    this.UnboundReadOnlyCVField.Value = person.CV;
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
    this.CVField.TextChanged += new System.EventHandler(this.CVField_TextChanged);
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.CVTestSetNullButton.Click += new System.EventHandler(this.CVTestSetNullButton_Click);
    this.CVTestSetNewValueButton.Click += new System.EventHandler(this.CVTestSetNewValueButton_Click);
    this.ReadOnlyCVTestSetNullButton.Click += new System.EventHandler(this.ReadOnlyCVTestSetNullButton_Click);
    this.ReadOnlyCVTestSetNewValueButton.Click += new System.EventHandler(this.ReadOnlyCVTestSetNewValueButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  protected override void OnPreRender(EventArgs e)
  {
    if (CVField.Value != null)
      CVFieldValueLabel.Text = StringUtility.ConcatWithSeperator (CVField.Value, "<br>");
    else
      CVFieldValueLabel.Text = "not set";
    
    if (ReadOnlyCVField.Value != null)
      ReadOnlyCVFieldValueLabel.Text = StringUtility.ConcatWithSeperator (ReadOnlyCVField.Value, "<br>");
    else
      ReadOnlyCVFieldValueLabel.Text = "not set";

    if (UnboundCVField.Value != null)
      UnboundCVFieldValueLabel.Text = StringUtility.ConcatWithSeperator (UnboundCVField.Value, "<br>");
    else
      UnboundCVFieldValueLabel.Text = "not set";

   if (UnboundReadOnlyCVField.Value != null)
      UnboundReadOnlyCVFieldValueLabel.Text = StringUtility.ConcatWithSeperator (UnboundReadOnlyCVField.Value, "<br>");
    else
      UnboundReadOnlyCVFieldValueLabel.Text = "not set";

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
      CVFieldTextChangedLabel.Text = StringUtility.ConcatWithSeperator (CVField.Value, "<br>");
    else
      CVFieldTextChangedLabel.Text = "not set";
  }
}

}
