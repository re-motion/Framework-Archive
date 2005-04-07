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

using Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Domain;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web.Test
{
public class SearchObjectPage : System.Web.UI.Page
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
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

	private void Page_Load(object sender, System.EventArgs e)
	{
    if (!IsPostBack)
      Session["SearchObject"] = new ClassWithAllDataTypesSearch ();

    CurrentSearchObject.BusinessObject = (ClassWithAllDataTypesSearch) Session["SearchObject"];;
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
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SearchButton_Click(object sender, System.EventArgs e)
  {
    if (SearchFormGridManager.Validate ())
    {
      CurrentSearchObject.SaveValues (false);
      ClassWithAllDataTypesSearch searchObject = (ClassWithAllDataTypesSearch) CurrentSearchObject.BusinessObject;
      ResultList.Value = ClientTransaction.Current.QueryManager.GetCollection (searchObject.CreateQuery ());
      ResultList.LoadValue (false);
    }
  }
}
}
