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
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;
using OBRTest;

namespace OBWTest
{

public class SingleBocReferenceValueForm: SingleBocTestWxeBasePage
{
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Web.Controls.BocReferenceValue PartnerField;
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected System.Web.UI.WebControls.Button PartnerTestSetNullButton;
  protected System.Web.UI.WebControls.Button PartnerTestSetNewItemButton;
  protected System.Web.UI.WebControls.Button ReadOnlyPartnerTestSetNullButton;
  protected System.Web.UI.WebControls.Button ReadOnlyPartnerTestSetNewItemButton;
  protected Rubicon.ObjectBinding.Web.Controls.BocReferenceValue UnboundReadOnlyPartnerField;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected System.Web.UI.WebControls.Label ReadOnlyPartnerFieldValueLabel;
  protected System.Web.UI.WebControls.Label UnboundPartnerFieldValueLabel;
  protected System.Web.UI.WebControls.Label PartnerFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.Controls.BocReferenceValue ReadOnlyPartnerField;
  protected Rubicon.ObjectBinding.Web.Controls.BocReferenceValue UnboundPartnerField;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyPartnerFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue LastNameField;
  protected System.Web.UI.WebControls.Label PartnerMenuEventArgsLabel;
  protected System.Web.UI.WebControls.Label PartnerFieldMenuClickEventArgsLabel;
  protected Rubicon.Web.UI.Controls.ValidationStateViewer ValidationStateViewer1;
  protected System.Web.UI.WebControls.Label PartnerFieldSelectionChangedLabel;

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
    
    if (! IsPostBack)
    {
      IBusinessObjectWithIdentity[] objects = ReflectionBusinessObjectStorage.GetObjects (person.GetType());
      PartnerField.RefreshBusinessObjectList (objects);
      UnboundPartnerField.RefreshBusinessObjectList (objects);
    }

    this.UnboundPartnerField.Property = (Rubicon.ObjectBinding.IBusinessObjectReferenceProperty) person.GetBusinessObjectProperty("Partner");
    //this.UnboundPartnerField.Value = person.Partner;
    this.UnboundReadOnlyPartnerField.Property = (Rubicon.ObjectBinding.IBusinessObjectReferenceProperty) person.GetBusinessObjectProperty("Partner");
    this.UnboundReadOnlyPartnerField.Value = person.Partner;
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
    this.PartnerField.MenuItemClick += new Rubicon.Web.UI.Controls.WebMenuItemClickEventHandler(this.PartnerField_MenuItemClick);
    this.PartnerField.SelectionChanged += new System.EventHandler(this.PartnerField_SelectionChanged);
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.PartnerTestSetNullButton.Click += new System.EventHandler(this.PartnerTestSetNullButton_Click);
    this.PartnerTestSetNewItemButton.Click += new System.EventHandler(this.PartnerTestSetNewItemButton_Click);
    this.ReadOnlyPartnerTestSetNullButton.Click += new System.EventHandler(this.ReadOnlyPartnerTestSetNullButton_Click);
    this.ReadOnlyPartnerTestSetNewItemButton.Click += new System.EventHandler(this.ReadOnlyPartnerTestSetNewItemButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  protected override void OnPreRender(EventArgs e)
  {
    if (PartnerField.Value != null)
      PartnerFieldValueLabel.Text = PartnerField.Value.ToString();
    else
      PartnerFieldValueLabel.Text = "not set";
    
    if (ReadOnlyPartnerField.Value != null)
      ReadOnlyPartnerFieldValueLabel.Text = ReadOnlyPartnerField.Value.ToString();
    else
      ReadOnlyPartnerFieldValueLabel.Text = "not set";

    if (UnboundPartnerField.Value != null)
      UnboundPartnerFieldValueLabel.Text = UnboundPartnerField.Value.ToString();
    else
      UnboundPartnerFieldValueLabel.Text = "not set";

   if (UnboundReadOnlyPartnerField.Value != null)
      UnboundReadOnlyPartnerFieldValueLabel.Text = UnboundReadOnlyPartnerField.Value.ToString();
    else
      UnboundReadOnlyPartnerFieldValueLabel.Text = "not set";

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
      if (person.Partner != null)
        person.Partner.SaveObject();
    }
    Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectStorage.Reset();
  }

  private void PartnerTestSetNullButton_Click(object sender, System.EventArgs e)
  {
    PartnerField.Value = null;
  }

  private void PartnerTestSetNewItemButton_Click(object sender, System.EventArgs e)
  {
    Person person = Person.CreateObject (Guid.NewGuid());
    person.LastName = person.ID.ToByteArray()[15].ToString();
    person.FirstName = "--";

    PartnerField.Value = person;
  }

  private void ReadOnlyPartnerTestSetNullButton_Click(object sender, System.EventArgs e)
  {
    ReadOnlyPartnerField.Value = null;
  }

  private void ReadOnlyPartnerTestSetNewItemButton_Click(object sender, System.EventArgs e)
  {
    Person person = Person.CreateObject (Guid.NewGuid());
    person.LastName = person.ID.ToByteArray()[15].ToString();
    person.FirstName = "--";

    ReadOnlyPartnerField.Value = person;
  }

  private void PartnerField_SelectionChanged(object sender, System.EventArgs e)
  {
    if (PartnerField.Value != null)
      PartnerFieldSelectionChangedLabel.Text = PartnerField.Value.ToString();
    else
      PartnerFieldSelectionChangedLabel.Text = "not set";
  }

  private void PartnerField_MenuItemClick(object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
  {
    PartnerFieldMenuClickEventArgsLabel.Text = e.Item.Text;
  }
}

}
