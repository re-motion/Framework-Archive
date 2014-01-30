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
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.Rules;

namespace Remotion.Validation
{
  //TODO AO: move to other file (...1)
  /// <summary>
  /// Defines an API to register component-specific rules for providing the validation semantic of <typeparamref name="TValidatedType"/>.
  /// </summary>
  public interface IComponentValidationCollector<TValidatedType> : IComponentValidationCollector
  {
    /// <summary>
    /// Registers a new validation rule for a property. 
    /// </summary>
    /// <typeparam name="TProperty">The <see cref="Type"/> of the validated property (used only for syntactical sugar).</typeparam>
    /// <param name="propertySelector">Specifies the property for which the validation rule is added.</param>
    /// <returns>A builder object used for specifying the validation rules of the property.</returns>
    /// <remarks>TODO MK: usage sample</remarks>
    IComponentAddingRuleBuilderOptions<TValidatedType, TProperty> AddRule<TProperty> (Expression<Func<TValidatedType, TProperty>> propertySelector);

    /// <summary>
    /// Registers which validation rules should be removed from the property. This is used to remove validation rules introduced by other validation 
    /// collectors of <typeparamref name="TValidatedType"/>.
    /// </summary>
    /// <typeparam name="TProperty">The <see cref="Type"/> of the validated property (used only for syntactical sugar).</typeparam>
    /// <param name="propertySelector">Specifies the property for which a specific validation rule should be removed.</param>
    /// <returns>A builder object used for specifying the validation rules to be removed from the property.</returns>
    /// <remarks>TODO MK: usage sample</remarks>
    IComponentRemovingRuleBuilderOptions<TValidatedType, TProperty> RemoveRule<TProperty> (
        Expression<Func<TValidatedType, TProperty>> propertySelector);

    /// <summary>
    /// Wraps multiple calls of <see cref="AddRule{T}"/> with a common <paramref name="predicate"/>. 
    /// The rule is only active if the predicate evaluates <see langword="true" />.
    /// It is not supported to remove validation rules or apply meta validation rules.
    /// </summary>
    /// <param name="predicate">The condition applied to each added validation rule.</param>
    /// <param name="action">Place calls to <see cref="AddRule{T}"/> within this delegate.</param>
    /// <remarks>TODO MK: usage sample</remarks>
    void When (Func<TValidatedType, bool> predicate, Action action);

    /// <summary>
    /// Wraps multiple calls of <see cref="AddRule{T}"/> with a common <paramref name="predicate"/>. 
    /// The rule is only active if the predicate evaluates <see langword="false" />.
    /// It is not supported to remove validation rules or apply meta validation rules.
    /// </summary>
    /// <param name="predicate">The condition applied to each added validation rule.</param>
    /// <param name="action">Place calls to <see cref="AddRule{T}"/> within this delegate.</param>
    /// <remarks>TODO MK: usage sample</remarks>
    void Unless (Func<TValidatedType, bool> predicate, Action action);
  }

  /// <summary>
  /// Defines an API to retrieve the component-specific rules for providing the validation semantic of a <see cref="Type"/>.
  /// </summary>
  public interface IComponentValidationCollector
  {
    //TODO AO: Add Property Type ValidatedType and updte doc to use ValidatedType instead of Type.

    /// <summary>
    /// Gets the <see cref="IAddingComponentPropertyRule"/>s added to the validation semtantic of the <see cref="Type"/>.
    /// </summary>
    IEnumerable<IAddingComponentPropertyRule> AddedPropertyRules { get; }

    /// <summary>
    /// Gets the <see cref="IAddingComponentPropertyMetaValidationRule"/>s added to the validation semtantic of the <see cref="Type"/>.
    /// Meta validation rules are used to ensure that the validation semantic of the type is still consistent after all valiation rules 
    /// from all components are applied.
    /// </summary>
    IEnumerable<IAddingComponentPropertyMetaValidationRule> AddedPropertyMetaValidationRules { get; }

    /// <summary>
    /// Gets the <see cref="IRemovingComponentPropertyRule"/>s used to remove validation rules from the <see cref="Type"/>.
    /// </summary>
    IEnumerable<IRemovingComponentPropertyRule> RemovedPropertyRules { get; }
  }
}