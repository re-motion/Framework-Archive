using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.Web.UI;
using Rubicon.Web;
using Rubicon.Web.UI.Controls;

namespace Rubicon.SecurityManager.Client.Web.UI
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
