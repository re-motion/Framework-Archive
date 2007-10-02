using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.Web.Test.Domain;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.Web.Test
{
public class ControlWithAllDataTypes : System.Web.UI.UserControl
{
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue1;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue2;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue10;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator7;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue12;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator17;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue Bocdatetimevalue4;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValueValidator BocDateTimeValueValidator4;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue7;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator14;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeValue1;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValueValidator BocDateTimeValueValidator1;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeValue7;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue16;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator11;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue15;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator8;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue1;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue19;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator3;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue11;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator12;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue4;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator5;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue20;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator13;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue1;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator2;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue2;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue3;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue3;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator Boctextvaluevalidator4;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue13;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator18;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue Bocdatetimevalue5;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValueValidator BocDateTimeValueValidator5;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue8;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator15;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeValue2;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValueValidator BocDateTimeValueValidator2;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue24;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator Boctextvaluevalidator23;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue25;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator Boctextvaluevalidator24;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue27;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator Boctextvaluevalidator26;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue22;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator Boctextvaluevalidator21;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue5;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator1;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue28;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator Boctextvaluevalidator27;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue3;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue4;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue21;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator Boctextvaluevalidator20;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue14;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator19;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue Bocdatetimevalue6;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValueValidator BocDateTimeValueValidator6;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue9;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator16;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeValue3;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocDateTimeValueValidator BocDateTimeValueValidator3;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue26;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator Boctextvaluevalidator25;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue18;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator10;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue17;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator Boctextvaluevalidator9;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue23;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator Boctextvaluevalidator22;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue6;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator BocTextValueValidator6;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue29;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValueValidator Boctextvaluevalidator28;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue2;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocReferenceValue BocReferenceValue1;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocReferenceValue BocReferenceValue2;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocList BocList1;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocList BocList2;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue5;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue1;
  protected Rubicon.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue2;

  private ClassWithAllDataTypes _objectWithAllDataTypes;

  public ClassWithAllDataTypes ObjectWithAllDataTypes
  {
    get { return _objectWithAllDataTypes; }
    set { _objectWithAllDataTypes = value; }
  }

	private void Page_Load(object sender, System.EventArgs e)
	{
		CurrentObject.BusinessObject = (IBusinessObject) ObjectWithAllDataTypes;
    CurrentObject.LoadValues (IsPostBack);
	}

  public bool Validate ()
  {
    return FormGridManager.Validate ();
  }

  public void Save ()
  {
    CurrentObject.SaveValues (false);
  }

  private void SaveButton_Click(object sender, EventArgs e)
  {
    if (Validate ())
    {
      Save ();
      
      ClientTransactionScope.CurrentTransaction.Commit ();
      ((WxePage) this.Page).ExecuteNextStep ();
    }
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
	///		Required method for Designer support - do not modify
	///		the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion
}
}
