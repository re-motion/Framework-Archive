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

namespace OBWTest
{

[MultiLingualResources ("OBWTest.Globalization.WebFormBase")]
public class WxeWebFormBase:
    WxePage, 
    IObjectWithResources, //  Provides the WebForm's ResourceManager via GetResourceManager() 
    IResourceUrlResolver //  Provides the URLs for this WebForm (i.e. to the FormGridManager)
{
  /// <summary> Hashtable&lt;type,IResourceManagers&gt; </summary>
  private static Hashtable s_chachedResourceManagers = new Hashtable();

  protected override void OnInit(EventArgs e)
  {
    if (! ControlHelper.IsDesignMode (this, Context))
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
      Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.UserLanguages[0]);
    }
    base.OnInit (e);
  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
    
    //  A call to the ResourceDispatcher to get have the automatic resources dispatched
    ResourceDispatcher.Dispatch (this);

    LiteralControl stack = new LiteralControl();

    System.Text.StringBuilder sb = new System.Text.StringBuilder();
    sb.Append ("<b>Stack:</b><br>");
    for (WxeStep step = CurrentStep; step != null; step = step.ParentStep)
      sb.AppendFormat ("{0}<br>", step.ToString());      
    stack.Text = sb.ToString();
    
    Controls.Add (stack);
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

  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
  {
    if (ControlHelper.IsDesignMode (this, this.Context))
      return resourceType.Name + "/" + relativeUrl;
    else
      return Page.ResolveUrl (resourceType.Name + "/" + relativeUrl);
  }
}

}
