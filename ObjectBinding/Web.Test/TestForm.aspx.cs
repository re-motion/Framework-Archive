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
using Rubicon.NullableValueTypes;

namespace OBWTest
{
/// <summary>
/// Summary description for WebFormMK.
/// </summary>
public class WebFormMK : System.Web.UI.Page, IImageUrlResolver

{
  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue LastNameField;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue DateOfBirthField;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue HeightField;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue GenderField;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue MarriageStatusField;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
  protected Rubicon.Web.UI.Controls.SmartLabel SmartLabel5;
  protected Rubicon.Web.UI.Controls.SmartLabel SmartLabel2;
  protected Rubicon.Web.UI.Controls.SmartLabel SmartLabel6;
  protected Rubicon.Web.UI.Controls.SmartLabel SmartLabel3;
  protected Rubicon.Web.UI.Controls.SmartLabel SmartLabel4;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator1;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator2;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSource reflectionBusinessObjectDataSource;
  protected System.Web.UI.WebControls.Button TestSetNullButton;

  protected System.Web.UI.WebControls.Button TestSetNewItemButton;
  protected System.Web.UI.WebControls.Button TestReadValueButton;
  protected System.Web.UI.WebControls.Label ReadValueLabel;
  protected Rubicon.ObjectBinding.Web.Controls.BocReferenceValue PartnerField;
  protected System.Web.UI.WebControls.Button PostBackButton;

	private void Page_Load(object sender, System.EventArgs e)
	{
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject (personID);
    Person partner;
    if (person == null)
    {
      person = Person.CreateObject (personID);
      person.FirstName = "Hugo";
      person.LastName = "Meier";
      person.DateOfBirth = new DateTime (1959, 4, 15);
      person.Height = 179;
      person.Income = 2000;

      partner = person.Partner = Person.CreateObject();
      partner.FirstName = "Sepp";
      partner.LastName = "Forcher";
    }
    else
    {
      partner = person.Partner;
    }

    reflectionBusinessObjectDataSource.BusinessObject = person;

    this.DataBind();
    if (!IsPostBack)
    {
      reflectionBusinessObjectDataSource.LoadValues ();
    }
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
    this.reflectionBusinessObjectDataSource = new Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSource();
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.TestSetNullButton.Click += new System.EventHandler(this.TestSetNullButton_Click);
    this.TestSetNewItemButton.Click += new System.EventHandler(this.TestSetNewItemButton_Click);
    this.TestReadValueButton.Click += new System.EventHandler(this.TestReadValueButton_Click);
    // 
    // reflectionBusinessObjectDataSource
    // 
    this.reflectionBusinessObjectDataSource.BusinessObject = null;
    this.reflectionBusinessObjectDataSource.EditMode = true;
    this.reflectionBusinessObjectDataSource.TypeName = "OBWTest.Person, OBWTest";
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SaveButton_Click(object sender, System.EventArgs e)
  {
    bool isValid = FormGridManager.Validate();
    if (isValid)
    {
      reflectionBusinessObjectDataSource.SaveValues();
      Person person = (Person) reflectionBusinessObjectDataSource.BusinessObject;
      person.SaveObject();
      if (person.Partner != null)
        person.Partner.SaveObject();
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

  private void TestSetNullButton_Click(object sender, System.EventArgs e)
  {
    PartnerField.Value = null;
  }

  private void TestSetNewItemButton_Click(object sender, System.EventArgs e)
  {
    Person person = Person.CreateObject (Guid.NewGuid());
    person.LastName = person.ID.ToByteArray()[15].ToString();
    person.FirstName = "--";

    PartnerField.Value = person;
  }

  private void TestReadValueButton_Click(object sender, System.EventArgs e)
  {
    if (PartnerField.Value != null)
      ReadValueLabel.Text = PartnerField.Value.ToString();
    else
      ReadValueLabel.Text = "not set";
  }

  /// <summary>
  ///   Directory for the images, starting at application root.
  /// </summary>
  protected virtual string ImageDirectory
  { get { return "images/"; } }

}

}
