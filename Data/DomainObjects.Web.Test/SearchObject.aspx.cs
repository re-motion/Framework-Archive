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

using Rubicon.Data.DomainObjects.Web.Test.Domain;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Data.DomainObjects.Web.Test.WxeFunctions;

namespace Rubicon.Data.DomainObjects.Web.Test
{
public class SearchObjectPage : WxePage
{
  protected Rubicon.Web.UI.Controls.FormGridManager SearchFormGridManager;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue StringPropertyValue;
  protected Rubicon.Data.DomainObjects.ObjectBinding.Web.DomainObjectDataSourceControl FoundObjects;
  protected System.Web.UI.WebControls.Button SearchButton;
  protected Rubicon.ObjectBinding.Web.Controls.BocList ResultList;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BytePropertyFromTextBox;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BytePropertyToTextBox;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue EnumPropertyValue;
  protected System.Web.UI.HtmlControls.HtmlTable SearchFormGrid;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue DatePropertyFromValue;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue DatePropertyToValue;
  protected Rubicon.Data.DomainObjects.ObjectBinding.Web.SearchObjectDataSourceControl CurrentSearchObject;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue DateTimeFromValue;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeValue2;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

  private SearchFunction MyFunction 
  {
    get { return (SearchFunction) CurrentFunction; }
  }

  private void Page_Load(object sender, System.EventArgs e)
	{
    ResultList.Value = MyFunction.Result;

    CurrentSearchObject.BusinessObject = MyFunction.SearchObject;
    CurrentSearchObject.LoadValues (IsPostBack);
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
    this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
    this.ResultList.EditedRowSaved += new Rubicon.ObjectBinding.Web.Controls.BocListItemEventHandler(this.ResultList_EditedRowSaved);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SearchButton_Click(object sender, System.EventArgs e)
  {
    if (SearchFormGridManager.Validate ())
    {
      CurrentSearchObject.SaveValues (false);
      
      MyFunction.Requery ();
      ResultList.Value = MyFunction.Result;
      ResultList.LoadValue (false);
    }
  }

  private void ResultList_EditedRowSaved(object sender, Rubicon.ObjectBinding.Web.Controls.BocListItemEventArgs e)
  {
    ClientTransaction.Current.Commit ();
  }
}
}
