using System;
using System.Web.UI;

using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.UI.Utilities;

namespace Rubicon.Web.UI.Controls
{

public class MultiLingualPage : Page
{
  // types

  // static members and constants

  // member fields

  private string _pageTitle = "###";

  // construction and disposing

  // methods and properties

  public string PageTitle
  {
    get { return _pageTitle; }
    set { _pageTitle = value; }
  }

  protected override void OnInit(EventArgs e)
  {
    // TODO: Check if change works: Changed to use MultiLingualResourcesAttribute instead of ResourceManagerPool
    //  if (ResourceManagerPool.ExistsResource (this))
    if (MultiLingualResourcesAttribute.ExistsResource (this))
    {
      ResourceDispatcher.Dispatch (this);
      
      // TODO: Check if change works: Changed to use MultiLingualResourcesAttribute instead of ResourceManagerPool
      //  if (ResourceManagerPool.ExistsResourceText (this, "auto:PageTitle"))
      //    this.PageTitle = ResourceManagerPool.GetResourceText (this, "auto:PageTitle");
      if (MultiLingualResourcesAttribute.ExistsResourceText (this, "auto:PageTitle"))
        this.PageTitle = MultiLingualResourcesAttribute.GetResourceText (this, "auto:PageTitle");
    }
          
    base.OnInit (e);
  }
  
  protected string GetResourceText (string name)
  {
    // TODO: Check if change works: Changed to use MultiLingualResourcesAttribute instead of ResourceManagerPool
    //  return ResourceManagerPool.GetResourceText (this, name);
    return MultiLingualResourcesAttribute.GetResourceText (this, name);
  }
}
}
