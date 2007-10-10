using System;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultBaseDependencyRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.BaseDependencyRules.Add (new DelegateValidationRule<BaseDependencyDefinition> (DependencyMustBeSatisfied));
    }

    private void DependencyMustBeSatisfied (DelegateValidationRule<BaseDependencyDefinition>.Args args)
    {
      SingleMust (args.Definition.GetImplementer() != null || args.Definition.IsAggregate, args.Log, args.Self);
    }
  }
}