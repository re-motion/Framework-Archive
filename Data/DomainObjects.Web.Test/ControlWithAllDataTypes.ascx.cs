using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Rubicon.ObjectBinding;

using Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Domain;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web.Test
{
public class ControlWithAllDataTypes : System.Web.UI.UserControl
{
  protected Rubicon.ObjectBinding.Web.Controls.BocBooleanValue BocBooleanValue1;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue2;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue10;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator7;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue12;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator17;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue Bocdatetimevalue4;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValueValidator BocDateTimeValueValidator4;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue7;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator14;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeValue1;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValueValidator BocDateTimeValueValidator1;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeValue7;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue16;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator11;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue15;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator8;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue1;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue19;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator3;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue11;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator12;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue4;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator5;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue20;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator13;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue1;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator2;
  protected Rubicon.ObjectBinding.Web.Controls.BocBooleanValue BocBooleanValue2;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue3;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue3;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator Boctextvaluevalidator4;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue13;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator18;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue Bocdatetimevalue5;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValueValidator BocDateTimeValueValidator5;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue8;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator15;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeValue2;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValueValidator BocDateTimeValueValidator2;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue24;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator Boctextvaluevalidator23;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue25;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator Boctextvaluevalidator24;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue27;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator Boctextvaluevalidator26;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue22;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator Boctextvaluevalidator21;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue5;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator1;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue28;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator Boctextvaluevalidator27;
  protected Rubicon.ObjectBinding.Web.Controls.BocBooleanValue BocBooleanValue3;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue4;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue21;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator Boctextvaluevalidator20;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue14;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator19;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue Bocdatetimevalue6;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValueValidator BocDateTimeValueValidator6;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue9;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator16;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeValue3;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValueValidator BocDateTimeValueValidator3;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue26;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator Boctextvaluevalidator25;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue18;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator10;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue17;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator Boctextvaluevalidator9;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue23;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator Boctextvaluevalidator22;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue6;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator6;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue29;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator Boctextvaluevalidator28;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue2;
  protected Rubicon.ObjectBinding.Web.Controls.BocReferenceValue BocReferenceValue1;
  protected Rubicon.ObjectBinding.Web.Controls.BocReferenceValue BocReferenceValue2;
  protected Rubicon.ObjectBinding.Web.Controls.BocList BocList1;
  protected Rubicon.ObjectBinding.Web.Controls.BocList BocList2;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.Data.DomainObjects.ObjectBinding.Web.DomainObjectDataSourceControl CurrentObject;
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;

  private ClassWithAllDataTypes _objectWithAllDataTypes;

  public ClassWithAllDataTypes ObjectWithAllDataTypes
  {
    get { return _objectWithAllDataTypes; }
    set { _objectWithAllDataTypes = value; }
  }

	private void Page_Load(object sender, System.EventArgs e)
	{
		CurrentObject.BusinessObject = ObjectWithAllDataTypes;
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
      
      ClientTransaction.Current.Commit ();
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
    this.Load += new System.EventHandler(this.Page_Load);
    this.SaveButton.Click += new EventHandler(SaveButton_Click);

  }
	#endregion
}
}
