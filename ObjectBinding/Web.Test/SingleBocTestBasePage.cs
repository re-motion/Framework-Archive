using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Threading;
using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Web.Utilities;
using Rubicon.Utilities;
using Rubicon.Globalization;
using Rubicon.Web.ExecutionEngine;

namespace OBWTest
{

public class WebFormBase:
  WxePage, 
  IObjectWithResources, //  Provides the WebForm's ResourceManager via GetResourceManager() 
  IResourceUrlResolver //  Provides the URLs for this WebForm (i.e. to the FormGridManager)
{
  /// <summary> Caches the IResourceManager returned by GetResourceManager. </summary>
  private static IResourceManager s_chachedResourceManager;

  protected override void OnInit(EventArgs e)
  {
    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
    Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.UserLanguages[0]);

    base.OnInit (e);
  }

  /// <summary>
  ///   Interface implementation: IObjectWithResources
  /// </summary>
  /// <returns></returns>
  public virtual IResourceManager GetResourceManager()
  {
    //  chache the resource manager
    lock (typeof(IResourceManager))
    {
      if (s_chachedResourceManager == null)
      {
        s_chachedResourceManager = MultiLingualResourcesAttribute.GetResourceManager (
          this.GetType(), true);
      }
    }  
  
    return s_chachedResourceManager;
  }

  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
  {
    if (ControlHelper.IsDesignMode (this, this.Context))
      return resourceType.Name + "/" + relativeUrl;
    else
      return Server.MapPath (resourceType.Name + "/" + relativeUrl);
  }
}

}
