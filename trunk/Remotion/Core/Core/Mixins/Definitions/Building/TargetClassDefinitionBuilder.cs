// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions.Building.DependencySorting;
using Remotion.Mixins.Utilities.DependencySort;
using Remotion.Utilities;
using ReflectionUtility=Remotion.Mixins.Utilities.ReflectionUtility;
using System.Linq;

namespace Remotion.Mixins.Definitions.Building
{
  public class TargetClassDefinitionBuilder
  {
    private readonly MixinDefinitionSorter _sorter = new MixinDefinitionSorter (
        new DependentMixinGrouper (), 
        new DependentObjectSorter<MixinDefinition> (new MixinDependencyAnalyzer ()));

    public TargetClassDefinition Build (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      if (classContext.Type.ContainsGenericParameters)
      {
        string message = string.Format ("The base class {0} contains generic parameters. This is not supported.", classContext.Type.FullName);
        throw new ConfigurationException (message);
      }

      var classDefinition = new TargetClassDefinition (classContext);

      var membersBuilder = new MemberDefinitionBuilder (classDefinition, ReflectionUtility.IsPublicOrProtectedOrExplicit);
      membersBuilder.Apply (classDefinition.Type);

      var attributesBuilder = new AttributeDefinitionBuilder (classDefinition);
      attributesBuilder.Apply (classDefinition.Type);

      foreach (Type faceInterface in classContext.CompleteInterfaces)
        classDefinition.RequiredTargetCallTypes.Add (new RequiredTargetCallTypeDefinition (classDefinition, faceInterface));

      ApplyMixins (classDefinition, classContext);
      ApplyMethodRequirements (classDefinition);

      AnalyzeOverrides (classDefinition);
      AnalyzeAttributeIntroductions (classDefinition);
      AnalyzeMemberAttributeIntroductions (classDefinition);
      return classDefinition;
    }

    private void ApplyMixins (TargetClassDefinition classDefinition, ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      // The MixinDefinitionSorter requires that the mixins have already been added to the class (and that the dependencoes have been set up
      // correctly). Therefore, we add all the mixins, then sort them, then re-add them in the correct order.

      var mixinDefinitionBuilder = new MixinDefinitionBuilder (classDefinition);
      foreach (var mixinContext in classContext.Mixins)
          mixinDefinitionBuilder.Apply (mixinContext, -1);

      var sortedMixins = _sorter.SortMixins (classDefinition);

      classDefinition.Mixins.Clear();
      foreach (var mixinDefinition in sortedMixins)
      {
        mixinDefinition.MixinIndex = classDefinition.Mixins.Count;
        classDefinition.Mixins.Add (mixinDefinition);
      }
    }
    
    // This can only be done once all the mixins are available, therefore, the TargetClassDefinitionBuilder has to do it.
    private void ApplyMethodRequirements (TargetClassDefinition classDefinition)
    {
      var methodRequirementBuilder = new RequiredMethodDefinitionBuilder (classDefinition);
      foreach (RequiredTargetCallTypeDefinition requirement in classDefinition.RequiredTargetCallTypes)
        methodRequirementBuilder.Apply (requirement);

      foreach (RequiredNextCallTypeDefinition requirement in classDefinition.RequiredNextCallTypes)
        methodRequirementBuilder.Apply (requirement);
    }

    private void AnalyzeOverrides (TargetClassDefinition definition)
    {
      var mixinMethods = definition.Mixins.SelectMany (m => m.Methods);
      var methodAnalyzer = new OverridesAnalyzer<MethodDefinition> (typeof (OverrideMixinAttribute), mixinMethods);
      foreach (var methodOverride in methodAnalyzer.Analyze (definition.Methods))
        InitializeOverride (methodOverride.Overrider, methodOverride.BaseMember);

      var mixinProperties = definition.Mixins.SelectMany (m => m.Properties);
      var propertyAnalyzer = new OverridesAnalyzer<PropertyDefinition> (typeof (OverrideMixinAttribute), mixinProperties);
      foreach (var propertyOverride in propertyAnalyzer.Analyze (definition.Properties))
        InitializeOverride (propertyOverride.Overrider, propertyOverride.BaseMember);

      var mixinEvents = definition.Mixins.SelectMany (m => m.Events);
      var eventAnalyzer = new OverridesAnalyzer<EventDefinition> (typeof (OverrideMixinAttribute), mixinEvents);
      foreach (var eventOverride in eventAnalyzer.Analyze (definition.Events))
        InitializeOverride (eventOverride.Overrider, eventOverride.BaseMember);
    }

    private void InitializeOverride (MemberDefinitionBase overrider, MemberDefinitionBase baseMember)
    {
      overrider.BaseAsMember = baseMember;
      baseMember.AddOverride (overrider);
    }

    private void AnalyzeAttributeIntroductions (TargetClassDefinition classDefinition)
    {
      var builder = new AttributeIntroductionDefinitionBuilder (classDefinition);

      var attributesOnMixins = from m in classDefinition.Mixins
                               from a in m.CustomAttributes
                               select a;
      var potentialSuppressors = classDefinition.CustomAttributes.Concat (attributesOnMixins);
      builder.AddPotentialSuppressors (potentialSuppressors);
      
      foreach (MixinDefinition mixin in classDefinition.Mixins)
        builder.Apply (mixin);
    }

    private void AnalyzeMemberAttributeIntroductions (TargetClassDefinition classDefinition)
    {
      // Check that SuppressAttributesAttribute cannot be applied to methods, properties, and fields.
      // As long as this holds, we don't need to deal with potential suppressors here.
      const AttributeTargets memberTargets = AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field;
      Assertion.IsTrue ((AttributeUtility.GetAttributeUsage (typeof (SuppressAttributesAttribute)).ValidOn & memberTargets) == 0, 
          "TargetClassDefinitionBuilder must be updated with AddPotentialSuppressors once SuppressAttributesAttribute supports members");

      foreach (MemberDefinitionBase member in classDefinition.GetAllMembers ())
      {
        if (member.Overrides.Count != 0)
        {
          var introductionBuilder = new AttributeIntroductionDefinitionBuilder (member);
          foreach (MemberDefinitionBase overrider in member.Overrides)
            introductionBuilder.Apply (overrider);
        }
      }
    }
  }
}

