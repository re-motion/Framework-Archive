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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Validation.IntegrationTests.Testdomain;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;

namespace Remotion.Data.DomainObjects.Validation.IntegrationTests
{
  public class ValidationRulesIntegrationTests : IntegrationTestBase
  {
    public override void SetUp ()
    {
      base.SetUp ();

      ShowLogOutput = false;
    }

    [Test]
    public void BuildProductValidator_MandatoryReStoreAttributeIsApplied ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var product1 = Product.NewObject ();
        var product2 = Product.NewObject ();
        product2.Order = Order.NewObject();;

        var validator = ValidationBuilder.BuildValidator<Product> ();

        var result1 = validator.Validate (product1);
        Assert.That (result1.IsValid, Is.False);
        Assert.That (result1.Errors.Count, Is.EqualTo (1));
        Assert.That (result1.Errors[0].ErrorMessage, Is.EqualTo ("'Order' must not be empty."));

        var result2 = validator.Validate (product2);
        Assert.That (result2.IsValid, Is.True);
      }
    }

    [Test]
    //TODO AO: should be work after DomainObject-Attributes are supported for mixin classes!
    public void BuildCustomerValidator_MandatoryReStoreAttributeAppliedOnMixinClass ()
    {
      ValidationBuilder.BuildValidator<Customer> ();
    }

    [Test]
    public void BuildOrderValidator_StringPropertyReStoreAttributeIsReplaced_MaxLengthMetaValidationRuleFails ()
    {
      Assert.That (
          () => ValidationBuilder.BuildValidator<Order> (),
          Throws.TypeOf<MetaValidationException> ().And.Message.EqualTo (
              "'RemotionMaxLengthMetaValidationRule' failed for property 'Remotion.Data.DomainObjects.Validation.IntegrationTests.Testdomain.Order.Number': "
              + "Max-length validation rule value '15' exceeds meta validation rule max-length value of '10'."));
    }
  }
}