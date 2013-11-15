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
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Rules;

namespace Remotion.Validation.RuleBuilders
{
  //TODO MK: Review
  public class AddingComponentRuleBuilder<T, TProperty> : IComponentAddingRuleBuilderOptions<T, TProperty>
  {
    private readonly IAddingComponentPropertyRule _addingComponentPropertyRule;
    private readonly IAddingComponentPropertyMetaValidationRule _addingMetaValidationPropertyRule;

    public AddingComponentRuleBuilder (IAddingComponentPropertyRule addingComponentPropertyPropertyRule, IAddingComponentPropertyMetaValidationRule addingMetaValidationPropertyRule)
    {
      ArgumentUtility.CheckNotNull ("addingComponentPropertyPropertyRule", addingComponentPropertyPropertyRule);
      ArgumentUtility.CheckNotNull ("addingMetaValidationPropertyRule", addingMetaValidationPropertyRule);
      
      _addingComponentPropertyRule = addingComponentPropertyPropertyRule;
      _addingMetaValidationPropertyRule = addingMetaValidationPropertyRule;
    }

    public IAddingComponentPropertyRule AddingComponentPropertyRule
    {
      get { return _addingComponentPropertyRule; }
    }

    public IAddingComponentPropertyMetaValidationRule AddingMetaValidationPropertyRule
    {
      get { return _addingMetaValidationPropertyRule; }
    }

    public IRuleBuilderOptions<T, TProperty> NotRemovable ()
    {
      _addingComponentPropertyRule.SetHardConstraint();
      return this;
    }

    public IRuleBuilderOptions<T, TProperty> AddMetaValidationRule<TValidator> (IMetaValidationRule<TValidator> metaValidationRule)
        where TValidator: IPropertyValidator
    {
      ArgumentUtility.CheckNotNull ("metaValidationRule", metaValidationRule);

      _addingMetaValidationPropertyRule.RegisterMetaValidationRule (metaValidationRule);
      return this;
    }

    public IRuleBuilderOptions<T, TProperty> AddMetaValidationRule<TValidator> (
        Func<IEnumerable<TValidator>, MetaValidationRuleValidationResult> metaValidationRuleExecutor) where TValidator: IPropertyValidator
    {
      ArgumentUtility.CheckNotNull ("metaValidationRuleExecutor", metaValidationRuleExecutor);

      var metaValidationRule = new DelegateMetaValidationRule<TValidator> (metaValidationRuleExecutor);
      _addingMetaValidationPropertyRule.RegisterMetaValidationRule (metaValidationRule);
      return this;
    }

    public IRuleBuilderOptions<T, TProperty> AddMetaValidationRule<TValidator> (
        Expression<Func<IEnumerable<TValidator>, bool>> metaValidationRuleExpression) where TValidator: IPropertyValidator
    {
      var metaValidationRuleExecutor = metaValidationRuleExpression.Compile();
     
      var metaValidationRule = new DelegateMetaValidationRule<TValidator> (
          validationRules =>
          {
            var isValid = metaValidationRuleExecutor (validationRules);
            if (isValid)
              return MetaValidationRuleValidationResult.CreateValidResult();

            return MetaValidationRuleValidationResult.CreateInvalidResult (
                "Meta validation rule '{0}' failed for validator '{1}' on property '{2}.{3}'.",
                metaValidationRuleExpression,
                typeof (TValidator).FullName,
                _addingComponentPropertyRule.Property.ReflectedType.FullName,
                _addingComponentPropertyRule.Property.Name);
          });

      _addingMetaValidationPropertyRule.RegisterMetaValidationRule (metaValidationRule);
      return this;
    }

    public IRuleBuilderOptions<T, TProperty> SetValidator (IPropertyValidator validator)
    {
      ArgumentUtility.CheckNotNull ("validator", validator);

      AddValidator (validator);
      return this;
    }

    IRuleBuilderOptions<T, TProperty> IRuleBuilder<T, TProperty>.SetValidator (IValidator<TProperty> validator)
        //called from FluentValidation ExtensionMethod
    {
      AddValidator (new ChildValidatorAdaptor (validator));
      return this;
    }

    IRuleBuilderOptions<T, TProperty> IConfigurable<PropertyRule, IRuleBuilderOptions<T, TProperty>>.Configure (Action<PropertyRule> configurator)
    {
      ArgumentUtility.CheckNotNull ("configurator", configurator);

      configurator ((PropertyRule) AddingComponentPropertyRule);
      return this;
    }

    private void AddValidator (IPropertyValidator validator)
    {
      _addingComponentPropertyRule.RegisterValidator (validator);
    }

    IRuleBuilderOptions<T, TProperty> IRuleBuilder<T, TProperty>.SetValidator (IValidator validator)
    {
      throw new NotSupportedException (
          "This overload of SetValidator is no longer used. If you are trying to set a child validator for a collection, use SetCollectionValidator instead.");
    }
  }
}