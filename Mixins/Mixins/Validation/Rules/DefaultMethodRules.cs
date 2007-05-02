using System;
using Mixins.Definitions;
using System.Reflection;

namespace Mixins.Validation.Rules
{
  public class DefaultMethodRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (OverriddenMethodMustBeVirtual));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (InitializationMethodCanOnlyHaveThisAndBaseArguments));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (InitializationMethodMustNotBeGeneric));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (InitializationMethodMustHaveUniqueName));
    }

    private void OverriddenMethodMustBeVirtual (DelegateValidationRule<MethodDefinition>.Args args)
    {
      SingleMust (args.Definition.Overrides.GetEnumerator ().MoveNext () ? args.Definition.MethodInfo.IsVirtual : true, args.Log, args.Self);
    }

    private void InitializationMethodCanOnlyHaveThisAndBaseArguments (DelegateValidationRule<MethodDefinition>.Args args)
    {
      MixinDefinition parent = args.Definition.DeclaringClass as MixinDefinition;
      if (parent != null && parent.InitializationMethods.HasItem(args.Definition.MethodInfo))
      {
        foreach (ParameterInfo parameter in args.Definition.MethodInfo.GetParameters())
        {
          if (!parameter.IsDefined (typeof (ThisAttribute), false) && !parameter.IsDefined (typeof(BaseAttribute), false))
          {
            args.Log.Fail (args.Self);
            return;
          }
        }
      }
      args.Log.Succeed (args.Self);
    }

    private void InitializationMethodMustNotBeGeneric (DelegateValidationRule<MethodDefinition>.Args args)
    {
      MixinDefinition parent = args.Definition.DeclaringClass as MixinDefinition;
      if (parent != null && parent.InitializationMethods.HasItem (args.Definition.MethodInfo))
      {
        SingleMust (!args.Definition.MethodInfo.IsGenericMethodDefinition, args.Log, args.Self);
      }
      else
      {
        args.Log.Succeed (args.Self);
      }
    }

    private void InitializationMethodMustHaveUniqueName (DelegateValidationRule<MethodDefinition>.Args args)
    {
      MixinDefinition parent = args.Definition.DeclaringClass as MixinDefinition;
      if (parent != null && parent.InitializationMethods.HasItem (args.Definition.MethodInfo))
      {
        foreach (MethodDefinition method in parent.Methods)
        {
          if (method != args.Definition && method.Name == args.Definition.Name)
          {
            args.Log.Fail (args.Self);
            return;
          }
        }
      }
      args.Log.Succeed (args.Self);
    }
  }
}
