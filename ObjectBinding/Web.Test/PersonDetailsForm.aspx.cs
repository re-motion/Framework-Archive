using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using Rubicon.ObjectBinding.Sample;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.UI.Controls;


namespace OBWTest
{

public class PersonDetailsForm : SingleBocTestWxeBasePage

{
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue GenderField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocReferenceValue PartnerField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue BirthdayField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue LastNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocBooleanValue DeceasedField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocMultilineTextValue CVField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocList JobList;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BusinessObjectReferenceDataSourceControl PartnerDataSource;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue ParterFirstNameField;
  protected System.Web.UI.WebControls.Button PostBackButton;

	private void Page_Load(object sender, System.EventArgs e)
	{
    string id = (string) Variables["id"];
    Guid personID = Guid.Empty;
    if (! StringUtility.IsNullOrEmpty (id))
      personID = new Guid (id);

    Person person = Person.GetObject (personID);
    Person partner;
    if (person != null)
      partner = person.Partner;

    CurrentObject.BusinessObject = (IBusinessObjectWithIdentity) person;
    CurrentObject.LoadValues (IsPostBack);

    if (! IsPostBack)
    {
      IBusinessObjectWithIdentity[] objects = (IBusinessObjectWithIdentity[]) ArrayUtility.Convert (
          XmlReflectionBusinessObjectStorageProvider.Current.GetObjects (typeof (Person)), typeof (IBusinessObjectWithIdentity));
      PartnerField.SetBusinessObjectList (objects);
    }
	}

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    if (!IsPostBack)
      XmlReflectionBusinessObjectStorageProvider.Current.Reset();

    InitalizePartnerFieldMenuItems();
    InitalizeJobListMenuItems();
	}

  private void InitalizePartnerFieldMenuItems()
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Open";
    menuItem.Text = "Open";
    menuItem.Category = "Object";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.Parameters = "objects";
    menuItem.Command.WxeFunctionCommand.TypeName = "OBWTest.ViewPersonsWxeFunction,OBWTest";
    PartnerField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Text = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    PartnerField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Cut";
    menuItem.Text = "Cut";
    menuItem.Category = "Edit";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    PartnerField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    PartnerField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    PartnerField.OptionsMenuItems.Add (menuItem);
  }

  private void InitalizeJobListMenuItems()
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.IsDisabled = true;
    menuItem.Command.Type = CommandType.Event;
    JobList.ListMenuItems.Add (menuItem);
  
    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    JobList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.ExactlyOne;
    menuItem.Command.Type = CommandType.Event;
    JobList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Open";
    menuItem.Text = "Open";
    menuItem.Category = "Object";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.Parameters = "objects";
    menuItem.Command.WxeFunctionCommand.TypeName = "OBWTest.ViewPersonsWxeFunction,OBWTest";
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Text = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Cut";
    menuItem.Text = "Cut";
    menuItem.Category = "Edit";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Duplicate";
    menuItem.Text = "Duplicate";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    JobList.OptionsMenuItems.Add (menuItem);
  }

	#region Web Form Designer generated code
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.PartnerField.SelectionChanged += new System.EventHandler(this.PartnerField_SelectionChanged);
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SaveButton_Click(object sender, System.EventArgs e)
  {
    bool isValid = FormGridManager.Validate();
    if (isValid)
    {
      CurrentObject.SaveValues (false);
      Person person = (Person) CurrentObject.BusinessObject;
      person.SaveObject();
      if (person.Partner != null)
        person.Partner.SaveObject();
    }
    XmlReflectionBusinessObjectStorageProvider.Current.Reset();
  }

  private void PartnerField_SelectionChanged(object sender, System.EventArgs e)
  {
    PartnerField.SaveValue (false);
    PartnerDataSource.LoadValue (false);
  }
}

}