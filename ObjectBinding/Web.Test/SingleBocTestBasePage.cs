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

[MultiLingualResources ("OBWTest.Globalization.WebFormBase")]
public class WebFormBase:
    Page, 
    IControl,
    IObjectWithResources //  Provides the WebForm's ResourceManager via GetResourceManager() 
    // IResourceUrlResolver //  Provides the URLs for this WebForm (e.g. to the FormGridManager)
{
  /// <summary> Hashtable&lt;type,IResourceManagers&gt; </summary>
  private static Hashtable s_chachedResourceManagers = new Hashtable();

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
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

    string url = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (FormGridManager), ResourceType.Html, "FormGrid.css");

    HtmlHeadAppender.Current.RegisterStylesheetLink ("FormGrid_Style", url);
  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
    
    //  A call to the ResourceDispatcher to get have the automatic resources dispatched
    ResourceDispatcher.Dispatch (this, this.GetResourceManager());
  }

  public virtual IResourceManager GetResourceManager()
  {
    // cache the resource manager
    Type type = this.GetType();
    if (s_chachedResourceManagers[type] == null)
    {
      lock (typeof (WebFormBase))
      {
        if (s_chachedResourceManagers[type] == null)
          s_chachedResourceManagers[type] = MultiLingualResourcesAttribute.GetResourceManager (type, true);
      }  
    }
  
    return (IResourceManager) s_chachedResourceManagers[type];
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
