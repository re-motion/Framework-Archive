using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Reflection;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.UI;
using Rubicon.Web;
using OBRTest;

namespace OBWTest
{
[MultiLingualResources ("OBWTest.Globalization.SingleBocTestListForm")]
public class SingleBocListForm : SingleBocTestWxeBasePage
{
  /// <summary> Hashtable&lt;type,IResourceManagers&gt; </summary>
  private static Hashtable s_chachedResourceManagers = new Hashtable();
  protected System.Web.UI.WebControls.Button SaveButton;
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected System.Web.UI.WebControls.Label ChildrenListEventArgsLabel;
  protected System.Web.UI.WebControls.CheckBox ChildrenListEventCheckBox;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents1;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue LastNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocList ChildrenList;
  protected Rubicon.ObjectBinding.Web.Controls.BocList JobList;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected System.Web.UI.HtmlControls.HtmlTable Table3;
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

    if (person.Children.Length == 0)
    {
      Person[] children = new Person[2];
      
      children[0] = Person.CreateObject (Guid.NewGuid());
      children[0].FirstName = "Jack";
      children[0].LastName = "Doe";
      children[0].DateOfBirth = new DateTime (1990, 4, 15);
      children[0].Height = 160;
      children[0].MarriageStatus = MarriageStatus.Single;

      children[1] = Person.CreateObject (Guid.NewGuid());
      children[1].FirstName = "Max";
      children[1].LastName = "Doe";
      children[1].DateOfBirth = new DateTime (1991, 4, 15);
      children[1].Height = 155;
      children[1].MarriageStatus = MarriageStatus.Single;

      person.Children = children;
    }

    if (person.Jobs.Length == 0)
    {
      Job[] jobs = new Job[2];
      
      jobs[0] = Job.CreateObject (Guid.NewGuid());
      jobs[0].Title = "Programmer";
      jobs[0].StartDate = new DateTime (2000, 1, 1);
      jobs[0].EndDate = new DateTime (2004, 12, 31);

      jobs[1] = Job.CreateObject (Guid.NewGuid());
      jobs[1].Title = "CEO";
      jobs[1].StartDate = new DateTime (2005, 1, 1);

      person.Jobs = jobs;
    }

    ReflectionBusinessObjectDataSourceControl.BusinessObject = person;
    

    this.DataBind();

    ReflectionBusinessObjectDataSourceControl.LoadValues (IsPostBack);
  }

	override protected void OnInit(EventArgs e)
	{
    InitializeComponent();
		base.OnInit(e);

    if (!IsPostBack)
      Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectStorage.Reset();

    IBusinessObjectProperty dateOfBirth = 
      ReflectionBusinessObjectDataSourceControl.BusinessObjectClass.GetPropertyDefinition ("DateOfBirth");
    IBusinessObjectProperty dateOfDeath = 
      ReflectionBusinessObjectDataSourceControl.BusinessObjectClass.GetPropertyDefinition ("DateOfDeath");
    IBusinessObjectProperty height = 
      ReflectionBusinessObjectDataSourceControl.BusinessObjectClass.GetPropertyDefinition ("Height");
    IBusinessObjectProperty gender = 
      ReflectionBusinessObjectDataSourceControl.BusinessObjectClass.GetPropertyDefinition ("Gender");


    //  Additional columns, in-code generated

    BocSimpleColumnDefinition birthdayColumnDefinition = new BocSimpleColumnDefinition();
    birthdayColumnDefinition.ColumnTitle = "Birthday";
    birthdayColumnDefinition.PropertyPath = dateOfBirth.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[]{dateOfBirth});

    BocSimpleColumnDefinition dayofDeathColumnDefinition = new BocSimpleColumnDefinition();
    dayofDeathColumnDefinition.ColumnTitle = "Day of Death";
    dayofDeathColumnDefinition.PropertyPath = dateOfDeath.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[]{dateOfDeath});

    BocSimpleColumnDefinition heightColumnDefinition = new BocSimpleColumnDefinition();
    heightColumnDefinition.PropertyPath = height.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[]{height});

    BocSimpleColumnDefinition genderColumnDefinition = new BocSimpleColumnDefinition();
    genderColumnDefinition.PropertyPath = gender.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[]{gender});

    BocColumnDefinitionSet datesColumnDefintionSet = new BocColumnDefinitionSet();
    datesColumnDefintionSet.Title = "Dates";
    datesColumnDefintionSet.ColumnDefinitions.AddRange (
          new BocColumnDefinition[] {birthdayColumnDefinition, dayofDeathColumnDefinition});

    BocColumnDefinitionSet statsColumnDefintionSet = new BocColumnDefinitionSet();
    statsColumnDefintionSet.Title = "Stats";
    statsColumnDefintionSet.ColumnDefinitions.AddRange (
        new BocColumnDefinition[] {heightColumnDefinition, genderColumnDefinition});

    ChildrenList.AvailableColumnDefinitionSets.AddRange (new BocColumnDefinitionSet[] {
      datesColumnDefintionSet,
      statsColumnDefintionSet});

    ChildrenList.SelectedColumnDefinitionSet = datesColumnDefintionSet;
  }

	#region Web Form Designer generated code
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.ChildrenList.ListItemCommandClick += new Rubicon.ObjectBinding.Web.Controls.BocListItemCommandClickEventHandler(this.ChildrenList_ListItemCommandClick);
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SaveButton_Click(object sender, System.EventArgs e)
  {
    bool isValid = FormGridManager.Validate();
    if (isValid)
    {
      ReflectionBusinessObjectDataSourceControl.SaveValues (false);
      Person person = (Person) ReflectionBusinessObjectDataSourceControl.BusinessObject;
      person.SaveObject();
      if (person.Children != null)
      {
        foreach (Person child in person.Children)
          child.SaveObject();
      }
      if (person.Jobs != null)
      {
        foreach (Job job in person.Jobs)
          job.SaveObject();
      }
    }
    Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectStorage.Reset();
  }

  protected override void OnPreRender(EventArgs e)
  {
    IResourceManager rm = MultiLingualResourcesAttribute.GetResourceManager (this.GetType(), true);
    System.Collections.Specialized.NameValueCollection strings = rm.GetAllStrings();

    base.OnPreRender (e);
    //  A call to the ResourceDispatcher to get have the automatic resources dispatched
    ResourceDispatcher.Dispatch (this, this.GetResourceManager());

  }

  private void ChildrenList_ListItemCommandClick(object sender, Rubicon.ObjectBinding.Web.Controls.BocListItemCommandClickEventArgs e)
  {
    ChildrenListEventCheckBox.Checked = true;
    ChildrenListEventArgsLabel.Text += string.Format ("ColumnID: {0}<br>", e.Column.ColumnID);
    if (e.BusinessObject is IBusinessObjectWithIdentity)
      ChildrenListEventArgsLabel.Text += string.Format ("BusinessObjectID: {0}<br>", ((IBusinessObjectWithIdentity) e.BusinessObject).UniqueIdentifier);
    ChildrenListEventArgsLabel.Text += string.Format ("ListIndex: {0}<br>", e.ListIndex);
  }

  public override IResourceManager GetResourceManager()
  {
    // cache the resource manager
    Type type = this.GetType();
    if (s_chachedResourceManagers[type] == null)
    {
      lock (typeof (SingleBocListForm))
      {
        if (s_chachedResourceManagers[type] == null)
          s_chachedResourceManagers[type] = MultiLingualResourcesAttribute.GetResourceManager (type, true);
      }  
    }

    //  TODO: Combine IResourceManager into a ResourceManagerSet
//    return ResourceManagerSet.Combine (
//      (IResourceManager) base.GetResourceManager(), 
// (IResourceManager) s_chachedResourceManagers[type]);
    return base.GetResourceManager();
  }
}


}
