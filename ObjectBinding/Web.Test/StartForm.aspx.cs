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
using Rubicon.Web.ExecutionEngine;

namespace OBWTest
{
/// <summary>
/// Summary description for Start.
/// </summary>
public class StartPage : System.Web.UI.Page
{
	private void Page_Load(object sender, System.EventArgs e)
	{
    Response.Redirect ("WxeHandler.ashx?WxeFunctionType=OBWTest.MainWxeFunction,OBWTest");
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
		this.Load += new System.EventHandler(this.Page_Load);
	}
	#endregion
}

public class MainWxeFunction: WxeFunction
{
  public MainWxeFunction ()
  {
    ReturnUrl = "Start.aspx";
  }

  // steps

  private WxeStep Step1 = new WxePageStep ("TestListPage.aspx");
  //private WxeStep Step1 = new WxePageStep ("TestListPage.aspx");

}

}
