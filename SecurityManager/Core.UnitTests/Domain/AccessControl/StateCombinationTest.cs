using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Data.DomainObjects;
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
      List<StateDefinition> states = CreateStateListFromCombination (combination);

      Assert.IsTrue (combination.MatchesStates (states));
    }

    [Test]
    public void MatchesStates_StatelessAndDemandDeliveredAndUnpaid ()
    {
      StateCombination deliverdAndUnpaidCombination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder ();
      List<StateDefinition> states = CreateStateListFromCombination (deliverdAndUnpaidCombination);
      StateCombination statelessCombination = GetStatelessCombinationForClass (deliverdAndUnpaidCombination.Class);

      Assert.IsFalse (statelessCombination.MatchesStates (states));
    }

    [Test]
    public void AttachState_NewState ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition);
      StatePropertyDefinition property = _testHelper.CreateTestProperty ();
      
      combination.AttachState (property["Test1"]);

      Assert.AreEqual (1, combination.StateUsages.Count);
      StateUsage stateUsage = (StateUsage) combination.StateUsages[0];
      Assert.AreSame (property["Test1"], stateUsage.StateDefinition);
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

    private StateCombination GetStatelessCombinationForClass (SecurableClassDefinition classDefinition)
    {
      foreach (StateCombination currentCombination in classDefinition.StateCombinations)
      {
        if (currentCombination.StateUsages.Count == 0)
          return currentCombination;
      }

      return null;
    }

    private List<StateDefinition> CreateStateListFromCombination (StateCombination stateCombination)
    {
      List<StateDefinition> states = new List<StateDefinition> ();

      foreach (StateUsage usage in stateCombination.StateUsages)
        states.Add (usage.StateDefinition);

      return states;
    }

    private static List<StateDefinition> CreateEmptyStateList ()
    {
      return new List<StateDefinition> ();
    }
  }
}
