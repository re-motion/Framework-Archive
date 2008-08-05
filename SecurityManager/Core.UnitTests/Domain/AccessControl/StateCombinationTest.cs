/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
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
    [ExpectedException (typeof (ConstraintViolationException), ExpectedMessage =
        "The securable class definition 'Remotion.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination "
        + "that has been defined twice.")]
    public void ValidateDuringCommit_ByTouchOnClassForChangedStateUsagesCollection ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateDefinition paidState = paymentProperty[new EnumWrapper (PaymentState.Paid).Name];
      StateDefinition notPaidState = paymentProperty[new EnumWrapper (PaymentState.None).Name];
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass, paidState);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, notPaidState);
      StateCombination combination3 = _testHelper.CreateStateCombination (orderClass);
      combination1.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());
      combination2.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());
      combination3.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());

      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        combination2.StateUsages.Remove (combination2.StateUsages[0]);
        combination2.AttachState (paidState);

        ClientTransaction.Current.Commit();
      }
    }

    [Test]
    public void Commit_DeletedStateCombination ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition();
      StateCombination combination = _testHelper.CreateStateCombination (orderClass);
      combination.AccessControlList.AccessControlEntries.Add (AccessControlEntry.NewObject());

      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (StateType.Unchanged, GetStateFromDataContainer (orderClass));
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.AreEqual (StateType.Unchanged, GetStateFromDataContainer (orderClass));
          combination.AccessControlList.Delete();
          Assert.IsNull (combination.Class);

          Assert.AreEqual (StateType.Unchanged, GetStateFromDataContainer (orderClass));
          ClientTransaction.Current.Commit();
        }
        Assert.AreEqual (StateType.Unchanged, GetStateFromDataContainer (orderClass));
      }
    }

    private StateType GetStateFromDataContainer (DomainObject orderClass)
    {
      DataContainer dataContainer =
          (DataContainer)
          PrivateInvoke.InvokeNonPublicMethod (orderClass, typeof (DomainObject), "GetDataContainerForTransaction", orderClass.ClientTransaction);
      return dataContainer.State;
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