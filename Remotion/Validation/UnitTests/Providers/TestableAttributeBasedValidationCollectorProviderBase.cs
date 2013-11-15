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
using System.Reflection;
using FluentValidation.Validators;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Providers;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.IntegrationTests.TestDomain.ComponentA;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Providers
{
  public class TestableAttributeBasedValidationCollectorProviderBase : AttributeBasedValidationCollectorProviderBase
  {
    private readonly IValidationPropertyRuleReflector _validationPropertyRuleReflectorMock;
    private readonly IPropertyValidator _propertyValidatorStub1;
    private readonly IPropertyValidator _propertyValidatorStub2;
    private readonly IPropertyValidator _propertyValidatorStub3;
    private readonly IPropertyValidator _propertyValidatorStub4;
    private readonly IPropertyValidator _propertyValidatorStub5;
    private readonly IPropertyValidator _propertyValidatorStub6;
    private readonly ValidatorRegistration _validatorRegistration1;
    private readonly ValidatorRegistration _validatorRegistration2;
    private readonly ValidatorRegistration _validatorRegistration3;
    private readonly ValidatorRegistration _validatorRegistration4;
    private readonly IMetaValidationRule _metaValidationRule1;
    private readonly IMetaValidationRule _metaValidationRule2;
    private readonly IMetaValidationRule _metaValidationRule3;

    public TestableAttributeBasedValidationCollectorProviderBase (
        IValidationPropertyRuleReflector validationPropertyRuleReflectorMock,
        IPropertyValidator propertyValidatorStub1 = null,
        IPropertyValidator propertyValidatorStub2 = null,
        IPropertyValidator propertyValidatorStub3 = null,
        IPropertyValidator propertyValidatorStub4 = null,
        IPropertyValidator propertyValidatorStub5 = null,
        IPropertyValidator propertyValidatorStub6 = null,
        ValidatorRegistration validatorRegistration1 = null,
        ValidatorRegistration validatorRegistration2 = null,
        ValidatorRegistration validatorRegistration3 = null,
        ValidatorRegistration validatorRegistration4 = null,
        IMetaValidationRule metaValidationRule1 = null,
        IMetaValidationRule metaValidationRule2 = null,
        IMetaValidationRule metaValidationRule3 = null)
    {
      _validationPropertyRuleReflectorMock = validationPropertyRuleReflectorMock;
      _propertyValidatorStub1 = propertyValidatorStub1;
      _propertyValidatorStub2 = propertyValidatorStub2;
      _propertyValidatorStub3 = propertyValidatorStub3;
      _propertyValidatorStub4 = propertyValidatorStub4;
      _propertyValidatorStub5 = propertyValidatorStub5;
      _propertyValidatorStub6 = propertyValidatorStub6;
      _validatorRegistration1 = validatorRegistration1;
      _validatorRegistration2 = validatorRegistration2;
      _validatorRegistration3 = validatorRegistration3;
      _validatorRegistration4 = validatorRegistration4;
      _metaValidationRule1 = metaValidationRule1;
      _metaValidationRule2 = metaValidationRule2;
      _metaValidationRule3 = metaValidationRule3;
    }

    protected override IValidationPropertyRuleReflector CreatePropertyRuleReflector (PropertyInfo property)
    {
      if (property.Name == "Position")
      {
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetAddingPropertyValidators()).Return (new[] { _propertyValidatorStub1 });
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetHardConstraintPropertyValidators())
                                            .Return (new[] { _propertyValidatorStub2 });
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetRemovingPropertyRegistrations()).Return (new ValidatorRegistration[0]);
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetMetaValidationRules()).Return (new IMetaValidationRule[0]);
      }
      else if (property.Name == "Salary")
      {
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetAddingPropertyValidators()).Return (new[] { _propertyValidatorStub3 });
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetHardConstraintPropertyValidators()).Return (new IPropertyValidator[0]);
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetRemovingPropertyRegistrations())
                                            .Return (new[] { _validatorRegistration1, _validatorRegistration2 });
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetMetaValidationRules()).Return (new IMetaValidationRule[0]);
      }
      else if (property.Name == "LastName")
      {
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetAddingPropertyValidators())
                                            .Return (new[] { _propertyValidatorStub4, _propertyValidatorStub5 });
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetHardConstraintPropertyValidators()).Return (new IPropertyValidator[0]);
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetRemovingPropertyRegistrations()).Return (new ValidatorRegistration[0]);
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetMetaValidationRules())
                                            .Return (new[] { _metaValidationRule1, _metaValidationRule3 });
      }
      else if (property.Name == "UserName")
      {
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetAddingPropertyValidators()).Return (new[] { _propertyValidatorStub6 });
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetHardConstraintPropertyValidators()).Return (new IPropertyValidator[0]);
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetRemovingPropertyRegistrations())
                                            .Return (new[] { _validatorRegistration3, _validatorRegistration4 });
        _validationPropertyRuleReflectorMock.Expect (mock => mock.GetMetaValidationRules()).Return (new[] { _metaValidationRule2 });
      }
      else
      {
        if(property.DeclaringType!=typeof(Person))
          throw new Exception (string.Format ("Property '{0}' not expected.", property.Name));
      }

      return _validationPropertyRuleReflectorMock;
    }
  }
}