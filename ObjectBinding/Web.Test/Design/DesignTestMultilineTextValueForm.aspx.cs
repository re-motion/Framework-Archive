using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Reflection;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;
using OBRTest;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

namespace OBWTest.Design
{
public class DesignTestMultilineTextValueForm : DesignTestWxeBasePage
{
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.Web.UI.Controls.WebButton PostBackButton;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue1;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue36;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue37;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue2;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue38;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue39;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue3;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue40;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue41;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue4;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue42;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue43;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue17;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue18;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue5;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue44;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue45;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue6;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue46;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue47;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue7;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue48;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue49;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue51;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue50;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue52;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue8;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue19;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue9;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue10;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue11;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue12;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue22;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue23;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue13;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue14;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue15;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue16;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue20;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue21;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue24;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue25;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue26;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue27;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue28;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue29;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue30;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue31;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue32;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue33;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue34;
  protected Rubicon.ObjectBinding.Web.Controls.BocMultilineTextValue BocMultilineTextValue35;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

  private void Page_Load(object sender, System.EventArgs e)
	{
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject (personID);
    Person partner = person.Partner;
    
    ReflectionBusinessObjectDataSourceControl.BusinessObject = person;
    ReflectionBusinessObjectDataSourceControl.LoadValues (IsPostBack);
  }

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    if (!IsPostBack)
      Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectStorage.Reset();
  }

	#region Web Form Designer generated code
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.EnableAbort = Rubicon.NullableValueTypes.NaBooleanEnum.False;
    this.EnableAbortConfirmation = Rubicon.NullableValueTypes.NaBooleanEnum.True;
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion
}

}
