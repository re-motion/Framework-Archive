using System;
using System.ComponentModel;
using System.Web.UI.Design;
using Rubicon.Design;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Design
{
  /// <summary>
  /// Implementation of the <see cref="IDesignModeHelper"/> interface for environments implementing the <see cref="IWebApplication"/> designer service.
  /// </summary>
  public class WebDesginModeHelper: DesignModeHelperBase
  {
    public WebDesginModeHelper (ISite site):
      base (site)
    {
    }

    public override string GetProjectPath()
    {
      return GetWebApplication().RootProjectItem.PhysicalPath;
    }

    public override System.Configuration.Configuration GetConfiguration()
    {
      return GetWebApplication().OpenWebConfiguration (true);
    }

    private IWebApplication GetWebApplication()
    {
      IWebApplication webApplication = (IWebApplication) Site.GetService (typeof (IWebApplication));
      Assertion.Assert (webApplication != null, "The 'IServiceProvider' failed to return an 'IWebApplication' service.");

      return webApplication;
    }
  }
}