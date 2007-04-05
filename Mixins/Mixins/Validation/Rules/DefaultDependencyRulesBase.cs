using System;
using System.Collections;
using System.Collections.Generic;
using Mixins.Definitions;

namespace Mixins.Validation.Rules
{
  public abstract class DefaultDependencyRulesBase<TDependencyDefinition, TRequirement> : RuleSetBase
      where TDependencyDefinition : DependencyDefinitionBase<TRequirement>
      where TRequirement : RequirementDefinitionBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      GetRules (visitor).Add (new DelegateValidationRule<TDependencyDefinition> (DependencyMustBeSatisfied));
      GetRules (visitor).Add (new DelegateValidationRule<TDependencyDefinition> (NoCircularDependencies));
    }

    protected abstract IList<IValidationRule<TDependencyDefinition>> GetRules (ValidatingVisitor visitor);

    protected abstract void DependencyMustBeSatisfied (TDependencyDefinition definition, IValidationLog log, DelegateValidationRule<TDependencyDefinition> self);

    protected void DependencyMustBeSatisfiedImpl (TDependencyDefinition definition, IValidationLog log, DelegateValidationRule<TDependencyDefinition> self)
    {
      SingleMust (definition.GetImplementer () != null, log, self);
    }

    protected abstract void NoCircularDependencies (TDependencyDefinition definition, IValidationLog log, DelegateValidationRule<TDependencyDefinition> self);

    protected void NoCircularDependenciesImpl (TDependencyDefinition definition, IValidationLog log, DelegateValidationRule<TDependencyDefinition> self)
    {
      List<MixinDefinition> requiredMixins = new List<MixinDefinition> ();
      if (CheckNoCircularities (definition, requiredMixins))
      {
        log.Succeed (self);
      }
      else
      {
        log.Fail (self);
      }
    }

    private bool CheckNoCircularities (TDependencyDefinition definition, List<MixinDefinition> requiredMixins)
    {
      ClassDefinition implementer = definition.GetImplementer ();
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
      foreach (TDependencyDefinition dependency in GetDependencies (mixin))
      {
        if (!CheckNoCircularities (dependency, requiredMixins))
        {
          return false;
        }
      }
      return true;
    }

    protected abstract IEnumerable<TDependencyDefinition> GetDependencies (MixinDefinition mixin);
  }
}
