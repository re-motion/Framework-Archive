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
using System.Globalization;
using System.Threading;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;
using Rubicon.NullableValueTypes;
using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Utilities;
using Rubicon.Globalization;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Controls;
namespace OBWTest
{

public class TestForm : Page
{
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected Rubicon.ObjectBinding.Web.Controls.BocList BocList;
  protected System.Web.UI.WebControls.Button Button1;
  protected System.Web.UI.WebControls.Label EventLabel;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;

	private void Page_Load(object sender, System.EventArgs e)
	{
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject (personID);

    ReflectionBusinessObjectDataSourceControl.BusinessObject = person;    
    ReflectionBusinessObjectDataSourceControl.LoadValues (IsPostBack);
	}

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);
	}

	#region Web Form Designer generated code
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.BocList.MenuItemClick += new MenuItemClickEventHandler(BocList_MenuItemClick);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void BocList_MenuItemClick(object sender, MenuItemClickEventArgs e)
  {
    EventLabel.Text = e.Item.Text;

  }
}

}