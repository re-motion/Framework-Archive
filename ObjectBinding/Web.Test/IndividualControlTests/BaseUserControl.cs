using System;
using System.Web;
using System.Web.UI.WebControls;

using Rubicon.Globalization;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;

namespace OBWTest.IndividualControlTests
{

public class BaseUserControl : 
    DataEditUserControl, 
    IObjectWithResources //  Provides the UserControl's ResourceManager via GetResourceManager() 
{
  protected virtual void RegisterEventHandlers ()
  {
  }
  
	override protected void OnInit(EventArgs e)
	{
    RegisterEventHandlers ();
		base.OnInit(e);
	}

  protected override void OnPreRender(EventArgs e)
  {
    //  A call to the ResourceDispatcher to get have the automatic resources dispatched
    ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));

    base.OnPreRender (e);
  }

  protected virtual IResourceManager GetResourceManager()
  {
    Type type = GetType();
    if (MultiLingualResourcesAttribute.ExistsResource (type))
      return MultiLingualResourcesAttribute.GetResourceManager (type, true);
    else
      return null;
  }

  IResourceManager IObjectWithResources.GetResourceManager()
  {
    return GetResourceManager();
  }
}

}
