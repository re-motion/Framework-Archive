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
using Microsoft.Web.UI.WebControls;
using Rubicon.Web.Utilities;
using Rubicon.Utilities;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Collections;

namespace OBWTest
{

public class TestTabbedForm : TestWxeBasePage
{
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected System.Web.UI.WebControls.LinkButton SaveButton;
  protected System.Web.UI.WebControls.LinkButton CancelButton;
  private IDataEditControl[] _dataEditControls;
  protected Rubicon.Web.UI.Controls.MultiPage PagesMultiPage;
  protected Rubicon.Web.UI.Controls.TabStrip PagesTabStrip;
  protected System.Web.UI.WebControls.Button PostBackButton;
  private bool _currentObjectSaved = false;

  protected TestTabbedFormWxeFunction Function
  {
    get { return (TestTabbedFormWxeFunction) CurrentFunction; }
  }

	private void Page_Load(object sender, System.EventArgs e)
	{
    if (! IsPostBack)
    {
      // add tabs 
      AddTab ("TestTabbedPersonDetailsUserControl", "Person Details");
      AddTab ("TestTabbedPersonJobsUserControl", "Jobs");
    }

    TypedArrayList dataEditControls = new TypedArrayList (typeof (IDataEditControl));
    // load editor pages
    IDataEditControl dataEditControl;
    dataEditControl = AddPage ("TestTabbedPersonDetailsUserControl", "TestTabbedPersonDetailsUserControl.ascx");
    if (dataEditControl != null)
      dataEditControls.Add (dataEditControl);
    dataEditControl = AddPage ("TestTabbedPersonJobsUserControl", "TestTabbedPersonJobsUserControl.ascx");
    if (dataEditControl != null)
      dataEditControls.Add (dataEditControl);

    _dataEditControls = (IDataEditControl[]) dataEditControls.ToArray();
	}

  private void AddTab (string id, string text)
  {
    Tab tab = new Tab ();
    tab.Text = text;
    tab.ID = id + "_tab";
    tab.TargetID = id + "_view";
    PagesTabStrip.Items.Add (tab);
    
    TabSeparator seperator = new TabSeparator ();
    seperator.DefaultStyle.Add ("width", "10px");
    PagesTabStrip.Items.Add (seperator);
  }

  private IDataEditControl AddPage (string id, string path)
  {
    PageView pageView = new PageView();
    pageView.ID = id + "_view";
    PagesMultiPage.Controls.Add (pageView);

    UserControl control = (UserControl) this.LoadControl (path);
    control.ID = Rubicon.Text.IdentifierGenerator.HtmlStyle.GetValidIdentifier (System.IO.Path.GetFileNameWithoutExtension (path));

    //EgoFormPageUserControl formPageControl = control as EgoFormPageUserControl;
    //if (formPageControl != null)
    //  formPageControl.FormPageObject = formPage;

    pageView.Controls.Add (control);

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
    CurrentStep.ExecuteNextStep();
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

    CurrentStep.ExecuteNextStep();
  }
}

}

