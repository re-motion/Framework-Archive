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
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;
using OBRTest;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

namespace OBWTest.Design
{
public class DesignTestTreeViewForm : DesignTestWxeBasePage
{
  protected Rubicon.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentObject;
  protected Rubicon.Web.UI.Controls.WebButton PostBackButton;
  protected OBRTest.PersonTreeView PersonTreeView;
  protected Rubicon.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

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
    this.ShowAbortConfirmation = Rubicon.Web.UI.ShowAbortConfirmation.Always;
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion
}

}
