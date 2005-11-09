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
public class UndefinedEnumTestPage : WxePage
{
  protected System.Web.UI.HtmlControls.HtmlTable SearchFormGrid;
  protected Rubicon.Data.DomainObjects.ObjectBinding.Web.DomainObjectDataSourceControl ExistingObjectWithUndefinedEnumDataSource;
  protected Rubicon.Data.DomainObjects.ObjectBinding.Web.DomainObjectDataSourceControl NewObjectWithUndefinedEnumDataSource;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue ExistingObjectEnumProperty;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue NewObjectEnumProperty;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue SearchObjectEnumProperty;
  protected Rubicon.Data.DomainObjects.ObjectBinding.Web.SearchObjectDataSourceControl SearchObjectWithUndefinedEnumDataSource;
  protected System.Web.UI.WebControls.Button TestButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

  private UndefinedEnumTestFunction MyFunction 
  {
    get { return (UndefinedEnumTestFunction) CurrentFunction; }
  }

  private void Page_Load(object sender, System.EventArgs e)
	{
    NewObjectWithUndefinedEnumDataSource.BusinessObject = MyFunction.NewObjectWithUndefinedEnum;
    ExistingObjectWithUndefinedEnumDataSource.BusinessObject = MyFunction.ExistingObjectWithUndefinedEnum;
    SearchObjectWithUndefinedEnumDataSource.BusinessObject = MyFunction.SearchObjectWithUndefinedEnum;

    NewObjectWithUndefinedEnumDataSource.LoadValues (IsPostBack);
    ExistingObjectWithUndefinedEnumDataSource.LoadValues (IsPostBack);
    SearchObjectWithUndefinedEnumDataSource.LoadValues (IsPostBack);
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
    this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void TestButton_Click(object sender, System.EventArgs e)
  {
    if (FormGridManager.Validate ())
    {
      NewObjectWithUndefinedEnumDataSource.SaveValues (false);
      ExistingObjectWithUndefinedEnumDataSource.SaveValues (false);
      SearchObjectWithUndefinedEnumDataSource.SaveValues (false);

      AreEqual (UndefinedEnum.Value1, MyFunction.NewObjectWithUndefinedEnum.UndefinedEnum);
      AreEqual (UndefinedEnum.Value1, MyFunction.ExistingObjectWithUndefinedEnum.UndefinedEnum);
      if (!Enum.IsDefined (typeof (UndefinedEnum), MyFunction.SearchObjectWithUndefinedEnum.UndefinedEnum))
        throw new TestFailureException ("SearchObjectWithUndefinedEnum.UndefinedEnum has an invalid value.");

      ExecuteNextStep ();
    }
  }

  private void AreEqual (UndefinedEnum expected, UndefinedEnum actual)
  {
    if (expected != actual)
      throw new TestFailureException (string.Format ("Actual value '{0}' does not match expected value '{1}'.", actual, expected));
  }
}
}
