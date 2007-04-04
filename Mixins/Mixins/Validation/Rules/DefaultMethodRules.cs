using System;
using Mixins.Definitions;

namespace Mixins.Validation.Rules
{
  public class DefaultMethodRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (OverriddenMethodMustBeVirtual));
    }

    private void OverriddenMethodMustBeVirtual (MethodDefinition definition, IValidationLog log, DelegateValidationRule<MethodDefinition> self)
    {
      SingleMust (definition.Overrides.GetEnumerator ().MoveNext () ? definition.MethodInfo.IsVirtual : true, log, self);
    }
  }
}
