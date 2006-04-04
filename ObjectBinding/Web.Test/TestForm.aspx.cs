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
using Rubicon.ObjectBinding.Web.UI.Controls;

namespace OBWTest
{

public class TestForm : Page
{
  protected System.Web.UI.WebControls.Button PostBackButton;
  protected Rubicon.Web.UI.Controls.LazyContainer LazyContainer;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue TextField;
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


}

}