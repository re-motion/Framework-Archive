using System;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.ValidationTests.ValidationSampleTypes;
using Remotion.Mixins.UnitTests.SampleTypes;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.UnitTests.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultRequiredBaseCallTypeRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfRequiredBaseTypeNotVisible ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithInvisibleBaseDependency));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultRequiredBaseCallTypeRules.RequiredBaseCallTypeMustBePublic", log));
    }
  }
}