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
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;
using Remotion.NullableValueTypes;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Utilities;
using Remotion.Globalization;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace OBWTest
{

public class TestForm : Page
{
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected Remotion.Web.UI.Controls.LazyContainer LazyContainer;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue TextField;
  protected System.Web.UI.WebControls.TextBox TextBox1;
  protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator1;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Remotion.Web.UI.Controls.FormGridManager fgm;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue field;
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
    this.PostBackButton.Click += new System.EventHandler(this.PostBackButton_Click);

  }
	#endregion

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad (e);

    if (! IsPostBack)
      TextField.Text = "Foo Bar";
  
    bool ensure = true;
    if (ensure)
      LazyContainer.Ensure();
  }

  public override void Validate()
  {
    base.Validate ();
  }

  private void PostBackButton_Click(object sender, System.EventArgs e)
  {
  
  }


}

}