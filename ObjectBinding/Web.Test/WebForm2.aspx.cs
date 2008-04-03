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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace OBWTest
{

public class WebForm2: System.Web.UI.Page
{
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue DateOfBirthField;
  protected Remotion.Web.UI.Controls.SmartLabel BocPropertyLabel3;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator1;
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl reflectionBusinessObjectDataSource1;

	private void Page_Load (object sender, System.EventArgs e)
	{
    Person p = Person.CreateObject();
    p.FirstName = "Hugo";
    p.LastName = "Meier";
    p.DateOfBirth = new DateTime (1959, 4, 15);
    p.Height = 179;

    reflectionBusinessObjectDataSource1.BusinessObject = (IBusinessObject) p;

    this.DataBind();
    reflectionBusinessObjectDataSource1.LoadValues (false);

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
    this.reflectionBusinessObjectDataSource1 = new BindableObjectDataSourceControl();
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    // 
    // reflectionBusinessObjectDataSource1
    // 
    this.reflectionBusinessObjectDataSource1.BusinessObject = null;
    this.reflectionBusinessObjectDataSource1.Mode = DataSourceMode.Edit;
    this.reflectionBusinessObjectDataSource1.Type = typeof (Person);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SaveButton_Click (object sender, System.EventArgs e)
  {
    reflectionBusinessObjectDataSource1.SaveValues (false);
    string s = ((Person)reflectionBusinessObjectDataSource1.BusinessObject).FirstName;
  }
}

}
