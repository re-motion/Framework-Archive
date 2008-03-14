using System;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Context.FluentBuilders;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.UnitTests.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultMixinDependencyRulesTests : ValidationTestBase
  {
    [Test]
    public void FailsIfClassMixinDependencyNotFulfilled ()
    {
      ClassContext context = new ClassContextBuilder (typeof (TargetClassWithAdditionalDependencies)).AddMixin<MixinWithAdditionalClassDependency> ().WithDependency<MixinWithNoAdditionalDependency> ().BuildClassContext ();

      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (context);
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (MixinWithAdditionalClassDependency)]);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinDependencyRules.DependencyMustBeSatisfiedByAnotherMixin", log));
    }

    [Test]
    public void FailsIfInterfaceMixinDependencyNotFulfilled ()
    {
      ClassContext context = new ClassContextBuilder (typeof (TargetClassWithAdditionalDependencies)).AddMixin<MixinWithAdditionalInterfaceDependency> ().WithDependency<IMixinWithAdditionalClassDependency> ().BuildClassContext ();

      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (context);
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (MixinWithAdditionalInterfaceDependency)]);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinDependencyRules.DependencyMustBeSatisfiedByAnotherMixin", log));
    }

  }
}