using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateCombinationTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
    }

    [Test]
    public void MatchesStates_StatelessAndWithoutDemandedStates ()
    {
      StateCombination combination = _testHelper.GetStateCombinationWithoutStates ();
      List<StateDefinition> states = CreateEmptyStateList ();

      Assert.IsTrue (combination.MatchesStates (states));
    }

    [Test]
    public void MatchesStates_StatefulAndWithoutDemandedStates ()
    {
      StateCombination combination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder ();
      List<StateDefinition> states = CreateEmptyStateList ();

      Assert.IsFalse (combination.MatchesStates (states));
    }

    [Test]
    public void MatchesStates_DeliveredAndUnpaid ()
    {
      StateCombination combination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder ();
      List<StateDefinition> states = combination.GetStates ();

      Assert.IsTrue (combination.MatchesStates (states));
    }

    [Test]
    public void MatchesStates_StatelessAndDemandDeliveredAndUnpaid ()
    {
      StateCombination deliverdAndUnpaidCombination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder ();
      List<StateDefinition> states = deliverdAndUnpaidCombination.GetStates ();
      StateCombination statelessCombination = GetStatelessCombinationForClass (deliverdAndUnpaidCombination.Class);

      Assert.IsFalse (statelessCombination.MatchesStates (states));
    }

    [Test]
    public void AttachState_NewState ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition);
      StatePropertyDefinition property = _testHelper.CreateTestProperty ();
      DateTime changedAt = classDefinition.ChangedAt;
      Thread.Sleep (50);
   
      combination.AttachState (property["Test1"]);

      Assert.AreEqual (1, combination.StateUsages.Count);
      StateUsage stateUsage = (StateUsage) combination.StateUsages[0];
      Assert.AreSame (property["Test1"], stateUsage.StateDefinition);
      Assert.Greater ((decimal) classDefinition.ChangedAt.Ticks, (decimal) changedAt.Ticks);
    }

    [Test]
    public void AttachState_WithoutClassDefinition ()
    {
      StateCombination combination = StateCombination.NewObject (_testHelper.Transaction);
      StatePropertyDefinition property = _testHelper.CreateTestProperty ();

      combination.AttachState (property["Test1"]);

      Assert.AreEqual (1, combination.StateUsages.Count);
    }

    [Test]
    public void GetStates_Empty ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition);

      List<StateDefinition> states = combination.GetStates ();

      Assert.AreEqual (0, states.Count);
    }

    [Test]
    public void GetStates_OneState ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition property = _testHelper.CreatePaymentStateProperty (classDefinition);
      StateDefinition state = (StateDefinition) property.DefinedStates[1];
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition, state);

      List<StateDefinition> states = combination.GetStates ();

      Assert.AreEqual (1, states.Count);
      Assert.AreSame (state, states[0]);
    }

    [Test]
    public void GetStates_MultipleStates ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (classDefinition);
      StateDefinition paidState = (StateDefinition) paymentProperty.DefinedStates[1];
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty (classDefinition);
      StateDefinition deliveredState = (StateDefinition) orderStateProperty.DefinedStates[1];
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition, paidState, deliveredState);

      List<StateDefinition> states = combination.GetStates ();

      Assert.AreEqual (2, states.Count);
      Assert.Contains (paidState, states);
      Assert.Contains (deliveredState, states);
    }

    [Test]
    [ExpectedException (typeof (ConstraintViolationException),
       ExpectedMessage = "The securable class definition 'Rubicon.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination, which has been defined twice.")]
    public void ValidateDuringCommit_ByTouchOnClassForChangedStateUsagesCollection ()
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

      combination2.StateUsages.Remove (combination2.StateUsages[0]);
      combination2.AttachState (paidState);

      _testHelper.Transaction.Commit ();
    }

    [Test]
    public void Commit_DeletedStateCombination ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();

      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StateCombination combination = _testHelper.CreateStateCombination (orderClass);
      combination.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject (_testHelper.Transaction));

      _testHelper.Transaction.Commit ();

      DateTime creationDate = orderClass.ChangedAt;
      Thread.Sleep (50);
      combination.AccessControlList.Delete ();
      Assert.IsNull (combination.Class);
      
      _testHelper.Transaction.Commit ();

      Assert.AreEqual ((decimal) orderClass.ChangedAt.Ticks, (decimal) creationDate.Ticks);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      StateCombination stateCombination = StateCombination.NewObject (_testHelper.Transaction);

      stateCombination.Index = 1;
      Assert.AreEqual (1, stateCombination.Index);
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
