using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
using Rubicon.Web.UI;

namespace FormGrid.Test
{
/// <summary>
/// Summary description for WebForm1.
/// </summary>
[MultiLingualResources("FormGrid.Test.Globalization.WebForm1")]
public class WebForm1 : 
  Page,
  IObjectWithResources, 
  IFormGridRowProvider,
  IResourceUrlResolver
{
  private static IResourceManager s_chachedResourceManager;

	private static readonly ILog s_log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
  protected Rubicon.Web.UI.Controls.FormGridLabel FormGridLabel;
  protected System.Web.UI.WebControls.Label ToBeHiddenLabel;
  protected System.Web.UI.WebControls.TextBox ToBeHiddenTextBox;

  private FormGridRowProvider _formGridRowProvider;

	private void Page_Load(object sender, System.EventArgs e)
	{

    if (!IsPostBack)
    {  
      TableDesignTimeFormGrid.Attributes["class"] = "mytest";
      PlaceField.CssClass = "mytest";
      GlobalFormGridManager.ControlsColumn = 3;
      GlobalFormGridManager.LabelsColumn = 1;

      GenderList.Visible = false;
    }
		// Put user code to initialize the page here
	}

  override protected void OnInit(EventArgs e)
	{
    _formGridRowProvider = new FormGridRowProvider();

    StringCollection hiddenRows = _formGridRowProvider.GetListOfHiddenRows (
      TableDesignTimeFormGrid.ID);

    hiddenRows.Add (ToBeHiddenTextBox.ID);
  
    FormGridRowPrototypeCollection newRows = _formGridRowProvider.GetListOfFormGridRowPrototypes (
      TableDesignTimeFormGrid.ID);

    TextBox textBox = new TextBox();
    textBox.ID = "MyNewTextBox";
    textBox.Text = "Eine neue Zeile";

    //  A new row
    newRows.Add (new FormGridRowPrototype(
      textBox, 
      FormGridRowPrototype.RowType.ControlInRowWithLabel, 
      AddressField.ID, 
      FormGridRowPrototype.RowPosition.AfterRowWithID));

    textBox = new TextBox();
    textBox.ID = "MyOtherNewTextBox";
    textBox.Text = "Noch eine neue Zeile, diesmal zweizeilig.";
    textBox.Width = Unit.Parse ("100%");
    textBox.Height = Unit.Parse ("3em");

    //  A second new row
    newRows.Add (new FormGridRowPrototype(
      textBox, 
      FormGridRowPrototype.RowType.ControlInRowAfterLabel, 
      SomeBigMultiFieldThing.ID, 
      FormGridRowPrototype.RowPosition.BeforeRowWithID));

		InitializeComponent();
		base.OnInit(e);
	}

	#region Web Form Designer generated code

  protected System.Web.UI.WebControls.DropDownList DropDownList2;
  protected System.Web.UI.WebControls.TextBox TextBox4;
  protected System.Web.UI.WebControls.TextBox TextBox5;
  protected System.Web.UI.HtmlControls.HtmlTable TableRunTime;
  protected System.Web.UI.WebControls.Label PersonDataLabel;
  protected System.Web.UI.WebControls.Label NameLabel;
  protected System.Web.UI.WebControls.TextBox NameField;
  protected System.Web.UI.WebControls.CompareValidator CompareValidator1;
  protected System.Web.UI.WebControls.DropDownList GenderList;
  protected System.Web.UI.WebControls.TextBox ZipField;
  protected System.Web.UI.WebControls.TextBox PlaceField;
  protected System.Web.UI.WebControls.Label AddressLabel;
  protected System.Web.UI.WebControls.TextBox AddressField;
  protected System.Web.UI.WebControls.TextBox TextBox1;
  protected System.Web.UI.WebControls.TextBox TextBox2;
  protected System.Web.UI.WebControls.Table SomeBigMultiFieldThing;
  protected System.Web.UI.WebControls.Button Button1;
  protected System.Web.UI.HtmlControls.HtmlTable TableDesignTimeFormGrid;
  protected System.Web.UI.WebControls.CompareValidator CompareValidator2;
  protected System.Web.UI.WebControls.CompareValidator CompareValidator3;
  protected System.Web.UI.WebControls.CompareValidator Comparevalidator4;
  protected Rubicon.Web.UI.Controls.FormGridManager GlobalFormGridManager;
  protected System.Web.UI.WebControls.Label Label1;
  protected Rubicon.Web.UI.Controls.ValidationStateViewer ValidationStateViewer;
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.GenderList.SelectedIndexChanged += new System.EventHandler(this.GenderList_SelectedIndexChanged);
    this.DropDownList2.SelectedIndexChanged += new System.EventHandler(this.DropDownList2_SelectedIndexChanged);
    this.Button1.Click += new System.EventHandler(this.Button1_Click);
    this.Load += new System.EventHandler(this.Page_Load);
    this.PreRender += new System.EventHandler(this.WebForm1_PreRender);

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

  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
  {
    return Server.MapPath (resourceType.Name + "/" + relativeUrl);
  }

  private void Button1_Click(object sender, System.EventArgs e)
  {
    GlobalFormGridManager.Validate();
  }

  private void GenderList_SelectedIndexChanged(object sender, System.EventArgs e)
  {
  
  }

  private void DropDownList2_SelectedIndexChanged(object sender, System.EventArgs e)
  {
  
  }

  private void WebForm1_PreRender(object sender, System.EventArgs e)
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