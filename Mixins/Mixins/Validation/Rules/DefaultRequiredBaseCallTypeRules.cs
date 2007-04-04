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

    private void BaseCallTypeMustBeInterface (RequiredBaseCallTypeDefinition definition, IValidationLog log, DelegateValidationRule<RequiredBaseCallTypeDefinition> self)
    {
      SingleMust (definition.Type.IsInterface, log, self);
    }

    private void BaseCallTypeMustBeIntroducedOrImplemented (RequiredBaseCallTypeDefinition definition, IValidationLog log, DelegateValidationRule<RequiredBaseCallTypeDefinition> self)
    {
      List<Type> implementedInterfaces = new List<Type>(definition.BaseClass.ImplementedInterfaces);
      List<Type> introducedInterfaces = new List<InterfaceIntroductionDefinition>(definition.BaseClass.IntroducedInterfaces).ConvertAll<Type>
        (delegate (InterfaceIntroductionDefinition i) { return i.Type; });
      SingleMust (implementedInterfaces.Contains (definition.Type) || introducedInterfaces.Contains (definition.Type), log, self);
    }
  }
}
