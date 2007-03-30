using System;
using Mixins.Context;
using Mixins.Configuration.Building;

namespace Mixins.Configuration
{
  public static class ConfigurationBuilder
  {
    public static ApplicationConfiguration CreateConfiguration (ApplicationContext context)
    {
      ApplicationConfigurationBuilder builder = new ApplicationConfigurationBuilder ();
      builder.Apply (context);
      return builder.GetConfiguration ();
    }

    public static BaseClassConfiguration GetMergedConfiguration (Type type, ApplicationConfiguration source)
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
