// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation.Validators;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.Utilities;

namespace Remotion.Validation.Providers
{
  public abstract class AttributeBasedValidationCollectorProviderBase : IValidationCollectorProvider
  {
    private static readonly MethodInfo s_GetValidationCollectorMethod =
        typeof (AttributeBasedValidationCollectorProviderBase).GetMethod ("GetValidationCollector", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo s_SetValidationRulesForPropertyMethod =
        typeof (AttributeBasedValidationCollectorProviderBase).GetMethod (
            "SetValidationRulesForProperty", BindingFlags.Instance | BindingFlags.NonPublic);

    public const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    protected AttributeBasedValidationCollectorProviderBase ()
    {
    }

    protected abstract IValidationPropertyRuleReflector CreatePropertyRuleReflector (PropertyInfo property);

    public IEnumerable<IEnumerable<ValidationCollectorInfo>> GetValidationCollectors (Type[] types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      return
          types.Select (type => (IComponentValidationCollector) s_GetValidationCollectorMethod.MakeGenericMethod (type).Invoke (this, null))
               .Select (collector => Enumerable.Repeat (new ValidationCollectorInfo (collector, GetType()), 1));
    }

    // ReSharper disable UnusedMember.Local
    private IComponentValidationCollector GetValidationCollector<T> ()
        // ReSharper restore UnusedMember.Local
    {
      var collectorInstance = new AttributeValidationCollector<T>();
      var properties = typeof (T).GetProperties (PropertyBindingFlags | BindingFlags.DeclaredOnly);
      foreach (var property in properties)
      {
        s_SetValidationRulesForPropertyMethod.MakeGenericMethod (typeof (T), property.PropertyType)
                                             .Invoke (this, new object[] { property, collectorInstance });
      }
      return collectorInstance;
    }

    // ReSharper disable UnusedMember.Local
    private void SetValidationRulesForProperty<T, TProperty> (PropertyInfo property, AttributeValidationCollector<T> collectorInstance)
        // ReSharper restore UnusedMember.Local
    {
      var propertyRuleReflector = CreatePropertyRuleReflector (property);
      var parameterExpression = Expression.Parameter (typeof (T), "t");
      var propertyPression = Expression.Property (parameterExpression, property);
      var propertyAccessExpression =
          (Expression<Func<T, TProperty>>) Expression.Lambda (typeof (Func<T, TProperty>), propertyPression, parameterExpression);

      AddValidationRules (collectorInstance, propertyRuleReflector, propertyAccessExpression);
      RemoveValidationRules (collectorInstance, propertyRuleReflector, propertyAccessExpression);
    }

    private void AddValidationRules<T, TProperty> (
        IComponentValidationCollector<T> collectorInstance,
        IValidationPropertyRuleReflector propertyRuleReflector,
        Expression<Func<T, TProperty>> propertyAccessExpression)
    {
      var addingValidators = propertyRuleReflector.GetAddingPropertyValidators().ToArray();
      if (addingValidators.Any())
        SetValidators (collectorInstance.AddRule (propertyAccessExpression), addingValidators, false);

      var hardConstraintValidators = propertyRuleReflector.GetHardConstraintPropertyValidators().ToArray();
      if (hardConstraintValidators.Any())
        SetValidators (collectorInstance.AddRule (propertyAccessExpression), hardConstraintValidators, true);

      var metaValidationRules = propertyRuleReflector.GetMetaValidationRules().ToArray();
      if (metaValidationRules.Any())
        SetMetaValidationRules (collectorInstance.AddRule (propertyAccessExpression), metaValidationRules);
    }

    private void RemoveValidationRules<T, TProperty> (
        IComponentValidationCollector<T> collectorInstance,
        IValidationPropertyRuleReflector propertyRuleReflector,
        Expression<Func<T, TProperty>> propertyAccessExpression)
    {
      var removingValidators = propertyRuleReflector.GetRemovingPropertyRegistrations().ToArray();
      if (removingValidators.Any())
      {
        var removingComponentRuleBuilder = collectorInstance.RemoveRule (propertyAccessExpression);
        foreach (var validatorRegistration in removingValidators)
          removingComponentRuleBuilder.Validator (validatorRegistration.ValidatorType, validatorRegistration.CollectorTypeToRemoveFrom);
      }
    }

    private void SetValidators<T, TProperty> (
        IComponentAddingRuleBuilderOptions<T, TProperty> componentRuleBuilder, IPropertyValidator[] addingValidators, bool isHardConstraint)
    {
      if (isHardConstraint)
        componentRuleBuilder.NotRemovable();
      foreach (var propertyValidator in addingValidators)
        componentRuleBuilder.SetValidator (propertyValidator);
    }

    private void SetMetaValidationRules<T, TProperty> (
        IComponentAddingRuleBuilderOptions<T, TProperty> componentRuleBuilder, IMetaValidationRule[] metaValidationRules)
    {
      var builderType = componentRuleBuilder.GetType();
      var genericAddMetadataRuleMethod = builderType.GetMethods().Single (GenericMethodFilter);

      foreach (var metaValidationRule in metaValidationRules)
      {
        var genericType = metaValidationRule.GetType().GetFirstGenericTypeParameterInHierarchy();
        var addMetadataRuleMethod = genericAddMetadataRuleMethod.MakeGenericMethod (genericType);
        addMetadataRuleMethod.Invoke (componentRuleBuilder, new[] { metaValidationRule });
      }
    }

    private static bool GenericMethodFilter (MethodInfo m)
    {
      return m.Name == "AddMetaValidationRule" && m.GetParameters().Any()
             && m.GetParameters().First().ParameterType.Namespace + "." + m.GetParameters().First().ParameterType.Name
             == typeof (IMetaValidationRule<>).FullName;
    }
  }
}