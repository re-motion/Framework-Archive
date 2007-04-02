using System;
using System.Reflection;
using Mixins.Context;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mixins.Definitions.Building
{
  public class MixinDefinitionBuilder
  {
    private BaseClassDefinition _baseClass;

    public MixinDefinitionBuilder (BaseClassDefinition baseClass)
    {
      _baseClass = baseClass;
    }

    public BaseClassDefinition BaseClass
    {
      get { return _baseClass; }
    }

    public void Apply (MixinContext mixinContext)
    {
      MixinDefinition mixin = new MixinDefinition(mixinContext.MixinType, BaseClass);
      BaseClass.AddMixin (mixin);

      InitializeMembers (mixin);

      AnalyzeInterfaceIntroductions (mixin);
      AnalyzeOverrides (mixin);
      AnalyzeInitializationMethods (mixin);
      ApplyRequiredFaceInterfacesToBaseClass (mixin);
    }

    private void InitializeMembers (MixinDefinition mixin)
    {
      foreach (MethodInfo method in mixin.Type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (method.IsPublic)
        {
          mixin.AddMember (new MethodDefinition (method, mixin));
        }
      }
    }

    private void AnalyzeInterfaceIntroductions (MixinDefinition mixin)
    {
      foreach (Type implementedInterface in mixin.ImplementedInterfaces)
      {
        InterfaceIntroductionDefinition introducedInterface = new InterfaceIntroductionDefinition (implementedInterface, mixin);
        mixin.AddInterfaceIntroduction (introducedInterface);
        BaseClass.AddIntroducedInterface (introducedInterface);
      }
    }

    private void AnalyzeOverrides (MixinDefinition mixin)
    {
      foreach (MemberDefinition member in mixin.Members)
      {
        if (member.MemberInfo.IsDefined (typeof (OverrideAttribute), true))
        {
          MemberDefinition baseMember = FindBaseMember (member, mixin);
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

    private MemberDefinition FindBaseMember (MemberDefinition overrider, MixinDefinition mixin)
    {
      foreach (MemberDefinition classMember in mixin.BaseClass.Members)
      {
        if (classMember.CanBeOverriddenBy (overrider))
        {
          return classMember;
        }
      }
      return null;
    }

    private void AnalyzeInitializationMethods (MixinDefinition mixin)
    {
      foreach (MethodInfo method in GetMixinInitializationMethods (mixin.Type))
      {
        MethodDefinition methodDefinition = new MethodDefinition (method, mixin);
        mixin.AddInitializationMethod (methodDefinition);
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

    private void ApplyRequiredFaceInterfacesToBaseClass (MixinDefinition mixin)
    {
      Type mixinBase = GetMixinBase(mixin);
      if (mixinBase != null)
      {
        Debug.Assert (mixinBase.IsGenericType);
        foreach (Type genericArgument in mixinBase.GetGenericArguments ())
        {
          ApplyGenericArgumentFaceRequirements(genericArgument, mixin);
        }
      }
    }

    private Type GetMixinBase (MixinDefinition mixin)
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

    private void ApplyGenericArgumentFaceRequirements(Type genericArgument, MixinDefinition mixin)
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

    private void ApplyRequiredFaceType(Type requiredFaceType, MixinDefinition mixin)
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
