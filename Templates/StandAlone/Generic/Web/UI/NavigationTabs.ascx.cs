using System;
using System.Web;
using System.Web.UI.WebControls;

using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Templates.Generic.Web.Classes;

namespace Rubicon.Templates.Generic.Web.UI
{
public class NavigationTabs : System.Web.UI.UserControl
{
  protected WxeTabControl NavigationTabControl;

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
    
    ShowStatusMessage ();

    string key = GetType().FullName + "_Style";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, null, ResourceType.Html, "NavigationTabs.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, url);
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
	///		Required method for Designer support - do not modify
	///		the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
  }
	#endregion

  public TabControl TabControl
  {
    get {return NavigationTabControl; }
  }

  private void ShowStatusMessage()
  {
    NavigationTabControl.StatusMessage = "Version " + GetVersion ();
  }

  private string GetVersion ()
  {
    Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
    return version.ToString (3);
  }
}

}
