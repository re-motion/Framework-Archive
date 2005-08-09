using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Collections;
using System.Threading;
using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Web.Utilities;
using Rubicon.Utilities;
using Rubicon.Globalization;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.UI.Controls;
using System.Web;

namespace OBWTest
{

[MultiLingualResources ("OBWTest.Globalization.SingleBocTestBasePage")]
public class SingleBocTestBasePage:
    Page, 
    IControl,
    IObjectWithResources //  Provides the WebForm's ResourceManager via GetResourceManager() 
    // IResourceUrlResolver //  Provides the URLs for this WebForm (e.g. to the FormGridManager)
{
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    if (! ControlHelper.IsDesignMode (this, Context))
    {
      try
      {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {}
      try
      {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {}
    }
  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);

    string key = GetType().FullName + "_Style";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (ResourceUrlResolver), ResourceType.Html, "Style.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, url);
    }
    
    //  A call to the ResourceDispatcher to get have the automatic resources dispatched
    ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));
  }

  public virtual IResourceManager GetResourceManager()
  {
    Type type = GetType();
    if (MultiLingualResourcesAttribute.ExistsResource (type))
      return MultiLingualResourcesAttribute.GetResourceManager (type, true);
    else
      return null;
  }

//  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
//  {
//    if (ControlHelper.IsDesignMode (this, this.Context))
//      return resourceType.Name + "/" + relativeUrl;
//    else
//      return Page.ResolveUrl (resourceType.Name + "/" + relativeUrl);
//  }
}

}
