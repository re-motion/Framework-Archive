using System;
using System.Threading;
using System.Globalization;
using Rubicon.Globalization;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;

namespace $PROJECT_ROOTNAMESPACE$.Classes
{
  [WebMultiLingualResources ("$PROJECT_ROOTNAMESPACE$.Globalization.Global")]
  public class BasePage: WxePage, IObjectWithResources
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      if (Request.UserLanguages.Length > 0)
      {
        try
        {
          string[] cultureInfo = Request.UserLanguages[0].Split (';');

          if (cultureInfo.Length > 0)
          {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo[0]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo[0]);
          }
        }
        catch (ArgumentException)
        {
          // if cultureInfo contains a invalid value we just ignore it
        }
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      // register CSS
      string url = Rubicon.Web.ResourceUrlResolver.GetResourceUrl (
          this, typeof(Rubicon.Web.ResourceUrlResolver), Rubicon.Web.ResourceType.Html, "style.css");

      Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink(GetType() + "style", url);

      // globalization
      IResourceManager rm = ResourceManagerUtility.GetResourceManager (this);

      if (rm != null)
        ResourceDispatcher.Dispatch (this, rm);

      base.OnPreRender(e);
    }

    IResourceManager IObjectWithResources.GetResourceManager()
    {
      return this.GetResourceManager();
    }

    protected virtual IResourceManager GetResourceManager()
    {
      Type type = this.GetType();

      if (MultiLingualResourcesAttribute.ExistsResource (type))
        return MultiLingualResourcesAttribute.GetResourceManager (type, true);
      else
        return null;
    }
  }
}
