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

using Remotion.Web.ExecutionEngine;
using Remotion.Templates.Generic.Web.Classes;
using Remotion.Globalization;
using Remotion.Web.UI.Globalization;
using Remotion.Web.UI;

namespace Remotion.Templates.Generic.Web.UI
{

[MultiLingualResources ("Remotion.Templates.Generic.Web.UI.Globalization.TemplateForm")]
public class TemplateForm : BaseWxePage
{
  protected Remotion.Web.UI.Controls.FormGridManager FormGridManager;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Remotion.ObjectBinding.Web.Controls.BocTextValue BocTextValue;
  protected Remotion.Web.UI.Controls.SmartLabel SmartLabel;
  protected Remotion.Web.UI.Controls.WebButton SubmitButton;
  protected Remotion.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

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
