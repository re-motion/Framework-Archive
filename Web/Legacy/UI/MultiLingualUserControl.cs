using System;
using System.Web.UI;
using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.Web.UI
{

public class MultiLingualUserControl : UserControl
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  // methods and properties

  protected override void OnInit(EventArgs e)
  {
    if (MultiLingualResourcesAttribute.ExistsResource (this))
    {
      IResourceManager resourceManager = MultiLingualResourcesAttribute.GetResourceManager (this.GetType(), true);

      ResourceDispatcher.Dispatch (this, resourceManager);
    }

    base.OnInit (e);
  }

  protected string GetResourceText (string name)
  {
    return MultiLingualResourcesAttribute.GetResourceText (this, name);
  }

}

}
