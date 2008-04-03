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
public class DesignTestEnumValueForm : DesignTestWxeBasePage
{
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
  protected Remotion.Web.UI.Controls.WebButton PostBackButton;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue1;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue2;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue3;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue4;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue5;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue6;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue7;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue8;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue9;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue10;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue11;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue12;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue13;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue14;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue1;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue36;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue37;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue2;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue38;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue39;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue3;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue40;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue41;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue4;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue42;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue43;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue17;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue18;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue44;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue45;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue46;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue47;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue48;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue49;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue51;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue50;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue52;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue19;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue22;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue23;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue15;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue16;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue20;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue21;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue24;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue25;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue26;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue27;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue28;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue29;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue30;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue31;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue32;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue33;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue34;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue BocEnumValue35;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue53;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue54;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue55;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue56;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue57;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue58;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue59;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue60;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue61;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue62;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue63;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue64;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue65;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue Bocenumvalue66;
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
