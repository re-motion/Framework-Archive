using System;
using Mixins.Context;
using System.Reflection;

namespace Mixins.Configuration.Building
{
  public class BaseClassConfigurationBuilder
  {
    private ApplicationConfiguration _configuration;

    public BaseClassConfigurationBuilder (ApplicationConfiguration configuration)
    {
      _configuration = configuration;
    }

    public ApplicationConfiguration Configuration
    {
      get { return _configuration; }
    }

    public void Apply (ClassContext classContext)
    {
      BaseClassConfiguration classConfiguration = new BaseClassConfiguration (classContext.Type);
      Configuration.AddBaseClassConfiguration (classConfiguration);

      InitializeMembers (classConfiguration);
      ApplyMixins(classConfiguration, classContext);

    }

    private void InitializeMembers (BaseClassConfiguration classConfiguration)
    {
      foreach (MethodInfo method in classConfiguration.Type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (method.IsPublic || method.IsFamily)
        {
          classConfiguration.AddMember (new MethodConfiguration (method, classConfiguration));
        }
      }
    }

    private static void ApplyMixins(BaseClassConfiguration classConfiguration, ClassContext classContext)
    {
      MixinConfigurationBuilder mixinConfigurationBuilder = new MixinConfigurationBuilder (classConfiguration);
      foreach (MixinContext mixinContext in classContext.MixinContexts)
      {
        mixinConfigurationBuilder.Apply (mixinContext);
      }
    }
  }
}
