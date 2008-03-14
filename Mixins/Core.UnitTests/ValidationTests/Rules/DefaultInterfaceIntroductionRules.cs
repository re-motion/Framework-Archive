using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.ValidationTests.ValidationSampleTypes;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.UnitTests.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultInterfaceIntroductionRules : ValidationTestBase
  {
    [Test]
    public void FailsIfImplementingIMixinTarget ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinImplementingIMixinTarget));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Rubicon.Mixins.Validation.Rules.DefaultInterfaceIntroductionRules.IMixinTargetCannotBeIntroduced", log));
    }

  }
}