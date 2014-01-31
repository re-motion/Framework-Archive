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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Remotion.Utilities;

namespace Remotion.Validation.Implementation
{
  //TODO AO: extract TypedValidatorDecorator<T>
  //TODO AO: add Create<T> method
  //TODO AO: pass validator type to ctor
  public sealed class Validator<T> : IValidator<T>
  {
    private readonly IReadOnlyCollection<IValidationRule> _validationRules;

    public Validator (IEnumerable<IValidationRule> validationRules)
    {
      ArgumentUtility.CheckNotNull ("validationRules", validationRules);

      _validationRules = validationRules.ToList().AsReadOnly();
    }

    public IReadOnlyCollection<IValidationRule> ValidationRules
    {
      get { return _validationRules; }
    }

    public ValidationResult Validate (T instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      return Validate (new ValidationContext<T> (instance, new PropertyChain(), new DefaultValidatorSelector()));
    }

    public ValidationResult Validate (ValidationContext<T> context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var failures = _validationRules.SelectMany (r => r.Validate (context)).ToList();
      return new ValidationResult (failures);
    }

    public IValidatorDescriptor CreateDescriptor ()
    {
      return new ValidatorDescriptor<T> (_validationRules);
    }

    public bool CanValidateInstancesOfType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return typeof (T).IsAssignableFrom (type);
    }

    ValidationResult IValidator.Validate (object instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      if (!CanValidateInstancesOfType (instance.GetType()))
      {
        throw new InvalidOperationException (
            string.Format (
                "Cannot validate instances of type '{0}'. This validator can only validate instances of type '{1}'.",
                instance.GetType().Name,
                typeof (T).Name));
      }

      return Validate ((T) instance);
    }

    ValidationResult IValidator.Validate (ValidationContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var newContext = new ValidationContext<T> ((T) context.InstanceToValidate, context.PropertyChain, context.Selector);
      return Validate (newContext);
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public IEnumerator<IValidationRule> GetEnumerator ()
    {
      return _validationRules.GetEnumerator();
    }

    CascadeMode IValidator<T>.CascadeMode
    {
      get { throw new NotSupportedException (string.Format ("CascadeMode is not supported for a '{0}'", typeof (Validator<>).FullName)); }
      set { throw new NotSupportedException (string.Format ("CascadeMode is not supported for a '{0}'", typeof (Validator<>).FullName)); }
    }
  }
}