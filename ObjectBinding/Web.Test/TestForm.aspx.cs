using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Utilities;

namespace OBWTest
{
/// <summary>
/// Summary description for WebFormMK.
/// </summary>
public class WebFormMK : System.Web.UI.Page, IImageUrlResolver

{
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Rubicon.Web.UI.Controls.SmartLabel BocPropertyLabel1;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
  protected Rubicon.Web.UI.Controls.SmartLabel BocPropertyLabel2;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue LastNameField;
  protected Rubicon.Web.UI.Controls.SmartLabel BocPropertyLabel3;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue DateOfBirthField;
  protected Rubicon.Web.UI.Controls.SmartLabel BocPropertyLabel4;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue HeightField;
  protected Rubicon.Web.UI.Controls.SmartLabel BocPropertyLabel5;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue GenderField;
  protected Rubicon.Web.UI.Controls.SmartLabel SmartLabel1;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue MarriageStatusField;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSource reflectionBusinessObjectDataSource1;

	private void Page_Load(object sender, System.EventArgs e)
	{
    Person person;
    string path = Server.MapPath ("person.xml");
    if (System.IO.File.Exists (path))
    {
      person = Person.LoadFromXml (path);
    }
    else
    {
      person = new Person();
      person.FirstName = "Hugo";
      person.LastName = "Meier";
      person.DateOfBirth = new DateTime (1959, 4, 15);
      person.Height = 179;
      person.Income = 2000;
    }

	  reflectionBusinessObjectDataSource1.BusinessObject = person;

    this.DataBind();
    if (!IsPostBack)
    {
      reflectionBusinessObjectDataSource1.LoadValues ();
    }
	}

	#region Web Form Designer generated code
	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);
	}
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.reflectionBusinessObjectDataSource1 = new Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSource();
    this.FirstNameField.TextChanged += new System.EventHandler(this.FirstNameField_TextChanged);
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.GenderField.Init += new System.EventHandler(this.GenderField_Init);
    // 
    // reflectionBusinessObjectDataSource1
    // 
    this.reflectionBusinessObjectDataSource1.BusinessObject = null;
    this.reflectionBusinessObjectDataSource1.EditMode = true;
    this.reflectionBusinessObjectDataSource1.TypeName = "OBWTest.Person, OBWTest";
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SaveButton_Click(object sender, System.EventArgs e)
  {
    if (Page.IsValid)
    {
      reflectionBusinessObjectDataSource1.SaveValues();
      Person person = (Person) reflectionBusinessObjectDataSource1.BusinessObject;
      person.SaveToXml (Server.MapPath ("person.xml"));
    }
  }

  private void RadioButtonList1_SelectedIndexChanged(object sender, System.EventArgs e)
  {
  
  }

  private void GenderField_Init(object sender, System.EventArgs e)
  {
  
  }

  private void FirstNameField_TextChanged(object sender, System.EventArgs e)
  {
  
  }

  /// <summary>
  ///   Interface method: IImageUrlResolver
  /// </summary>
  /// <param name="relativeUrl"></param>
  /// <returns></returns>
  public virtual string GetImageUrl (string relativeUrl)
  {
    //  Build the relative URL appended to the application root
    StringBuilder imageUrlBuilder = new StringBuilder (200);

    //  Insert your own logic to get translate the relatveURL passed to this method
    //  into a relative URL compatible with this applications folder structure.
    imageUrlBuilder.Append (ImageDirectory);
    imageUrlBuilder.Append (relativeUrl);

    //  Join the relative URL with the applications root
    return UrlUtility.Combine (
        HttpContext.Current.Request.ApplicationPath,
        imageUrlBuilder.ToString());
  }

  /// <summary>
  ///   Directory for the images, starting at application root.
  /// </summary>
  protected virtual string ImageDirectory
  { get { return "images/"; } }

}

}
