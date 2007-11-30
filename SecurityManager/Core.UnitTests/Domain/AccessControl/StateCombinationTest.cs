using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Development.UnitTesting;
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
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void MatchesStates_StatelessAndWithoutDemandedStates ()
    {
      StateCombination combination = _testHelper.GetStateCombinationWithoutStates();
      List<StateDefinition> states = CreateEmptyStateList();

      Assert.IsTrue (combination.MatchesStates (states));
    }

    [Test]
    public void MatchesStates_StatefulAndWithoutDemandedStates ()
    {
      StateCombination combination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder();
      List<StateDefinition> states = CreateEmptyStateList();

      Assert.IsFalse (combination.MatchesStates (states));
    }

    [Test]
    public void MatchesStates_DeliveredAndUnpaid ()
    {
      StateCombination combination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder();
      List<StateDefinition> states = combination.GetStates();

      Assert.IsTrue (combination.MatchesStates (states));
    }

    [Test]
    public void MatchesStates_StatelessAndDemandDeliveredAndUnpaid ()
    {
      StateCombination deliverdAndUnpaidCombination = _testHelper.GetStateCombinationForDeliveredAndUnpaidOrder();
      List<StateDefinition> states = deliverdAndUnpaidCombination.GetStates();
      StateCombination statelessCombination = GetStatelessCombinationForClass (deliverdAndUnpaidCombination.Class);

      Assert.IsFalse (statelessCombination.MatchesStates (states));
    }

    [Test]
    public void AttachState_NewState ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition);
      StatePropertyDefinition property = _testHelper.CreateTestProperty();
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (StateType.Unchanged, classDefinition.State);

        combination.AttachState (property["Test1"]);

        Assert.AreEqual (1, combination.StateUsages.Count);
        StateUsage stateUsage = (StateUsage) combination.StateUsages[0];
        Assert.AreSame (property["Test1"], stateUsage.StateDefinition);
        Assert.AreEqual (StateType.Changed, classDefinition.State);
      }
    }

    [Test]
    public void AttachState_WithoutClassDefinition ()
    {
      StateCombination combination = StateCombination.NewObject();
      StatePropertyDefinition property = _testHelper.CreateTestProperty();

      combination.AttachState (property["Test1"]);

      Assert.AreEqual (1, combination.StateUsages.Count);
    }

    [Test]
    public void GetStates_Empty ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition);

      List<StateDefinition> states = combination.GetStates();

      Assert.AreEqual (0, states.Count);
    }

    [Test]
    public void GetStates_OneState ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition property = _testHelper.CreatePaymentStateProperty (classDefinition);
      StateDefinition state = (StateDefinition) property.DefinedStates[1];
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition, state);

      List<StateDefinition> states = combination.GetStates();

      Assert.AreEqual (1, states.Count);
      Assert.AreSame (state, states[0]);
    }

    [Test]
    public void GetStates_MultipleStates ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (classDefinition);
      StateDefinition paidState = (StateDefinition) paymentProperty.DefinedStates[1];
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty (classDefinition);
      StateDefinition deliveredState = (StateDefinition) orderStateProperty.DefinedStates[1];
      StateCombination combination = _testHelper.CreateStateCombination (classDefinition, paidState, deliveredState);

      List<StateDefinition> states = combination.GetStates();

      Assert.AreEqual (2, states.Count);
      Assert.Contains (paidState, states);
      Assert.Contains (deliveredState, states);
    }

    [Test]
    [ExpectedException (typeof (ConstraintViolationException),
        ExpectedMessage =
        "The securable class definition 'Rubicon.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination, which has been defined twice."
        )]
    public void ValidateDuringCommit_ByTouchOnClassForChangedStateUsagesCollection ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateEmptyDomain();

      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateDefinition paidState = paymentProperty["Paid"];
      StateDefinition notPaidState = paymentProperty["None"];
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass, paidState);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, notPaidState);
      StateCombination combination3 = _testHelper.CreateStateCombination (orderClass);
      combination1.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());
      combination2.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());
      combination3.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());

      _testHelper.Transaction.Commit();

      combination2.StateUsages.Remove (combination2.StateUsages[0]);
      combination2.AttachState (paidState);

      _testHelper.Transaction.Commit();
    }

    [Test]
    [Ignore ("TODO: MK - check this test")]
    public void Commit_DeletedStateCombination ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateEmptyDomain();

      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StateCombination combination = _testHelper.CreateStateCombination (orderClass);
      combination.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());

      _testHelper.Transaction.Commit();

      Assert.AreEqual (StateType.Unchanged, GetStateFromDataContainer(orderClass));
      using (_testHelper.Transaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, GetStateFromDataContainer (orderClass));
        combination.AccessControlList.Delete();
        Assert.IsNull (combination.Class);

        Assert.AreEqual (StateType.Unchanged, GetStateFromDataContainer (orderClass));
        ClientTransaction.Current.Commit();
      }
      Assert.AreEqual (StateType.Changed, GetStateFromDataContainer (orderClass));
    }

    private StateType GetStateFromDataContainer (DomainObject orderClass)
    {
      return ((DataContainer)PrivateInvoke.InvokeNonPublicMethod (orderClass, typeof (DomainObject), "GetDataContainer")).State;
    }

    [Test]
    public void SetAndGet_Index ()
    {
      StateCombination stateCombination = StateCombination.NewObject();

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
      return new List<StateDefinition>();
    }
  }
}