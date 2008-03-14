using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.ValidationTests.ValidationSampleTypes;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.UnitTests.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultBaseDependencyRulesTests : ValidationTestBase
  {
    [Test]
    public void FailsIfEmptyBaseDependencyNotFulfilled ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (MixinWithUnsatisfiedEmptyBaseDependency));
      DefaultValidationLog log = Validator.Validate (
          definition.Mixins[typeof (MixinWithUnsatisfiedEmptyBaseDependency)].
              BaseDependencies[typeof (IEmptyInterface)]);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultBaseDependencyRules.DependencyMustBeSatisfied", log));
    }

    [Test]
    public void SucceedsIfDuckBaseDependency ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassFulfillingAllMemberRequirementsDuck),
          typeof (MixinRequiringAllMembersBase));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfAggregateBaseDependencyIsFullyImplemented ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Base));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfEmptyAggregateBaseDependencyIsNotAvailable ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (NullTarget), typeof (MixinWithUnsatisfiedEmptyAggregateBaseDependency));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultBaseDependencyRules.DependencyMustBeSatisfied", log));
    }
  }
}