using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Utilities;

namespace OBWTest
{

public class WebFormBase: Page, IResourceUrlResolver
{
  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
  {
    if (! ControlHelper.IsDesignMode (this, this.Context))
      return Server.MapPath (resourceType.Name + "/" + relativeUrl);
    else
      return resourceType.Name + "/" + relativeUrl;
  }
}

}
