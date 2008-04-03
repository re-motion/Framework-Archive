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
public class DesignTestBooleanValueForm : DesignTestWxeBasePage
{
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
  protected Remotion.Web.UI.Controls.WebButton PostBackButton;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue1;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue2;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue3;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue4;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue17;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue18;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue5;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue6;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue7;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue8;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue19;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue9;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue10;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue11;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue12;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue22;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue23;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue13;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue14;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue15;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue16;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue20;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue21;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue24;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue25;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue26;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue27;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue28;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue29;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue30;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue31;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue32;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue33;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue34;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue BocBooleanValue35;
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
