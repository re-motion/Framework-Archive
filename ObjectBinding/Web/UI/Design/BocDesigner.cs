using System;
using System.Web.UI.Design;
using Rubicon.ObjectBinding.Web.Controls;

namespace Rubicon.ObjectBinding.Web.Design
{

/// <summary>
///   A desinger that requries the complete loading of the control.
/// </summary>
public class BocDesigner: ControlDesigner
{
  public override bool DesignTimeHtmlRequiresLoadComplete
  {
    get { return true; }
  }

  public override string GetDesignTimeHtml()
  {
    BusinessObjectBoundWebControl control = (BusinessObjectBoundWebControl)Component;
    try
    {
      control.PreRenderForDesignMode();
    }
    catch (Exception e)
    {
      System.Diagnostics.Debug.WriteLine (e.Message);
    }
    return base.GetDesignTimeHtml ();
  }

}

}
