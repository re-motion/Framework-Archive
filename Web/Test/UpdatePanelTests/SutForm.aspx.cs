using System;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;

namespace Rubicon.Web.Test.UpdatePanelTests
{
  public partial class SutForm : WxePage
  {
    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "style",
          Rubicon.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (WxePage), Rubicon.Web.ResourceType.Html, "Style.css"));
      Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink (
          "fontsize080",
          Rubicon.Web.ResourceUrlResolver.GetResourceUrl (this, typeof (WxePage), Rubicon.Web.ResourceType.Html, "FontSize080.css"));
    }
  }
}