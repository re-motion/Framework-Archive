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

	private void Page_Load(object sender, System.EventArgs e)
	{
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

  }
	#endregion

  private void SaveButton_Click(object sender, System.EventArgs e)
  {
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
