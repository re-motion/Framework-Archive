using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.UnitTests.AccessControl
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
      StateCombination combination = GetStateCombinationWithoutStates ();
      List<StateDefinition> states = CreateEmptyStateList ();

      Assert.IsTrue (combination.MatchesStates (states));
    }

    [Test]
    public void MatchesStates_StatefulAndWithoutDemandedStates ()
    {
      StateCombination combination = GetStateCombinationForDeliveredAndUnpaidOrder ();
      List<StateDefinition> states = CreateEmptyStateList ();

      Assert.IsFalse (combination.MatchesStates (states));
    }

    [Test]
    public void MatchesStates_DeliveredAndUnpaid ()
    {
      StateCombination combination = GetStateCombinationForDeliveredAndUnpaidOrder ();
      List<StateDefinition> states = CreateStateListFromCombination (combination);

      Assert.IsTrue (combination.MatchesStates (states));
    }

    [Test]
    public void MatchesStates_StatelessAndDemandDeliveredAndUnpaid ()
    {
      StateCombination deliverdAndUnpaidCombination = GetStateCombinationForDeliveredAndUnpaidOrder ();
      List<StateDefinition> states = CreateStateListFromCombination (deliverdAndUnpaidCombination);
      StateCombination statelessCombination = GetStatelessCombinationForClass (deliverdAndUnpaidCombination.ClassDefinition);

      Assert.IsFalse (statelessCombination.MatchesStates (states));
    }

    [Test]
    public void AttachState_NewState ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateSecurableClassDefinition ();
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition);
      StatePropertyDefinition property = _testHelper.CreateStateProperty ("Test");
      property.AddState ("Test1", 0);
      property.AddState ("Test2", 1);

      combination.AttachState (property["Test1"]);

      Assert.AreEqual (1, combination.StateUsages.Count);
      StateUsage stateUsage = (StateUsage) combination.StateUsages[0];
      Assert.AreSame (property["Test1"], stateUsage.StateDefinition);
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

    private StateCombination GetStateCombinationForDeliveredAndUnpaidOrder ()
    {
      List<StateCombination> stateCombinations = _testHelper.CreateStateCombinations ();
      return stateCombinations[2];
    }

    private StateCombination GetStateCombinationWithoutStates ()
    {
      List<StateCombination> stateCombinations = _testHelper.CreateStateCombinations ();
      return stateCombinations[4];
    }
  }
}
