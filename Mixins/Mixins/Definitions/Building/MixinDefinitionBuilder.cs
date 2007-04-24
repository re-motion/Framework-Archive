using System;
using System.Reflection;
using Mixins;
using Mixins.Context;
using System.Collections.Generic;
using System.Diagnostics;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Mixins.Definitions.Building
{
  public class MixinDefinitionBuilder
  {
    private BaseClassDefinition _baseClass;
    private RequirementsAnalyzer _faceRequirementsAnalyzer; 
    private RequirementsAnalyzer _baseRequirementsAnalyzer;
    private OverridesAnalyzer _overridesAnalyzer;

    public MixinDefinitionBuilder (BaseClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);
      _baseClass = baseClass;
      _faceRequirementsAnalyzer = new RequirementsAnalyzer (baseClass, typeof (ThisAttribute));
      _baseRequirementsAnalyzer = new RequirementsAnalyzer (baseClass, typeof (BaseAttribute));
      _overridesAnalyzer = new OverridesAnalyzer();
    }

    public BaseClassDefinition BaseClass
    {
      get { return _baseClass; }
    }

    public MixinDefinition Apply (MixinContext mixinContext)
    {
      ArgumentUtility.CheckNotNull ("mixinContext", mixinContext);

      MixinDefinition mixin = new MixinDefinition (mixinContext.MixinType, BaseClass);
      BaseClass.Mixins.Add (mixin);

      const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      MemberDefinitionBuilder membersBuilder = new MemberDefinitionBuilder (mixin, delegate (MethodInfo m) { return m.IsPublic; });
      membersBuilder.Apply (mixin.Type.GetProperties (bindingFlags), mixin.Type.GetEvents (bindingFlags),
        mixin.Type.GetMethods (bindingFlags));

      AttributeDefinitionBuilder attributesBuilder = new AttributeDefinitionBuilder (mixin);
      attributesBuilder.Apply (CustomAttributeData.GetCustomAttributes (mixin.Type));

      AnalyzeInterfaceIntroductions (mixin);
      AnalyzeOverrides (mixin);
      AnalyzeInitializationMethods (mixin);

      AnalyzeDependencies(mixin);

      return mixin;
    }

    private void AnalyzeInterfaceIntroductions (MixinDefinition mixin)
    {
      InterfaceIntroductionBuilder introductionBuilder = new InterfaceIntroductionBuilder (mixin);

      foreach (Type implementedInterface in mixin.ImplementedInterfaces)
      {
        if (!implementedInterface.Equals (typeof (System.Runtime.Serialization.ISerializable)))
        {
          introductionBuilder.Apply (implementedInterface);
        }
      }
    }

    private void AnalyzeOverrides (MixinDefinition mixin)
    {
      foreach (Tuple<MethodDefinition, MethodDefinition> methodOverride in _overridesAnalyzer.Analyze(mixin.Methods, _baseClass.Methods))
        InitializeMethodOverride (methodOverride.A, methodOverride.B);
      foreach (Tuple<PropertyDefinition, PropertyDefinition> propertyOverride in _overridesAnalyzer.Analyze (mixin.Properties, _baseClass.Properties))
        InitializePropertyOverride (propertyOverride.A, propertyOverride.B);
      foreach (Tuple<EventDefinition, EventDefinition> eventOverride in _overridesAnalyzer.Analyze (mixin.Events, _baseClass.Events))
        InitializeEventOverride (eventOverride.A, eventOverride.B);
    }

    private void InitializeMemberOverride (MemberDefinition overrider, MemberDefinition baseMember)
    {
      overrider.Base = baseMember;
      baseMember.AddOverride (overrider);
    }

    private void InitializeMethodOverride (MethodDefinition overrider, MethodDefinition baseMember)
    {
      InitializeMemberOverride (overrider, baseMember);
    }

    private void InitializePropertyOverride (PropertyDefinition overrider, PropertyDefinition baseMember)
    {
      InitializeMemberOverride (overrider, baseMember);
      if (overrider.GetMethod != null && baseMember.GetMethod != null)
        InitializeMethodOverride (overrider.GetMethod, baseMember.GetMethod);
      if (overrider.SetMethod != null && baseMember.SetMethod != null)
        InitializeMethodOverride (overrider.SetMethod, baseMember.SetMethod);
    }

    private void InitializeEventOverride (EventDefinition overrider, EventDefinition baseMember)
    {
      InitializeMemberOverride (overrider, baseMember);
      InitializeMethodOverride (overrider.AddMethod, baseMember.AddMethod);
      InitializeMethodOverride (overrider.RemoveMethod, baseMember.RemoveMethod);
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

    private void AnalyzeDependencies (MixinDefinition mixin)
    {
      ThisDependencyDefinitionBuilder thisDependencyBuilder = new ThisDependencyDefinitionBuilder (mixin);
      thisDependencyBuilder.Apply (_faceRequirementsAnalyzer.Analyze (mixin));

      BaseDependencyDefinitionBuilder baseDependencyBuilder = new BaseDependencyDefinitionBuilder (mixin);
      baseDependencyBuilder.Apply (_baseRequirementsAnalyzer.Analyze (mixin));
    }
  }
}
