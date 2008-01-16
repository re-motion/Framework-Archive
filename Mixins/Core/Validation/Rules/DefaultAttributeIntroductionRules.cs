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

    [DelegateRuleDescription (Message = "Multiple attributes of the same attribute type are introduced by mixins, but the attribute type does "
        + "not specify 'AllowMultiple = true' in its AttributeUsage declaration.")]
    private void AllowMultipleRequiredIfAttributeIntroducedMultipleTimes (DelegateValidationRule<AttributeIntroductionDefinition>.Args args)
    {
      SingleMust (AttributeUtility.IsAttributeAllowMultiple (args.Definition.AttributeType)
        || args.Definition.Target.IntroducedAttributes.GetItemCount (args.Definition.AttributeType) < 2, args.Log, args.Self);
    }
  }
}
