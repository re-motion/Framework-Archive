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

    private void InterfaceShouldNotBeImplementedTwice (InterfaceIntroductionDefinition definition, IValidationLog log, DelegateValidationRule<InterfaceIntroductionDefinition> self)
    {
      List<Type> interfaces = new List<Type>(definition.BaseClass.ImplementedInterfaces);
      SingleShould (!interfaces.Contains (definition.Type), log, self);
    }
  }
}
