using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

using log4net;

using Rubicon.Globalization;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.UI.Utilities;

namespace FormGrid.Sample
{

/// <summary>
/// Summary description for WebForm.
/// </summary>
[MultiLingualResources("FormGrid.Sample.Globalization.WebForm")]
public class WebForm : 
  System.Web.UI.Page,
  IObjectWithResources, 
  IFormGridRowProvider,
  IImageUrlResolver,
  IHelpUrlResolver

{
  private static IResourceManager s_chachedResourceManager;

	private static readonly ILog s_log = LogManager.GetLogger (
    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  private FormGridRowProvider _formGridRowProvider;

	private void Page_Load(object sender, System.EventArgs e)
	{
		// Put user code to initialize the page here
	}
	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    _formGridRowProvider = new FormGridRowProvider();
	}
	

	#region Web Form Designer generated code

  protected System.Web.UI.WebControls.Label PersonDataLabel;
  protected System.Web.UI.WebControls.Label NameLabel;
  protected System.Web.UI.WebControls.TextBox NameField;
  protected System.Web.UI.WebControls.DropDownList GenderList;
  protected System.Web.UI.WebControls.TextBox ZipField;
  protected System.Web.UI.WebControls.TextBox PlaceField;
  protected System.Web.UI.WebControls.Label AddressLabel;
  protected System.Web.UI.WebControls.TextBox AddressField;
  protected System.Web.UI.WebControls.TextBox TextBox1;
  protected System.Web.UI.WebControls.TextBox TextBox2;
  protected System.Web.UI.HtmlControls.HtmlTable MainFormGrid;
  protected Rubicon.Web.UI.Controls.FormGridLabel FormGridLabel;
  protected Rubicon.Web.UI.Controls.ValidationStateViewer ValidationStateViewer;
  protected System.Web.UI.WebControls.CompareValidator NameFieldCompareValidator;
  protected System.Web.UI.WebControls.CompareValidator GenderListCompareValidator;
  protected System.Web.UI.WebControls.Table CompaniesTable;
  protected System.Web.UI.WebControls.RequiredFieldValidator CompaniesTableRequiredFieldValidator;
  protected System.Web.UI.WebControls.Button submitButton;
  protected Rubicon.Web.UI.Controls.FormGridManager GlobalFormGridManager;

  /// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);
    this.PreRender += new System.EventHandler(this.WebForm_PreRender);

  }
	#endregion

    /// <summary>
  /// Returns the <c>IResourceManager</c> for this page
  /// </summary>
  /// <returns></returns>
  public virtual IResourceManager GetResourceManager()
  {
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

  public virtual StringCollection GetListOfHiddenRows (string table)
  {
    return _formGridRowProvider.GetListOfHiddenRows (table);
  }

  public virtual FormGridRowPrototypeCollection GetListOfFormGridRowPrototypes (string table)
  {
    return _formGridRowProvider.GetListOfFormGridRowPrototypes (table);
  }

  public virtual string GetImageUrl (string relativeUrl)
  {
    StringBuilder imageUrlBuilder = new StringBuilder (200);

    imageUrlBuilder.Append (ImageDirectory);
    imageUrlBuilder.Append (relativeUrl);

    return UrlUtility.Combine (
        HttpContext.Current.Request.ApplicationPath,
        imageUrlBuilder.ToString());
  }

  public virtual string GetHelpUrl (string relativeUrl)
  {
    StringBuilder helpUrlBuilder = new StringBuilder (200);

    helpUrlBuilder.Append (HelpDirectory);
    helpUrlBuilder.Append (relativeUrl);

    return UrlUtility.Combine (
        HttpContext.Current.Request.ApplicationPath,
        helpUrlBuilder.ToString());
  }

  private void submitButton_Click(object sender, System.EventArgs e)
  {
    GlobalFormGridManager.Validate();
  }

  private void WebForm_PreRender(object sender, System.EventArgs e)
  {
    ResourceDispatcher.Dispatch (this);
  }

  /// <summary>
  ///   Directory for the images, starting at application root.
  /// </summary>
  protected virtual string ImageDirectory
  { get { return "images/"; } }

  /// <summary>
  ///   Directory for the help files, starting at application root.
  /// </summary>
  protected virtual string HelpDirectory
  { get { return "help/"; } }

}
}
