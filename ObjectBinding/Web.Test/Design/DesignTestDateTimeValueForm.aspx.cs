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
public class DesignTestDateTimeValueForm : DesignTestWxeBasePage
{
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
  protected Remotion.Web.UI.Controls.WebButton PostBackButton;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale1;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale2;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale3;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale4;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale17;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale18;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale5;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale6;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale8;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale19;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale24;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale25;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale26;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale27;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale28;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale29;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale36;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale37;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale38;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale39;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale40;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale41;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale42;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale43;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale44;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale45;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale46;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale47;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale7;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale48;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale49;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale51;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale50;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale52;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale9;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale10;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale11;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale12;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale22;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale23;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale13;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale14;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale15;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale16;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale20;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale21;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale30;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale31;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale32;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale33;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale34;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeVale35;
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
