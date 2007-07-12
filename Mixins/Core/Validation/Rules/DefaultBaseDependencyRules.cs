using System;
using System.Collections.Generic;
using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultBaseDependencyRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      // no rules ATM
    }

    private void NoCircularDependencies (DelegateValidationRule<BaseDependencyDefinition>.Args args)
    {
      // Now throws a ConfigurationException if violated
    }

    private void DependencyMustBeSatisfied (DelegateValidationRule<BaseDependencyDefinition>.Args args)
    {
      // Now throws a ConfigurationException if violated
    }

    private void AggregateDependencyMustBeFullyImplemented (DelegateValidationRule<BaseDependencyDefinition>.Args args)
    {
      // Now throws a ConfigurationException if violated
    }
  }
}
