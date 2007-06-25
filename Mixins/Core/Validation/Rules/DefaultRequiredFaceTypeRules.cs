using System;
using System.Collections.Generic;
using Mixins.Definitions;
using Mixins.Validation;

namespace Mixins.Validation.Rules
{
  public class DefaultRequiredFaceTypeRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredFaceTypeRules.Add (new DelegateValidationRule<RequiredFaceTypeDefinition> (FaceClassMustBeAssignableFromTargetType));
      visitor.RequiredFaceTypeRules.Add (new DelegateValidationRule<RequiredFaceTypeDefinition> (FaceInterfaceMustBeIntroducedOrImplemented));
      visitor.RequiredFaceTypeRules.Add (new DelegateValidationRule<RequiredFaceTypeDefinition> (RequiredFaceTypeMustBePublic));
    }

    private void FaceClassMustBeAssignableFromTargetType (DelegateValidationRule<RequiredFaceTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsClass ? args.Definition.Type.IsAssignableFrom (args.Definition.BaseClass.Type) : true, args.Log, args.Self);
    }

    private void FaceInterfaceMustBeIntroducedOrImplemented (DelegateValidationRule<RequiredFaceTypeDefinition>.Args args)
    {
      List<Type> implementedInterfaces = new List<Type> (args.Definition.BaseClass.ImplementedInterfaces);
      List<Type> introducedInterfaces = new List<InterfaceIntroductionDefinition> (args.Definition.BaseClass.IntroducedInterfaces).ConvertAll<Type>
        (delegate (InterfaceIntroductionDefinition i) { return i.Type; });

      SingleMust (args.Definition.Type.IsClass || args.Definition.IsEmptyInterface
          || implementedInterfaces.Contains(args.Definition.Type) || introducedInterfaces.Contains (args.Definition.Type), args.Log, args.Self);
    }

    private void RequiredFaceTypeMustBePublic (DelegateValidationRule<RequiredFaceTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
    }
  }
}
