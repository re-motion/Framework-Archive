using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Rubicon.Web.UI;

namespace OBWTest
{

public class WebFormBase: Page, IResourceUrlResolver
{
  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
  {
    return Server.MapPath (resourceType.Name + "/" + relativeUrl);
  }
}

}
