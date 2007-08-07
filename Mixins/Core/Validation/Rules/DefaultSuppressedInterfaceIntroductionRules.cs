using System;
using System.Collections.Generic;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultSuppressedInterfaceIntroductionRules: RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.SuppressedInterfaceIntroductionRules.Add (
          new DelegateValidationRule<SuppressedInterfaceIntroductionDefinition> (InterfaceIsShadowedByTargetClass));
    }

    private void InterfaceIsShadowedByTargetClass (DelegateValidationRule<SuppressedInterfaceIntroductionDefinition>.Args args)
    {
      SingleShould (!args.Definition.IsShadowed, args.Log, args.Self);
    }
  }
}