using System;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateCombinationComparerTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void Equals_TwoStatelessCombinations ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.IsTrue (comparer.Equals (combination1, combination2));
    }

    [Test]
    public void Equals_OneStatelessAndOneWithAState ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, paymentProperty["Paid"]);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.IsFalse (comparer.Equals (combination1, combination2));
    }

    [Test]
    public void Equals_TwoDifferent ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass, paymentProperty["None"]);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, paymentProperty["Paid"]);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.IsFalse (comparer.Equals (combination1, combination2));
    }

    [Test]
    public void GetHashCode_TwoStatelessCombinations ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.AreEqual (comparer.GetHashCode (combination1), comparer.GetHashCode (combination2));
    }

    [Test]
    public void GetHashCode_OneStatelessAndOneWithAState ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, paymentProperty["Paid"]);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.AreNotEqual (comparer.GetHashCode (combination1), comparer.GetHashCode (combination2));
    }

    [Test]
    public void GetHashCode_TwoDifferent ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination combination1 = _testHelper.CreateStateCombination (orderClass, paymentProperty["None"]);
      StateCombination combination2 = _testHelper.CreateStateCombination (orderClass, paymentProperty["Paid"]);

      StateCombinationComparer comparer = new StateCombinationComparer ();

      Assert.AreNotEqual (comparer.GetHashCode (combination1), comparer.GetHashCode (combination2));
    }
  }
}
