using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.ValidationTests.ValidationSampleTypes;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.UnitTests.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultAttributeIntroductionRulesTests : ValidationTestBase
  {
    [Test]
    public void SucceedsIfTargetClassWinsWhenDefiningAttributes ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1),
          typeof (MixinAddingBT1Attribute));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsTwiceIfDuplicateAttributeAddedByMixin ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType2), typeof (MixinAddingBT1Attribute),
          typeof (MixinAddingBT1Attribute2));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure (
          "Rubicon.Mixins.Validation.Rules.DefaultAttributeIntroductionRules.AllowMultipleRequiredIfAttributeIntroducedMultipleTimes", log));
      Assert.AreEqual (2, log.GetNumberOfFailures ());
    }

    [Test]
    public void FailsTwiceIfDuplicateAttributeAddedByMixinToMember ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithVirtualMethod),
          typeof (MixinAddingBT1AttributeToMember), typeof (MixinAddingBT1AttributeToMember2));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (
          HasFailure (
              "Rubicon.Mixins.Validation.Rules.DefaultAttributeIntroductionRules.AllowMultipleRequiredIfAttributeIntroducedMultipleTimes", log));
      Assert.AreEqual (2, log.GetNumberOfFailures ());
    }

    [Test]
    public void SucceedsIfDuplicateAttributeAddedByMixinAllowsMultiple ()
    {
      TargetClassDefinition definition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseTypeWithAllowMultiple), typeof (MixinAddingAllowMultipleToClassAndMember));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }
  }
}