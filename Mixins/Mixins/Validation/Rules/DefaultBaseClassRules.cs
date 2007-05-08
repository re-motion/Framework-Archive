using System;
using Mixins.Definitions;

namespace Mixins.Validation.Rules
{
  public class DefaultBaseClassRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.BaseClassRules.Add (new DelegateValidationRule<BaseClassDefinition> (BaseClassMustNotBeSealed));
      visitor.BaseClassRules.Add (new DelegateValidationRule<BaseClassDefinition> (BaseClassMustNotBeAnInterface));
    }

    private void BaseClassMustNotBeSealed (DelegateValidationRule<BaseClassDefinition>.Args args)
    {
      SingleMust(!args.Definition.Type.IsSealed, args.Log, args.Self);
    }

    private void BaseClassMustNotBeAnInterface (DelegateValidationRule<BaseClassDefinition>.Args args)
    {
      SingleMust (!args.Definition.Type.IsInterface, args.Log, args.Self);
    }
  }
}
