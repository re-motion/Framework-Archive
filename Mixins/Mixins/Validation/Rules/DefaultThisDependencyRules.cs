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

    protected override void NoCircularDependencies (ThisDependencyDefinition definition, IValidationLog log, DelegateValidationRule<ThisDependencyDefinition> self)
    {
      NoCircularDependenciesImpl (definition, log, self);
    }

    protected override void DependencyMustBeSatisfied (ThisDependencyDefinition definition, IValidationLog log, DelegateValidationRule<ThisDependencyDefinition> self)
    {
      DependencyMustBeSatisfiedImpl (definition, log, self);
    }
  }
}
