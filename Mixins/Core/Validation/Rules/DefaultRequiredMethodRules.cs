using System;
using System.Collections.Generic;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultRequiredMethodRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredMethodRules.Add (new DelegateValidationRule<RequiredMethodDefinition> (RequiredBaseCallMethodMustBePublicOrProtected));
    }

    private void RequiredBaseCallMethodMustBePublicOrProtected (DelegateValidationRule<RequiredMethodDefinition>.Args args)
    {
      SingleMust (!(args.Definition.DeclaringRequirement is RequiredBaseCallTypeDefinition)
          || args.Definition.ImplementingMethod.MethodInfo.IsPublic || args.Definition.ImplementingMethod.MethodInfo.IsFamily, args.Log, args.Self);
    }
  }
}
