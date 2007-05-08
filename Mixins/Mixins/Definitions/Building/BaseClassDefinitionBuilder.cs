using System;
using System.Collections.Generic;
using Mixins.Context;
using System.Reflection;
using Rubicon;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Mixins.Definitions.Building
{
  public class BaseClassDefinitionBuilder
  {
    private ApplicationDefinition _application;

    public BaseClassDefinitionBuilder (ApplicationDefinition application)
    {
      ArgumentUtility.CheckNotNull ("application", application);
      _application = application;
    }

    public ApplicationDefinition Application
    {
      get { return _application; }
    }

    public void Apply (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      BaseClassDefinition classDefinition = new BaseClassDefinition (_application, classContext.Type);
      Application.BaseClasses.Add (classDefinition);

      const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      MemberDefinitionBuilder membersBuilder = new MemberDefinitionBuilder(classDefinition, IsVisibleToInheritors);
      membersBuilder.Apply (classDefinition.Type.GetProperties (bindingFlags), classDefinition.Type.GetEvents (bindingFlags), 
        classDefinition.Type.GetMethods (bindingFlags));

      AttributeDefinitionBuilder attributesBuilder = new AttributeDefinitionBuilder (classDefinition);
      attributesBuilder.Apply (CustomAttributeData.GetCustomAttributes (classDefinition.Type));

      ApplyMixins(classDefinition, classContext);
      ApplyBaseCallMethodRequirements (classDefinition);

      AnalyzeOverrides (classDefinition);
    }

    private void AnalyzeOverrides (BaseClassDefinition definition)
    {
      OverridesAnalyzer<MethodDefinition> methodAnalyzer = new OverridesAnalyzer<MethodDefinition> (definition.GetAllMixinMethods);
      foreach (Tuple<MethodDefinition, MethodDefinition> methodOverride in methodAnalyzer.Analyze (definition.Methods))
        InitializeOverride (methodOverride.A, methodOverride.B);

      OverridesAnalyzer<PropertyDefinition> propertyAnalyzer = new OverridesAnalyzer<PropertyDefinition> (definition.GetAllMixinProperties);
      foreach (Tuple<PropertyDefinition, PropertyDefinition> propertyOverride in propertyAnalyzer.Analyze (definition.Properties))
        InitializeOverride (propertyOverride.A, propertyOverride.B);

      OverridesAnalyzer<EventDefinition> eventAnalyzer = new OverridesAnalyzer<EventDefinition> (definition.GetAllMixinEvents);
      foreach (Tuple<EventDefinition, EventDefinition> eventOverride in eventAnalyzer.Analyze (definition.Events))
        InitializeOverride (eventOverride.A, eventOverride.B);
    }

    private void InitializeOverride (MemberDefinition overrider, MemberDefinition baseMember)
    {
      overrider.BaseAsMember = baseMember;
      baseMember.AddOverride (overrider);
    }

    private static bool IsVisibleToInheritors (MethodInfo method)
    {
      return method.IsPublic || method.IsFamily;
    }

    private static void ApplyMixins (BaseClassDefinition classDefinition, ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      MixinDefinitionBuilder mixinDefinitionBuilder = new MixinDefinitionBuilder (classDefinition);
      IEnumerator<MixinContext> enumerator = classContext.MixinContexts.GetEnumerator();
      for (int i = 0; enumerator.MoveNext(); ++i)
      {
        MixinDefinition mixin = mixinDefinitionBuilder.Apply (enumerator.Current);
        mixin.MixinIndex = i;
      }
    }

    private void ApplyBaseCallMethodRequirements (BaseClassDefinition classDefinition)
    {
      RequiredBaseCallMethodDefinitionBuilder builder = new RequiredBaseCallMethodDefinitionBuilder (classDefinition);
      foreach (RequiredBaseCallTypeDefinition requirement in classDefinition.RequiredBaseCallTypes)
        builder.Apply (requirement);
    }
  }
}
