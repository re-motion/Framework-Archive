using System;
using NUnit.Framework;
using Remotion.Core.UnitTests.Mixins.SampleTypes;
using Remotion.Core.UnitTests.Mixins.ValidationTests.ValidationSampleTypes;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.Core.UnitTests.Mixins.ValidationTests.Rules
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