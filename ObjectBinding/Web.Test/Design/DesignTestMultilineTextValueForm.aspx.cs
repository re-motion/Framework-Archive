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
public class DesignTestMultilineTextValueForm : DesignTestWxeBasePage
{
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
  protected Remotion.Web.UI.Controls.WebButton PostBackButton;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue1;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue36;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue37;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue2;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue38;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue39;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue3;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue40;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue41;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue4;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue42;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue43;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue17;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue18;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue5;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue44;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue45;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue6;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue46;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue47;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue7;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue48;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue49;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue51;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue50;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue52;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue8;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue19;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue9;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue10;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue11;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue12;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue22;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue23;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue13;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue14;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue15;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue16;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue20;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue21;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue24;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue25;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue26;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue27;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue28;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue29;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue30;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue31;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue32;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue33;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue34;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue BocMultilineTextValue35;
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
