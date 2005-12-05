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

#if NET11
  protected static readonly string ErrorDesignTimeHtmlTemplate = "<table cellpadding=\"4\" cellspacing=\"0\" style=\"font: messagebox; color: buttontext; background-color: buttonface; border: solid 1px; border-top-color: buttonhighlight; border-left-color: buttonhighlight; border-bottom-color: buttonshadow; border-right-color: buttonshadow\">\r\n                <tr><td nowrap><span style=\"font-weight: bold; color: red\">{0}</span> - {1}</td></tr>\r\n                <tr><td>{2}</td></tr>\r\n              </table>";
  
  protected static string CreateErrorDesignTimeHtml (string errorMessage, Exception e, IComponent component)
  {
    errorMessage = HttpUtility.HtmlEncode (StringUtility.NullToEmpty (errorMessage));
    if (e != null)
      errorMessage = errorMessage + "<br />" + HttpUtility.HtmlEncode(e.Message);
    return string.Format (
        WebControlDesigner.ErrorDesignTimeHtmlTemplate, 
        "Error rendering Control", 
        HttpUtility.HtmlEncode (component.Site.Name), 
        errorMessage);
  }
 
  protected override string GetErrorDesignTimeHtml (Exception e)
  {
    return CreateErrorDesignTimeHtml ("An unhandled exception has occurred.", e, Component);
  }
#endif

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
