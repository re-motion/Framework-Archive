using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.UnitTests.ValidationTests
{
  [TestFixture]
  public class DelegateRuleDescriptionTests
  {
    private void NonDescribedSampleRule (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
    }

    [DelegateRuleDescription]
    private void DescribedSampleRule_NoChanges (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
    }

    [DelegateRuleDescription (RuleName = "Fritz", Message = "Echo?")]
    private void DescribedSampleRule_Changes (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
    }

    [Test]
    public void DefaultDescription ()
    {
      IValidationRule rule = new DelegateValidationRule<TargetClassDefinition> (NonDescribedSampleRule);
      Assert.AreEqual ("Rubicon.Mixins.UnitTests.ValidationTests.DelegateRuleDescriptionTests.NonDescribedSampleRule", rule.RuleName);
      Assert.AreEqual ("Non described sample rule", rule.Message);
    }

    [Test]
    public void DescriptionAttribute_NoChanges ()
    {
      IValidationRule rule = new DelegateValidationRule<TargetClassDefinition> (DescribedSampleRule_NoChanges);
      Assert.AreEqual ("Rubicon.Mixins.UnitTests.ValidationTests.DelegateRuleDescriptionTests.DescribedSampleRule_NoChanges", rule.RuleName);
      Assert.AreEqual ("Described sample rule_ no changes", rule.Message);
    }

    [Test]
    public void DescriptionAttribute_Changes ()
    {
      IValidationRule rule = new DelegateValidationRule<TargetClassDefinition> (DescribedSampleRule_Changes);
      Assert.AreEqual ("Fritz", rule.RuleName);
      Assert.AreEqual ("Echo?", rule.Message);
    }
  }
}