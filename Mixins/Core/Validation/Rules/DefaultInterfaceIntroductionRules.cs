using System;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultInterfaceIntroductionRules: RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.InterfaceIntroductionRules.Add (new DelegateValidationRule<InterfaceIntroductionDefinition> (IMixinTargetCannotBeIntroduced));
      visitor.InterfaceIntroductionRules.Add (new DelegateValidationRule<InterfaceIntroductionDefinition> (IntroducedInterfaceMustBePublic));
    }

    private void IMixinTargetCannotBeIntroduced (DelegateValidationRule<InterfaceIntroductionDefinition>.Args args)
    {
      SingleMust (!typeof (IMixinTarget).Equals (args.Definition.Type), args.Log, args.Self);
    }

    private void IntroducedInterfaceMustBePublic (DelegateValidationRule<InterfaceIntroductionDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
    }
  }
}