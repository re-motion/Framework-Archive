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
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.TypePipe;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.UnitTests.IntegrationTests.TestDomain.ComponentA;
using Remotion.Validation.UnitTests.IntegrationTests.TestDomain.ComponentB;

namespace Remotion.Validation.UnitTests.IntegrationTests
{
  public class ValidationRulesIntegrationTests : IntegrationTestBase
  {
    public override void SetUp ()
    {
      base.SetUp ();

      ShowLogOutput = false;
    }

    [Test]
    public void BuildCustomerValidator_InvalidCustomerUserName_EqualsValidatorFailed ()
    {
      var customer = ObjectFactory.Create<Customer> (ParamList.Empty);
      customer.UserName = "Test";
      customer.LastName = "Muster";

      var validator = ValidationBuilder.BuildValidator<Customer>();

      var result = validator.Validate (customer);

      Assert.That (result.IsValid, Is.False);
      Assert.That (result.Errors.Count(), Is.EqualTo (2));
      Assert.That (
          result.Errors.Select (e => e.ErrorMessage),
          Is.EquivalentTo (new[] { "'UserName' should not be equal to 'Test'.", "'LocalizedFirstName' must not be empty." }));
    }

    [Test]
    public void BuildCustomerValidator_PersonLastNameEqualsValidatorRemovedByCollector ()
    {
      var customer = ObjectFactory.Create<Customer> (ParamList.Empty);
      customer.UserName = "cust1";
      customer.LastName = "Test";
      customer.FirstName = "Firstname";

      var validator = ValidationBuilder.BuildValidator<Customer>();

      var result = validator.Validate (customer);

      Assert.That (result.IsValid, Is.True);
      Assert.That (result.Errors.Any(), Is.False);
    }

    [Test]
    public void BuildSpecialCustomerValidator_CustomerUsernameMaxLengthAndAllFirstNameNotNullValidatorsRemoved ()
        //2 NotNull Validators removed (IPerson + CustomerValidationCollector!)
    {
      var specialCustomer = ObjectFactory.Create<SpecialCustomer1> (ParamList.Empty);
      specialCustomer.UserName = "Test123456";
      specialCustomer.LastName = "Test1234";
      specialCustomer.FirstName = "Test456";
      var validator = ValidationBuilder.BuildValidator<SpecialCustomer1>();

      var result = validator.Validate (specialCustomer);

      Assert.That (result.IsValid, Is.True);
      Assert.That (result.Errors.Any(), Is.False);
    }

    [Test]
    public void BuildSpecialCustomerValidator_InvalidCustomerLastName_LengthValidatorFailed ()
        //HardConstraintLengthValidator defined in CustomerValidationCollector1 not removed by SpecialCustomerValidationCollector1!
    {
      var specialCustomer = ObjectFactory.Create<SpecialCustomer1> (ParamList.Empty);
      specialCustomer.UserName = "Test123456";
      specialCustomer.LastName = "LastNameTooLong";
      var validator = ValidationBuilder.BuildValidator<SpecialCustomer1>();

      var result = validator.Validate (specialCustomer);

      Assert.That (result.IsValid, Is.False);
      Assert.That (result.Errors[0].ErrorMessage, Is.EqualTo ("'LocalizedLastName' must be between 2 and 8 characters. You entered 15 characters."));
    }

    [Test]
    public void BuildSpecialCustomer_RemoveLastNameHardConstraint_ThrowsException ()
    {
      Assert.That (() => ValidationBuilder.BuildValidator<SpecialCustomer2> (), 
        Throws.TypeOf<ComponentValidationException> ().And.Message.EqualTo (
        "Hard constraint validator(s) 'LengthValidator' on property "
        + "'Remotion.Validation.UnitTests.IntegrationTests.TestDomain.ComponentA.Person.LastName' cannot be removed."));
    }

    [Test]
    public void BuildAdressValidator_WhenAndUnlessConditionApplied ()
    {
      var address1 = new Address { Country = "Deutschland", PostalCode = "DE - 432134" };
      var address2 = new Address { Country = "Deutschland", PostalCode = "AT - 1220" };
      var address3 = new Address { Street = "Maria Hilferstrasse 145", City = "Wien", PostalCode = "1090" };
      var address4 = new Address { Street = "Maria Hilferstrasse 145", City = "Salzburg", PostalCode = "1090" };
      var address5 = new Address { Country = "Brunei" };
      var address6 = new Address { Country = "Tschiputi" };

      var validator = ValidationBuilder.BuildValidator<Address>();

      var result1 = validator.Validate (address1);
      Assert.That (result1.IsValid, Is.True);

      var result2 = validator.Validate (address2);
      Assert.That (result2.IsValid, Is.False);
      Assert.That (result2.Errors.Count, Is.EqualTo (1));
      Assert.That (result2.Errors[0].ErrorMessage, Is.EqualTo ("'PostalCode' is not in the correct format."));

      var result3 = validator.Validate (address3);
      Assert.That (result3.IsValid, Is.True);

      var result4 = validator.Validate (address4);
      Assert.That (result4.IsValid, Is.False);
      Assert.That (result4.Errors.Count, Is.EqualTo (1));
      Assert.That (result4.Errors[0].ErrorMessage, Is.EqualTo ("'City' is not in the correct format."));

      var result5 = validator.Validate (address5);
      Assert.That (result5.IsValid, Is.True);

      var result6 = validator.Validate (address6);
      Assert.That (result6.IsValid, Is.False);
      Assert.That (result6.Errors.Count, Is.EqualTo (1));
      Assert.That (result6.Errors[0].ErrorMessage, Is.EqualTo ("'PostalCode' must not be empty."));
    }

    [Test]
    public void BuildOrderValidator_StringPropertyReStoreAttributeIsReplaced_MaxLengthMetaValidationRuleFails ()
    {
      Assert.That (
          () => ValidationBuilder.BuildValidator<Order>(),
          Throws.TypeOf<MetaValidationException>().And.Message.EqualTo (
              "'RemotionMaxLengthMetaValidationRule' failed for property 'Remotion.Validation.UnitTests.IntegrationTests.TestDomain.ComponentA.Order.Number': "
              + "Max-length validation rule value '15' exceeds meta validation rule max-length value of '10'."));
    }

    [Test]
    public void BuildProductValidator_MandatoryReStoreAttributeIsApplied ()
    {
      var product1 = new Product();
      var product2 = new Product { Name = "Test1" };

      var validator = ValidationBuilder.BuildValidator<Product>();

      var result1 = validator.Validate (product1);
      Assert.That (result1.IsValid, Is.False);
      Assert.That (result1.Errors.Count, Is.EqualTo (1));
      Assert.That (result1.Errors[0].ErrorMessage, Is.EqualTo ("'Name' must not be empty."));

      var result2 = validator.Validate (product2);
      Assert.That (result2.IsValid, Is.True);
    }

    [Test]
    public void BuildEmployeeValidator_ConditionalMessage ()
    {
      var employee = new Employee { FirstName = "FirstName", LastName = "LastName" };

      var validator = ValidationBuilder.BuildValidator<Employee>();

      var result = validator.Validate (employee);

      Assert.That (result.IsValid, Is.False);
      Assert.That (result.Errors.Count, Is.EqualTo (1));
      Assert.That (result.Errors[0].ErrorMessage, Is.EqualTo ("Conditional Message Test: Kein Gehalt definiert"));
    }

    [Test]
    public void BuildOrderItemValidator_SetValueTypeToDefaulValue_ValidationFails ()
    {
      var orderItem = new OrderItem();
      orderItem.Quantity = 0;

      var validator = ValidationBuilder.BuildValidator<OrderItem>();

      var result = validator.Validate (orderItem);

      Assert.That (result.IsValid, Is.False);
      Assert.That (result.Errors.Count, Is.EqualTo (1));
      Assert.That (result.Errors[0].ErrorMessage, Is.EqualTo ("'Quantity' should not be empty."));
    }

    [Test]
    public void BuildCustomerValidator_CustomerMixinTargetValidator ()
    {
      var customer = ObjectFactory.Create<Customer> (ParamList.Empty);
      customer.FirstName = "something";
      customer.LastName = "Mayr";
      customer.UserName = "mm2";

      var validator = ValidationBuilder.BuildValidator<Customer>();

      var result = validator.Validate (customer);

      Assert.That (result.IsValid, Is.False);
      Assert.That (result.Errors.Count, Is.EqualTo (1));
      Assert.That (result.Errors[0].ErrorMessage, Is.EqualTo ("'LocalizedFirstName' should not be equal to 'something'."));
    }

    [Test]
    public void BuildCustomerValidator_CustomerMixinIntroducedValidator_MixinInterfaceIntroducedValidatorIsRemovedByApplyWithMixinCollector ()
    {
      var customer = ObjectFactory.Create<Customer> (ParamList.Empty);
      customer.FirstName = "Ralf";
      customer.LastName = "Mayr";
      customer.UserName = "mm2";
      ((ICustomerIntroduced) customer).Title = "Chef3";

      var validator = ValidationBuilder.BuildValidator<Customer>();

      var result = validator.Validate (customer);

      Assert.That (result.IsValid, Is.True);
    }

    [Test]
    public void BuildCustomerValidator_CustomerMixinIntroducedValidator_AttributeBaseRuleNotRemoveByRuleWithRemoveFrom ()
    {
      var customer = ObjectFactory.Create<Customer> (ParamList.Empty);
      customer.FirstName = "Ralf";
      customer.LastName = "Mayr";
      customer.UserName = "mm2";
      ((ICustomerIntroduced) customer).Title = "Chef1";

      var validator = ValidationBuilder.BuildValidator<Customer> ();

      var result = validator.Validate (customer);

      Assert.That (result.IsValid, Is.False);
      Assert.That (result.Errors.Count, Is.EqualTo (1));
      Assert.That (result.Errors[0].ErrorMessage, Is.EqualTo ("'LocalizedTitle' should not be equal to 'Chef1'."));
    }
  }
}