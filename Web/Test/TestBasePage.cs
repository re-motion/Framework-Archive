using System;
using System.Web.UI;
using Rubicon.Web.UI;

namespace Rubicon.Web.Test
{
  public class TestBasePage : Page
  {
    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "style",
          Rubicon.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (SmartPage), Rubicon.Web.ResourceType.Html, "Style.css"));
      Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "fontsize080",
          Rubicon.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (SmartPage), Rubicon.Web.ResourceType.Html, "FontSize080.css"));
    }
  }
}