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
using Remotion.ObjectBinding.Sample;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;
using Remotion.NullableValueTypes;
using Remotion.ObjectBinding;

using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Globalization;
using Remotion.Web.UI.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

namespace OBWTest.Design
{
public class DesignTestCheckBoxForm : DesignTestWxeBasePage
{
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
  protected Remotion.Web.UI.Controls.WebButton PostBackButton;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox1;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox2;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox3;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox4;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox17;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox18;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox5;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox6;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox7;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox8;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox19;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox9;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox10;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox11;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox12;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox22;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox23;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox13;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox14;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox15;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox16;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox20;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox21;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox24;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox25;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox26;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox27;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox28;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox29;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox30;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox31;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox32;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox33;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox34;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox BocCheckBox35;
  protected Remotion.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

  private void Page_Load(object sender, System.EventArgs e)
	{
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject (personID);
    Person partner = person.Partner;

    CurrentObject.BusinessObject = (IBusinessObject) person;
    CurrentObject.LoadValues (IsPostBack);
  }

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    if (!IsPostBack)
      XmlReflectionBusinessObjectStorageProvider.Current.Reset();
  }

	#region Web Form Designer generated code
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.EnableAbort = false;
    this.ShowAbortConfirmation = Remotion.Web.UI.ShowAbortConfirmation.Always;
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion
}

}
