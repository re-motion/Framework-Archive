using System;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Validation;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultAttributeIntroductionRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.AttributeIntroductionRules.Add (
          new DelegateValidationRule<AttributeIntroductionDefinition> (AllowMultipleRequiredIfAttributeIntroducedMultipleTimes));
    }

    private void AllowMultipleRequiredIfAttributeIntroducedMultipleTimes (DelegateValidationRule<AttributeIntroductionDefinition>.Args args)
    {
      SingleMust (AttributeUtility.IsAttributeAllowMultiple (args.Definition.AttributeType)
        || args.Definition.Target.IntroducedAttributes.GetItemCount (args.Definition.AttributeType) < 2, args.Log, args.Self);
    }
  }
}
