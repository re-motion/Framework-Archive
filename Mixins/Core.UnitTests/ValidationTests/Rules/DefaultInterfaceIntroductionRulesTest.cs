using System;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.ValidationTests.ValidationSampleTypes;
using Remotion.Mixins.UnitTests.SampleTypes;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.UnitTests.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultInterfaceIntroductionRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfIntroducedInterfaceNotVisible ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType2), typeof (MixinIntroducingInternalInterface));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultInterfaceIntroductionRules.IntroducedInterfaceMustBePublic", log));
    }
  }
}