using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;
using Mixins.Validation;

namespace Mixins.Validation.Rules
{
  public class DefaultInterfaceIntroductionRules: RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.InterfaceIntroductionRules.Add (new DelegateValidationRule<InterfaceIntroductionDefinition> (InterfaceWillShadowBaseClassInterface));
      visitor.InterfaceIntroductionRules.Add (new DelegateValidationRule<InterfaceIntroductionDefinition> (IMixinTargetCannotBeIntroduced));
    }


    private void InterfaceWillShadowBaseClassInterface (DelegateValidationRule<InterfaceIntroductionDefinition>.Args args)
    {
      List<Type> interfaces = new List<Type> (args.Definition.BaseClass.ImplementedInterfaces);
      SingleShould (!interfaces.Contains (args.Definition.Type), args.Log, args.Self);
    }

    private void IMixinTargetCannotBeIntroduced (DelegateValidationRule<InterfaceIntroductionDefinition>.Args args)
    {
      SingleMust (!typeof (IMixinTarget).Equals (args.Definition.Type), args.Log, args.Self);
    }
  }
}