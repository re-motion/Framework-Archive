using System;
using Mixins.Context;

namespace Mixins.Configuration.Building
{
  public class ApplicationConfigurationBuilder
  {
    private ApplicationConfiguration _newConfiguration = new ApplicationConfiguration ();

    public void Apply (ApplicationContext context)
    {
      BaseClassConfigurationBuilder analyzer = new BaseClassConfigurationBuilder (_newConfiguration);

      foreach (ClassContext classContext in context.ClassContexts)
      {
        analyzer.Apply (classContext);
      }
    }

    public ApplicationConfiguration GetConfiguration ()
    {
      return _newConfiguration;
    }
  }
}
