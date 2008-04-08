using System;
using NUnit.Framework;
using Remotion.Core.UnitTests.Mixins.SampleTypes;
using Remotion.Core.UnitTests.Mixins.ValidationTests.ValidationSampleTypes;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.Core.UnitTests.Mixins.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultInterfaceIntroductionRules : ValidationTestBase
  {
    [Test]
    public void FailsIfImplementingIMixinTarget ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinImplementingIMixinTarget));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultInterfaceIntroductionRules.IMixinTargetCannotBeIntroduced", log));
    }

  }
}