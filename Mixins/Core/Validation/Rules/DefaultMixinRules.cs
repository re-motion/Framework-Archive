using System;
using System.Reflection;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Utilities;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultMixinRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinCannotBeInterface));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinMustBePublic));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinWithOverriddenMembersMustHavePublicOrProtectedDefaultCtor));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinCannotMixItself));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinCannotMixItsBase));
    }

    private void MixinCannotBeInterface (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (!args.Definition.Type.IsInterface, args.Log, args.Self);
    }

    private void MixinMustBePublic (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
    }

    private void MixinWithOverriddenMembersMustHavePublicOrProtectedDefaultCtor (DelegateValidationRule<MixinDefinition>.Args args)
    {
      ConstructorInfo defaultCtor = args.Definition.Type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null, Type.EmptyTypes, null);
      SingleMust (!args.Definition.HasOverriddenMembers() || (defaultCtor != null && ReflectionUtility.IsPublicOrProtected (defaultCtor)),
          args.Log, args.Self);
    }

    private void MixinCannotMixItself (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (args.Definition.Type != args.Definition.TargetClass.Type, args.Log, args.Self);
    }

    private void MixinCannotMixItsBase (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (!args.Definition.TargetClass.Type.IsAssignableFrom (args.Definition.Type), args.Log, args.Self);
    }
  }
}