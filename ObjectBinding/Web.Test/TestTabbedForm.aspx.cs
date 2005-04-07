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
using Rubicon.Web.Utilities;
using Rubicon.Utilities;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Collections;
using Rubicon.Web.UI.Controls;

namespace OBWTest
{

public class TestTabbedForm : TestWxeBasePage
{
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  private IDataEditControl[] _dataEditControls;
  protected Rubicon.Web.UI.Controls.WebTabStrip PagesTabStrip;
  protected Rubicon.Web.UI.Controls.ValidationStateViewer ValidationStateViewer;
  protected Rubicon.Web.UI.Controls.TabbedMultiView MultiView;
  private PlaceHolder _wxeControlsPlaceHolder;
  private bool _currentObjectSaved = false;

  protected TestTabbedFormWxeFunction Function
  {
    get { return (TestTabbedFormWxeFunction) CurrentFunction; }
  }

	private void Page_Load(object sender, System.EventArgs e)
	{
    // add tabs 
    AddTab ("1", "Test Tab 1", null);
    AddTab ("2", "Test Tab 2 foo bar", null);
    AddTab ("3", "Test Tab 3 foo", null);
    AddTab ("4", "Test Tab 4 foo foo bar", null);
    AddTab ("5", "Test Tab 5", null);
    AddTab ("6", "Test Tab 6 foo", null);
    AddTab ("7", "Test Tab 7 foo foo bar", null);

    TypedArrayList dataEditControls = new TypedArrayList (typeof (IDataEditControl));
    // load editor pages
    IDataEditControl dataEditControl;
    dataEditControl = AddPage ("TestTabbedPersonDetailsUserControl", "Person Details", new IconInfo ("Images/OBRTest.Person.gif"), "TestTabbedPersonDetailsUserControl.ascx");
    if (dataEditControl != null)
      dataEditControls.Add (dataEditControl);
    dataEditControl = AddPage ("TestTabbedPersonJobsUserControl", "Jobs", new IconInfo ("Images/OBRTest.Job.gif"), "TestTabbedPersonJobsUserControl.ascx");
    if (dataEditControl != null)
      dataEditControls.Add (dataEditControl);
    _dataEditControls = (IDataEditControl[]) dataEditControls.ToArray();

  }

  private void AddTab (string id, string text, IconInfo icon)
  {
    PagesTabStrip.Tabs.Add (WebTab.GetSeparator());
    
    WebTab tab = new WebTab ();
    tab.Text = text;
    tab.TabID = id ;
    tab.Icon = icon;
    PagesTabStrip.Tabs.Add (tab);
  }

  private IDataEditControl AddPage (string id, string title, IconInfo icon, string path)
  {
    TabView view = new TabView();
    view.ID = id+ "_View";
    view.Title = title;
    view.Icon = icon;
    MultiView.Views.Add (view);

    UserControl control = (UserControl) this.LoadControl (path);
    control.ID = Rubicon.Text.IdentifierGenerator.HtmlStyle.GetValidIdentifier (System.IO.Path.GetFileNameWithoutExtension (path));

    //EgoFormPageUserControl formPageControl = control as EgoFormPageUserControl;
    //if (formPageControl != null)
    //  formPageControl.FormPageObject = formPage;

    view.Controls.Add (control);

    IDataEditControl dataEditControl = control as IDataEditControl;
    if (dataEditControl != null)
    {
      dataEditControl.BusinessObject = Function.Object;
      dataEditControl.LoadValues (IsPostBack);
      dataEditControl.EditMode = ! Function.ReadOnly;
      return dataEditControl;
    }

    return null;
  }

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();

    WebButton saveButton = new WebButton ();
    saveButton.ID = "SaveButton";
    saveButton.Text = "Save";
    saveButton.Style["margin-right"] = "10pt";
    saveButton.Click += new EventHandler(SaveButton_Click);
    MultiView.TopControls.Add (saveButton);

    WebButton cancelButton = new WebButton ();
    cancelButton.ID = "CancelButton";
    cancelButton.Text = "Cancel";
    cancelButton.Style["margin-right"] = "10pt";
    cancelButton.Click += new EventHandler(CancelButton_Click);
    MultiView.TopControls.Add (cancelButton);

    WebButton postBackButton = new WebButton();
    postBackButton.ID = "PostBackButton";
    postBackButton.Text = "Postback";
    postBackButton.Style["margin-right"] = "10pt";
    MultiView.BottomControls.Add (postBackButton);

    WebButton validateButton = new WebButton();
    validateButton.ID = "ValidateButton";
    validateButton.Text = "Validate";
    validateButton.Style["margin-right"] = "10pt";
    validateButton.Click += new EventHandler(ValidateButton_Click);
    MultiView.BottomControls.Add (validateButton);

    _wxeControlsPlaceHolder = new PlaceHolder();
    MultiView.BottomControls.Add (_wxeControlsPlaceHolder);
		
    base.OnInit(e);
	}
	#region Web Form Designer generated code

	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.Unload += new System.EventHandler(this.Page_Unload);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void Page_Unload (object sender, System.EventArgs e)
  {
    if (_currentObjectSaved)
      return;

    foreach (IDataEditControl control in _dataEditControls)
      control.DataSource.SaveValues (true);
  }

  private void CancelButton_Click(object sender, System.EventArgs e)
  {
    ExecuteNextStep();
  }

  private void SaveButton_Click(object sender, System.EventArgs e)
  {
    // validate all tabs
    foreach (IDataEditControl control in _dataEditControls)
    {
      bool isValid = control.Validate();
      if (! isValid)
        return;
    }

    // save all tabs
    foreach (IDataEditControl control in _dataEditControls)
      control.DataSource.SaveValues (false);

    ExecuteNextStep();
  }

  private void ValidateButton_Click(object sender, System.EventArgs e)
  {
    foreach (UserControl control in _dataEditControls)
    {
      Rubicon.Web.UI.Controls.FormGridManager formGridManager = 
          control.FindControl("FormGridManager") as Rubicon.Web.UI.Controls.FormGridManager;
      if (formGridManager != null)
        formGridManager.Validate();
    }
  }

  protected override ControlCollection WxeControls
  {
    get { return _wxeControlsPlaceHolder.Controls; }
  }

}

}

