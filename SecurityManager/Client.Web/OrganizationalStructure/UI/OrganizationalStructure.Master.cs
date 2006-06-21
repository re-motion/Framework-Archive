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

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI
{
  public partial class OrganizationalStructure : System.Web.UI.MasterPage
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      string key = typeof (TabbedMultiView).FullName + "_Style";
      if (!HtmlHeadAppender.Current.IsRegistered (key))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            this, Context, typeof (TabbedMultiView), ResourceType.Html, "TabbedMultiView.css");

        HtmlHeadAppender.Current.RegisterStylesheetLink (key, styleSheetUrl, HtmlHeadAppender.Priority.Library);
      }
    }
  }
}
