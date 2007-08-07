using System;
using System.Collections.Generic;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultRequiredFaceTypeRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredFaceTypeRules.Add (new DelegateValidationRule<RequiredFaceTypeDefinition> (FaceClassMustBeAssignableFromTargetType));
      visitor.RequiredFaceTypeRules.Add (new DelegateValidationRule<RequiredFaceTypeDefinition> (RequiredFaceTypeMustBePublic));
    }

    private void FaceClassMustBeAssignableFromTargetType (DelegateValidationRule<RequiredFaceTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsClass ? args.Definition.Type.IsAssignableFrom (args.Definition.TargetClass.Type) : true, args.Log, args.Self);
    }

    private void RequiredFaceTypeMustBePublic (DelegateValidationRule<RequiredFaceTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
    }
  }
}
