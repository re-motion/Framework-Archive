using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.UnitTests.Configuration.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultRequiredMethodRulesTests : ValidationTestBase
  {
    [Test]
    public void FailsIfRequiredBaseMethodIsExplit ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<ClassFulfillingAllMemberRequirementsExplicitly> ().Clear ().AddMixins (typeof (MixinRequiringAllMembersBase)).EnterScope ())
      {
        TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
            typeof (ClassFulfillingAllMemberRequirementsExplicitly), typeof (MixinRequiringAllMembersBase));
        DefaultValidationLog log = Validator.Validate (definition);

        Assert.IsTrue (
            HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultRequiredMethodRules.RequiredBaseCallMethodMustBePublicOrProtected", log));
      }
    }

    [Test]
    public void SucceedsIfRequiredFaceMethodIsExplit ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<ClassFulfillingAllMemberRequirementsExplicitly> ().Clear ().AddMixins (typeof (MixinRequiringAllMembersFace)).EnterScope ())
      {
        TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
            typeof (ClassFulfillingAllMemberRequirementsExplicitly), typeof (MixinRequiringAllMembersFace));
        DefaultValidationLog log = Validator.Validate (definition);

        AssertSuccess (log);
      }
    }
  }
}