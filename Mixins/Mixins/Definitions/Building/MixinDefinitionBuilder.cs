using System;
using System.Reflection;
using Mixins;
using Mixins.Context;
using System.Collections.Generic;
using System.Diagnostics;
using Rubicon;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Mixins.Definitions.Building
{
  public class MixinDefinitionBuilder
  {
    private BaseClassDefinition _baseClass;
    private RequirementsAnalyzer _faceRequirementsAnalyzer; 
    private RequirementsAnalyzer _baseRequirementsAnalyzer;

    public MixinDefinitionBuilder (BaseClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);
      _baseClass = baseClass;
      _faceRequirementsAnalyzer = new RequirementsAnalyzer (baseClass, typeof (ThisAttribute));
      _baseRequirementsAnalyzer = new RequirementsAnalyzer (baseClass, typeof (BaseAttribute));
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
      MemberDefinitionBuilder membersBuilder = new MemberDefinitionBuilder (mixin, delegate (MethodInfo m)
      {
        return m.IsPublic || m.IsFamily || (m.IsPrivate && m.IsVirtual);
      }, bindingFlags);
      membersBuilder.Apply (mixin.Type);

      AttributeDefinitionBuilder attributesBuilder = new AttributeDefinitionBuilder (mixin);
      attributesBuilder.Apply (CustomAttributeData.GetCustomAttributes (mixin.Type));

      AnalyzeInterfaceIntroductions (mixin);
      AnalyzeOverrides (mixin);

      AnalyzeDependencies(mixin);

      return mixin;
    }

    private void AnalyzeInterfaceIntroductions (MixinDefinition mixin)
    {
      InterfaceIntroductionBuilder introductionBuilder = new InterfaceIntroductionBuilder (mixin);

      foreach (Type implementedInterface in mixin.ImplementedInterfaces)
      {
        if (!implementedInterface.Equals (typeof (System.Runtime.Serialization.ISerializable)))
          introductionBuilder.Apply (implementedInterface);
      }
    }

    private void AnalyzeOverrides (MixinDefinition mixin)
    {
      OverridesAnalyzer<MethodDefinition> methodAnalyzer = new OverridesAnalyzer<MethodDefinition> (delegate { return _baseClass.Methods; });
      foreach (Tuple<MethodDefinition, MethodDefinition> methodOverride in methodAnalyzer.Analyze (mixin.Methods))
        InitializeOverride (methodOverride.A, methodOverride.B);

      OverridesAnalyzer<PropertyDefinition> propertyAnalyzer = new OverridesAnalyzer<PropertyDefinition> (delegate { return _baseClass.Properties; });
      foreach (Tuple<PropertyDefinition, PropertyDefinition> propertyOverride in propertyAnalyzer.Analyze (mixin.Properties))
        InitializeOverride (propertyOverride.A, propertyOverride.B);

      OverridesAnalyzer<EventDefinition> eventAnalyzer = new OverridesAnalyzer<EventDefinition> (delegate { return _baseClass.Events; });
      foreach (Tuple<EventDefinition, EventDefinition> eventOverride in eventAnalyzer.Analyze (mixin.Events))
        InitializeOverride (eventOverride.A, eventOverride.B);
    }
    
    private void InitializeOverride (MemberDefinition overrider, MemberDefinition baseMember)
    {
      overrider.BaseAsMember = baseMember;
      if (baseMember.Overrides.HasItem (overrider.DeclaringClass.Type))
      {
        string message = string.Format ("Mixin {0} overrides method {1} twice: {2} and {3} both target the same method.",
            overrider.DeclaringClass.FullName, baseMember.FullName, overrider.FullName, baseMember.Overrides[overrider.DeclaringClass.Type].FullName);
        throw new ConfigurationException (message);
      }
      baseMember.AddOverride (overrider);
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
