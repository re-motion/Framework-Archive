using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI.Design;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;

namespace Rubicon.Web.UI.Design
{

/// <summary>
///   A desinger that requries the complete loading of the control.
/// </summary>
public class WebControlDesigner: ControlDesigner
{
  public override bool DesignTimeHtmlRequiresLoadComplete
  {
    get { return true; }
  }

  public override string GetDesignTimeHtml()
  {
    try
    {
      IControlWithDesignTimeSupport control = Component as IControlWithDesignTimeSupport;
      if (control != null)
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
