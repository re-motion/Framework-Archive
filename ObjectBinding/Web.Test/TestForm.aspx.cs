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
using System.Globalization;
using System.Threading;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;
using Rubicon.NullableValueTypes;
using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Utilities;
using Rubicon.Globalization;

namespace OBWTest
{

[MultiLingualResources ("OBWTest.Globalization.WebFormMK")]
public class WebFormMK :
  Page,
  IObjectWithResources, //  Provides the WebForm's ResourceManager via GetResourceManager() 
  IResourceUrlResolver //  Provides the URLs for this WebForm (i.e. to the FormGridManager)
{
  /// <summary> Caches the IResourceManager returned by GetResourceManager. </summary>
  private static IResourceManager s_chachedResourceManager;

  protected System.Web.UI.HtmlControls.HtmlTable FormGrid;
  protected System.Web.UI.WebControls.Button SaveButton;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValue FirstNameField;
  protected Rubicon.ObjectBinding.Reflection.ReflectionBusinessObjectDataSource reflectionBusinessObjectDataSource;
  protected Rubicon.ObjectBinding.Web.Controls.BocTextValueValidator BocTextValueValidator1;
  protected Rubicon.ObjectBinding.Web.Controls.BocEnumValue GenderField;
  protected Rubicon.ObjectBinding.Web.Controls.BocReferenceValue PartnerField;
  protected Rubicon.ObjectBinding.Web.Controls.BocDateTimeValue BirthdayField;
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
      reflectionBusinessObjectDataSource.LoadValues (false);
    }
	}

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
    Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.UserLanguages[0]);

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
      reflectionBusinessObjectDataSource.SaveValues (false);
      Person person = (Person) reflectionBusinessObjectDataSource.BusinessObject;
      person.SaveObject();
      if (person.Partner != null)
        person.Partner.SaveObject();
    }
  }

  /// <summary>
  ///   Interface implementation: IObjectWithResources
  /// </summary>
  /// <returns></returns>
  public virtual IResourceManager GetResourceManager()
  {
    //  chache the resource manager
    lock (typeof(IResourceManager))
    {
      if (s_chachedResourceManager == null)
      {
        s_chachedResourceManager = MultiLingualResourcesAttribute.GetResourceManager (
          this.GetType(), true);
      }
    }  
  
    return s_chachedResourceManager;
  }

  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
  {
    if (ControlHelper.IsDesignMode (this, this.Context))
      return resourceType.Name + "/" + relativeUrl;
    else
      return Server.MapPath (resourceType.Name + "/" + relativeUrl);
  }
}

}