using System;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Validation;
using System.Reflection;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultBaseClassRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.BaseClassRules.Add (new DelegateValidationRule<BaseClassDefinition> (BaseClassMustNotBeSealed));
      visitor.BaseClassRules.Add (new DelegateValidationRule<BaseClassDefinition> (BaseClassMustNotBeAnInterface));
      visitor.BaseClassRules.Add (new DelegateValidationRule<BaseClassDefinition> (BaseClassMustNotBeAbstract));
      visitor.BaseClassRules.Add (new DelegateValidationRule<BaseClassDefinition> (BaseClassMustHavePublicOrProtectedCtor));
    }

    private void BaseClassMustNotBeSealed (DelegateValidationRule<BaseClassDefinition>.Args args)
    {
      SingleMust(!args.Definition.Type.IsSealed, args.Log, args.Self);
    }

    private void BaseClassMustNotBeAnInterface (DelegateValidationRule<BaseClassDefinition>.Args args)
    {
      SingleMust (!args.Definition.Type.IsInterface, args.Log, args.Self);
    }

    private void BaseClassMustNotBeAbstract (DelegateValidationRule<BaseClassDefinition>.Args args)
    {
      SingleMust (!args.Definition.Type.IsAbstract, args.Log, args.Self);
    }

    private void BaseClassMustHavePublicOrProtectedCtor (DelegateValidationRule<BaseClassDefinition>.Args args)
    {
      ConstructorInfo[] ctors = args.Definition.Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      ConstructorInfo[] publicOrProtectedCtors = Array.FindAll (ctors, delegate (ConstructorInfo ctor) { return ctor.IsPublic || ctor.IsFamily; });
      SingleMust (publicOrProtectedCtors.Length > 0, args.Log, args.Self);
    }
  }
}
