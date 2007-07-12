using System;
using System.Collections.Generic;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.Validation.Rules
{
  public abstract class DefaultDependencyRulesBase<TDependency, TRequirement> : RuleSetBase
      where TDependency : DependencyDefinitionBase<TRequirement, TDependency>
      where TRequirement : RequirementDefinitionBase<TRequirement, TDependency>
  {
    public override void Install (ValidatingVisitor visitor)
    {
      GetRules (visitor).Add (new DelegateValidationRule<TDependency> (DependencyMustBeSatisfied));
      GetRules (visitor).Add (new DelegateValidationRule<TDependency> (NoCircularDependencies));
      GetRules (visitor).Add (new DelegateValidationRule<TDependency> (AggregateDependencyMustBeFullyImplemented));
    }

    protected abstract IList<IValidationRule<TDependency>> GetRules (ValidatingVisitor visitor);

    protected abstract void DependencyMustBeSatisfied (DelegateValidationRule<TDependency>.Args args);

    protected void DependencyMustBeSatisfiedImpl (DelegateValidationRule<TDependency>.Args args)
    {
      SingleMust (args.Definition.GetImplementer () != null || args.Definition.IsAggregate, args.Log, args.Self);
    }

    protected abstract void AggregateDependencyMustBeFullyImplemented (DelegateValidationRule<TDependency>.Args args);

    protected void AggregateDependencyMustBeFullyImplementedImpl (DelegateValidationRule<TDependency>.Args args)
    {
      if (args.Definition.IsAggregate)
      {
        foreach (TDependency dependency in args.Definition.AggregatedDependencies)
        {
          dependency.Accept (args.Validator);
        }
      }
    }

    protected abstract void NoCircularDependencies (DelegateValidationRule<TDependency>.Args args);

    protected void NoCircularDependenciesImpl (DelegateValidationRule<TDependency>.Args args)
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

    private bool CheckNoCircularities (TDependency definition, List<MixinDefinition> requiredMixins)
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
      foreach (TDependency dependency in GetDependencies (mixin))
      {
        if (!CheckNoCircularities (dependency, requiredMixins))
        {
          return false;
        }
      }
      return true;
    }

    protected abstract IEnumerable<TDependency> GetDependencies (MixinDefinition mixin);
  }
}
