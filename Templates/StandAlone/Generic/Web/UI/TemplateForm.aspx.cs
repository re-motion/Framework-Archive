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
using Rubicon.Templates.Generic.Web.Classes;
using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.UI;

namespace Rubicon.Templates.Generic.Web.UI
{

[MultiLingualResources ("Rubicon.Templates.Generic.Web.UI.Globalization.TemplateForm")]
public class TemplateForm : BaseWxePage
{
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue;
  protected Rubicon.Web.UI.Controls.SmartLabel SmartLabel;
  protected Rubicon.Web.UI.Controls.WebButton SubmitButton;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

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
    this.PreRender += new System.EventHandler(this.TemplateForm_PreRender);

  }
	#endregion

  private void Page_Load(object sender, System.EventArgs e)
	{
  }

  private void TemplateForm_PreRender(object sender, System.EventArgs e)
  {
    IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager (this);
    HtmlHeadAppender.Current.SetTitle (resourceManager.GetString ("Title"));
  }
}

}
