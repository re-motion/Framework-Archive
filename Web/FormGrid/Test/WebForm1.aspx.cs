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
using Rubicon.Web.Utilities;
using Rubicon.Web.UI;
using Rubicon.Web;
using Rubicon.Collections;

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

  private AutoInitHashtable _listOfFormGridRowInfos =
      new AutoInitHashtable (typeof (FormGridRowInfoCollection));
  protected Rubicon.Web.UI.Controls.ValidationStateViewer ValidationStateViewer1;
  protected Rubicon.Web.UI.Controls.FormGridManager FormGridManager1;
  private AutoInitHashtable _listOfHiddenRows = 
      new AutoInitHashtable (typeof (StringCollection));

  protected HtmlGenericControl HtmlHead;
  protected Rubicon.Web.UI.Controls.FormGridLabel FormGridLabel;
  protected System.Web.UI.WebControls.Label ToBeHiddenLabel;
  protected System.Web.UI.WebControls.TextBox ToBeHiddenTextBox;
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
  protected Rubicon.Web.UI.Controls.FormGridLabel AddressLabel;
  protected Rubicon.Web.UI.Controls.ValidationStateViewer ValidationStateViewer;
	
  override protected void OnLoad(System.EventArgs e)
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

    StringCollection hiddenRows = (StringCollection)_listOfHiddenRows[TableDesignTimeFormGrid];

    hiddenRows.Add (ToBeHiddenTextBox.ID);
  
    FormGridRowInfoCollection newRows = 
        (FormGridRowInfoCollection)_listOfFormGridRowInfos[TableDesignTimeFormGrid];

    TextBox textBox = new TextBox();
    textBox.ID = "MyNewTextBox";
    textBox.Text = "Eine neue Zeile";

    //  A new row
    newRows.Add (new FormGridRowInfo(
      textBox, 
      FormGridRowInfo.RowType.ControlInRowWithLabel, 
      AddressField.ID, 
      FormGridRowInfo.RowPosition.AfterRowWithID));

    textBox = new TextBox();
    textBox.ID = "MyOtherNewTextBox";
    textBox.Text = "Noch eine neue Zeile, diesmal zweizeilig.";
    textBox.Width = Unit.Parse ("100%");
    textBox.Height = Unit.Parse ("3em");

    //  A second new row
    newRows.Add (new FormGridRowInfo(
      textBox, 
      FormGridRowInfo.RowType.ControlInRowAfterLabel, 
      SomeBigMultiFieldThing.ID, 
      FormGridRowInfo.RowPosition.BeforeRowWithID));

    string url = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (FormGridManager), ResourceType.Html, "FormGrid.css");
    HtmlHeadAppender.Current.RegisterStylesheetLink ("FormGrid_Style", url);

		InitializeComponent();
		base.OnInit(e);
	}

	#region Web Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.GenderList.SelectedIndexChanged += new System.EventHandler(this.GenderList_SelectedIndexChanged);
    this.DropDownList2.SelectedIndexChanged += new System.EventHandler(this.DropDownList2_SelectedIndexChanged);
    this.Button1.Click += new System.EventHandler(this.Button1_Click);

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

  public virtual StringCollection GetHiddenRows (HtmlTable table)
  {
    return (StringCollection) _listOfHiddenRows[table];
  }

  public virtual FormGridRowInfoCollection GetAdditionalRows (HtmlTable table)
  {
    return (FormGridRowInfoCollection) _listOfFormGridRowInfos[table];
  }

  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
  {
    return Page.ResolveUrl (resourceType.Name + "/" + relativeUrl);
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

  protected override void OnPreRender (System.EventArgs e)
  {
    base.OnPreRender (e);
    ResourceDispatcher.Dispatch (this, this.GetResourceManager());
  }
	
  protected override void RenderChildren(HtmlTextWriter writer)
  {
    HtmlHeadAppender.Current.EnsureAppended (HtmlHead.Controls);
    base.RenderChildren (writer);
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