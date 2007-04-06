using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

namespace Mixins.Validation.Rules
{
  public class DefaultThisDependencyRules : DefaultDependencyRulesBase<ThisDependencyDefinition, RequiredFaceTypeDefinition>
  {
    protected override IList<IValidationRule<ThisDependencyDefinition>> GetRules (ValidatingVisitor visitor)
    {
      return visitor.ThisDependencyRules;
    }

    protected override IEnumerable<ThisDependencyDefinition> GetDependencies (MixinDefinition mixin)
    {
      return mixin.ThisDependencies;
    }

    protected override void NoCircularDependencies (DelegateValidationRule<ThisDependencyDefinition>.Args args)
    {
      NoCircularDependenciesImpl (args);
    }

    protected override void DependencyMustBeSatisfied (DelegateValidationRule<ThisDependencyDefinition>.Args args)
    {
      DependencyMustBeSatisfiedImpl (args);
    }

    protected override void AggregateDependencyMustBeFullyImplemented (DelegateValidationRule<ThisDependencyDefinition>.Args args)
    {
      AggregateDependencyMustBeFullyImplementedImpl (args);
    }
  }
}
