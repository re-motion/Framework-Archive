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

    protected override void NoCircularDependencies (BaseDependencyDefinition definition, IValidationLog log, DelegateValidationRule<BaseDependencyDefinition> self)
    {
      NoCircularDependenciesImpl (definition, log, self);
    }

    protected override void DependencyMustBeSatisfied (BaseDependencyDefinition definition, IValidationLog log, DelegateValidationRule<BaseDependencyDefinition> self)
    {
      DependencyMustBeSatisfiedImpl (definition, log, self);
    }
  }
}
