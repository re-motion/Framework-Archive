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
public class DesignTestTextValueForm : DesignTestWxeBasePage
{
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
  protected Remotion.Web.UI.Controls.WebButton PostBackButton;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue1;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue36;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue37;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue2;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue38;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue39;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue3;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue40;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue41;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue4;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue42;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue43;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue17;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue18;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue5;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue44;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue45;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue6;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue46;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue47;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue7;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue48;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue49;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue51;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue50;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue Boctextvalue52;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue8;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue19;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue9;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue10;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue11;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue12;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue22;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue23;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue13;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue14;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue15;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue16;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue20;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue21;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue24;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue25;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue26;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue27;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue28;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue29;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue30;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue31;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue32;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue33;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue34;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BocTextValue35;
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
