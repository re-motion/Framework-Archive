using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.ValidationTests.ValidationSampleTypes;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.UnitTests.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultMixinRulesTests : ValidationTestBase
  {
    [Test]
    public void FailsIfMixinAppliedToItself ()
    {
      TargetClassDefinition bc = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (object), typeof (object));
      DefaultValidationLog log = Validator.Validate (bc);
      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinCannotMixItself", log));
    }

    [Test]
    public void FailsIfMixinAppliedToItsBase ()
    {
      TargetClassDefinition bc = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (object), typeof (NullMixin));
      DefaultValidationLog log = Validator.Validate (bc);
      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinCannotMixItsBase", log));
    }

    [Test]
    public void FailsIfMixinIsInterface ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (IBT1Mixin1));
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (IBT1Mixin1)]);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinCannotBeInterface", log));
    }

    [Test]
    public void FailsIfMixinNonPublic ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin2));
      DefaultValidationLog log = Validator.Validate (definition.Mixins[typeof (BT5Mixin2)]);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void SucceedsIfNestedPublicMixin ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (PublicNester.PublicNested));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfNestedPublicMixinInNonPublic ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (InternalNester.PublicNested));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void FailsIfNestedPrivateMixin ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (PublicNester.InternalNested));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void FailsIfNestedPrivateMixinInNonPublic ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (InternalNester.InternalNested));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log));
    }

    [Test]
    public void FailsIfNoPublicOrProtectedDefaultCtorInMixinClassWithOverriddenMembers ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod),
          typeof (MixinWithPrivateCtorAndVirtualMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultMixinRules.MixinWithOverriddenMembersMustHavePublicOrProtectedDefaultCtor",
          log));
    }

    [Test]
    public void SucceedsIfNoPublicOrProtectedDefaultCtorInMixinClassWithoutOverriddenMembers ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (NullTarget),
          typeof (MixinWithPrivateCtorAndVirtualMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

  }
}