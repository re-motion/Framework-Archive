using System;
using System.Web.UI;

using Rubicon.Findit.Globalization.Classes;

namespace Rubicon.Web.UI.Controls
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
    if (ResourceManagerPool.ExistsResource (this))
      ResourceDispatcher.Dispatch (this);

    base.OnInit (e);
  }

  protected string GetResourceText (string name)
  {
    return ResourceManagerPool.GetResourceText (this, name);
  }

}

}
