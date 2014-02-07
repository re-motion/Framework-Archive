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
using System.Reflection;
using FluentValidation.Validators;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.MetaValidation.Rules.Custom;
using Remotion.Validation.Rules;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Create <see cref="IPropertyValidator"/>s based on the <see cref="ILengthConstrainedPropertyAttribute"/> and the <see cref="INullablePropertyAttribute"/>.
  /// </summary>
  public class DomainObjectAttributesBasedValidationPropertyRuleReflector : IAttributesBasedValidationPropertyRuleReflector
  {
    private readonly PropertyInfo _property;

    public DomainObjectAttributesBasedValidationPropertyRuleReflector (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      _property = property;
    }

    public PropertyInfo ValidatedProperty
    {
      get { return _property; }
    }

    public IEnumerable<IPropertyValidator> GetAddingPropertyValidators ()
    {
      var lengthConstraintAttribute = AttributeUtility.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (_property, false);
      if (lengthConstraintAttribute != null && lengthConstraintAttribute.MaximumLength.HasValue)
        yield return new LengthValidator (0, lengthConstraintAttribute.MaximumLength.Value);
    }

    public IEnumerable<IPropertyValidator> GetHardConstraintPropertyValidators ()
    {
      var nullableAttribute = AttributeUtility.GetCustomAttribute<INullablePropertyAttribute> (_property, false);
      if (nullableAttribute != null && !nullableAttribute.IsNullable)
        yield return new NotNullValidator();
    }

    public IEnumerable<ValidatorRegistration> GetRemovingPropertyRegistrations ()
    {
      return Enumerable.Empty<ValidatorRegistration>();
    }

    public IEnumerable<IMetaValidationRule> GetMetaValidationRules ()
    {
      var lengthConstraintAttribute = AttributeUtility.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (_property, false);
      if (lengthConstraintAttribute != null && lengthConstraintAttribute.MaximumLength.HasValue)
        yield return new RemotionMaxLengthMetaValidationRule (_property, lengthConstraintAttribute.MaximumLength.Value);
    }
  }
}