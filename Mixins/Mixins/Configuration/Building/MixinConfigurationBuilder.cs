using System;
using System.Reflection;
using Mixins.Context;
using System.Collections.Generic;
using System.Diagnostics;

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

    public void Apply (MixinContext mixinContext)
    {
      MixinConfiguration mixin = new MixinConfiguration(mixinContext.MixinType, BaseClass);
      BaseClass.AddMixin (mixin);

      InitializeMembers (mixin);

      AnalyzeInterfaceIntroductions (mixin);
      AnalyzeOverrides (mixin);
      AnalyzeInitializationMethods (mixin);
      AnalyzeRequiredFaceInterfaces (mixin);
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

    private void AnalyzeRequiredFaceInterfaces (MixinConfiguration mixin)
    {
      Type mixinBase = GetMixinBase(mixin);
      if (mixinBase != null)
      {
        Debug.Assert (mixinBase.IsGenericType);
        foreach (Type genericArgument in mixinBase.GetGenericArguments ())
        {
          AnalyzeGenericArgumentFaceRequirements(genericArgument, mixin);
        }
      }
    }

    private Type GetMixinBase (MixinConfiguration mixin)
    {
      Type mixinBase = mixin.Type.BaseType;
      while (mixinBase != null && !IsSpecializationOf(mixinBase, typeof(Mixin<,>)))
      {
        mixinBase = mixinBase.BaseType;
      }
      return mixinBase;
    }

    private bool IsSpecializationOf (Type typeToCheck, Type requestedType)
    {
      if (requestedType.IsAssignableFrom (typeToCheck))
      {
        return true;
      }
      else if (typeToCheck.IsGenericType && !typeToCheck.IsGenericTypeDefinition)
      {
        Type typeDefinition = typeToCheck.GetGenericTypeDefinition ();
        return IsSpecializationOf(typeDefinition, requestedType);
      }
      else
      {
        return false;
      }
    }

    private void AnalyzeGenericArgumentFaceRequirements(Type genericArgument, MixinConfiguration mixin)
    {
      if (genericArgument.IsGenericParameter)
      {
        Type[] constraints = genericArgument.GetGenericParameterConstraints ();
        foreach (Type constraint in constraints)
        {
          ApplyRequiredFaceType(constraint, mixin);
        }
      }
      else
      {
        ApplyRequiredFaceType (genericArgument, mixin);
      }
    }

    private void ApplyRequiredFaceType(Type requiredFaceType, MixinConfiguration mixin)
    {
      if (requiredFaceType.IsClass)
      {
        if (!requiredFaceType.IsAssignableFrom (mixin.BaseClass.Type))
        {
          string message = string.Format ("Mixin {0} requires its target {1} to derive from base type {2}.", mixin.FullName,
                                          mixin.BaseClass.FullName, requiredFaceType.FullName);
          throw new ConfigurationException (message);
        }
      }
      else
      {
        if (!mixin.BaseClass.HasRequiredFaceInterface (requiredFaceType))
        {
          mixin.BaseClass.AddRequiredFaceInterface (requiredFaceType);
        }
      }
    }
  }
}
