using System;
using System.Web;
using System.Web.UI.WebControls;

using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;

using OBRTest;

namespace OBWTest.IndividualControlTests
{

[WebMultiLingualResources ("OBWTest.Globalization.IndividualControlTests.BocListUserControl")]
public class BocListUserControl : BaseUserControl
{
  protected System.Web.UI.HtmlControls.HtmlTable Table3;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue LastNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocList JobList;
  protected OBRTest.TestBocList ChildrenList;
  protected OBRTest.TestBocList EmptyList;
  protected System.Web.UI.WebControls.Button ChildrenListEndEditModeButton;
  protected System.Web.UI.WebControls.Button ChildrenListAddAndEditButton;
  protected System.Web.UI.WebControls.CheckBox ChildrenListEventCheckBox;
  protected System.Web.UI.WebControls.Label ChildrenListEventArgsLabel;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl EmptyDataSourceControl;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected OBRTest.TestBocListValidator EmptyListValidator;
  protected OBRTest.TestBocList Testboclist1;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocList AllColumnsList;
  protected System.Web.UI.HtmlControls.HtmlGenericControl NonVisualControls;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl CurrentObject;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    Init += new EventHandler(BocListUserControl_Init);
    Load += new EventHandler (BocListUserControl_Load);

    ChildrenListAddAndEditButton.Click += new EventHandler(AddAndEditButton_Click);
    ChildrenListEndEditModeButton.Click += new EventHandler(ChildrenListEndEditModeButton_Click);
    
    ChildrenList.ListItemCommandClick += new Rubicon.ObjectBinding.Web.UI.Controls.BocListItemCommandClickEventHandler(this.ChildrenList_ListItemCommandClick);
    ChildrenList.MenuItemClick += new Rubicon.Web.UI.Controls.WebMenuItemClickEventHandler(this.ChildrenList_MenuItemClick);
    
    ChildrenList.DataRowRender += new Rubicon.ObjectBinding.Web.UI.Controls.BocListDataRowRenderEventHandler(this.ChildrenList_DataRowRender);
    
    ChildrenList.SavingEditedRow += new Rubicon.ObjectBinding.Web.UI.Controls.BocListRowEditModeEventHandler(this.ChildrenList_SavingEditedRow);
    ChildrenList.EditedRowSaved += new Rubicon.ObjectBinding.Web.UI.Controls.BocListItemEventHandler(this.ChildrenList_EditedRowSaved);
    
    ChildrenList.SortingOrderChanging += new Rubicon.ObjectBinding.Web.UI.Controls.BocListSortingOrderChangeEventHandler(this.ChildrenList_SortingOrderChanging);
    ChildrenList.SortingOrderChanged += new Rubicon.ObjectBinding.Web.UI.Controls.BocListSortingOrderChangeEventHandler(this.ChildrenList_SortingOrderChanged);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  private void BocListUserControl_Init(object sender, EventArgs e)
  {
   InitializeMenuItems();
  }

  private void InitializeMenuItems()
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Event";
    menuItem.Text = "Event";
    menuItem.Category = "PostBacks";
    menuItem.Command.Type = CommandType.Event;
    JobList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Enum.Href";
    menuItem.Text = "Href";
    menuItem.Category = "Links";
    menuItem.Style = WebMenuItemStyle.Text;
    menuItem.Command.Type = CommandType.Href;
    menuItem.Command.HrefCommand.Href = "link.htm";
    JobList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Wxe";
    menuItem.Category = "PostBacks";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.TypeName = "MyType, MyAssembly";
    JobList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Wxe";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.TypeName = "MyType, MyAssembly";
    menuItem.Command.WxeFunctionCommand.Parameters = "Test'Test";
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Event";
    menuItem.Command.Type = CommandType.Event;
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Href";
    menuItem.Command.Type = CommandType.Href;
    menuItem.Command.HrefCommand.Href = "link.htm";
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Invisible Item";
    menuItem.IsVisible = false;
    ChildrenList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Invisible Item";
    menuItem.IsVisible = false;
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.IsDisabled = true;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add (menuItem);
  
    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon = "Images/DeleteItem.gif";
    menuItem.DisabledIcon = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.ExactlyOne;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Open";
    menuItem.Text = "Open";
    menuItem.Category = "Object";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.Parameters = "objects";
    menuItem.Command.WxeFunctionCommand.TypeName = "OBWTest.ViewPersonsWxeFunction,OBWTest";
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Text = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Cut";
    menuItem.Text = "Cut";
    menuItem.Category = "Edit";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Duplicate";
    menuItem.Text = "Duplicate";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon = "Images/DeleteItem.gif";
    menuItem.DisabledIcon = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Invisible Item";
    menuItem.IsVisible = false;
    ChildrenList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Invisible Item";
    menuItem.IsVisible = false;
    ChildrenList.OptionsMenuItems.Add (menuItem);
  }

  private void BocListUserControl_Load (object sender, EventArgs e)
  {
    IBusinessObjectProperty dateOfBirth = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("DateOfBirth");
    IBusinessObjectProperty dateOfDeath = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("DateOfDeath");
    IBusinessObjectProperty height = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Height");
    IBusinessObjectProperty gender = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Gender");
    IBusinessObjectProperty cv = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("CV");


    //  Additional columns, in-code generated

    BocSimpleColumnDefinition birthdayColumnDefinition = new BocSimpleColumnDefinition();
    birthdayColumnDefinition.ColumnTitle = "Birthday";
    birthdayColumnDefinition.PropertyPath = dateOfBirth.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[]{dateOfBirth});
    birthdayColumnDefinition.Width = Unit.Parse ("6em");
    birthdayColumnDefinition.EnforceWidth = true;

    BocSimpleColumnDefinition dayofDeathColumnDefinition = new BocSimpleColumnDefinition();
    dayofDeathColumnDefinition.ColumnTitle = "Day of Death";
    dayofDeathColumnDefinition.PropertyPath = dateOfDeath.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[]{dateOfDeath});
    dayofDeathColumnDefinition.Width = Unit.Parse ("4em");
    dayofDeathColumnDefinition.EnforceWidth = true;

    BocSimpleColumnDefinition heightColumnDefinition = new BocSimpleColumnDefinition();
    heightColumnDefinition.PropertyPath = height.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[]{height});

    BocSimpleColumnDefinition genderColumnDefinition = new BocSimpleColumnDefinition();
    genderColumnDefinition.PropertyPath = gender.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[]{gender});

    BocSimpleColumnDefinition cvColumnDefinition = new BocSimpleColumnDefinition();
    cvColumnDefinition.PropertyPath = cv.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[]{cv});

    BocListView datesView = new BocListView();
    datesView.Title = "Dates";
    datesView.ColumnDefinitions.AddRange (
          new BocColumnDefinition[] {birthdayColumnDefinition, dayofDeathColumnDefinition});

    BocListView statsView = new BocListView();
    statsView.Title = "Stats";
    statsView.ColumnDefinitions.AddRange (
        new BocColumnDefinition[] {heightColumnDefinition, genderColumnDefinition});

    BocListView cvView = new BocListView();
    cvView.Title = "CV";
    cvView.ColumnDefinitions.AddRange (
        new BocColumnDefinition[] {cvColumnDefinition});

    ChildrenList.AvailableViews.AddRange (new BocListView[] {
      datesView,
      statsView,
      cvView});

    if (! IsPostBack)
      ChildrenList.SelectedView = datesView;

    CurrentObject.LoadValues (IsPostBack);
    if (CurrentObject.BusinessObject is Person)
    {
      Person person = (Person) CurrentObject.BusinessObject;
      AllColumnsList.LoadUnboundValue (person.Children, IsPostBack);
    }

    if (!IsPostBack)
    {
      ChildrenList.SetSortingOrder (
          new BocListSortingOrderEntry[] {
              new BocListSortingOrderEntry ((BocColumnDefinition) ChildrenList.FixedColumns[7], SortingDirection.Ascending) });
    }
    if (IsPostBack)
    {
//      BocListSortingOrderEntry[] sortingOrder = ChildrenList.GetSortingOrder();
    }
  }

  private void AddAndEditButton_Click(object sender, System.EventArgs e)
  {
    Person person = Person.CreateObject (Guid.NewGuid());
    ChildrenList.AddAndEditRow (person);
  }

  private void ChildrenListEndEditModeButton_Click(object sender, System.EventArgs e)
  {
    ChildrenList.EndEditDetailsMode(true);
  }

  private void ChildrenList_ListItemCommandClick(object sender, Rubicon.ObjectBinding.Web.UI.Controls.BocListItemCommandClickEventArgs e)
  {
    ChildrenListEventCheckBox.Checked = true;
    ChildrenListEventArgsLabel.Text += string.Format ("ColumnID: {0}<br>", e.Column.ItemID);
    if (e.BusinessObject is IBusinessObjectWithIdentity)
      ChildrenListEventArgsLabel.Text += string.Format ("BusinessObjectID: {0}<br>", ((IBusinessObjectWithIdentity) e.BusinessObject).UniqueIdentifier);
    ChildrenListEventArgsLabel.Text += string.Format ("ListIndex: {0}<br>", e.ListIndex);

    if (e.Column.ItemID == "Edit")
      ChildrenList.SwitchRowIntoEditMode (e.ListIndex);
  }

  private void ChildrenList_MenuItemClick(object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
  {
    ChildrenListEventArgsLabel.Text = e.Item.ItemID;
  }

  private void ChildrenList_DataRowRender(object sender, Rubicon.ObjectBinding.Web.UI.Controls.BocListDataRowRenderEventArgs e)
  {
    if (e.ListIndex == 3)
      e.IsModifiableRow = false;
  }

  private void ChildrenList_SavingEditedRow(object sender, Rubicon.ObjectBinding.Web.UI.Controls.BocListRowEditModeEventArgs e)
  {
  
  }

  private void ChildrenList_EditedRowSaved(object sender, Rubicon.ObjectBinding.Web.UI.Controls.BocListItemEventArgs e)
  {
  
  }

  private void ChildrenList_SortingOrderChanging(object sender, Rubicon.ObjectBinding.Web.UI.Controls.BocListSortingOrderChangeEventArgs e)
  {
  
  }

  private void ChildrenList_SortingOrderChanged(object sender, Rubicon.ObjectBinding.Web.UI.Controls.BocListSortingOrderChangeEventArgs e)
  {
  
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
