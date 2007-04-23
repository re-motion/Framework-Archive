using System;
using System.ComponentModel;
using Rubicon.Design;
using Rubicon.Utilities;

namespace Rubicon.Design
{
  /// <summary>
  /// Base implementation of the <see cref="IDesignModeHelper"/> interface.
  /// </summary>
  public abstract class DesignModeHelperBase: IDesignModeHelper
  {
    private ISite _site;

    protected DesignModeHelperBase (ISite site)
    {
      ArgumentUtility.CheckNotNull ("site", site);
      if (!site.DesignMode)
        throw new ArgumentException (string.Format ("The '{0}' requires that DesignMode is active for the site.", GetType().Name), "site");

      _site = site;
    }

    public abstract string GetProjectPath();

    public abstract System.Configuration.Configuration GetConfiguration();

    public ISite Site
    {
      get { return _site; }
    }
  }
}