using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.Configuration.ValidationTests.ValidationSampleTypes;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.UnitTests.Configuration.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultTargetClassRulesTests : ValidationTestBase
  {
    [Test]
    public void FailsIfSealedTargetClass ()
    {
      TargetClassDefinition bc = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (DateTime));
      DefaultValidationLog log = Validator.Validate (bc);
      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustNotBeSealed", log));
      Assert.AreEqual (0, log.GetNumberOfWarnings ());
    }

    [Test]
    public void SucceedsIfAbstractTargetClass ()
    {
      TargetClassDefinition bc = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (MixinWithAbstractMembers));
      DefaultValidationLog log = Validator.Validate (bc);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfTargetClassDefinitionIsInterface ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (IBaseType2));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustNotBeAnInterface", log));
    }

    [Test]
    public void FailsIfNoPublicOrProtectedCtorInTargetClass ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithPrivateCtor),
          typeof (NullMixin));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustHavePublicOrProtectedCtor", log));
    }

		[Test]
		public void FailsIfTargetClassIsNotPublic ()
		{
			TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (InternalClass),
					typeof (NullMixin));
			DefaultValidationLog log = Validator.Validate (definition);

			Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustBePublic", log));
		}

		[Test]
		public void FailsIfNestedTargetClassIsNotPublic ()
		{
			TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (PublicNester.InternalNested),
					typeof (NullMixin));
			DefaultValidationLog log = Validator.Validate (definition);

			Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustBePublic", log));
		}

		[Test]
		public void SucceedsIfNestedTargetClassIsPublic ()
		{
			TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (PublicNester.PublicNested),
					typeof (NullMixin));
			DefaultValidationLog log = Validator.Validate (definition);

			AssertSuccess (log);
		}
  }
}