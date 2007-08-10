using System;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.UnitTests.Domain.AccessControl;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class SecurableClassValidationResultTest : DomainTest
  {
    [Test]
    public void IsValid_Valid ()
    {
      SecurableClassValidationResult result = new SecurableClassValidationResult ();

      Assert.IsTrue (result.IsValid);
    }

    [Test]
    public void IsValid_InvalidStateCombination ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StateCombination stateCombination = testHelper.CreateStateCombination (orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddInvalidStateCombination (stateCombination);

        Assert.IsFalse (result.IsValid);
      }
    }

    [Test]
    public void InvalidStateCombinations_AllValid ()
    {
      SecurableClassValidationResult result = new SecurableClassValidationResult ();

      Assert.AreEqual (0, result.InvalidStateCombinations.Count);
    }

    [Test]
    public void InvalidStateCombinations_OneInvalidStateCombination ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StateCombination stateCombination = testHelper.CreateStateCombination (orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddInvalidStateCombination (stateCombination);

        Assert.AreEqual (1, result.InvalidStateCombinations.Count);
        Assert.Contains (stateCombination, result.InvalidStateCombinations);
      }
    }

    [Test]
    public void InvalidStateCombinations_TwoInvalidStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty (orderClass);
        StateCombination statelessCombination = testHelper.CreateStateCombination (orderClass);
        StateCombination paidStateCombination = testHelper.CreateStateCombination (orderClass, paymentProperty["Paid"]);

        SecurableClassValidationResult result = new SecurableClassValidationResult();

        result.AddInvalidStateCombination (statelessCombination);
        result.AddInvalidStateCombination (paidStateCombination);

        Assert.AreEqual (2, result.InvalidStateCombinations.Count);
        Assert.Contains (statelessCombination, result.InvalidStateCombinations);
        Assert.Contains (paidStateCombination, result.InvalidStateCombinations);
      }
    }
  }
}
