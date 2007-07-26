using System;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using Rubicon.Collections;
using Rubicon.ObjectBinding.Sample;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.UI.Controls;

using Rubicon.ObjectBinding;

namespace OBWTest
{
public class CompleteBocUserControl : 
    System.Web.UI.UserControl,
    IFormGridRowProvider //  Provides new rows and rows to hide to the FormGridManager
{

  private AutoInitHashtable _listOfFormGridRowInfos = new AutoInitHashtable (typeof (FormGridRowInfoCollection));
  private AutoInitHashtable _listOfHiddenRows = new AutoInitHashtable (typeof (StringCollection));

  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue LastNameField;
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue TextField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue DateTimeField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue EnumField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocReferenceValue ReferenceField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocList ListField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocBooleanValue BooleanField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocMultilineTextValue MultilineTextField;
  protected Rubicon.Web.UI.Controls.TabbedMultiView MultiView;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;

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

    CurrentObject.BusinessObject = (IBusinessObject) person;
    CurrentObject.LoadValues (IsPostBack);

    if (! IsPostBack)
    {
      IBusinessObjectWithIdentity[] objects = (IBusinessObjectWithIdentity[]) ArrayUtility.Convert (
          XmlReflectionBusinessObjectStorageProvider.Current.GetObjects (typeof (Person)), typeof (IBusinessObjectWithIdentity));
      ReferenceField.SetBusinessObjectList (objects);
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

    FormGridRowInfoCollection newRows = (FormGridRowInfoCollection)_listOfFormGridRowInfos[FormGrid];

    BocTextValue incomeField = new BocTextValue();
    incomeField.ID = "IncomeField";
    incomeField.DataSourceControl = CurrentObject.ID;
    incomeField.PropertyIdentifier = "Income";
    incomeField.Visible = false;
    //  A new row
    newRows.Add (new FormGridRowInfo(
        incomeField, 
        FormGridRowInfo.RowType.ControlInRowWithLabel, 
        BooleanField.ID, 
        FormGridRowInfo.RowPosition.AfterRowWithID));

    InitalizeReferenceFieldMenuItems ();
  }

  private void InitalizeReferenceFieldMenuItems()
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
    ReferenceField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Text = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    ReferenceField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Cut";
    menuItem.Text = "Cut";
    menuItem.Category = "Edit";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    ReferenceField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    ReferenceField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    ReferenceField.OptionsMenuItems.Add (menuItem);
  }


  private void SaveButton_Click(object sender, System.EventArgs e)
  {
    bool isValid = FormGridManager.Validate();
    if (isValid)
    {
      CurrentObject.SaveValues (false);
      Person person = (Person) CurrentObject.BusinessObject;
      person.SaveObject();
    }
  }

  public virtual StringCollection GetHiddenRows (HtmlTable table)
  {
    return (StringCollection) _listOfHiddenRows[table];
  }

  public virtual FormGridRowInfoCollection GetAdditionalRows (HtmlTable table)
  {
    return (FormGridRowInfoCollection) _listOfFormGridRowInfos[table];
  }

	#region Web Form Designer generated code
	
	/// <summary>
	///		Required method for Designer support - do not modify
	///		the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion
	}
}
