using System;
using Mixins.Definitions;

namespace Mixins.Validation.Rules
{
  public class DefaultMixinRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinCannotBeInterface));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinMustBePublic));
    }

    private void MixinCannotBeInterface (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (!args.Definition.Type.IsInterface, args.Log, args.Self);
    }

    private void MixinMustBePublic (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (IsPublicRecursive (args.Definition.Type), args.Log, args.Self);
    }

    private bool IsPublicRecursive (Type type)
    {
      if (!type.IsNested)
      {
        return type.IsPublic;
      }
      else
      {
        return type.IsNestedPublic && IsPublicRecursive (type.DeclaringType);
      }
    }
  }
}