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

using Rubicon.Globalization;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.Utilities;
using Rubicon.Web;
using Rubicon.Collections;

namespace FormGrid.Sample
{

/// <summary>
/// Summary description for WebForm.
/// </summary>

//  The resource file used by the page and subsequently the embedded FormGridManager
[MultiLingualResources ("FormGrid.Sample.Globalization.WebForm")]
public class WebForm : 
  System.Web.UI.Page,
  IObjectWithResources, //  Provides the WebForm's ResourceManager via GetResourceManager() 
  IFormGridRowProvider, //  Provides new rows and rows to hide to the FormGridManager
  IResourceUrlResolver //  Provides the URLs for this WebForm (i.e. to the FormGridManager)
{
  /// <summary> Caches the IResourceManager returned by GetResourceManager. </summary>
  private static IResourceManager s_chachedResourceManager;

  private AutoInitHashtable _listOfFormGridRowInfos =
      new AutoInitHashtable (typeof (FormGridRowInfoCollection));
  private AutoInitHashtable _listOfHiddenRows = 
      new AutoInitHashtable (typeof (StringCollection));
  protected Rubicon.Web.UI.Controls.FormGridManager GlobalFormGridManager;
  protected Rubicon.Web.UI.Controls.ValidationStateViewer ValidationStateViewer;
  protected Rubicon.Web.UI.Controls.FormGridLabel AddressLabel;

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    //  Demonstration of IFormGridRowProvider
    //  Data would normally be read by this WebForm from some external source
    //  This would either have to happen during page initialization
    //  or dynamically when the interface methods are called.

    StringCollection hiddenRows = (StringCollection)_listOfHiddenRows[MainFormGrid.ID];

    //  This row should be hidden
    hiddenRows.Add (FirstNameField.ID);
  
    FormGridRowInfoCollection newRows = 
        (FormGridRowInfoCollection)_listOfFormGridRowInfos[MainFormGrid.ID];

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
      CompaniesTable.ID, 
      FormGridRowInfo.RowPosition.BeforeRowWithID));

	}
	

	#region Web Form Designer generated code

  protected System.Web.UI.WebControls.Label PersonDataLabel;
  protected System.Web.UI.WebControls.Label NameLabel;
  protected System.Web.UI.WebControls.TextBox NameField;
  protected System.Web.UI.WebControls.DropDownList GenderList;
  protected System.Web.UI.WebControls.TextBox ZipField;
  protected System.Web.UI.WebControls.TextBox PlaceField;
  protected System.Web.UI.WebControls.TextBox AddressField;
  protected System.Web.UI.HtmlControls.HtmlTable MainFormGrid;
  protected System.Web.UI.WebControls.CompareValidator NameFieldCompareValidator;
  protected System.Web.UI.WebControls.CompareValidator GenderListCompareValidator;
  protected System.Web.UI.WebControls.Table CompaniesTable;
  protected System.Web.UI.WebControls.RequiredFieldValidator CompaniesTableRequiredFieldValidator;
  protected System.Web.UI.WebControls.Button submitButton;
  protected System.Web.UI.WebControls.Label EducationLavel;
  protected System.Web.UI.WebControls.TextBox EducationField;
  protected System.Web.UI.WebControls.TextBox FirstNameField;
  protected System.Web.UI.WebControls.TextBox LastNameField;
  protected System.Web.UI.WebControls.Label CombinedName_Label;

  /// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
    this.PreRender += new System.EventHandler(this.WebForm_PreRender);

  }
	#endregion

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

  /// <summary>
  ///   Interface method: IFormGridRowProvider
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  public virtual StringCollection GetListOfHiddenRows (string table)
  {
    //  Logic sufficient if all loading happens during OnInit, as shown in this example
    return (StringCollection) _listOfHiddenRows[table];
  }

  /// <summary>
  ///   Interface method: IFormGridRowProvider
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  public virtual FormGridRowInfoCollection GetListOfFormGridRowInfos (string table)
  {
    //  Logic sufficient if all loading happens during OnInit, as shown in this example
    return (FormGridRowInfoCollection) _listOfFormGridRowInfos[table];
  }

  /// <summary>
  ///   Implementation of <see cref="IResourceUrlResolver.GetResourceUrl"/>.
  /// </summary>
  /// <param name="relativeUrl"></param>
  /// <returns></returns>
  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
  {
    return Server.MapPath (resourceType.Name + "/" + relativeUrl);
  }

  private void WebForm_PreRender(object sender, System.EventArgs e)
  {
    //  A call to the ResourceDispatcher to get have the automatic resources dispatched
    ResourceDispatcher.Dispatch (this);
  }

  private void submitButton_Click(object sender, System.EventArgs e)
  {
    //  Validate the form grid manager
    GlobalFormGridManager.Validate();
  }
}
}
