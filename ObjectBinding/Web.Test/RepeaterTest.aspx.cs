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

using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding.Web.Controls;

using OBRTest;

namespace OBWTest
{
/// <summary>
/// Summary description for RepeaterTest.
/// </summary>
public class RepeaterTest : System.Web.UI.Page
{
  protected BocTextValue BocTextValue1;
  protected BusinessObjectDataSourceControl ItemDataSourceControl;
  protected OBRTest.ObjectBoundRepeater Repeater2;
  protected ReflectionBusinessObjectDataSourceControl CurrentObject;
  protected OBRTest.ObjectBoundRepeater Repeater3;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected Rubicon.Web.UI.Controls.WebButton SaveButton;
  protected System.Web.UI.WebControls.Repeater Repeater1;

	private void Page_Load(object sender, System.EventArgs e)
	{
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject (personID);

    Repeater1.DataSource = person.Children;
    Repeater1.DataBind();

    CurrentObject.BusinessObject = person;
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
    this.Repeater1.ItemCreated += new System.Web.UI.WebControls.RepeaterItemEventHandler(this.Repeater1_ItemCreated);
    this.Repeater1.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(this.Repeater1_ItemDataBound);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void Repeater1_ItemCreated(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
  {
  
  }

  private void Repeater1_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
  {
  
  }

  private void SaveButton_Click(object sender, System.EventArgs e)
  {
    CurrentObject.SaveValues (false);
  }
}
}
