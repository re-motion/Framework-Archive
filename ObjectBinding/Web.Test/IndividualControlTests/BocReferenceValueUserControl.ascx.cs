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

[WebMultiLingualResources ("OBWTest.Globalization.IndividualControlTests.BocReferenceValueUserControl")]
public class BocReferenceValueUserControl : BaseUserControl
{
  protected System.Web.UI.HtmlControls.HtmlGenericControl NonVisualControls;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue LastNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocReferenceValue PartnerField;
  protected System.Web.UI.WebControls.Label PartnerFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocReferenceValue ReadOnlyPartnerField;
  protected System.Web.UI.WebControls.Label ReadOnlyPartnerFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocReferenceValue UnboundPartnerField;
  protected System.Web.UI.WebControls.Label UnboundPartnerFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocReferenceValue UnboundReadOnlyPartnerField;
  protected System.Web.UI.WebControls.Label UnboundReadOnlyPartnerFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocReferenceValue DisabledPartnerField;
  protected System.Web.UI.WebControls.Label DisabledPartnerFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocReferenceValue DisabledReadOnlyPartnerField;
  protected System.Web.UI.WebControls.Label DisabledReadOnlyPartnerFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocReferenceValue DisabledUnboundPartnerField;
  protected System.Web.UI.WebControls.Label DisabledUnboundPartnerFieldValueLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocReferenceValue DisabledUnboundReadOnlyPartnerField;
  protected System.Web.UI.WebControls.Label DisabledUnboundReadOnlyPartnerFieldValueLabel;
  protected System.Web.UI.WebControls.Label PartnerFieldSelectionChangedLabel;
  protected System.Web.UI.WebControls.Label PartnerFieldMenuClickEventArgsLabel;
  protected Rubicon.Web.UI.Controls.WebButton PartnerTestSetNullButton;
  protected Rubicon.Web.UI.Controls.WebButton PartnerTestSetNewItemButton;
  protected Rubicon.Web.UI.Controls.WebButton ReadOnlyPartnerTestSetNullButton;
  protected Rubicon.Web.UI.Controls.WebButton ReadOnlyPartnerTestSetNewItemButton;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected System.Web.UI.WebControls.Label PartnerCommandClickLabel;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    PartnerField.CommandClick += new BocCommandClickEventHandler (PartnerField_CommandClick);
    PartnerField.MenuItemClick += new WebMenuItemClickEventHandler (PartnerField_MenuItemClick);
    PartnerField.SelectionChanged += new EventHandler (PartnerField_SelectionChanged);
    ReadOnlyPartnerField.CommandClick += new BocCommandClickEventHandler (ReadOnlyPartnerField_CommandClick);
    PartnerTestSetNullButton.Click += new EventHandler (PartnerTestSetNullButton_Click);
    PartnerTestSetNewItemButton.Click += new EventHandler (PartnerTestSetNewItemButton_Click);
    ReadOnlyPartnerTestSetNullButton.Click += new EventHandler (ReadOnlyPartnerTestSetNullButton_Click);
    ReadOnlyPartnerTestSetNewItemButton.Click += new EventHandler (ReadOnlyPartnerTestSetNewItemButton_Click);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

	override protected void OnInit(EventArgs e)
	{
		InitializeComponent();

    base.OnInit (e);

    WebMenuItem menuItem = new WebMenuItem();
    menuItem.ItemID = "webmenuitem";
    menuItem.Text = "webmenuitem";
    PartnerField.OptionsMenuItems.Add (menuItem);

    InitalizeReferenceValueMenuItems (PartnerField);
    InitalizeReferenceValueMenuItems (ReadOnlyPartnerField);
    InitalizeReferenceValueMenuItems (UnboundPartnerField);
    InitalizeReferenceValueMenuItems (UnboundReadOnlyPartnerField);
    InitalizeReferenceValueMenuItems (DisabledPartnerField);
    InitalizeReferenceValueMenuItems (DisabledReadOnlyPartnerField);
    InitalizeReferenceValueMenuItems (DisabledUnboundPartnerField);
  }

  private void InitalizeReferenceValueMenuItems (BocReferenceValue referenceValue)
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.Text = "Invisible Item";
    menuItem.IsVisible = false;
    referenceValue.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Open";
    menuItem.Text = "Open";
    menuItem.Category = "Object";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.Parameters = "objects";
    menuItem.Command.WxeFunctionCommand.MappingID = "ViewPersons";
    referenceValue.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Text = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    referenceValue.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Cut";
    menuItem.Text = "Cut";
    menuItem.Category = "Edit";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    referenceValue.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    referenceValue.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    referenceValue.OptionsMenuItems.Add (menuItem);
  
    menuItem = new BocMenuItem();
    menuItem.Text = "Invisible Item";
    menuItem.IsVisible = false;
    referenceValue.OptionsMenuItems.Add (menuItem);
  }

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    Person person = (Person) CurrentObject.BusinessObject;

    if (! IsPostBack)
    {
      IBusinessObjectWithIdentity[] objects = (IBusinessObjectWithIdentity[]) ArrayUtility.Convert (
          XmlReflectionBusinessObjectStorageProvider.Current.GetObjects (typeof (Person)), typeof (IBusinessObjectWithIdentity));
      PartnerField.SetBusinessObjectList (objects);
      UnboundPartnerField.SetBusinessObjectList (objects);
      DisabledUnboundPartnerField.SetBusinessObjectList (objects);
    }

    UnboundPartnerField.Property = (Rubicon.ObjectBinding.IBusinessObjectReferenceProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Partner");
    //UnboundPartnerField.LoadUnboundValue (person.Partner, IsPostBack);
    UnboundReadOnlyPartnerField.Property = (Rubicon.ObjectBinding.IBusinessObjectReferenceProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Partner");
    UnboundReadOnlyPartnerField.LoadUnboundValue ((IBusinessObjectWithIdentity)person.Partner, IsPostBack);
    DisabledUnboundPartnerField.Property = (Rubicon.ObjectBinding.IBusinessObjectReferenceProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Partner");
    DisabledUnboundPartnerField.LoadUnboundValue ((IBusinessObjectWithIdentity) person.Partner, IsPostBack);
    DisabledUnboundReadOnlyPartnerField.Property = (Rubicon.ObjectBinding.IBusinessObjectReferenceProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Partner");
    DisabledUnboundReadOnlyPartnerField.LoadUnboundValue ((IBusinessObjectWithIdentity) person.Partner, IsPostBack);
  
    if (!IsPostBack)
    {
      if (Page is ISmartNavigablePage)
        ((ISmartNavigablePage) Page).SetFocus (PartnerField);
    }
  }

  override protected void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    SetDebugLabel (PartnerField, PartnerFieldValueLabel);
    SetDebugLabel (ReadOnlyPartnerField, ReadOnlyPartnerFieldValueLabel);
    SetDebugLabel (UnboundPartnerField, UnboundPartnerFieldValueLabel);
    SetDebugLabel (UnboundReadOnlyPartnerField, UnboundReadOnlyPartnerFieldValueLabel);
    SetDebugLabel (DisabledPartnerField, DisabledPartnerFieldValueLabel);
    SetDebugLabel (DisabledReadOnlyPartnerField, DisabledReadOnlyPartnerFieldValueLabel);
    SetDebugLabel (DisabledUnboundPartnerField, DisabledUnboundPartnerFieldValueLabel);
    SetDebugLabel (DisabledUnboundReadOnlyPartnerField, DisabledUnboundReadOnlyPartnerFieldValueLabel);
  }

  private void SetDebugLabel (IBusinessObjectBoundWebControl control, Label label)
  {
   if (control.Value != null)
      label.Text = control.Value.ToString();
    else
      label.Text = "not set";
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

    PartnerField.Value = (IBusinessObjectWithIdentity) person;
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

    ReadOnlyPartnerField.Value = (IBusinessObjectWithIdentity) person;
  }

  private void PartnerField_CommandClick(object sender, BocCommandClickEventArgs e)
  {
    PartnerCommandClickLabel.Text = "PartnerField clicked";
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

  private void ReadOnlyPartnerField_CommandClick(object sender, Rubicon.ObjectBinding.Web.UI.Controls.BocCommandClickEventArgs e)
  {
    PartnerCommandClickLabel.Text = "ReadOnlyPartnerField clicked";
  }

	#region Web Form Designer generated code
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
