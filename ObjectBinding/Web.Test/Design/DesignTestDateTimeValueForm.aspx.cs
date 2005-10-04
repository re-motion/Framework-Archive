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
public class DesignTestDateTimeValueForm : DesignTestWxeBasePage
{
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.Web.UI.Controls.WebButton PostBackButton;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale1;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale2;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale3;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale4;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale17;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale18;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale5;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale6;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale8;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale19;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale24;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale25;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale26;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale27;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale28;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale29;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale36;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale37;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale38;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale39;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale40;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale41;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale42;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale43;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale44;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale45;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale46;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale47;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale7;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale48;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale49;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale51;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale50;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale52;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale9;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale10;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale11;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale12;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale22;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale23;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale13;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale14;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale15;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale16;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale20;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale21;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale30;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale31;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale32;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale33;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale34;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BocDateTimeVale35;
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
