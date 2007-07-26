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
using Rubicon.ObjectBinding;

using Rubicon.ObjectBinding.Sample;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI;
using ObjectBoundRepeater=Rubicon.ObjectBinding.Sample.ObjectBoundRepeater;

namespace OBWTest
{
/// <summary>
/// Summary description for RepeaterTest.
/// </summary>
public class RepeaterTest : SmartPage
{
  protected BindableObjectDataSourceControl CurrentObject;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected ObjectBoundRepeater Repeater2;
  protected ObjectBoundRepeater Repeater3;
  protected Rubicon.Web.UI.Controls.WebButton SaveButton;

	private void Page_Load(object sender, System.EventArgs e)
	{
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject (personID);

    CurrentObject.BusinessObject = (IBusinessObject) person;
    CurrentObject.LoadValues (IsPostBack);
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
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SaveButton_Click(object sender, System.EventArgs e)
  {
    PrepareValidation();
    bool isValid = CurrentObject.Validate();
    if (isValid)
      CurrentObject.SaveValues (false);
  }
}
}
