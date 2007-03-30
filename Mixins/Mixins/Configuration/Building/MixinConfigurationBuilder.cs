using System;
using System.Reflection;
using Mixins.Context;
using System.Collections.Generic;

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
      MixinConfiguration mixin = new MixinConfiguration(mixinDefinition.MixinType, BaseClass);
      BaseClass.AddMixin (mixin);

      InitializeMembers (mixin);

      AnalyzeInterfaceIntroductions (mixin);
      AnalyzeOverrides (mixin);
      
      // TODO: adjust face interfaces accordingly
      AnalyzeInitializationMethods (mixin);
    }

    private void InitializeMembers (MixinConfiguration mixin)
    {
      foreach (MethodInfo method in mixin.Type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (method.IsPublic)
        {
          mixin.AddMember (new MethodConfiguration (method, mixin));
        }
      }
    }

    private void AnalyzeInterfaceIntroductions (MixinConfiguration mixin)
    {
      foreach (Type implementedInterface in mixin.ImplementedInterfaces)
      {
        InterfaceIntroductionConfiguration introducedInterface = new InterfaceIntroductionConfiguration (implementedInterface, mixin);
        mixin.AddInterfaceIntroduction (introducedInterface);
        BaseClass.AddIntroducedInterface (introducedInterface);
      }
    }

    private void AnalyzeOverrides (MixinConfiguration mixin)
    {
      foreach (MemberConfiguration member in mixin.Members)
      {
        if (member.MemberInfo.IsDefined (typeof (OverrideAttribute), true))
        {
          MemberConfiguration baseMember = FindBaseMember (member, mixin);
          if (baseMember == null)
          {
            string message = string.Format ("Could not find virtual base member for overrider {0}.", member.FullName);
            throw new ConfigurationException (message);
          }
          member.Base = baseMember;
          baseMember.AddOverride (member);
        }
      }
    }

    private MemberConfiguration FindBaseMember (MemberConfiguration overrider, MixinConfiguration mixin)
    {
      foreach (MemberConfiguration classMember in mixin.BaseClass.Members)
      {
        if (classMember.CanBeOverriddenBy (overrider))
        {
          return classMember;
        }
      }
      return null;
    }

    private void AnalyzeInitializationMethods (MixinConfiguration mixin)
    {
      foreach (MethodInfo method in GetMixinInitializationMethods (mixin.Type))
      {
        MethodConfiguration methodConf = new MethodConfiguration (method, mixin);
        mixin.AddInitializationMethod (methodConf);
      }
    }

    private IEnumerable<MethodInfo> GetMixinInitializationMethods (Type type)
    {
      foreach (MethodInfo method in type.GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
      {
        if (method.IsDefined (typeof (MixinInitializationMethodAttribute), true))
        {
          yield return method;
        }
      }
    }
  }
}
