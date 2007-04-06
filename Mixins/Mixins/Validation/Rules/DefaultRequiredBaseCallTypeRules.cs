using System;
using System.Collections.Generic;
using Mixins.Definitions;

namespace Mixins.Validation.Rules
{
  public class DefaultRequiredBaseCallTypeRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredBaseCallTypeRules.Add (new DelegateValidationRule<RequiredBaseCallTypeDefinition> (BaseCallTypeMustBeInterface));
      visitor.RequiredBaseCallTypeRules.Add (new DelegateValidationRule<RequiredBaseCallTypeDefinition> (BaseCallTypeMustBeIntroducedOrImplemented));
    }

    private void BaseCallTypeMustBeInterface (DelegateValidationRule<RequiredBaseCallTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsInterface, args.Log, args.Self);
    }

    private void BaseCallTypeMustBeIntroducedOrImplemented (DelegateValidationRule<RequiredBaseCallTypeDefinition>.Args args)
    {
      List<Type> implementedInterfaces = new List<Type>(args.Definition.BaseClass.ImplementedInterfaces);
      List<Type> introducedInterfaces = new List<InterfaceIntroductionDefinition>(args.Definition.BaseClass.IntroducedInterfaces).ConvertAll<Type>
        (delegate (InterfaceIntroductionDefinition i) { return i.Type; });
      SingleMust (args.Definition.IsEmptyInterface
          || implementedInterfaces.Contains (args.Definition.Type) || introducedInterfaces.Contains (args.Definition.Type), args.Log, args.Self);
    }
  }
}
