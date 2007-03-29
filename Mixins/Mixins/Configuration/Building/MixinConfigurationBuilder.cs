using System;
using System.Reflection;
using Mixins.Context;

namespace Mixins.Configuration.Building
{
  public class MixinConfigurationBuilder
  {
    private BaseClassConfiguration _baseClass;

    public MixinConfigurationBuilder (BaseClassConfiguration baseClass)
    {
      _baseClass = baseClass;
    }

    public BaseClassConfiguration BaseClass
    {
      get { return _baseClass; }
    }

    public void Apply (MixinDefinition mixinDefinition)
    {
      MixinConfiguration mixinConfiguration = new MixinConfiguration(mixinDefinition.MixinType, BaseClass);
      BaseClass.AddMixin (mixinConfiguration);

      InitializeMembers (mixinConfiguration);

      InitializeInterfaceIntroductions (mixinConfiguration);
      // TODO: adjust face interfaces and overrides accordingly

    }

    private void InitializeMembers (MixinConfiguration mixinConfiguration)
    {
      foreach (MethodInfo method in mixinConfiguration.Type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (method.IsPublic)
        {
          mixinConfiguration.AddMember (new MemberConfiguration (method, mixinConfiguration));
        }
      }
    }

    private void InitializeInterfaceIntroductions (MixinConfiguration mixinConfiguration)
    {
      foreach (Type implementedInterface in mixinConfiguration.ImplementedInterfaces)
      {
        InterfaceIntroductionConfiguration introducedInterface = new InterfaceIntroductionConfiguration (implementedInterface, mixinConfiguration);
        mixinConfiguration.AddInterfaceIntroduction (introducedInterface);
        BaseClass.AddIntroducedInterface (introducedInterface);
      }
    }
  }
}
