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
public class DesignTestTextValueForm : DesignTestWxeBasePage
{
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.Web.UI.Controls.WebButton PostBackButton;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue1;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue36;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue37;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue2;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue38;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue39;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue3;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue40;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue41;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue4;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue42;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue43;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue17;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue18;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue5;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue44;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue45;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue6;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue46;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue47;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue7;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue48;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue49;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue51;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue50;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue Boctextvalue52;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue8;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue19;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue9;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue10;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue11;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue12;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue22;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue23;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue13;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue14;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue15;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue16;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue20;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue21;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue24;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue25;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue26;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue27;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue28;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue29;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue30;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue31;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue32;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue33;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue34;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue BocTextValue35;
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
