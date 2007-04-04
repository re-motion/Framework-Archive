using System;
using Mixins.Definitions;

namespace Mixins.Validation.Rules
{
  public class DefaultBaseClassRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.BaseClassRules.Add (new DelegateValidationRule<BaseClassDefinition> (BaseClassMustNotBeSealed));
    }

    private void BaseClassMustNotBeSealed (BaseClassDefinition definition, IValidationLog log, DelegateValidationRule<BaseClassDefinition> self)
    {
      SingleMust(!definition.Type.IsSealed, log, self);
    }
  }
}
