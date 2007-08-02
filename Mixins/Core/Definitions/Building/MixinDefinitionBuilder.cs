using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Collections;
using Rubicon.Mixins.Context;
using Rubicon.Utilities;
using ReflectionUtility=Rubicon.Mixins.Utilities.ReflectionUtility;

namespace Rubicon.Mixins.Definitions.Building
{
  public class MixinDefinitionBuilder
  {
    private readonly BaseClassDefinition _baseClass;
    private readonly RequirementsAnalyzer _faceRequirementsAnalyzer; 
    private readonly RequirementsAnalyzer _baseRequirementsAnalyzer;
    private readonly AttributeIntroductionBuilder _attributeIntroductionBuilder;

    public MixinDefinitionBuilder (BaseClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);
      _baseClass = baseClass;
      _faceRequirementsAnalyzer = new RequirementsAnalyzer (baseClass, typeof (ThisAttribute));
      _baseRequirementsAnalyzer = new RequirementsAnalyzer (baseClass, typeof (BaseAttribute));
      _attributeIntroductionBuilder = new AttributeIntroductionBuilder (_baseClass);
    }

    public BaseClassDefinition BaseClass
    {
      get { return _baseClass; }
    }

    public void Apply (MixinContext mixinContext, int index)
    {
      ArgumentUtility.CheckNotNull ("mixinContext", mixinContext);
      ArgumentUtility.CheckNotNull ("index", index);

      Type mixinType = BaseClass.MixinTypeInstantiator.GetConcreteMixinType (mixinContext.MixinType);
      MixinDefinition mixin = new MixinDefinition (mixinType, BaseClass);
      BaseClass.Mixins.Add (mixin);

      const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      MemberDefinitionBuilder membersBuilder = new MemberDefinitionBuilder (mixin, IsVisibleToInheritorsOrExplicitInterfaceImpl, bindingFlags);
      membersBuilder.Apply (mixin.Type);

      AttributeDefinitionBuilder attributesBuilder = new AttributeDefinitionBuilder (mixin);
      attributesBuilder.Apply (CustomAttributeData.GetCustomAttributes (mixin.Type));

      _attributeIntroductionBuilder.Apply (mixin);

      AnalyzeInterfaceIntroductions (mixin);
      AnalyzeOverrides (mixin);

      AnalyzeDependencies(mixin, mixinContext.ExplicitDependencies);
    }

    private bool IsVisibleToInheritorsOrExplicitInterfaceImpl (MethodInfo method)
    {
      return ReflectionUtility.IsPublicOrProtectedOrExplicit (method);
    }

    private void AnalyzeInterfaceIntroductions (MixinDefinition mixin)
    {
      InterfaceIntroductionBuilder introductionBuilder = new InterfaceIntroductionBuilder (mixin);
      introductionBuilder.Apply ();
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

      AnalyzeMemberAttributeIntroductions (mixin);
    }

    private void InitializeOverride (MemberDefinition overrider, MemberDefinition baseMember)
    {
      overrider.BaseAsMember = baseMember;
      if (baseMember.Overrides.ContainsKey (overrider.DeclaringClass.Type))
      {
        string message = string.Format ("Mixin {0} overrides method {1} twice: {2} and {3} both target the same method.",
            overrider.DeclaringClass.FullName, baseMember.FullName, overrider.FullName, baseMember.Overrides[overrider.DeclaringClass.Type].FullName);
        throw new ConfigurationException (message);
      }
      baseMember.AddOverride (overrider);
    }

    private void AnalyzeMemberAttributeIntroductions (MixinDefinition mixin)
    {
      foreach (MemberDefinition mixinMember in mixin.GetAllMembers ())
      {
        if (mixinMember.BaseAsMember != null)
        {
          AttributeIntroductionBuilder introductionBuilder = new AttributeIntroductionBuilder (mixinMember.BaseAsMember);
          introductionBuilder.Apply (mixinMember);
        }
      }
    }

    private void AnalyzeDependencies (MixinDefinition mixin, IEnumerable<Type> additionalDependencies)
    {
      ThisDependencyDefinitionBuilder thisDependencyBuilder = new ThisDependencyDefinitionBuilder (mixin);
      thisDependencyBuilder.Apply (_faceRequirementsAnalyzer.Analyze (mixin));

      BaseDependencyDefinitionBuilder baseDependencyBuilder = new BaseDependencyDefinitionBuilder (mixin);
      baseDependencyBuilder.Apply (_baseRequirementsAnalyzer.Analyze (mixin));
      baseDependencyBuilder.Apply (additionalDependencies);
    }
  }
}
