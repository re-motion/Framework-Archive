using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Threading;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Utilities;

namespace OBWTest
{

public class WebFormBase: Page, IResourceUrlResolver
{
  protected override void OnInit(EventArgs e)
  {
    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
    Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.UserLanguages[0]);

    base.OnInit (e);
  }

  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
  {
    if (! ControlHelper.IsDesignMode (this, this.Context))
      return Server.MapPath (resourceType.Name + "/" + relativeUrl);
    else
      return resourceType.Name + "/" + relativeUrl;
  }
}

}
