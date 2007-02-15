using System;
using Rubicon.Web;
using Rubicon.Web.UI;

namespace Rubicon.SecurityManager.Clients.Web.UI
{
  public partial class SecurityManagerMasterPage : System.Web.UI.MasterPage
  {
    // types

    // static members and constants
    private const string c_contentViewStyleFileUrl = "ContentViewStyle.css";
    private const string c_contentViewStyleFileKey = "SecurityManagerContentViewStyle";

    // member fields

    // construction and disposing

    // methods and properties

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (!HtmlHeadAppender.Current.IsRegistered (c_contentViewStyleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (
            this, typeof (SecurityManagerMasterPage), ResourceType.Html, c_contentViewStyleFileUrl);
        HtmlHeadAppender.Current.RegisterStylesheetLink (c_contentViewStyleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }

    }
  }
}
