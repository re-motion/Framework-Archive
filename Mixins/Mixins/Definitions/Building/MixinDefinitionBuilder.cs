using System;
using System.Reflection;
using Mixins;
using Mixins.Context;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mixins.Definitions.Building
{
  public class MixinDefinitionBuilder
  {
    private BaseClassDefinition _baseClass;
    private RequirementsAnalyzer _faceRequirementsAnalyzer; 
    private RequirementsAnalyzer _baseRequirementsAnalyzer;

    public MixinDefinitionBuilder (BaseClassDefinition baseClass)
    {
      _baseClass = baseClass;
      _faceRequirementsAnalyzer = new RequirementsAnalyzer (baseClass, typeof (ThisAttribute));
      _baseRequirementsAnalyzer = new RequirementsAnalyzer (baseClass, typeof (BaseAttribute));
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

      ThisDependencyDefinitionBuilder thisDependencyBuilder = new ThisDependencyDefinitionBuilder (mixin);
      thisDependencyBuilder.Apply (_faceRequirementsAnalyzer.Analyze (mixin));

      BaseDependencyDefinitionBuilder baseDependencyBuilder = new BaseDependencyDefinitionBuilder (mixin);
      baseDependencyBuilder.Apply (_baseRequirementsAnalyzer.Analyze (mixin));
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
        BaseClass.IntroducedInterfaces.Add (introducedInterface);
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
            string message = string.Format ("Could not find base member for overrider {0}.", member.FullName);
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
        if (classMember.Name == overrider.Name && classMember.CanBeOverriddenBy (overrider))
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
