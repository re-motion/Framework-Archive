using System;
using System.Collections.Generic;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Utilities;
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
          || ReflectionUtility.IsPublicOrProtected (args.Definition.ImplementingMethod.MethodInfo), args.Log, args.Self);
    }
  }
}
