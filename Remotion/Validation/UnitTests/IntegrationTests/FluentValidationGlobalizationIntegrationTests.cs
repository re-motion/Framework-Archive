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
using System.Linq;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Resources;
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Validation.Globalization;
using Remotion.Validation.UnitTests.IntegrationTests.CustomImplementations;
using Remotion.Validation.UnitTests.IntegrationTests.TestDomain.ComponentA;
using Remotion.Validation.UnitTests.TestHelpers;

namespace Remotion.Validation.UnitTests.IntegrationTests
{
  public class FluentValidationGlobalizationIntegrationTests : IntegrationTestBase
  {
    private PropertyRule _propertyRule;
    private Customer _customer;

    /* Note: 
     *   > ValidationFailure.ErrorMessage displays a user-friendly property name (generated by PropertyRule.GetDisplayName) 
     *   > ValidationFailure.PropertyName represents the technical property name (generated by PropertyRule.BuildPropertyName)
     */

    public override void SetUp ()
    {
      base.SetUp();

      _propertyRule = PropertyRule.Create (ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName));
      _propertyRule.AddValidator (new NotNullValidator());
      _customer = new Customer();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_propertyRule.DisplayName, Is.TypeOf(typeof(LazyStringSource)));
      Assert.That (_propertyRule.DisplayName.GetString(), Is.Null);
      Assert.That (_propertyRule.PropertyName, Is.EqualTo ("LastName"));
      Assert.That (_propertyRule.GetDisplayName(), Is.EqualTo ("Last Name"));
    }

    [Test]
    public void Validate_NoExplicitPropertyNameSet ()
    {
      var result = _propertyRule.Validate (new ValidationContext (_customer)).ToArray().First();

      Assert.That (result.PropertyName, Is.EqualTo ("LastName"));
      Assert.That (result.ErrorMessage, Is.EqualTo ("'Last Name' must not be empty."));
    }

    [Test]
    public void Validate_ExplicitPropertyNameSet ()
    {
      _propertyRule.PropertyName = "ChangedPropertyName";
      var result = _propertyRule.Validate (new ValidationContext (_customer)).ToArray().First();

      Assert.That (result.PropertyName, Is.EqualTo ("ChangedPropertyName"));
      Assert.That (result.ErrorMessage, Is.EqualTo ("'Changed Property Name' must not be empty."));
    }

    [Test]
    public void Validate_ExplicitDisplayNameSet ()
    {
      _propertyRule.DisplayName = new StaticStringSource ("ChangedDisplayName");
      var result = _propertyRule.Validate (new ValidationContext (_customer)).ToArray().First();

      Assert.That (result.PropertyName, Is.EqualTo ("LastName"));
      Assert.That (result.ErrorMessage, Is.EqualTo ("'ChangedDisplayName' must not be empty."));
    }

    [Test]
    public void Validate_ExplicitDisplayAndPropertyNameSet_DisplayNameIsUsed ()
    {
      _propertyRule.PropertyName = "ChangedPropertyName";
      _propertyRule.DisplayName = new StaticStringSource ("ChangedDisplayName");
      var result = _propertyRule.Validate (new ValidationContext (_customer)).ToArray().First();

      Assert.That (result.PropertyName, Is.EqualTo ("ChangedPropertyName"));
      Assert.That (result.ErrorMessage, Is.EqualTo ("'ChangedDisplayName' must not be empty."));
    }

    [Test]
    public void Validate_PropertyChain ()
    {
      var result =
          _propertyRule.Validate (
              new ValidationContext (_customer, new PropertyChain (new[] { "ChainedProperty1", "Chainedroperty2" }), new DefaultValidatorSelector()))
                       .ToArray();

      var validationResult = result.Single();
      Assert.That (validationResult.PropertyName, Is.EqualTo ("ChainedProperty1.Chainedroperty2.LastName"));
      Assert.That (validationResult.ErrorMessage, Is.EqualTo ("'Last Name' must not be empty."));
    }

    [Test]
    public void DefaultErrorMessage_NotNullValidator ()
    {
      var validator = new NotNullValidator();

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.notnull_error));
    }

    [Test]
    public void DefaultErrorMessage_NotEmptyValidator ()
    {
      var validator = new NotEmptyValidator (null);

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.notempty_error));
    }

    [Test]
    public void DefaultErrorMessage_NotEqualValidator ()
    {
      var validator = new NotEqualValidator (null);

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.notequal_error));
    }

    [Test]
    public void DefaultErrorMessage_EqualValidator ()
    {
      var validator = new EqualValidator (null);

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.equal_error));
    }

    [Test]
    public void DefaultErrorMessage_CreditCardValidator ()
    {
      var validator = new CreditCardValidator();

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString(), Is.EqualTo (Messages.CreditCardError));
    }

    [Test]
    public void DefaultErrorMessage_EmailValidator ()
    {
      var validator = new EmailValidator ();

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.email_error));
    }

    [Test]
    public void DefaultErrorMessage_ExactLengthValidator ()
    {
      var validator = new ExactLengthValidator (10);

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.exact_length_error));
    }

    [Test]
    public void DefaultErrorMessage_MaxLengthValidator ()
    {
      var validator = new LengthValidator (1, 3);

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.length_error));
    }

    [Test]
    public void DefaultErrorMessage_ExclusiveBetweenValidator ()
    {
      var validator = new ExclusiveBetweenValidator (1, 3);

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.exclusivebetween_error));
    }

    [Test]
    public void DefaultErrorMessage_InclusiveBetweenValidator ()
    {
      var validator = new InclusiveBetweenValidator (1, 3);

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.inclusivebetween_error));
    }

    [Test]
    public void DefaultErrorMessage_LessThanValidator ()
    {
      var validator = new LessThanValidator (1);

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.lessthan_error));
    }

    [Test]
    public void DefaultErrorMessage_LessThanOrEqualValidator ()
    {
      var validator = new LessThanOrEqualValidator (1);

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.lessthanorequal_error));
    }

    [Test]
    public void DefaultErrorMessage_GreaterThanValidator ()
    {
      var validator = new GreaterThanValidator (1);

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.greaterthan_error));
    }

    [Test]
    public void DefaultErrorMessage_GreaterThanOrEqualValidator ()
    {
      var validator = new GreaterThanOrEqualValidator (1);

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.greaterthanorequal_error));
    }

    [Test]
    public void DefaultErrorMessage_PredicateValidator ()
    {
      var validator = new PredicateValidator ((o1, o2, o3) => true);

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.predicate_error));
    }

    [Test]
    public void DefaultErrorMessage_RegularExpressionValidator ()
    {
      var validator = new RegularExpressionValidator ("");

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.regex_error));
    }

    [Test]
    public void DefaultErrorMessage_ScalePrecisionValidator ()
    {
      var validator = new ScalePrecisionValidator (2, 5);

      Assert.That (validator.ErrorMessageSource, Is.TypeOf (typeof (LocalizedStringSource)));
      Assert.That (validator.ErrorMessageSource.GetString (), Is.EqualTo (Messages.scale_precision_error));
    }
  }
}