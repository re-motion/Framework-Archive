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
    private RequiredTypesBuilder<RequiredFaceTypeDefinition> _requiredFacesBuilder;
    private RequiredTypesBuilder<RequiredBaseCallTypeDefinition> _requiredBaseCallTypesBuilder;

    public MixinDefinitionBuilder (BaseClassDefinition baseClass)
    {
      _baseClass = baseClass;
      _requiredFacesBuilder = new RequiredTypesBuilder<RequiredFaceTypeDefinition> (_baseClass, _baseClass.RequiredFaceTypes, typeof (ThisAttribute),
          delegate (BaseClassDefinition bc, Type t) { return new RequiredFaceTypeDefinition (bc, t); });
      _requiredBaseCallTypesBuilder = new RequiredTypesBuilder<RequiredBaseCallTypeDefinition> (_baseClass, _baseClass.RequiredBaseCallTypes,
          typeof (BaseAttribute), delegate (BaseClassDefinition bc, Type t) { return new RequiredBaseCallTypeDefinition (bc, t); });
    }

    public BaseClassDefinition BaseClass
    {
      get { return _baseClass; }
    }

    public void Apply (MixinContext mixinContext)
    {
      MixinDefinition mixin = new MixinDefinition (mixinContext.MixinType, BaseClass);
      BaseClass.Mixins.Add (mixin);

      InitializeMembers (mixin);

      AnalyzeInterfaceIntroductions (mixin);
      AnalyzeOverrides (mixin);
      AnalyzeInitializationMethods (mixin);

      _requiredFacesBuilder.Apply (mixin);
      _requiredBaseCallTypesBuilder.Apply (mixin);
    }

    private void InitializeMembers (MixinDefinition mixin)
    {
      foreach (MethodInfo method in mixin.Type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (method.IsPublic)
        {
          mixin.Members.Add (new MethodDefinition (method, mixin));
        }
      }
    }

    private void AnalyzeInterfaceIntroductions (MixinDefinition mixin)
    {
      foreach (Type implementedInterface in mixin.ImplementedInterfaces)
      {
        InterfaceIntroductionDefinition introducedInterface = new InterfaceIntroductionDefinition (implementedInterface, mixin);
        mixin.InterfaceIntroductions.Add (introducedInterface);
      }
    }

    private void AnalyzeOverrides (MixinDefinition mixin)
    {
      foreach (MemberDefinition member in mixin.Members)
      {
        if (member.MemberInfo.IsDefined (typeof (OverrideAttribute), true))
        {
          MemberDefinition baseMember = FindBaseMember (member);
          if (baseMember == null)
          {
            string message = string.Format ("Could not find virtual base member for overrider {0}.", member.FullName);
            throw new ConfigurationException (message);
          }
          member.Base = baseMember;
          baseMember.Overrides.Add (member);
        }
      }
    }

    private MemberDefinition FindBaseMember (MemberDefinition overrider)
    {
      foreach (MemberDefinition classMember in BaseClass.Members)
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
        mixin.InitializationMethods.Add (methodDefinition);
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
