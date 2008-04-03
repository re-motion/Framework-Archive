using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using Remotion.Web.Utilities;
using Remotion.Utilities;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding;

using Remotion.Web.ExecutionEngine;
using Remotion.Collections;
using Remotion.Web.UI.Controls;

namespace OBWTest
{

public class ClientForm : TestWxeBasePage
{
  protected Remotion.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected Remotion.Web.UI.Controls.WebTabStrip PagesTabStrip;
  protected Remotion.Web.UI.Controls.ValidationStateViewer ValidationStateViewer;
  protected Remotion.Web.UI.Controls.TabbedMultiView MultiView;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeValue1;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue1;
  private PlaceHolder _wxeControlsPlaceHolder;
  private IDataEditControl[] _dataEditControls;
  private DropDownMenu _ddm = new DropDownMenu();

  protected ClientFormWxeFunction Function
  {
    get { return (ClientFormWxeFunction) CurrentFunction; }
  }

	private void Page_Load(object sender, System.EventArgs e)
	{
    List<IDataEditControl> dataEditControls = new List<IDataEditControl>();
    // load editor pages
    IDataEditControl dataEditControl;
    dataEditControl = AddPage ("TestTabbedPersonDetailsUserControl", "Person Details", new IconInfo ("Images/Remotion.ObjectBinding.Sample.Person.gif"), "TestTabbedPersonDetailsUserControl.ascx");
    if (dataEditControl != null)
      dataEditControls.Add (dataEditControl);
    dataEditControl = AddPage ("TestTabbedPersonJobsUserControl", "Jobs", new IconInfo ("Images/Remotion.ObjectBinding.Sample.Job.gif"), "TestTabbedPersonJobsUserControl.ascx");
    if (dataEditControl != null)
      dataEditControls.Add (dataEditControl);
    _dataEditControls = (IDataEditControl[]) dataEditControls.ToArray();

    Response.Cache.SetMaxAge (TimeSpan.Zero);
    Response.Cache.SetCacheability (HttpCacheability.NoCache);
  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
  }


  private IDataEditControl AddPage (string id, string title, IconInfo icon, string path)
  {
    TabView view = new TabView();
    view.ID = id+ "_View";
    view.Title = title;
    view.Icon = icon;
    MultiView.Views.Add (view);

    UserControl control = (UserControl) this.LoadControl (path);
    control.ID = Remotion.Text.IdentifierGenerator.HtmlStyle.GetValidIdentifier (System.IO.Path.GetFileNameWithoutExtension (path));

    //EgoFormPageUserControl formPageControl = control as EgoFormPageUserControl;
    //if (formPageControl != null)
    //  formPageControl.FormPageObject = formPage;

    view.Controls.Add (control);

    IDataEditControl dataEditControl = control as IDataEditControl;
    if (dataEditControl != null)
    {
      dataEditControl.BusinessObject = (IBusinessObject) Function.Object;
      dataEditControl.LoadValues (IsPostBack);
      dataEditControl.Mode = Function.ReadOnly ? DataSourceMode.Read : DataSourceMode.Edit;
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
		
    _ddm.ID = "ddm";
    _ddm.Height = new Unit("1%");
    _ddm.Width = new Unit("1%");
    _ddm.TitleText = "Options Menu";
    MultiView.TopControls.Add (_ddm);

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
    cancelButton.UseSubmitBehavior = false;
    MultiView.TopControls.Add (cancelButton);

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
  }

  private void CancelButton_Click(object sender, System.EventArgs e)
  {
    ExecuteNextStep();
  }

  private void SaveButton_Click(object sender, System.EventArgs e)
  {
    ExecuteNextStep();
  }

  protected override ControlCollection WxeControls
  {
    get { return _wxeControlsPlaceHolder.Controls; }
  }

}

}

