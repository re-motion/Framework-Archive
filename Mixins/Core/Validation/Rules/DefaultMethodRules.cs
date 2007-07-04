using System;
using Rubicon.Mixins.Definitions;
using System.Reflection;
using Rubicon.Mixins.Utilities;
using Rubicon.Mixins.Validation;
using Rubicon.Mixins.CodeGeneration;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultMethodRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (OverriddenMethodMustBeVirtual));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (OverriddenMethodMustNotBeFinal));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (AbstractMethodMustBeOverridden));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (NoCircularOverrides));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (OverridingMixinMethodsOnlyPossibleWhenMixinDerivedFromMixinBase));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (OverridingMethodMustBePublic));
    }

    private void OverriddenMethodMustBeVirtual (DelegateValidationRule<MethodDefinition>.Args args)
    {
      SingleMust (args.Definition.Overrides.Count > 0 ? args.Definition.MethodInfo.IsVirtual : true, args.Log, args.Self);
    }

    private void OverriddenMethodMustNotBeFinal (DelegateValidationRule<MethodDefinition>.Args args)
    {
      SingleMust (args.Definition.Overrides.Count > 0 ? !args.Definition.MethodInfo.IsFinal : true, args.Log, args.Self);
    }

    private void AbstractMethodMustBeOverridden (DelegateValidationRule<MethodDefinition>.Args args)
    {
      SingleMust (!args.Definition.MethodInfo.IsAbstract || args.Definition.Overrides.Count > 0, args.Log, args.Self);
    }

    private void NoCircularOverrides (DelegateValidationRule<MethodDefinition>.Args args)
    {
      MethodDefinition originalMethod = args.Definition;
      MethodDefinition method = args.Definition.Base;
      while (method != null && method != originalMethod)
        method = method.Base;
      SingleMust (method != originalMethod, args.Log, args.Self);
    }

    private void OverridingMixinMethodsOnlyPossibleWhenMixinDerivedFromMixinBase (DelegateValidationRule<MethodDefinition>.Args args)
    {
      SingleMust (!(args.Definition.DeclaringClass is MixinDefinition) || args.Definition.Overrides.Count == 0
          || MixinReflector.GetMixinBaseType (args.Definition.DeclaringClass.Type) != null, args.Log, args.Self);
    }

    private void OverridingMethodMustBePublic (DelegateValidationRule<MethodDefinition>.Args args)
    {
      SingleMust (args.Definition.Base == null || args.Definition.MethodInfo.IsPublic, args.Log, args.Self);
    }
  }
}
