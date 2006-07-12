using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateCombinationComparerTest : DomainTest
  {
    [Test]
    public void Equals_TwoStatelessCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      StateCombination combination1 = testHelper.CreateStateCombination (orderClass);
      StateCombination combination2 = testHelper.CreateStateCombination (orderClass);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.IsTrue (comparer.Equals (combination1, combination2));
    }

    [Test]
    public void Equals_OneStatelessAndOneWithAState ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination combination1 = testHelper.CreateStateCombination (orderClass);
      StateCombination combination2 = testHelper.CreateStateCombination (orderClass, paymentProperty["Paid"]);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.IsFalse (comparer.Equals (combination1, combination2));
    }

    [Test]
    public void Equals_TwoDifferent ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination combination1 = testHelper.CreateStateCombination (orderClass, paymentProperty["None"]);
      StateCombination combination2 = testHelper.CreateStateCombination (orderClass, paymentProperty["Paid"]);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.IsFalse (comparer.Equals (combination1, combination2));
    }

    [Test]
    public void GetHashCode_TwoStatelessCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      StateCombination combination1 = testHelper.CreateStateCombination (orderClass);
      StateCombination combination2 = testHelper.CreateStateCombination (orderClass);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.AreEqual (comparer.GetHashCode (combination1), comparer.GetHashCode (combination2));
    }

    [Test]
    public void GetHashCode_OneStatelessAndOneWithAState ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination combination1 = testHelper.CreateStateCombination (orderClass);
      StateCombination combination2 = testHelper.CreateStateCombination (orderClass, paymentProperty["Paid"]);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.AreNotEqual (comparer.GetHashCode (combination1), comparer.GetHashCode (combination2));
    }

    [Test]
    public void GetHashCode_TwoDifferent ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination combination1 = testHelper.CreateStateCombination (orderClass, paymentProperty["None"]);
      StateCombination combination2 = testHelper.CreateStateCombination (orderClass, paymentProperty["Paid"]);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.AreNotEqual (comparer.GetHashCode (combination1), comparer.GetHashCode (combination2));
    }
  }
}
