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
public class SingleBocTestWxeBasePage: TestWxeBasePage
{
  /// <summary> Hashtable&lt;type,IResourceManagers&gt; </summary>
  private static Hashtable s_chachedResourceManagers = new Hashtable();

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
  }
  public override IResourceManager GetResourceManager()
  {
    // cache the resource manager
    Type type = this.GetType();
    if (s_chachedResourceManagers[type] == null)
    {
      lock (typeof (SingleBocTestWxeBasePage))
      {
        if (s_chachedResourceManagers[type] == null)
          s_chachedResourceManagers[type] = MultiLingualResourcesAttribute.GetResourceManager (type, true);
      }  
    }

    //  TODO: Combine IResourceManager into a ResourceManagerSet
    //    return ResourceManagerSet.Combine (
    //      (IResourceManager) base.GetResourceManager(), 
    // (IResourceManager) s_chachedResourceManagers[type]);
    return base.GetResourceManager();
  }
}

}
