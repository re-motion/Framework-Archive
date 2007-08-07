using System;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Utilities;
using Rubicon.Mixins.Validation;
using System.Reflection;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultTargetClassRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.TargetClassRules.Add (new DelegateValidationRule<TargetClassDefinition> (TargetClassMustNotBeSealed));
      visitor.TargetClassRules.Add (new DelegateValidationRule<TargetClassDefinition> (TargetClassMustNotBeAnInterface));
      visitor.TargetClassRules.Add (new DelegateValidationRule<TargetClassDefinition> (TargetClassMustHavePublicOrProtectedCtor));
    }

    private void TargetClassMustNotBeSealed (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
      SingleMust(!args.Definition.Type.IsSealed, args.Log, args.Self);
    }

    private void TargetClassMustNotBeAnInterface (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
      SingleMust (!args.Definition.Type.IsInterface, args.Log, args.Self);
    }

    private void TargetClassMustHavePublicOrProtectedCtor (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
      ConstructorInfo[] ctors = args.Definition.Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      ConstructorInfo[] publicOrProtectedCtors = Array.FindAll (ctors,
          delegate (ConstructorInfo ctor) { return ReflectionUtility.IsPublicOrProtected (ctor); });
      SingleMust (publicOrProtectedCtors.Length > 0, args.Log, args.Self);
    }
  }
}
