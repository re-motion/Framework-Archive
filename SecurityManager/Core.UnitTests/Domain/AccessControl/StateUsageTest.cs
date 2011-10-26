// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateUsageTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    [ExpectedException (typeof (ConstraintViolationException), ExpectedMessage =
        "The securable class definition 'Remotion.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination "
        + "that has been defined twice.")]
    public void ValidateDuringCommit ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateDefinition paidState = paymentProperty[EnumWrapper.Get (PaymentState.Paid).Name];
      StateDefinition notPaidState = paymentProperty[EnumWrapper.Get (PaymentState.None).Name];
      _testHelper.CreateStateCombination (orderClass, paidState);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, notPaidState);
      _testHelper.CreateStateCombination (orderClass);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        StateUsage stateUsage = combination2.StateUsages[0];
        stateUsage.StateDefinition = paidState;

        ClientTransaction.Current.Commit();
      }
    }

    [Test]
    public void TouchClassOnCommit ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateDefinition paidState = paymentProperty[EnumWrapper.Get (PaymentState.Paid).Name];
      StateCombination combination = _testHelper.CreateStateCombination (orderClass, paidState);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        orderClass.Committing += delegate { commitOnClassWasCalled = true; };
        StateUsage stateUsage = combination.StateUsages[0];
        stateUsage.MarkAsChanged();

        ClientTransaction.Current.Commit();

        Assert.That (commitOnClassWasCalled, Is.True);
      }
    }
  }
}
