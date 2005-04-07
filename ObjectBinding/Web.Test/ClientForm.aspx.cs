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

public class ClientForm : TestWxeBasePage
{
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected Rubicon.Web.UI.Controls.WebTabStrip PagesTabStrip;
  protected Rubicon.Web.UI.Controls.ValidationStateViewer ValidationStateViewer;
  protected Rubicon.Web.UI.Controls.TabbedMultiView MultiView;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeValue1;
  protected Rubicon.ObjectBinding.Web.Controls.BocBooleanValue BocBooleanValue1;
  private PlaceHolder _wxeControlsPlaceHolder;

  protected ClientFormWxeFunction Function
  {
    get { return (ClientFormWxeFunction) CurrentFunction; }
  }

	private void Page_Load(object sender, System.EventArgs e)
	{
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

