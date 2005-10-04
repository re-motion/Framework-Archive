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
public class DesignTestCheckBoxForm : DesignTestWxeBasePage
{
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSourceControl ReflectionBusinessObjectDataSourceControl;
  protected Rubicon.Web.UI.Controls.WebButton PostBackButton;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox1;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox2;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox3;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox4;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox17;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox18;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox5;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox6;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox7;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox8;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox19;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox9;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox10;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox11;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox12;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox22;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox23;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox13;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox14;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox15;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox16;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox20;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox21;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox24;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox25;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox26;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox27;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox28;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox29;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox30;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox31;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox32;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox33;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox34;
  protected Rubicon.ObjectBinding.Web.Controls.BocCheckBox BocCheckBox35;
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
