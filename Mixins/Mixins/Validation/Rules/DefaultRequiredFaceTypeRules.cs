using System;
using System.Collections.Generic;
using Mixins.Definitions;

namespace Mixins.Validation.Rules
{
  public class DefaultRequiredFaceTypeRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredFaceTypeRules.Add (new DelegateValidationRule<RequiredFaceTypeDefinition> (FaceClassMustBeAssignableFromTargetType));
      visitor.RequiredFaceTypeRules.Add (new DelegateValidationRule<RequiredFaceTypeDefinition> (FaceInterfaceMustBeIntroducedOrImplemented));
    }

    private void FaceClassMustBeAssignableFromTargetType (RequiredFaceTypeDefinition definition, IValidationLog log, DelegateValidationRule<RequiredFaceTypeDefinition> self)
    {
      SingleMust(definition.Type.IsClass ? definition.Type.IsAssignableFrom(definition.BaseClass.Type) : true, log, self);
    }

    private void FaceInterfaceMustBeIntroducedOrImplemented (RequiredFaceTypeDefinition definition, IValidationLog log, DelegateValidationRule<RequiredFaceTypeDefinition> self)
    {
      List<Type> implementedInterfaces = new List<Type>(definition.BaseClass.ImplementedInterfaces);
      List<Type> introducedInterfaces = new List<InterfaceIntroductionDefinition>(definition.BaseClass.IntroducedInterfaces).ConvertAll<Type>
        (delegate (InterfaceIntroductionDefinition i) { return i.Type; });
      SingleMust (definition.Type.IsInterface ? implementedInterfaces.Contains(definition.Type) || introducedInterfaces.Contains(definition.Type) : true, log, self);
    }
  }
}
