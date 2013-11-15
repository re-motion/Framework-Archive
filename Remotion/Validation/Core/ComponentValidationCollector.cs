﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using FluentValidation.Internal;
using Remotion.Utilities;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.Rules;
using Remotion.Validation.Utilities;

namespace Remotion.Validation
{
  public abstract class ComponentValidationCollector<T> : IComponentValidationCollector<T>
  {
    private readonly TrackingCollection<IAddingComponentPropertyRule> _addedPropertyRules;
    private readonly List<IAddingComponentPropertyMetaValidationRule> _addedPropertyMetaValidationRules;
    private readonly List<IRemovingComponentPropertyRule> _removedPropertyRules;
    
    protected ComponentValidationCollector ()
    {
      _addedPropertyRules = new TrackingCollection<IAddingComponentPropertyRule>();
      _addedPropertyMetaValidationRules = new List<IAddingComponentPropertyMetaValidationRule>();
      _removedPropertyRules = new List<IRemovingComponentPropertyRule>();
    }

    public IEnumerable<IAddingComponentPropertyRule> AddedPropertyRules
    {
      get { return _addedPropertyRules.ToList().AsReadOnly(); }
    }

    public IEnumerable<IAddingComponentPropertyMetaValidationRule> AddedPropertyMetaValidationRules
    {
      get { return _addedPropertyMetaValidationRules.AsReadOnly(); }
    }

    public IEnumerable<IRemovingComponentPropertyRule> RemovedPropertyRules
    {
      get { return _removedPropertyRules.AsReadOnly(); }
    }

    public IComponentAddingRuleBuilderOptions<T, TProperty> AddRule<TProperty> (Expression<Func<T, TProperty>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      CheckNoMixinType ();

      var componentPropertyRule = AddingComponentPropertyRule.Create (expression, GetType());
      _addedPropertyRules.Add (componentPropertyRule);

      var metaValidationPropertyRule = AddingComponentPropertyMetaValidationRule.Create (expression, GetType());
      _addedPropertyMetaValidationRules.Add (metaValidationPropertyRule);

      return new AddingComponentRuleBuilder<T, TProperty> (componentPropertyRule, metaValidationPropertyRule);
    }

    public IComponentRemovingRuleBuilderOptions<T, TProperty> RemoveRule<TProperty> (Expression<Func<T, TProperty>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      CheckNoMixinType ();

      var componentPropertyRule = RemovingComponentPropertyRule.Create (expression, GetType());
      _removedPropertyRules.Add (componentPropertyRule);

      return new RemovingComponentRuleBuilder<T, TProperty> (componentPropertyRule);
    }

    public void When (Func<T, bool> predicate, Action action)
    {
      var addedPropertyRules = new List<IAddingComponentPropertyRule>();
      Action<IAddingComponentPropertyRule> onRuleAdded = addedPropertyRules.Add;

      using (_addedPropertyRules.OnItemAdded (onRuleAdded))
      {
        action();
      }

      // Must apply the predictae after the rule has been fully created to ensure any rules-specific conditions have already been applied.
      addedPropertyRules.ForEach (x => x.ApplyCondition (predicate.CoerceToNonGeneric()));
    }

    public void Unless (Func<T, bool> predicate, Action action)
    {
      When (x => !predicate (x), action);
    }

    private static void CheckNoMixinType ()
    {
      if (MixinHelper.IsMixinType (typeof (T)))
      {
        throw new NotSupportedException (
            string.Format (
                "Validation rules for concrete mixins are not supported. Please ensure to apply the rules to 'ITargetInterface' or 'IIntroducedInterface' of mixin '{0}' instead.",
                typeof (T).FullName));
      }
    }

  }
}