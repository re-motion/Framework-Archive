using System;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.ValidationTests.ValidationSampleTypes;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.UnitTests.ValidationTests.Rules
{
  [TestFixture]
  public class DefaultPropertyRulesTest : ValidationTestBase
  {
    [Test]
    public void WarnsIfPropertyOverrideAddsMethods ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseWithGetterOnly), typeof (MixinOverridingSetterOnly));
      DefaultValidationLog log =
          Validator.Validate (definition.Properties[typeof (BaseWithGetterOnly).GetProperty ("Property")].Overrides[0]);

      Assert.IsTrue (HasWarning ("Remotion.Mixins.Validation.Rules.DefaultPropertyRules.NewMemberAddedByOverride", log));
    }
  }
}