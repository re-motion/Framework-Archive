using System;
using System.Web.UI;

using Rubicon.Findit.Globalization.Classes;

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
    if (ResourceManagerPool.ExistsResource (this))
    {
      ResourceDispatcher.Dispatch (this);
      
      if (ResourceManagerPool.ExistsResourceText (this, "auto:PageTitle"))
        this.PageTitle = ResourceManagerPool.GetResourceText (this, "auto:PageTitle");
    }
          
    base.OnInit (e);
  }
  
  protected string GetResourceText (string name)
  {
    return ResourceManagerPool.GetResourceText (this, name);
  }
}
}
