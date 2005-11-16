using System;
using System.Collections;
using System.Collections.Specialized;
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
using Rubicon.Web.ExecutionEngine;
using Rubicon.Collections;
using Rubicon.Web.UI.Controls;

namespace Rubicon.PageTransition
{

public class MultiplePostbackCatcherForm : WxePage
{
  protected Rubicon.Web.UI.Controls.WebButton Button3;
  protected Rubicon.Web.UI.Controls.WebButton Button4;
  protected System.Web.UI.WebControls.Button Button1;
  protected System.Web.UI.WebControls.LinkButton Button6;
  protected System.Web.UI.WebControls.LinkButton Button8;
  protected System.Web.UI.WebControls.LinkButton Button10;
  protected System.Web.UI.WebControls.LinkButton Button12;
  protected System.Web.UI.WebControls.LinkButton Linkbutton1;
  protected System.Web.UI.WebControls.Button Button2;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton1;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton2;
  protected System.Web.UI.WebControls.LinkButton Linkbutton2;
  protected System.Web.UI.WebControls.LinkButton Linkbutton3;
  protected System.Web.UI.WebControls.LinkButton Linkbutton4;
  protected System.Web.UI.WebControls.LinkButton Linkbutton5;
  protected System.Web.UI.WebControls.LinkButton Linkbutton6;
  protected System.Web.UI.WebControls.Button Button5;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton3;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton4;
  protected System.Web.UI.WebControls.LinkButton Linkbutton7;
  protected System.Web.UI.WebControls.LinkButton Linkbutton8;
  protected System.Web.UI.WebControls.LinkButton Linkbutton9;
  protected System.Web.UI.WebControls.LinkButton Linkbutton10;
  protected System.Web.UI.WebControls.LinkButton Linkbutton11;
  protected System.Web.UI.WebControls.Button Button7;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton5;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton6;
  protected System.Web.UI.WebControls.LinkButton Linkbutton12;
  protected System.Web.UI.WebControls.LinkButton Linkbutton13;
  protected System.Web.UI.WebControls.LinkButton Linkbutton14;
  protected System.Web.UI.WebControls.LinkButton Linkbutton15;
  protected System.Web.UI.WebControls.LinkButton Linkbutton16;
  protected System.Web.UI.WebControls.Button Button9;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton7;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton8;
  protected System.Web.UI.WebControls.LinkButton Linkbutton17;
  protected System.Web.UI.WebControls.LinkButton Linkbutton18;
  protected System.Web.UI.WebControls.LinkButton Linkbutton19;
  protected System.Web.UI.WebControls.LinkButton Linkbutton20;
  protected System.Web.UI.WebControls.LinkButton Linkbutton21;
  protected System.Web.UI.WebControls.Button Button11;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton9;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton10;
  protected System.Web.UI.WebControls.LinkButton Linkbutton22;
  protected System.Web.UI.WebControls.LinkButton Linkbutton23;
  protected System.Web.UI.WebControls.LinkButton Linkbutton24;
  protected System.Web.UI.WebControls.LinkButton Linkbutton25;
  protected System.Web.UI.WebControls.LinkButton Linkbutton26;
  protected System.Web.UI.WebControls.Button Button13;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton11;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton12;
  protected System.Web.UI.WebControls.LinkButton Linkbutton27;
  protected System.Web.UI.WebControls.LinkButton Linkbutton28;
  protected System.Web.UI.WebControls.LinkButton Linkbutton29;
  protected System.Web.UI.WebControls.LinkButton Linkbutton30;
  protected System.Web.UI.WebControls.LinkButton Linkbutton31;
  protected System.Web.UI.WebControls.Button Button14;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton13;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton14;
  protected System.Web.UI.WebControls.LinkButton Linkbutton32;
  protected System.Web.UI.WebControls.LinkButton Linkbutton33;
  protected System.Web.UI.WebControls.LinkButton Linkbutton34;
  protected System.Web.UI.WebControls.LinkButton Linkbutton35;
  protected System.Web.UI.WebControls.LinkButton Linkbutton36;
  protected System.Web.UI.WebControls.Button Button15;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton15;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton16;
  protected System.Web.UI.WebControls.LinkButton Linkbutton37;
  protected System.Web.UI.WebControls.LinkButton Linkbutton38;
  protected System.Web.UI.WebControls.LinkButton Linkbutton39;
  protected System.Web.UI.WebControls.LinkButton Linkbutton40;
  protected System.Web.UI.WebControls.LinkButton Linkbutton41;
  protected System.Web.UI.WebControls.Button Button16;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton17;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton18;
  protected System.Web.UI.WebControls.LinkButton Linkbutton42;
  protected System.Web.UI.WebControls.LinkButton Linkbutton43;
  protected System.Web.UI.WebControls.LinkButton Linkbutton44;
  protected System.Web.UI.WebControls.LinkButton Linkbutton45;
  protected System.Web.UI.WebControls.LinkButton Linkbutton46;
  protected System.Web.UI.WebControls.Button Button17;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton19;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton20;
  protected System.Web.UI.WebControls.LinkButton Linkbutton47;
  protected System.Web.UI.WebControls.LinkButton Linkbutton48;
  protected System.Web.UI.WebControls.LinkButton Linkbutton49;
  protected System.Web.UI.WebControls.LinkButton Linkbutton50;
  protected System.Web.UI.WebControls.LinkButton Linkbutton51;
  protected System.Web.UI.WebControls.Button Button18;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton21;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton22;
  protected System.Web.UI.WebControls.LinkButton Linkbutton52;
  protected System.Web.UI.WebControls.LinkButton Linkbutton53;
  protected System.Web.UI.WebControls.LinkButton Linkbutton54;
  protected System.Web.UI.WebControls.LinkButton Linkbutton55;
  protected System.Web.UI.WebControls.LinkButton Linkbutton56;
  protected System.Web.UI.WebControls.Button Button19;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton23;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton24;
  protected System.Web.UI.WebControls.LinkButton Linkbutton57;
  protected System.Web.UI.WebControls.LinkButton Linkbutton58;
  protected System.Web.UI.WebControls.LinkButton Linkbutton59;
  protected System.Web.UI.WebControls.LinkButton Linkbutton60;
  protected System.Web.UI.WebControls.LinkButton Linkbutton61;
  protected System.Web.UI.WebControls.LinkButton Linkbutton62;
  protected System.Web.UI.WebControls.Button Button20;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton25;
  protected Rubicon.Web.UI.Controls.WebButton Webbutton26;
  protected System.Web.UI.WebControls.LinkButton Linkbutton63;
  protected System.Web.UI.WebControls.LinkButton Linkbutton64;
  protected System.Web.UI.WebControls.LinkButton Linkbutton65;
  protected System.Web.UI.WebControls.LinkButton Linkbutton66;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;


	private void Page_Load(object sender, System.EventArgs e)
	{
    if (IsPostBack)
      System.Threading.Thread.Sleep (2000);  
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);
    Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
        "style", 
        Rubicon.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (WxePage), Rubicon.Web.ResourceType.Html, "Style.css"));
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
    this.EnableAbortConfirmation = Rubicon.NullableValueTypes.NaBooleanEnum.True;
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion
}

}

