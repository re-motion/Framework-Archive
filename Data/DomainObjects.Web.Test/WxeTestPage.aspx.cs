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

using Rubicon.Data.DomainObjects.Web.Test.WxeFunctions;
using Rubicon.Data.DomainObjects.Web.Test.Domain;

namespace Rubicon.Data.DomainObjects.Web.Test
{
	/// <summary>
	/// Summary description for FirstPage.
	/// </summary>
	public class FirstPage : WxePage
	{
    protected WxeTestPageFunction CurrentWxeTestPageFunction
    {
      get { return (WxeTestPageFunction) CurrentFunction; }
    }

    protected Rubicon.Data.DomainObjects.ObjectBinding.Web.DomainObjectDataSourceControl ClassWithAllDataTypesDataSource;
    protected Rubicon.ObjectBinding.Web.Controls.BocTextValue StringValue;
    protected System.Web.UI.WebControls.Button OpenWithNewClientTransactionButton;
    protected System.Web.UI.WebControls.Button OpenWithSameClientTransactionButton;
    protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
    protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue1;
  
		private void Page_Load(object sender, System.EventArgs e)
		{
      ClassWithAllDataTypesDataSource.BusinessObject = ClassWithAllDataTypes.GetObject (CurrentWxeTestPageFunction.ClassWithAllDataTypesID);
      ClassWithAllDataTypesDataSource.LoadValues (IsPostBack);
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
      this.OpenWithNewClientTransactionButton.Click += new System.EventHandler(this.OpenWithNewClientTransactionButton_Click);
      this.OpenWithSameClientTransactionButton.Click += new System.EventHandler(this.OpenWithSameClientTransactionButton_Click);
      this.Load += new System.EventHandler(this.Page_Load);

    }
		#endregion

    private void OpenWithNewClientTransactionButton_Click(object sender, System.EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        WxeTestPageFunction wxeTestPageFunction = new WxeTestPageFunction (
            CurrentWxeTestPageFunction.ClassWithAllDataTypesID);

        ExecuteFunction (wxeTestPageFunction, "_blank", OpenWithNewClientTransactionButton, false);
      }
    }

    private void OpenWithSameClientTransactionButton_Click(object sender, System.EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        WxeTestPageFunction wxeTestPageFunction = new WxeTestPageFunction (
            CurrentWxeTestPageFunction.ClassWithAllDataTypesID, 
            CurrentWxeTestPageFunction.ClientTransaction);

        ExecuteFunction (wxeTestPageFunction, "_blank", OpenWithSameClientTransactionButton, false);
      }
    }

	}
}
