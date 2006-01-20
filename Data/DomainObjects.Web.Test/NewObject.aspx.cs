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

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.Test.Domain;
using Rubicon.Data.DomainObjects.Web.Test.WxeFunctions;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.Web.Test
{
	public class NewObjectPage : WxePage
	{
    protected Rubicon.Web.UI.Controls.HtmlHeadContents Htmlheadcontents1;

    protected ControlWithAllDataTypes ControlWithAllDataTypesControl;

    private NewObjectFunction MyFunction
    {
      get { return (NewObjectFunction) CurrentFunction; }
    }

		private void Page_Load(object sender, System.EventArgs e)
		{
      ControlWithAllDataTypesControl.ObjectWithAllDataTypes = MyFunction.ObjectWithAllDataTypes;
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
}
