using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultThisDependencyRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.ThisDependencyRules.Add (new DelegateValidationRule<ThisDependencyDefinition> (DependencyMustBeSatisfied));
      visitor.ThisDependencyRules.Add (new DelegateValidationRule<ThisDependencyDefinition> (NoCircularDependencies));
      visitor.ThisDependencyRules.Add (new DelegateValidationRule<ThisDependencyDefinition> (AggregateDependencyMustBeFullyImplemented));
    }

    private void NoCircularDependencies (DelegateValidationRule<ThisDependencyDefinition>.Args args)
    {
      List<MixinDefinition> requiredMixins = new List<MixinDefinition> ();
      if (CheckNoCircularities (args.Definition, requiredMixins))
      {
        args.Log.Succeed (args.Self);
      }
      else
      {
        args.Log.Fail (args.Self);
      }
    }

    private bool CheckNoCircularities (ThisDependencyDefinition definition, List<MixinDefinition> requiredMixins)
    {
      ClassDefinitionBase implementer = definition.GetImplementer ();
      MixinDefinition implementingMixin = implementer as MixinDefinition;
      if (implementingMixin == null || implementer == definition.Depender)
      {
        return true;
      }
      else if (requiredMixins.Contains (implementingMixin))
      {
        return false;
      }
      else
      {
        requiredMixins.Add (implementingMixin);
        return CheckNoCircularities (implementingMixin, requiredMixins);
      }
    }

    private bool CheckNoCircularities (MixinDefinition mixin, List<MixinDefinition> requiredMixins)
    {
      foreach (ThisDependencyDefinition dependency in mixin.ThisDependencies)
      {
        if (!CheckNoCircularities (dependency, requiredMixins))
        {
          return false;
        }
      }
      return true;
    }

    private void DependencyMustBeSatisfied (DelegateValidationRule<ThisDependencyDefinition>.Args args)
    {
      SingleMust (args.Definition.GetImplementer () != null || args.Definition.IsAggregate, args.Log, args.Self);
    }

    private void AggregateDependencyMustBeFullyImplemented (DelegateValidationRule<ThisDependencyDefinition>.Args args)
    {
      if (args.Definition.IsAggregate)
      {
        foreach (ThisDependencyDefinition dependency in args.Definition.AggregatedDependencies)
        {
          dependency.Accept (args.Validator);
        }
      }
    }
  }
}
