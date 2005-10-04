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
public class DesignTestEnumValueForm : DesignTestWxeBasePage
{
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.Web.UI.Controls.WebButton PostBackButton;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue1;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue2;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue3;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue4;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue5;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue6;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue7;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue8;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue9;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue10;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue11;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue12;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue13;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue14;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue1;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue36;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue37;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue2;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue38;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue39;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue3;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue40;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue41;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue4;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue42;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue43;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue17;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue18;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue44;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue45;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue46;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue47;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue48;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue49;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue51;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue50;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue52;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue19;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue22;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue23;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue15;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue16;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue20;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue21;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue24;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue25;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue26;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue27;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue28;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue29;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue30;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue31;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue32;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue33;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue34;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue BocEnumValue35;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue53;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue54;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue55;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue56;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue57;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue58;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue59;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue60;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue61;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue62;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue63;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue64;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue65;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue Bocenumvalue66;
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
