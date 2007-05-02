using System;
using System.Collections.Generic;
using Mixins.Definitions;

namespace Mixins.Validation.Rules
{
  public class DefaultBaseDependencyRules : DefaultDependencyRulesBase<BaseDependencyDefinition, RequiredBaseCallTypeDefinition>
  {
    protected override IList<IValidationRule<BaseDependencyDefinition>> GetRules (ValidatingVisitor visitor)
    {
      return visitor.BaseDependencyRules;
    }

    protected override IEnumerable<BaseDependencyDefinition> GetDependencies (MixinDefinition mixin)
    {
      return mixin.BaseDependencies;
    }

    protected override void NoCircularDependencies (DelegateValidationRule<BaseDependencyDefinition>.Args args)
    {
      NoCircularDependenciesImpl (args);
    }

    protected override void DependencyMustBeSatisfied (DelegateValidationRule<BaseDependencyDefinition>.Args args)
    {
      // DependencyMustBeSatisfiedImpl (args);
      // Now throws a ConfigurationException if violated
    }

    protected override void AggregateDependencyMustBeFullyImplemented (DelegateValidationRule<BaseDependencyDefinition>.Args args)
    {
      // AggregateDependencyMustBeFullyImplementedImpl (args);
      // Now throws a ConfigurationException if violated
    }
  }
}
