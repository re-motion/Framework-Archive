using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.ValidationTests.ValidationSampleTypes;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.UnitTests.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultInterfaceIntroductionRulesTests : ValidationTestBase
  {
    [Test]
    public void FailsIfIntroducedInterfaceNotVisible ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType2), typeof (MixinIntroducingInternalInterface));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultInterfaceIntroductionRules.IntroducedInterfaceMustBePublic", log));
    }
  }
}