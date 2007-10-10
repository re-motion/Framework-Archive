using System;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultMixinDependencyRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MixinDependencyRules.Add (new DelegateValidationRule<MixinDependencyDefinition> (DependencyMustBeSatisfiedByAnotherMixin));
    }

    private void DependencyMustBeSatisfiedByAnotherMixin (DelegateValidationRule<MixinDependencyDefinition>.Args args)
    {
      SingleMust (args.Definition.GetImplementer() != null || args.Definition.IsAggregate, args.Log, args.Self);
    }
  }
}