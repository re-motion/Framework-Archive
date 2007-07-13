using System;
using System.Collections.Generic;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultRequiredBaseCallTypeRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredBaseCallTypeRules.Add (new DelegateValidationRule<RequiredBaseCallTypeDefinition> (RequiredBaseCallTypeMustBePublic));
    }

    // Now throws ConfigurationException when violated
    //private void BaseCallTypeMustBeInterface (DelegateValidationRule<RequiredBaseCallTypeDefinition>.Args args)
    //{
    //  SingleMust (args.Definition.Type.IsInterface, args.Log, args.Self);
    //}

    private void RequiredBaseCallTypeMustBePublic (DelegateValidationRule<RequiredBaseCallTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
    }

  }
}
