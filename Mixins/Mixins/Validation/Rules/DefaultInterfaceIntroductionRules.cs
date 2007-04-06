using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

namespace Mixins.Validation.Rules
{
  public class DefaultInterfaceIntroductionRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.InterfaceIntroductionRules.Add (new DelegateValidationRule<InterfaceIntroductionDefinition> (InterfaceShouldNotBeImplementedTwice));
    }

    private void InterfaceShouldNotBeImplementedTwice (DelegateValidationRule<InterfaceIntroductionDefinition>.Args args)
    {
      List<Type> interfaces = new List<Type>(args.Definition.BaseClass.ImplementedInterfaces);
      SingleShould (!interfaces.Contains (args.Definition.Type), args.Log, args.Self);
    }
  }
}
