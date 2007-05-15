using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateUsageTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
    }


    [Test]
    [ExpectedException (typeof (ConstraintViolationException),
       ExpectedMessage = "The securable class definition 'Rubicon.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination, which has been defined twice.")]
    public void ValidateDuringCommit_ByTouchOnClass ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();

      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateDefinition paidState = paymentProperty["Paid"];
      StateDefinition notPaidState = paymentProperty["None"];
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass, paidState);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, notPaidState);
      StateCombination combination3 = _testHelper.CreateStateCombination (orderClass);
      combination1.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject (_testHelper.Transaction));
      combination2.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject (_testHelper.Transaction));
      combination3.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject (_testHelper.Transaction));

      _testHelper.Transaction.Commit ();

      StateUsage stateUsage = (StateUsage) combination2.StateUsages[0];
      stateUsage.StateDefinition = paidState;

      _testHelper.Transaction.Commit ();
    }

    private StateCombination GetStatelessCombinationForClass (SecurableClassDefinition classDefinition)
    {
      foreach (StateCombination currentCombination in classDefinition.StateCombinations)
      {
        if (currentCombination.StateUsages.Count == 0)
          return currentCombination;
      }

      return null;
    }

    private static List<StateDefinition> CreateEmptyStateList ()
    {
      return new List<StateDefinition> ();
    }
  }
}
