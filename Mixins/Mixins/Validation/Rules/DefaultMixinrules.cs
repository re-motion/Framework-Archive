using System;
using Mixins.Definitions;

namespace Mixins.Validation.Rules
{
  public class DefaultMixinRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinCannotBeInterface));
    }

    private void MixinCannotBeInterface (MixinDefinition definition, IValidationLog log, DelegateValidationRule<MixinDefinition> self)
    {
      SingleMust (!definition.Type.IsInterface, log, self);
    }
  }
}