using System;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.ValidationTests.ValidationSampleTypes;
using Remotion.Mixins.UnitTests.SampleTypes;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.UnitTests.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultRequiredFaceTypeRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfRequiredFaceClassNotAvailable ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassLookingLikeBaseType3), typeof (MixinWithClassThisDependency));
      DefaultValidationLog log = Validator.Validate (definition.RequiredFaceTypes[typeof (BaseType3)]);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultRequiredFaceTypeRules.FaceClassMustBeAssignableFromTargetType", log));
    }

    [Test]
    public void FailsIfRequiredFaceTypeNotVisible ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithInvisibleThisDependency));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultRequiredFaceTypeRules.RequiredFaceTypeMustBePublic", log));
    }

  }
}