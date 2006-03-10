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

public class BocListAsGridUserControl : BaseUserControl
{
  protected System.Web.UI.HtmlControls.HtmlTable Table3;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue LastNameField;
  protected OBRTest.TestBocList ChildrenList;
  protected OBRTest.TestBocList EmptyList;
  protected System.Web.UI.WebControls.CheckBox ChildrenListEventCheckBox;
  protected System.Web.UI.WebControls.Label ChildrenListEventArgsLabel;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl EmptyDataSourceControl;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected OBRTest.TestBocListValidator EmptyListValidator;
  protected OBRTest.TestBocList Testboclist1;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocList AllColumnsList;
  protected System.Web.UI.HtmlControls.HtmlGenericControl NonVisualControls;
  protected Rubicon.Web.UI.Controls.WebButton SwitchToEditModeButton;
  protected Rubicon.Web.UI.Controls.WebButton EndEditModeButton;
  protected Rubicon.Web.UI.Controls.WebButton AddRowButton;
  protected Rubicon.Web.UI.Controls.WebButton AddRowsButton;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue NumberOfNewRowsField;
  protected Rubicon.Web.UI.Controls.WebButton RemoveRows;
  protected Rubicon.Web.UI.Controls.WebButton CancelEditModeButton;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl CurrentObject;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    SwitchToEditModeButton.Click += new EventHandler(SwitchToEditModeButton_Click);
    EndEditModeButton.Click += new EventHandler(EndEditModeButton_Click);
    CancelEditModeButton.Click += new EventHandler(CancelEditModeButton_Click);

    AddRowButton.Click += new EventHandler (AddRowButton_Click);
    AddRowsButton.Click += new EventHandler (AddRowsButton_Click);
    RemoveRows.Click += new EventHandler (RemoveRows_Click);
    
    ChildrenList.ListItemCommandClick += new Rubicon.ObjectBinding.Web.UI.Controls.BocListItemCommandClickEventHandler(this.ChildrenList_ListItemCommandClick);
    ChildrenList.MenuItemClick += new Rubicon.Web.UI.Controls.WebMenuItemClickEventHandler(this.ChildrenList_MenuItemClick);
    
    ChildrenList.DataRowRender += new Rubicon.ObjectBinding.Web.UI.Controls.BocListDataRowRenderEventHandler(this.ChildrenList_DataRowRender);

    ChildrenList.ModifiedRowCanceling += new BocListModifiableRowEventHandler (ChildrenList_ModifiedRowCanceling);
    ChildrenList.ModifiedRowCanceled += new BocListItemEventHandler (ChildrenList_ModifiedRowCanceled);
    ChildrenList.ModifiedRowSaving += new BocListModifiableRowEventHandler (ChildrenList_ModifiedRowSaving);
    ChildrenList.ModifiedRowSaved += new BocListItemEventHandler (ChildrenList_ModifiedRowSaved);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

	override protected void OnInit(EventArgs e)
	{
		InitializeComponent();
    base.OnInit (e);
    InitializeMenuItems();
  }

  private void InitializeMenuItems()
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Category = "Edit";
    menuItem.Text = "Copy";
    menuItem.Icon.Url = "~/Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.ExactlyOne;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add (menuItem);
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Category = "Edit";
    menuItem.Text = "Paste";
    menuItem.IsDisabled = false;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add (menuItem);
    ChildrenList.OptionsMenuItems.Add (menuItem);
  }

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    IBusinessObjectProperty dateOfBirth = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("DateOfBirth");
    IBusinessObjectProperty dateOfDeath = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("DateOfDeath");
    IBusinessObjectProperty height = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Height");
    IBusinessObjectProperty gender = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Gender");
    IBusinessObjectProperty cv = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("CV");
    IBusinessObjectProperty income = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Income");


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

    BocSimpleColumnDefinition incomeColumnDefinition = new BocSimpleColumnDefinition();
    incomeColumnDefinition.PropertyPath = cv.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[]{income});

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

    BocListView incomeView = new BocListView();
    incomeView.Title = "Income";
    incomeView.ColumnDefinitions.AddRange (
        new BocColumnDefinition[] {incomeColumnDefinition});

    ChildrenList.AvailableViews.AddRange (new BocListView[] {
      datesView,
      statsView,
      cvView,
      incomeView});

    if (! IsPostBack)
      ChildrenList.SelectedView = datesView;

    if (!IsPostBack)
    {
//      ChildrenList.SetSortingOrder (
//          new BocListSortingOrderEntry[] {
//              new BocListSortingOrderEntry ((BocColumnDefinition) ChildrenList.FixedColumns[7], SortingDirection.Ascending) });
    }

    if (! IsPostBack)
      ChildrenList.SwitchListIntoEditMode();

    if (! IsPostBack)
      NumberOfNewRowsField.Value = 1;
    NumberOfNewRowsField.IsDirty = false;
  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);

    SwitchToEditModeButton.Enabled = ! ChildrenList.IsListEditModeActive;
    EndEditModeButton.Enabled = ChildrenList.IsListEditModeActive;
  }

  private void SwitchToEditModeButton_Click(object sender, EventArgs e)
  {
    ChildrenList.SwitchListIntoEditMode ();
  }

  private void EndEditModeButton_Click(object sender, EventArgs e)
  {
    ChildrenList.EndListEditMode (true);
  }

  private void CancelEditModeButton_Click(object sender, System.EventArgs e)
  {
    ChildrenList.EndListEditMode (false);
  }

  private void AddRowButton_Click(object sender, System.EventArgs e)
  {
    Person person = Person.CreateObject (Guid.NewGuid());
    ChildrenList.AddRow (person);
  }

  private void AddRowsButton_Click(object sender, System.EventArgs e)
  {
    int count = 0;
    
    if (NumberOfNewRowsField.Validate())
      count = (int) NumberOfNewRowsField.Value;

    Person[] persons = new Person[count];
    for (int i = 0; i < count; i++)
      persons[i] = Person.CreateObject (Guid.NewGuid());
    
    ChildrenList.AddRows (persons);
  }

  private void RemoveRows_Click(object sender, System.EventArgs e)
  {
    IBusinessObject[] selectedBusinessObjects = ChildrenList.GetSelectedBusinessObjects();
    ChildrenList.RemoveRows (selectedBusinessObjects);
  }

  private void ChildrenList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
  {
    ChildrenListEventCheckBox.Checked = true;
    ChildrenListEventArgsLabel.Text += string.Format ("ColumnID: {0}<br>", e.Column.ItemID);
    if (e.BusinessObject is IBusinessObjectWithIdentity)
      ChildrenListEventArgsLabel.Text += string.Format ("BusinessObjectID: {0}<br>", ((IBusinessObjectWithIdentity) e.BusinessObject).UniqueIdentifier);
    ChildrenListEventArgsLabel.Text += string.Format ("ListIndex: {0}<br>", e.ListIndex);
  }

  private void ChildrenList_MenuItemClick (object sender, WebMenuItemClickEventArgs e)
  {
    ChildrenListEventArgsLabel.Text = e.Item.ItemID;
  }

  private void ChildrenList_DataRowRender (object sender, BocListDataRowRenderEventArgs e)
  {
    if (e.ListIndex == 3)
      e.IsModifiableRow = false;
  }

  private void ChildrenList_ModifiedRowCanceling(object sender, BocListModifiableRowEventArgs e)
  {
  }

  private void ChildrenList_ModifiedRowCanceled(object sender, BocListItemEventArgs e)
  {
  }

  private void ChildrenList_ModifiedRowSaving(object sender, BocListModifiableRowEventArgs e)
  {
  }

  private void ChildrenList_ModifiedRowSaved(object sender, BocListItemEventArgs e)
  {
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
