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
  protected System.Web.UI.WebControls.LinkButton SaveButton;
  protected System.Web.UI.WebControls.LinkButton CancelButton;
  private IDataEditControl[] _dataEditControls;
  protected Rubicon.Web.UI.Controls.MultiPage PagesMultiPage;
  protected Rubicon.Web.UI.Controls.WebTabStrip PagesTabStrip;
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected System.Web.UI.WebControls.Button ValidateButton;
  protected Rubicon.Web.UI.Controls.ValidationStateViewer ValidationStateViewer;
  protected Rubicon.Web.UI.Controls.TabbedMultiView MultiView;
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
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
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
}

}

